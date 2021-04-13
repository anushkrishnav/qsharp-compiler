//------------------------------------------------------------------------------
// This code was generated by a tool.
// <auto-generated />
//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#include <fstream>
#include <iostream>
#include <map>
#include <memory>
#include <vector>

#include "CLI11.hpp"

#include "QirContext.hpp"
#include "QirRuntime.hpp"
#include "SimFactory.hpp"

using namespace Microsoft::Quantum;
using namespace std;
    

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

// This is the function corresponding to the QIR entry-point.
extern "C" void UseMiscArgs( // NOLINT
    char IntegerArgInteropValue,
    char PauliArgInteropValue,
    InteropArray * ResultArrayArgInteropValue
);


const char InteropFalseAsChar = 0x0;
const char InteropTrueAsChar = 0x1;
map<string, bool> BoolAsCharMap{
    {"0", InteropFalseAsChar},
    {"false", InteropFalseAsChar},
    {"1", InteropTrueAsChar},
    {"true", InteropTrueAsChar}};

map<string, PauliId> PauliMap{
    {"PauliI", PauliId::PauliId_I},
    {"PauliX", PauliId::PauliId_X},
    {"PauliY", PauliId::PauliId_Y},
    {"PauliZ", PauliId::PauliId_Z}};


char TranslatePauliToChar(PauliId& pauli)
{
    return static_cast<char>(pauli);
}

const char InteropResultZeroAsChar = 0x0;
const char InteropResultOneAsChar = 0x1;
map<string, char> ResultAsCharMap{
    {"0", InteropResultZeroAsChar},
    {"Zero", InteropResultZeroAsChar},
    {"1", InteropResultOneAsChar},
    {"One", InteropResultOneAsChar}
};

int main(int argc, char* argv[])
{
    CLI::App app("QIR Standalone Entry Point Inputs Reference");

    // Initialize simulator.
    unique_ptr<IRuntimeDriver> sim = CreateFullstateSimulator();
    QirContextScope qirctx(sim.get(), false /*trackAllocatedObjects*/);

    // Add the --simulation-output options.
    // N.B. This option should be present in all standalone drivers.
    string simulationOutputFile;
    CLI::Option* simulationOutputFileOpt = app.add_option(
        "--simulation-output", simulationOutputFile,
        "File where the output produced during the simulation is written");

    char IntegerArgCliValue;
    IntegerArgCliValue = InteropFalseAsChar;
    app.add_option("--IntegerArg", IntegerArgCliValue, "A bool value for the IntegerArg argument")->required()
        ->transform(CLI::CheckedTransformer(BoolAsCharMap, CLI::ignore_case));

    PauliId PauliArgCliValue;
    PauliArgCliValue = PauliId::PauliId_I;
    app.add_option("--PauliArg", PauliArgCliValue, "A Pauli value for the PauliArg argument")->required()
        ->transform(CLI::CheckedTransformer(PauliMap, CLI::ignore_case));

    vector<char> ResultArrayArgCliValue;
    app.add_option("--ResultArrayArg", ResultArrayArgCliValue, "A Result array value for the ResultArrayArg argument")->required()
        ->transform(CLI::CheckedTransformer(ResultAsCharMap, CLI::ignore_case));

    // With all the options added, parse arguments from the command line.
    CLI11_PARSE(app, argc, argv);

    char IntegerArgInteropValue = IntegerArgCliValue;
    // Translate a PauliID value to its char representation.
    char PauliArgInteropValue = TranslatePauliToChar(PauliArgCliValue);
    // Translate values to its final form after parsing.
    // Create an interop array of values.
    unique_ptr<InteropArray> ResultArrayArgInteropValue = CreateInteropArray(ResultArrayArgCliValue);
    // Redirect the simulator output from std::cout if the --simulation-output option is present.
    ostream* simulatorOutputStream = &cout;
    ofstream simulationOutputFileStream;
    if (!simulationOutputFileOpt->empty())
    {
        simulationOutputFileStream.open(simulationOutputFile);
        SetOutputStream(simulationOutputFileStream);
        simulatorOutputStream = &simulationOutputFileStream;
    }

    // Run simulation and write the output of the operation to the corresponding stream.
    UseMiscArgs(
        IntegerArgInteropValue,
        PauliArgInteropValue,
        ResultArrayArgInteropValue.get()
    );


    simulatorOutputStream->flush();
    if (simulationOutputFileStream.is_open())
    {
        simulationOutputFileStream.close();
    }
}
