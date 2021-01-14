﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Quantum.QIR;
using Microsoft.Quantum.QIR.Emission;
using Microsoft.Quantum.QsCompiler.SyntaxTokens;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Ubiquity.NET.Llvm.Types;
using Ubiquity.NET.Llvm.Values;

namespace Microsoft.Quantum.QsCompiler.QIR
{
    using ResolvedTypeKind = QsTypeKind<ResolvedType, UserDefinedType, QsTypeParameter, CallableInformation>;

    /// <summary>
    /// This class is used to track the validity of variables and values, to track access and reference counts,
    /// and to release and unreference values when they go out of scope.
    /// <para>
    /// There are two primary ways to leave a scope: close it and continue in the parent scope, or exit the current
    /// scope and all parent scopes and leave the callable altogether (i.e., return).
    /// While each scope has a single place it is closed, there may be many exits.
    /// </para>
    /// </summary>
    internal class ScopeManager
    {
        private class Scope
        {
            private readonly ScopeManager parent;

            /// <summary>
            /// Maps variable names to the corresponding value.
            /// Mutable variables are represented as PointerValues.
            /// </summary>
            private readonly Dictionary<string, IValue> variables = new Dictionary<string, IValue>();

            /// <summary>
            /// Contains all values that require unreferencing upon closing the scope.
            /// </summary>
            private readonly List<IValue> trackedValues = new List<IValue>();

            /// <summary>
            /// Contains the values that require invoking a release function upon closing the scope,
            /// as well as the name of the release function to invoke.
            /// </summary>
            private readonly List<(IValue, string)> requiredReleases = new List<(IValue, string)>();

            public Scope(ScopeManager parent)
            {
                this.parent = parent;
            }

            // public and internal methods

            public void AddVariable(string varName, IValue value) =>
                this.variables.Add(varName, value);

            public void AddValue(IValue value, string? releaseFunction = null)
            {
                if (releaseFunction != null)
                {
                    this.requiredReleases.Add((value, releaseFunction));
                }
                if (this.parent.ReferencesUpdateFunctionForType(value.LlvmType) != null)
                {
                    this.trackedValues.Add(value);
                }
            }

            public bool TryGetVariable(string varName, out IValue value) =>
                this.variables.TryGetValue(varName, out value);

            /// <summary>
            /// Returns true if the given value will be unreferenced by <see cref="ExecutePendingCalls" />
            /// unless it is explicitly excluded.
            /// </summary>
            internal bool WillBeUnreferenced(IValue value) =>
                this.trackedValues.Exists(tracked => tracked.Value == value.Value);

            /// <summary>
            /// Generates the necessary calls to unreference the tracked values, decrease the access count for registered variables,
            /// and invokes the specified release functions for values if necessary.
            /// Skips unreferencing the values specified in omitUnreferencing, removing them from the list.
            /// </summary>
            /// <param name="omitUnreferencing">
            /// Values for which to omit the call to unreference them; for each value at most one call will be omitted and the value will be removed from the list
            /// </param>
            internal void ExecutePendingCalls(List<IValue>? omitUnreferencing = null) =>
                ExecutePendingCalls(this.parent, omitUnreferencing ?? new List<IValue>(), this);

            internal static void ExecutePendingCalls(ScopeManager parent, List<IValue> omitUnreferencing, params Scope[] scopes)
            {
                foreach (var (value, funcName) in scopes.SelectMany(s => s.requiredReleases))
                {
                    var func = parent.sharedState.GetOrCreateRuntimeFunction(funcName);
                    parent.sharedState.CurrentBuilder.Call(func, value.Value);
                }

                var pendingUnreferences = new Queue<IValue>();
                foreach (var value in scopes.SelectMany(s => s.trackedValues))
                {
                    var omitted = omitUnreferencing.FirstOrDefault(omitted => omitted.Value == value.Value);
                    if (!omitUnreferencing.Remove(omitted))
                    {
                        pendingUnreferences.Enqueue(value);
                    }
                }

                parent.DecreaseAccessCount(scopes.SelectMany(s => s.variables).Select(kv => kv.Value).ToArray());
                parent.DecreaseReferenceCount(pendingUnreferences.ToArray());
            }
        }

