﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Microsoft.Quantum.QsCompiler.Templates
{
    using Microsoft.Quantum.QsCompiler.BondSchemas.EntryPoint;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class QirDriverCpp : QirDriverCppBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"//------------------------------------------------------------------------------
// This code was generated by a tool.
// <auto-generated />
//------------------------------------------------------------------------------

#include <fstream>
#include <iostream>
#include <map>
#include <memory>
#include <vector>

#include ""CLI11.hpp""

#include ""QirContext.hpp""
#include ""QirRuntime.hpp""
#include ""SimFactory.hpp""

using namespace Microsoft::Quantum;
using namespace std;
");
 if (entryPointOperation.ContainsArgumentType(DataType.ArrayType)) { 
            this.Write(@"
// Auxiliary functions for interop with Q# Array type.
struct InteropArray
{
    int64_t Size;
    void* Data;

    InteropArray(int64_t size, void* data) :
        Size(size),
        Data(data){}
};

template<typename T>
unique_ptr<InteropArray> CreateInteropArray(vector<T>& v)
{
    unique_ptr<InteropArray> array(new InteropArray(v.size(), v.data()));
    return array;
}

template<typename S, typename D>
void TranslateVector(vector<S>& sourceVector, vector<D>& destinationVector, function<D(S&)> translationFunction)
{
    destinationVector.resize(sourceVector.size());
    transform(sourceVector.begin(), sourceVector.end(), destinationVector.begin(), translationFunction);
}
");
 } 
 if (entryPointOperation.ContainsArgumentType(DataType.RangeType) || entryPointOperation.ContainsArrayType(DataType.RangeType)) { 
            this.Write(@"
// Auxiliary functions for interop with Q# Range type.
using RangeTuple = tuple<int64_t, int64_t, int64_t>;
struct InteropRange
{
    int64_t Start;
    int64_t Step;
    int64_t End;

    InteropRange() :
        Start(0),
        Step(0),
        End(0){}

    InteropRange(RangeTuple rangeTuple) :
        Start(get<0>(rangeTuple)),
        Step(get<1>(rangeTuple)),
        End(get<2>(rangeTuple)){}
};

unique_ptr<InteropRange> CreateInteropRange(RangeTuple rangeTuple)
{
    unique_ptr<InteropRange> range(new InteropRange(rangeTuple));
    return range;
}

InteropRange* TranslateRangeTupleToInteropRangePointer(RangeTuple& rangeTuple)
{
    InteropRange* range = new InteropRange(rangeTuple);
    return range;
}
");
 } 
 if (entryPointOperation.ContainsArrayType(DataType.RangeType)) { 
            this.Write("\r\n// Auxiliary functions for interop with Q# Range[] type\r\ntemplate<typename T>\r\n" +
                    "void FreePointerVector(vector<T*>& v)\r\n{\r\n    for (auto p : v)\r\n    {\r\n        d" +
                    "elete p;\r\n    }\r\n}\r\n");
 } 
 if (entryPointOperation.ContainsArgumentType(DataType.BoolType) || entryPointOperation.ContainsArrayType(DataType.BoolType)) { 
            this.Write("\r\n// Auxiliary functions for interop with Q# Bool type.\r\nconst char InteropFalseA" +
                    "sChar = 0x0;\r\nconst char InteropTrueAsChar = 0x1;\r\nmap<string, bool> ");
            this.Write(this.ToStringHelper.ToStringWithCulture(ArgumentCpp.DataTypeTransformerMapName(DataType.BoolType)));
            this.Write("{\r\n    {\"0\", InteropFalseAsChar},\r\n    {\"false\", InteropFalseAsChar},\r\n    {\"1\", " +
                    "InteropTrueAsChar},\r\n    {\"true\", InteropTrueAsChar}\r\n};\r\n");
 } 
 if (entryPointOperation.ContainsArgumentType(DataType.PauliType) || entryPointOperation.ContainsArrayType(DataType.PauliType)) { 
            this.Write("\r\n// Auxiliary functions for interop with Q# Pauli type.\r\nmap<string, PauliId> ");
            this.Write(this.ToStringHelper.ToStringWithCulture(ArgumentCpp.DataTypeTransformerMapName(DataType.PauliType)));
            this.Write("{\r\n    {\"PauliI\", PauliId::PauliId_I},\r\n    {\"PauliX\", PauliId::PauliId_X},\r\n    " +
                    "{\"PauliY\", PauliId::PauliId_Y},\r\n    {\"PauliZ\", PauliId::PauliId_Z}\r\n};\r\n\r\nchar " +
                    "TranslatePauliToChar(PauliId& pauli)\r\n{\r\n    return static_cast<char>(pauli);\r\n}" +
                    "\r\n");
 } 
 if (entryPointOperation.ContainsArgumentType(DataType.ResultType) || entryPointOperation.ContainsArrayType(DataType.ResultType)) { 
            this.Write("\r\n// Auxiliary functions for interop with Q# Result type.\r\nconst char InteropResu" +
                    "ltZeroAsChar = 0x0;\r\nconst char InteropResultOneAsChar = 0x1;\r\nmap<string, char>" +
                    " ");
            this.Write(this.ToStringHelper.ToStringWithCulture(ArgumentCpp.DataTypeTransformerMapName(DataType.ResultType)));
            this.Write("{\r\n    {\"0\", InteropResultZeroAsChar},\r\n    {\"Zero\", InteropResultZeroAsChar},\r\n " +
                    "   {\"1\", InteropResultOneAsChar},\r\n    {\"One\", InteropResultOneAsChar}\r\n};\r\n");
 } 
 if (entryPointOperation.ContainsArgumentType(DataType.StringType) || entryPointOperation.ContainsArrayType(DataType.StringType)) { 
            this.Write("\r\n// Auxiliary functions for interop with Q# String type.\r\nconst char* TranslateS" +
                    "tringToCharBuffer(string& s)\r\n{\r\n    return s.c_str();\r\n}\r\n");
 } 
            this.Write("\r\nextern \"C\" void ");
            this.Write(this.ToStringHelper.ToStringWithCulture(entryPointOperation.Name));
            this.Write("(\r\n");
 for (int i = 0; i < entryPointOperation.InteropArguments.Count; i++) {
    var arg = entryPointOperation.InteropArguments[i];
    var isLastArg = i == (entryPointOperation.InteropArguments.Count-1); 
            this.Write("    ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CppType()));
            this.Write(" ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.Name));
            this.Write(this.ToStringHelper.ToStringWithCulture((isLastArg) ? "" : ","));
            this.Write("\r\n");
 } 
            this.Write(@"); // QIR interop function.

int main(int argc, char* argv[])
{
    CLI::App app(""QIR Standalone Entry Point"");

    // Initialize simulator.
    unique_ptr<IRuntimeDriver> sim = CreateFullstateSimulator();
    QirContextScope qirctx(sim.get(), false /*trackAllocatedObjects*/);

    // Add the --simulation-output option.
    string simulationOutputFile;
    CLI::Option* simulationOutputFileOpt = app.add_option(
        ""--simulation-output"", simulationOutputFile,
        ""File where the output produced during the simulation is written"");

");
 foreach (var arg in entryPointOperation.InteropArguments) { 
            this.Write("    ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CppCliValueType()));
            this.Write(" ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CliValueVariableName()));
            this.Write(";\r\n");
 if (arg.CppCliVariableInitialValue() != null) { 
            this.Write("    ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CliValueVariableName()));
            this.Write(" = ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CppCliVariableInitialValue()));
            this.Write(";\r\n");
 } 
            this.Write("    app.add_option(\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CliOptionString()));
            this.Write("\", ");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CliValueVariableName()));
            this.Write(", \"");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.CliDescription()));
            this.Write("\")\r\n        ->required()");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.TransformerMapName() != null ? "" : ";"));
            this.Write("\r\n");
 if (arg.TransformerMapName() != null) { 
            this.Write("        ->transform(CLI::CheckedTransformer(");
            this.Write(this.ToStringHelper.ToStringWithCulture(arg.TransformerMapName()));
            this.Write(", CLI::ignore_case));\r\n");
 } 
            this.Write("\r\n");
 } 
            this.Write("\r\n    // With all the options added, parse arguments from the command line.\r\n    " +
                    "CLI11_PARSE(app, argc, argv);\r\n\r\n");
 foreach (var arg in entryPointOperation.InteropArguments) {
    switch (arg.Type) {
        case DataType.PauliType:
            WriteLine("    // Translate a PauliID value to its char representation.");
            WriteLine($"    char {arg.InteropVariableName()} = TranslatePauliToChar({arg.CliValueVariableName()});");
            break;
        case DataType.RangeType:
            WriteLine("    // Create an interop range.");
            WriteLine($"    unique_ptr<InteropRange> {arg.InteropVariableName()} = CreateInteropRange({arg.CliValueVariableName()});");
            break;
        case DataType.ArrayType:
            switch (arg.ArrayType) {
                case DataType.IntegerType:
                case DataType.DoubleType:
                case DataType.BoolType:
                case DataType.ResultType:
                    WriteLine("    // Translate values to its final form after parsing.");
                    WriteLine("    // Create an interop array of values.");
                    WriteLine($"    unique_ptr<InteropArray> {arg.InteropVariableName()} = CreateInteropArray({arg.CliValueVariableName()});");
                    break;
                case DataType.PauliType:
                    WriteLine("    // Create an interop array of Pauli values represented as chars.");
                    WriteLine($"    vector<char> {arg.IntermediateVariableName()};");
                    WriteLine($"    TranslateVector<PauliId, char>({arg.CliValueVariableName()}, {arg.IntermediateVariableName()}, TranslatePauliToChar);");
                    WriteLine($"    unique_ptr<InteropArray> {arg.InteropVariableName()} = CreateInteropArray({arg.IntermediateVariableName()});");
                    break;
                case DataType.RangeType:
                    WriteLine($"    vector<InteropRange*> {arg.IntermediateVariableName()};");
                    WriteLine($"    TranslateVector<RangeTuple, InteropRange*>({arg.CliValueVariableName()}, {arg.IntermediateVariableName()}, TranslateRangeTupleToInteropRangePointer);");
                    WriteLine($"    unique_ptr<InteropArray> {arg.InteropVariableName()} = CreateInteropArray({arg.IntermediateVariableName()});");
                    break;
                case DataType.StringType:
                    WriteLine("    // Create an interop array of String values.");
                    WriteLine($"    vector<const char *> {arg.IntermediateVariableName()};");
                    WriteLine($"    TranslateVector<string, const char*>({arg.CliValueVariableName()}, {arg.IntermediateVariableName()}, TranslateStringToCharBuffer);");
                    WriteLine($"    unique_ptr<InteropArray> {arg.InteropVariableName()} = CreateInteropArray({arg.IntermediateVariableName()});");
                    break;
            }
            break;
        default:
            WriteLine($"    {arg.CppCliValueType()} {arg.InteropVariableName()} = {arg.CliValueVariableName()};");
            break;
    }
} 
            this.Write(@"    // Redirect the simulator output from std::cout if the --simulation-output option is present.
    ostream* simulatorOutputStream = &cout;
    ofstream simulationOutputFileStream;
    if (!simulationOutputFileOpt->empty())
    {
        simulationOutputFileStream.open(simulationOutputFile);
        SetOutputStream(simulationOutputFileStream);
        simulatorOutputStream = &simulationOutputFileStream;
    }

    // Run simulation and write the output of the operation to the corresponding stream.
    ");
            this.Write(this.ToStringHelper.ToStringWithCulture(entryPointOperation.Name));
            this.Write("(\r\n");
 for (int i = 0; i < entryPointOperation.InteropArguments.Count; i++) {
    var arg = entryPointOperation.InteropArguments[i];
    switch (arg.Type) {
        case DataType.StringType:
            Write($"        {arg.InteropVariableName()}.c_str()");
            break;
        case DataType.ArrayType:
            Write($"        {arg.InteropVariableName()}.get()");
            break;
        default:
            Write($"        {arg.InteropVariableName()}");
            break;
    }
    if (i < entryPointOperation.InteropArguments.Count-1 ) {
        WriteLine(",");
    }
} 
            this.Write("\r\n    );\r\n\r\n");
 foreach (var arg in entryPointOperation.InteropArguments) {
    if (arg.Type == DataType.ArrayType && arg.Type == DataType.RangeType) {
        Write($"    FreePointerVector({arg.IntermediateVariableName()});");
    }
} 
            this.Write("\r\n    simulatorOutputStream->flush();\r\n    if (simulationOutputFileStream.is_open" +
                    "())\r\n    {\r\n        simulationOutputFileStream.close();\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class QirDriverCppBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
