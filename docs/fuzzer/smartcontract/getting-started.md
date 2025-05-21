# Getting Started with Neo Smart Contract Fuzzer

This guide will help you get started with the Neo Smart Contract Fuzzer, from installation to running your first fuzzing session.

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download) or later
- A Neo N3 smart contract (NEF file and manifest)
- Basic understanding of Neo smart contracts

## Installation

The Neo Smart Contract Fuzzer is part of the Neo DevPack .NET repository. To get started:

1. Clone the repository:
   ```bash
   git clone https://github.com/neo-project/neo-devpack-dotnet.git
   cd neo-devpack-dotnet
   ```

2. Build the project:
   ```bash
   dotnet build src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj
   ```

3. Alternatively, you can use the provided convenience scripts:
   - Windows: `run-fuzzer.bat`
   - Linux/macOS: `run-fuzzer.sh`

## Basic Usage

The fuzzer can be run using the convenience scripts:

```bash
# Windows
run-fuzzer.bat --nef path/to/contract.nef --manifest path/to/contract.manifest.json

# Linux/macOS
./run-fuzzer.sh --nef path/to/contract.nef --manifest path/to/contract.manifest.json
```

Or directly from the command line:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --iterations 1000
```

### Required Parameters

- `--nef`: Path to the NEF file of your smart contract
- `--manifest`: Path to the manifest file of your smart contract

### Optional Parameters

- `--output`: Directory to save execution results (default: "fuzzer-output")
- `--iterations`: Number of fuzzing iterations per method (default: 1000)
- `--gas-limit`: Gas limit per execution (default: 20 GAS)
- `--seed`: Random seed for reproducibility (default: current timestamp)
- `--coverage`: Enable code coverage measurement (default: false)
- `--coverage-format`: Coverage report format (html, json, text) (default: "html")
- `--methods`: Comma-separated list of methods to include in fuzzing (default: all public methods)
- `--exclude`: Comma-separated list of methods to exclude from fuzzing
- `--config`: Path to a JSON configuration file
- `--feedback`: Enable feedback-guided fuzzing (default: true)
- `--static-analysis`: Enable static analysis (default: true)
- `--symbolic`: Enable symbolic execution (default: false)
- `--engine`: Execution engine to use (neo-express, rpc, in-memory) (default: neo-express)

For a complete list of options, run:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj -- --help
```

## Using a Configuration File

For more complex configurations, you can use a JSON configuration file:

```json
{
  "NefPath": "path/to/contract.nef",
  "ManifestPath": "path/to/contract.manifest.json",
  "OutputDirectory": "fuzzer-results",
  "IterationsPerMethod": 1000,
  "GasLimit": 20000000,
  "Seed": 42,
  "EnableCoverage": true,
  "CoverageFormat": "html",
  "PersistStateBetweenCalls": false,
  "SaveFailingInputsOnly": true,
  "MethodsToFuzz": ["transfer", "balanceOf"],
  "MethodsToExclude": ["destroy"],
  "EnableFeedbackGuidedFuzzing": true,
  "EnableStaticAnalysis": true,
  "EnableSymbolicExecution": false,
  "SymbolicExecutionDepth": 100,
  "SymbolicExecutionPaths": 1000,
  "EnableTestCaseMinimization": true,
  "ExecutionEngine": "neo-express",
  "ReportFormats": ["json", "html"]
}
```

Then run the fuzzer with:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --config path/to/config.json
```

## Understanding the Output

After running the fuzzer, you'll find the following in your output directory:

```
fuzzer-results/
├── SampleContract/
│   ├── issues/
│   │   ├── issue_1.json    # Detailed issue report
│   │   └── ...
│   └── report.json         # Summary report
└── README.md               # Explanation of results
```

The output includes:

- **Contract information**: Name, hash, and other metadata
- **Fuzzing statistics**: Total executions, successful/failed executions, issues found
- **Issues found**: Detailed reports of vulnerabilities and bugs
- **Coverage information**: Methods covered, code paths explored
- **Performance metrics**: Gas consumption, execution time

## Replaying a Specific Input

If you find an interesting or problematic input, you can replay it:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --replay path/to/fuzzer-results/SampleContract/issues/issue_1.json
```

This will execute the contract with the exact inputs that triggered the issue, allowing you to debug and fix the problem.

## Verbose Logging

To enable verbose logging for debugging purposes, set the `NEO_FUZZER_VERBOSE` environment variable:

```bash
# Windows
set NEO_FUZZER_VERBOSE=1

# Linux/macOS
export NEO_FUZZER_VERBOSE=1
```

## Next Steps

- Read the [Technical Overview](./technical-overview.md) to understand how the fuzzer works
- Learn about [Configuration Options](./configuration-guide.md) for more advanced usage
- Explore [Vulnerability Detection](./vulnerability-detection.md) to understand how the fuzzer identifies issues
- Dive into [Symbolic Execution](./symbolic-execution.md) to learn about advanced path exploration
- Check out [Advanced Usage](./advanced-usage.md) for more sophisticated fuzzing techniques