        private readonly GenerationContext sharedState;
        private readonly IValue minusOne;
        private readonly IValue plusOne;

        /// <summary>
        /// New variables and values are always added to the scope on top of the stack.
        /// When looking for a name, the stack is searched top-down.
        /// </summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>
        /// Is true when there are currently no stack frames tracked.
        /// Stack frames are added and removed by OpenScope and CloseScope respectively.
        /// </summary>
        public bool IsEmpty => !this.scopes.Any();

        /// <summary>
        /// Creates a new ref count scope manager.
        /// </summary>
        /// <param name="ctx">The generation context the new manager should be associated with</param>
        public ScopeManager(GenerationContext ctx)
        {
            this.sharedState = ctx;
            var intType = ResolvedType.New(ResolvedTypeKind.Int);
            this.minusOne = ctx.Values.FromSimpleValue(ctx.Context.CreateConstant(-1L), intType);
            this.plusOne = ctx.Values.FromSimpleValue(ctx.Context.CreateConstant(1L), intType);
        }

        // private helpers

        /// <summary>
        /// Gets the name of the runtime function to update the access count for a given LLVM type.
        /// </summary>
        /// <param name="t">The LLVM type</param>
        /// <returns>The name of the function to update the access count for this type</returns>
        private string? AccessUpdateFunctionForType(ITypeRef t)
        {
            if (t.IsPointer)
            {
                if (Types.IsTypedTuple(t))
                {
                    return RuntimeLibrary.TupleUpdateAccessCount;
                }
                else if (Types.IsArray(t))
                {
                    return RuntimeLibrary.ArrayUpdateAccessCount;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the name of the runtime function to update the reference count for a given LLVM type.
        /// </summary>
        /// <param name="t">The LLVM type</param>
        /// <returns>The name of the function to update the reference count for this type</returns>
        private string? ReferencesUpdateFunctionForType(ITypeRef t)
        {
            if (t.IsPointer)
            {
                if (Types.IsTypedTuple(t))
                {
                    return RuntimeLibrary.TupleUpdateReferenceCount;
                }
                else if (Types.IsArray(t))
                {
                    return RuntimeLibrary.ArrayUpdateReferenceCount;
                }
                else if (Types.IsCallable(t))
                {
                    return RuntimeLibrary.CallableUpdateReferenceCount;
                }
                else if (Types.IsResult(t))
                {
                    return RuntimeLibrary.ResultUpdateReferenceCount;
                }
                else if (Types.IsString(t))
                {
                    return RuntimeLibrary.StringUpdateReferenceCount;
                }
                else if (Types.IsBigInt(t))
                {
                    return RuntimeLibrary.BigIntUpdateReferenceCount;
                }
            }
            return null;
        }

        /// <summary>
        /// Given a callable value, changes the reference count of its capture tuple by the given value.
        /// The given value is expected to be either a 64-bit integer, or a tuple with a 64-bit integer.
        /// </summary>
        private void UpdateCaptureTupleReferenceCount(IValue change, CallableValue callable)
        {
            bool IsInt(ITypeRef type) => type == this.sharedState.Types.Int;
            bool IsIntTuple(IStructType type) => type.Members.Count == 1 && IsInt(type.Members[0]);
            var argTuple = change is TupleValue tuple && IsIntTuple(tuple.StructType) ? tuple :
                IsInt(change.LlvmType) ? this.sharedState.Values.CreateTuple(registerWithScopeManager: false, change) :
                throw new ArgumentException("epecting a 64-bit integer or tuple containing a 64-bit integer");

            // Each callable table contains a function pointer that can be invoked via a call to the runtime function
            // CallableMemoryManagement. For each callable value, we instantiate that pointer to a function to modify the
            // reference count of the capture tuple an all its inner items, and we use it here to modify the reference count.
            var updateCaptureReferences = this.sharedState.GetOrCreateRuntimeFunction(RuntimeLibrary.CallableMemoryManagement);
            this.sharedState.CurrentBuilder.Call(updateCaptureReferences, callable.Value, argTuple.OpaquePointer, this.sharedState.Values.Unit.Value);

            if (!(change is TupleValue))
            {
                var tupleUnreference = this.sharedState.GetOrCreateRuntimeFunction(RuntimeLibrary.TupleUpdateReferenceCount);
                this.sharedState.CurrentBuilder.Call(tupleUnreference, argTuple.OpaquePointer, this.minusOne.Value);
            }
        }

        /// <summary>
        /// If the given function returns a function name for the given value,
        /// applies the runtime function with that name to the given value, casting the value if necessary,
        /// Recurs into contained items.
        /// </summary>
        private void RecursivelyModifyCounts(Func<ITypeRef, string?> getFunctionName, IValue change, params IValue[] values)
        {
            TupleValue? changeTuple = null;
            void UnreferenceChangeTuple()
            {
                if (changeTuple != null)
                {
                    var tupleUnreference = this.sharedState.GetOrCreateRuntimeFunction(RuntimeLibrary.TupleUpdateReferenceCount);
                    this.sharedState.CurrentBuilder.Call(tupleUnreference, changeTuple.OpaquePointer, this.minusOne.Value);
                    changeTuple = null;
                }
            }

            void ModifyCounts(string funcName, IValue value)
            {
                var func = this.sharedState.GetOrCreateRuntimeFunction(funcName);

                if (value is PointerValue pointer)
                {
                    ModifyCounts(funcName, pointer.LoadValue());
                }
                else if (value is TupleValue tuple)
                {
                    for (var i = 0; i < tuple.StructType.Members.Count; ++i)
                    {
                        var itemFuncName = getFunctionName(tuple.StructType.Members[i]);
                        if (itemFuncName != null)
                        {
                            var item = tuple.GetTupleElement(i);
                            ModifyCounts(itemFuncName, item);
                        }
                    }

                    this.sharedState.CurrentBuilder.Call(func, tuple.OpaquePointer, change.Value);
                }
                else if (value is ArrayValue array)
                {
                    var itemFuncName = getFunctionName(array.ElementType);
                    if (itemFuncName != null)
                    {
                        UnreferenceChangeTuple();
                        this.sharedState.IterateThroughArray(array, arrItem => ModifyCounts(itemFuncName, arrItem));
                    }
                    this.sharedState.CurrentBuilder.Call(func, array.OpaquePointer, change.Value);
                }
                else if (value is CallableValue callable)
                {
                    Debug.Assert(funcName.Contains("reference_count"));
                    if (this.sharedState.IsWithinLoop)
                    {
                        // The change tuple is deallocated before entering a loop for modifying counts,
                        // and needs to be reallocated here since it may be invalid.
                        // Since we don't generate any branches during modifying counts,
                        // we don't need to check for them.
                        this.UpdateCaptureTupleReferenceCount(change, callable);
                    }
                    else
                    {
                        changeTuple ??= this.sharedState.Values.CreateTuple(registerWithScopeManager: false, change);
                        this.UpdateCaptureTupleReferenceCount(changeTuple, callable);
                    }
                    this.sharedState.CurrentBuilder.Call(func, callable.Value, change.Value);
                }
                else
                {
                    this.sharedState.CurrentBuilder.Call(func, value.Value, change.Value);
                }
            }

            foreach (var value in values)
            {
                var func = getFunctionName(value.LlvmType);
                if (func != null)
                {
                    ModifyCounts(func, value);
                }
            }
            UnreferenceChangeTuple();
        }

        // public and internal methods

        /// <summary>
        /// Opens a new scope and pushes it on top of the naming scope stack.
        /// Opening a new scope automatically opens a new naming scope as well.
        /// </summary>
        public void OpenScope()
        {
            this.scopes.Push(new Scope(this));
        }

        /// <summary>
        /// Closes the current scope by popping it off of the stack.
        /// Closing the scope automatically also closes the current naming scope as well.
        /// Emits the queued calls to unreference, release, and/or decrease the access counts for values going out of scope.
        /// If the current basic block is already terminated, presumably by a return, the calls are not generated.
        /// </summary>
        public void CloseScope(bool isTerminated)
        {
            var scope = this.scopes.Pop();
            if (!isTerminated)
            {
                scope.ExecutePendingCalls();
            }
        }

        /// <summary>
        /// Closes the current scope by popping it off of the stack.
        /// Closing the scope automatically also closes the current naming scope as well.
        /// Emits the queued calls to unreference, release, and/or decrease the access counts for values going out of scope.
        /// Increases the reference count of the returned value by 1, either by omitting to unreference it or by explicitly increasing it.
        /// </summary>
        public void CloseScope(IValue returned)
        {
            var scope = this.scopes.Pop();

            if (!scope.WillBeUnreferenced(returned))
            {
                this.IncreaseReferenceCount(returned);
            }

            scope.ExecutePendingCalls(new List<IValue>() { returned });
        }

        /// <summary>
        /// Exits the current scope by emitting the calls to unreference, release,
        /// and/or decrease the access counts for values going out of scope,
        /// decreasing access counts and invoking release functions if necessary.
        /// Exiting the current scope does *not* close the scope.
        /// </summary>
        public void ExitScope(bool isTerminated)
        {
            var scope = this.scopes.Peek();
            if (!isTerminated)
            {
                scope.ExecutePendingCalls();
            }
        }

        /// <summary>
        /// Returns true if reference counts are tracked for values of the given LLVM type.
        /// </summary>
        internal bool RequiresReferenceCount(ITypeRef t) =>
            this.ReferencesUpdateFunctionForType(t) != null;

        /// <summary>
        /// Adds a call to a runtime library function to increase the reference count for the given value if necessary.
        /// </summary>
        /// <param name="value">The value which is referenced</param>
        public void IncreaseReferenceCount(params IValue[] values) =>
            this.RecursivelyModifyCounts(this.ReferencesUpdateFunctionForType, this.plusOne, values);

        /// <summary>
        /// Adds a call to a runtime library function to decrease the reference count for the given value if necessary.
        /// </summary>
        /// <param name="value">The value which is unreferenced</param>
        public void DecreaseReferenceCount(params IValue[] values) =>
            this.RecursivelyModifyCounts(this.ReferencesUpdateFunctionForType, this.minusOne, values);

        /// <summary>
        /// Adds a call to a runtime library function to change the reference count for the given value.
        /// </summary>
        /// <param name="value">The value for which to change the reference count</param>
        /// <param name="change">The amount by which to change the reference count given as i64</param>
        internal void UpdateReferenceCount(IValue change, params IValue[] values) =>
            this.RecursivelyModifyCounts(this.ReferencesUpdateFunctionForType, change, values);

        /// <summary>
        /// Given a callable value, increases the reference count of its capture tuple by 1.
        /// </summary>
        /// <param name="callable">The callable whose capture tuple to reference</param>
        internal void ReferenceCaptureTuple(CallableValue callable) =>
            this.UpdateCaptureTupleReferenceCount(this.plusOne, callable);

        /// <summary>
        /// Returns true if access counts are tracked for values of the given LLVM type.
        /// </summary>
        internal bool RequiresAccessCount(ITypeRef t) =>
            this.AccessUpdateFunctionForType(t) != null;

        /// <summary>
        /// Adds a call to a runtime library function to increase the access count for the given value if necessary.
        /// </summary>
        /// <param name="value">The value which is assigned to a handle</param>
        internal void IncreaseAccessCount(params IValue[] values) =>
            this.RecursivelyModifyCounts(this.AccessUpdateFunctionForType, this.plusOne, values);

        /// <summary>
        /// Adds a call to a runtime library function to decrease the access count for the given value if necessary.
        /// </summary>
        /// <param name="value">The value which is unassigned from a handle</param>
        internal void DecreaseAccessCount(params IValue[] values) =>
            this.RecursivelyModifyCounts(this.AccessUpdateFunctionForType, this.minusOne, values);

        /// <summary>
        /// Adds a call to a runtime library function to change the access count for the given value.
        /// </summary>
        /// <param name="value">The value for which to change the access count</param>
        /// <param name="countChange">The amount by which to change the access count given as i64</param>
        internal void UpdateAccessCount(IValue change, params IValue[] values) =>
            this.RecursivelyModifyCounts(this.AccessUpdateFunctionForType, change, values);

        /// <summary>
        /// Queues a call to a suitable runtime library function that unreferences the value
        /// when the scope is closed or exited.
        /// </summary>
        /// <param name="value">Value that is created within the current scope</param>
        public void RegisterValue(IValue value)
        {
            this.scopes.Peek().AddValue(value);
        }

        /// <summary>
        /// Adds a value constructed as part of a qubit allocation to the current topmost scope.
        /// Makes sure that all allocated qubits are released when the scope is closed or exited
        /// and that the value and all its items are unreferenced.
        /// </summary>
        public void RegisterAllocatedQubits(IValue value)
        {
            var releaser =
                Types.IsArray(value.LlvmType) ? RuntimeLibrary.QubitReleaseArray :
                Types.IsQubit(this.sharedState.Types.Qubit) ? RuntimeLibrary.QubitRelease :
                throw new ArgumentException("AddQubitValue expects an argument of type Qubit or Qubit[]");
            this.scopes.Peek().AddValue(value, releaser);
        }

        /// <summary>
        /// Registers a variable name as an alias for an LLVM value.
        /// </summary>
        /// <param name="name">The name to register</param>
        /// <param name="value">The LLVM value</param>
        internal void RegisterVariable(string name, IValue value)
        {
            value.RegisterName(this.sharedState.InlinedName(name));
            this.IncreaseAccessCount(value);
            this.scopes.Peek().AddVariable(name, value);
        }

        /// <summary>
        /// Gets the value of a named variable.
        /// The name must have been registered as an alias for the value using
        /// <see cref="RegisterVariable(string, Value, bool)"/>.
        /// </summary>
        /// <param name="name">The registered variable name to look for</param>
        internal IValue GetVariable(string name)
        {
            foreach (var scope in this.scopes)
            {
                if (scope.TryGetVariable(name, out IValue value))
                {
                    return value;
                }
            }
            throw new KeyNotFoundException($"Could not find a Value for local symbol {name}");
        }

        /// <summary>
        /// Exits the current function by emitting the calls to unreference values going out of scope for all open scopes,
        /// decreasing access counts and invoking release functions if necessary.
        /// Increases the reference count of the returned value by 1, either by omitting to unreference it or by explicitly increasing it.
        /// The calls are generated in using the current builder.
        /// Exiting the current function does *not* close the scopes.
        /// </summary>
        /// <param name="returned">The value that is returned and expected to remain valid after exiting.</param>
        public void ExitFunction(IValue returned)
        {
            // To avoid increasing the reference count for the returned value and all contained items
            // followed by immediately decreasing it again, we check whether we can avoid that.
            // There are a couple of pitfalls to watch out for when doing this:
            // a) It is possible that the returned value is or contains items that are not going to be
            //    unreferenced here, e.g. when the returned value has been passed as argument to the
            //    the callable we are exiting here. If that's the case, then we need to first increase
            //    the reference count for the returned value before processing all unreference calls,
            //    since otherwise an item contained in the returned value might get deallocated.
            // b) Conversely, it is also possible that the returned value is unreferenced multiple times.
            //    we hence need to make sure that we only omit one of these calls.
            // c) The stack of pending calls contains more than just those related to modifying reference
            //    counts; we need to make sure that any omitted call indeed was to unreference the
            //    returned value rather than e.g. to release qubits.
            // d) We can't modify the pending calls; they may be used by other execution paths that
            //    don't return the same value.

            var returnWillBeUnreferenced = this.scopes.Any(scope => scope.WillBeUnreferenced(returned));
            if (!returnWillBeUnreferenced)
            {
                this.IncreaseReferenceCount(returned);
            }

            // We need to extract the scopes to iterate over since for loops to release array items
            // will create new scopes and hence modify the collection.
            var currentScopes = this.scopes.ToArray();
            var omittedUnreferences = new List<IValue>() { returned };
            Scope.ExecutePendingCalls(this, omittedUnreferences, currentScopes);
        }
    }
}
