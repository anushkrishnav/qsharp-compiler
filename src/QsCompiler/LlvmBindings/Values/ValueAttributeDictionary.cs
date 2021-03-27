﻿// -----------------------------------------------------------------------
// <copyright file="ValueAttributeDictionary.cs" company="Ubiquity.NET Contributors">
// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Portions Copyright (c) Microsoft Corporation
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ubiquity.NET.Llvm.Values
{
    /// <summary>re-usable implementation of IAttributeDictionary for containers that implement IAttributeAccessor.</summary>
    /// <remarks>
    /// This uses the low-level methods of IAttributeAccessor to abstract out the differences in the
    /// LLVM-C API for attributes on CallSites vs. Functions.
    /// </remarks>
    internal class ValueAttributeDictionary
        : IAttributeDictionary
    {
        private readonly Func<IrFunction> FunctionFetcher;
        private readonly IAttributeAccessor Container;

        internal ValueAttributeDictionary(IAttributeAccessor container, Func<IrFunction> functionFetcher)
        {
            this.Container = container;
            this.FunctionFetcher = functionFetcher;
        }

        public Context Context => this.Container.Context;

        public IEnumerable<FunctionAttributeIndex> Keys
            => new ReadOnlyCollection<FunctionAttributeIndex>(this.GetValidKeys().ToList());

        public IEnumerable<ICollection<AttributeValue>> Values
            => new ReadOnlyCollection<ICollection<AttributeValue>>(this.Select(kvp => kvp.Value).ToList());

        public int Count => this.GetValidKeys().Count();

        public ICollection<AttributeValue> this[FunctionAttributeIndex key]
        {
            get
            {
                if (!this.ContainsKey(key))
                {
                    throw new KeyNotFoundException();
                }

                return new ValueAttributeCollection(this.Container, key);
            }
        }

        public bool ContainsKey(FunctionAttributeIndex key) => this.GetValidKeys().Any(k => k == key);

        public IEnumerator<KeyValuePair<FunctionAttributeIndex, ICollection<AttributeValue>>> GetEnumerator()
        {
            return (from key in this.GetValidKeys()
                    select new KeyValuePair<FunctionAttributeIndex, ICollection<AttributeValue>>(key, this[key]))
                   .GetEnumerator();
        }

        public bool TryGetValue(FunctionAttributeIndex key, out ICollection<AttributeValue> value)
        {
            value = default;
            if (this.ContainsKey(key))
            {
                return false;
            }

            value = new ValueAttributeCollection(this.Container, key);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private IEnumerable<FunctionAttributeIndex> GetValidKeys()
        {
            var endIndex = FunctionAttributeIndex.Parameter0 + this.FunctionFetcher().Parameters.Count;
            for (var index = FunctionAttributeIndex.Function; index < endIndex; ++index)
            {
                if (this.Container.GetAttributeCountAtIndex(index) > 0)
                {
                    yield return index;
                }
            }
        }
    }
}
