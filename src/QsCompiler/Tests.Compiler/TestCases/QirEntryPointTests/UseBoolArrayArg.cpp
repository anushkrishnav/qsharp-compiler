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
extern "C" void UseBoolArrayArg( // NOLINT
    InteropArray * BoolArrayArg
);


const char InteropFalseAsChar = 0x0;
const char InteropTrueAsChar = 0x1;
map<string, bool> BoolAsCharMap{
    {"0", InteropFalseAsChar},
    {"false", InteropFalseAsChar},
    {"1", InteropTrueAsChar},
    {"true", InteropTrueAsChar}};

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
    

    vector<char> BoolArrayArg;
    app.add_option("--BoolArrayArg", BoolArrayArg, "A bool array value for the BoolArrayArg argument")->required()
        ->transform(CLI::CheckedTransformer(BoolAsCharMap, CLI::ignore_case));

    // With all the options added, parse arguments from the command line.
    CLI11_PARSE(app, argc, argv);

    // Create an interop array of bool values.
    unique_ptr<InteropArray> BoolArrayArgArray = CreateInteropArray(BoolArrayArg);
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
    UseBoolArrayArg(
        BoolArrayArgArray.get()
    );


    simulatorOutputStream->flush();
    if (simulationOutputFileStream.is_open())
    {
        simulationOutputFileStream.close();
    }
}
