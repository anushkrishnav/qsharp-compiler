﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Quantum.QsCompiler.BondSchemas.EntryPoint;

namespace Microsoft.Quantum.QsCompiler.Templates
{
    internal class ArgumentCpp : Argument
    {
        public ArgumentCpp(Argument argument)
        {
            this.Name = argument.Name;
            this.Position = argument.Position;
            this.Type = argument.Type;
            this.ArrayType = argument.ArrayType;
            this.Values = argument.Values.ToList();
        }

        public string CppType()
        {
            return this.Type switch
            {
                DataType.BoolType => "char",
                DataType.IntegerType => "int64_t",
                DataType.DoubleType => "double",
                DataType.PauliType => "char",
                DataType.RangeType => "InteropRange*",
                DataType.ResultType => "char",
                DataType.StringType => "const char*",
                DataType.ArrayType => "InteropArray*",
                _ => throw new NotSupportedException($"Unsupported argument type {this.Type}")
            };
        }

        public string CliOptionString()
        {
            if (this.Name.Length == 1)
            {
                return $"-{this.Name}";
            }
            else
            {
                return $"--{this.Name}";
            }
        }

        public string CliTypeDescription()
        {
            return this.Type switch
            {
                DataType.BoolType => $"bool",
                DataType.IntegerType => $"integer",
                DataType.DoubleType => $"double",
                DataType.PauliType => $"Pauli",
                DataType.RangeType => $"Range (start, step, end)",
                DataType.ResultType => $"Result",
                DataType.StringType => $"String",
                DataType.ArrayType => this.ArrayType switch
                {
                    DataType.BoolType => $"bool array",
                    DataType.IntegerType => $"integer array",
                    DataType.DoubleType => $"double array",
                    DataType.PauliType => $"Pauli array",
                    DataType.RangeType => $"Range array",
                    DataType.ResultType => $"Result array",
                    DataType.StringType => $"String array",
                    _ => throw new NotSupportedException($"Unsupported array type {this.Type}")
                },
                _ => throw new NotSupportedException($"Unsupported argument type {this.Type}")
            };
        }

        public string CliDescription() => $"A {this.CliTypeDescription()} value for the {this.Name} argument";

        public string CppCliValueType()
        {
            return this.Type switch
            {
                DataType.BoolType => "char",
                DataType.IntegerType => "int64_t",
                DataType.DoubleType => "double_t",
                DataType.PauliType => "PauliId",
                DataType.RangeType => "RangeTuple",
                DataType.ResultType => "char",
                DataType.StringType => "string",
                DataType.ArrayType => this.ArrayType switch
                {
                    DataType.BoolType => "vector<char>",
                    DataType.IntegerType => "vector<int64_t>",
                    DataType.DoubleType => "vector<double_t>",
                    DataType.PauliType => "std::vector<PauliId>",
                    DataType.RangeType => "vector<RangeTuple>",
                    DataType.ResultType => "vector<char>",
                    DataType.StringType => "vector<string>",
                    _ => throw new NotSupportedException($"Unsupported array type {this.Type}")
                },
                _ => throw new NotSupportedException($"Unsupported argument type {this.Type}")
            };
        }

        public string? CppCliVariableInitialValue()
        {
            return this.Type switch
            {
                DataType.BoolType => "InteropFalseAsChar",
                DataType.IntegerType => "0",
                DataType.DoubleType => "0.0",
                DataType.PauliType => "PauliId::PauliId_I",
                DataType.RangeType => null,
                DataType.ResultType => "InteropResultZeroAsChar",
                DataType.StringType => null,
                DataType.ArrayType => this.ArrayType switch
                {
                    DataType.BoolType => null,
                    DataType.IntegerType => null,
                    DataType.DoubleType => null,
                    DataType.PauliType => null,
                    DataType.RangeType => null,
                    DataType.ResultType => null,
                    DataType.StringType => null,
                    _ => throw new NotSupportedException($"Unsupported array type {this.Type}")
                },
                _ => throw new NotSupportedException($"Unsupported argument type {this.Type}")
            };
        }

        public static string? DataTypeTransformerMapName(DataType what)
        {
            return what switch
            {
                DataType.BoolType => "BoolAsCharMap",
                DataType.IntegerType => null,
                DataType.DoubleType => null,
                DataType.PauliType => "PauliMap",
                DataType.RangeType => null,
                DataType.ResultType => "ResultAsCharMap",
                DataType.StringType => null,
                DataType.ArrayType => null,
                _ => throw new NotSupportedException($"Unsupported argument type {what}")
            };
        }

        public string? TransformerMapName()
        {
            if (this.Type == DataType.ArrayType)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                return DataTypeTransformerMapName((DataType)this.ArrayType);
#pragma warning restore CS8629 // Nullable value type may be null.
            }
            else
            {
                return DataTypeTransformerMapName(this.Type);
            }
        }

        public string CliValueVariableName()
        {
            return "v" + this.Name + "CliValue";
        }

        public string InteropVariableName()
        {
            return "v" + this.Name + "InteropValue";
        }

        public string IntermediateVariableName()
        {
            return "v" + this.Name + "IntermediateValue";
        }
    }

    internal class EntryPointOperationCpp : EntryPointOperation
    {
        public List<ArgumentCpp> InteropArguments;

        public EntryPointOperationCpp(EntryPointOperation entryPointOperation)
        {
            this.Name = entryPointOperation.Name;
            this.Arguments = entryPointOperation.Arguments.ToList();
            this.Arguments.Sort((a, b) => a.Position.CompareTo(b.Position));
            this.InteropArguments = this.Arguments.Select(arg => new ArgumentCpp(arg)).ToList();
        }

        public bool ContainsArgumentType(DataType type)
        {
            foreach (Argument arg in this.Arguments)
            {
                if (arg.Type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsArrayType(DataType type)
        {
            foreach (Argument arg in this.Arguments)
            {
                if (arg.ArrayType == type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
