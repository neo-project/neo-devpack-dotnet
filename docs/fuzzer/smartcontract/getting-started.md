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

## Basic Usage

The fuzzer can be run directly from the command line:

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
- `--coverage-format`: Coverage report format (default: "coz")
- `--methods`: Comma-separated list of methods to include in fuzzing (default: all public methods)
- `--exclude`: Comma-separated list of methods to exclude from fuzzing
- `--config`: Path to a JSON configuration file

## Using a Configuration File

For more complex configurations, you can use a JSON configuration file:

```json
{
  "nefPath": "path/to/contract.nef",
  "manifestPath": "path/to/contract.manifest.json",
  "outputDirectory": "fuzzer-results",
  "iterationsPerMethod": 1000,
  "gasLimit": 20000000,
  "seed": 42,
  "enableCoverage": true,
  "coverageFormat": "html",
  "persistStateBetweenCalls": false,
  "saveFailingInputsOnly": true,
  "methodsToInclude": ["transfer", "balanceOf"],
  "methodsToExclude": ["destroy"]
}
```

Then run the fuzzer with:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --config path/to/config.json
```

## Understanding the Output

After running the fuzzer, you'll find the following in your output directory:

- **Method directories**: One directory for each method that was fuzzed
  - **Result files**: JSON files containing the inputs, outputs, and execution details for each iteration
- **Coverage report**: If coverage was enabled, a report showing which parts of the contract were executed
- **Vulnerabilities directory**: Contains reports of any potential vulnerabilities found

## Replaying a Specific Input

If you find an interesting or problematic input, you can replay it:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --replay path/to/fuzzer-results/methodName/result_123.json
```

## Next Steps

- Read the [Technical Overview](./technical-overview.md) to understand how the fuzzer works
- Learn about [Configuration Options](./configuration-guide.md) for more advanced usage
- Explore [Vulnerability Detection](./vulnerability-detection.md) to understand how the fuzzer identifies issues
