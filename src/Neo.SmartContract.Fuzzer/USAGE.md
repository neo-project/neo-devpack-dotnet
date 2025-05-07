# Neo Smart Contract Fuzzer Usage Guide

This guide explains how to use the Neo Smart Contract Fuzzer to test Neo smart contracts by fuzzing their methods with randomly generated inputs.

## Overview

The Neo Smart Contract Fuzzer is a tool designed to help developers test Neo smart contracts by automatically generating random inputs for contract methods and executing them. This helps identify potential issues, vulnerabilities, or unexpected behaviors in the contract.

## Prerequisites

- .NET 9.0 SDK or later
- Neo N3 contract NEF and manifest files

## Installation

The Neo Smart Contract Fuzzer is part of the Neo DevPack .NET. You can build it from source:

```bash
git clone https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet
dotnet build src/Neo.SmartContract.Fuzzer
```

## Basic Usage

To fuzz a Neo smart contract, you need the contract's NEF file and manifest file. Run the fuzzer with the following command:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef <path-to-nef-file> --manifest <path-to-manifest-file>
```

For example:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json
```

## Command Line Options

The Neo Smart Contract Fuzzer supports the following command line options:

- `--nef <path>`: Path to the NEF file of the contract to fuzz (required)
- `--manifest <path>`: Path to the manifest file of the contract to fuzz (required)
- `--output <directory>`: Output directory for fuzzing results (default: FuzzingResults)
- `--iterations <number>`: Number of fuzzing iterations per method (default: 10)
- `--seed <number>`: Random seed for reproducible fuzzing
- `--help`: Show help message

## Advanced Options

```
--skip-parameterless       Skip methods with no parameters
--include-private          Include private methods in fuzzing
--timeout, -t <ms>         Execution timeout in milliseconds (default: 5000)
--include-method <name>    Include only specific method (can be used multiple times)
--exclude-method <name>    Exclude specific method (can be used multiple times)
```

## Code Coverage Options

```
--enable-coverage          Enable code coverage analysis (default)
--disable-coverage         Disable code coverage analysis
--coverage-format <format> Format for coverage report (html, json, text) (default: html)
--coverage-output <dir>    Output directory for coverage report (default: coverage-report)
```

## Output

The fuzzer generates the following outputs in the specified output directory:

- `Inputs/`: Directory containing the generated inputs for each method
- `Results/`: Directory containing the execution results for each method
- `<contract-name>_fuzzing_report.md`: A summary report of the fuzzing results

## Example Report

Here's an example of a fuzzing report:

```markdown
# Fuzzing Report for MyContract

Date: 2025-04-18 15:30:45

## Summary

- Contract name: MyContract
- Methods tested: 5
- Iterations per method: 10
- Total iterations: 50

## Method Details

| Method | Parameters | Return Type |
|--------|------------|-------------|
| transfer | from: Hash160, to: Hash160, amount: Integer | Boolean |
| balanceOf | account: Hash160 | Integer |
| decimals | | Integer |
| symbol | | String |
| totalSupply | | Integer |

## Test Results

- Successful executions: 45
- Failed executions: 5
- Success rate: 90%

## Issues Found

1. Method `transfer` failed with input:
   - from: 0x0000000000000000000000000000000000000001
   - to: 0x0000000000000000000000000000000000000002
   - amount: -100
   Error: "Amount must be positive"

2. Method `balanceOf` failed with input:
   - account: null
   Error: "Account cannot be null"
```

## Advanced Usage

### Customizing Parameter Generation

The fuzzer automatically generates random inputs for contract methods based on their parameter types. The default generation strategy covers most common Neo contract parameter types, including:

- Boolean
- Integer
- String
- ByteArray
- Hash160
- Hash256
- PublicKey
- Array
- Map

### Reproducing Issues

To reproduce a specific issue, you can use the `--seed` option to set the random seed to a specific value. This ensures that the same inputs are generated for each method:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --seed 12345
```

### Increasing Test Coverage

To increase test coverage, you can increase the number of iterations per method:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --iterations 100
```

## Configuration File

You can also use a JSON configuration file to specify fuzzing parameters:

```bash
neo-sc-fuzzer --config config.json
```

Example configuration file:

```json
{
  "NefPath": "path/to/contract.nef",
  "ManifestPath": "path/to/contract.manifest.json",
  "OutputDirectory": "FuzzingResults",
  "IterationsPerMethod": 10,
  "Seed": 12345,
  "GasLimit": 20000000,
  "Verbose": true,
  "SkipParameterlessMethods": false,
  "IncludePrivateMethods": true,
  "ExecutionTimeout": 5000,
  "IncludeMethods": ["transfer", "balanceOf"],
  "ExcludeMethods": ["_deploy"],
  "EnableCoverage": true,
  "CoverageFormat": "html",
  "CoverageOutput": "coverage-report"
}
```

## Troubleshooting

### Common Issues

1. **Contract not found**: Ensure that the paths to the NEF and manifest files are correct.

2. **Invalid contract**: The NEF or manifest file might be corrupted or not compatible with the current Neo version.

3. **Out of memory**: If you're fuzzing a complex contract with many methods or using a large number of iterations, you might run out of memory. Try reducing the number of iterations or using a more powerful machine.

4. **Execution timeout**: Some contract methods might take too long to execute. The fuzzer has a timeout mechanism to prevent infinite loops.

## Contributing

Contributions to the Neo Smart Contract Fuzzer are welcome! Please feel free to submit issues or pull requests to the [Neo DevPack .NET repository](https://github.com/neo-project/neo-devpack-dotnet).

## License

The Neo Smart Contract Fuzzer is licensed under the MIT License. See the LICENSE file for details.