# Neo Smart Contract Fuzzer Usage Guide

This document provides detailed instructions for using the Neo Smart Contract Fuzzer to test Neo smart contracts for vulnerabilities.

## Prerequisites

Before using the fuzzer, ensure you have the following:

- .NET 9.0 SDK or later
- Neo N3 contract NEF and manifest files
- Basic understanding of Neo smart contracts

## Installation

The Neo Smart Contract Fuzzer is part of the Neo DevPack. You can build it from source:

```bash
git clone https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet
dotnet build
```

## Basic Usage

To run the fuzzer on a Neo smart contract, use the following command:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef <path-to-nef-file> --manifest <path-to-manifest-file>
```

This will run the fuzzer with default settings, which includes:
- 100 iterations per method
- Random seed based on current time
- No static analysis
- No symbolic execution
- Results output to the `FuzzingResults` directory

## Configuration Options

The fuzzer supports the following configuration options:

### Input Files

- `--nef <path>`: Path to the NEF file of the contract to fuzz (required)
- `--manifest <path>`: Path to the manifest file of the contract to fuzz (required)
- `--source <path>`: Path to the source code directory (optional, enables more detailed static analysis)

### Output Options

- `--output <path>`: Directory where the fuzzer will store its output (default: `FuzzingResults`)
- `--report-formats <formats>`: Comma-separated list of report formats to generate (default: `Markdown,Json`)
  - Supported formats: `Markdown`, `Json`, `Html`, `Csv`

### Fuzzing Options

- `--iterations <number>`: Number of iterations to perform for each method (default: 100)
- `--seed <number>`: Seed for the random number generator (default: based on current time)
- `--gas-limit <number>`: Gas limit for contract execution (default: 10,000,000)
- `--methods <methods>`: Comma-separated list of methods to fuzz (default: all methods)
- `--exclude-methods <methods>`: Comma-separated list of methods to exclude from fuzzing

### Analysis Options

- `--enable-static-analysis`: Enable static analysis (default: false)
- `--enable-symbolic-execution`: Enable symbolic execution (default: false)
- `--symbolic-execution-depth <number>`: Maximum depth for symbolic execution (default: 100)
- `--symbolic-execution-max-paths <number>`: Maximum number of paths to explore in symbolic execution (default: 10)

### Advanced Options

- `--timeout <seconds>`: Timeout for the entire fuzzing process in seconds (default: 3600)
- `--method-timeout <seconds>`: Timeout for each method execution in seconds (default: 10)
- `--enable-coverage`: Enable code coverage analysis (default: false)
- `--coverage-format <format>`: Format for code coverage report (default: `Html`)
  - Supported formats: `Html`, `Xml`, `Json`, `Cobertura`

## Examples

### Basic Fuzzing

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json
```

### Fuzzing with Static Analysis

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --enable-static-analysis --source MyContractSource/
```

### Fuzzing with Symbolic Execution

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --enable-symbolic-execution --symbolic-execution-depth 50 --symbolic-execution-max-paths 20
```

### Comprehensive Analysis

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --enable-static-analysis --source MyContractSource/ --enable-symbolic-execution --symbolic-execution-depth 50 --symbolic-execution-max-paths 20 --iterations 200 --seed 42 --gas-limit 20000000 --report-formats Markdown,Json,Html --enable-coverage --coverage-format Html
```

### Fuzzing Specific Methods

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --methods Transfer,BalanceOf
```

### Excluding Methods

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --exclude-methods Initialize,Destroy
```

## Programmatic Usage

You can also use the fuzzer programmatically in your own code:

```csharp
using Neo.SmartContract.Fuzzer;
using Neo.SmartContract.Fuzzer.Controller;

// Create a fuzzer configuration
var config = new FuzzerConfiguration
{
    NefPath = "MyContract.nef",
    ManifestPath = "MyContract.manifest.json",
    OutputDirectory = "FuzzingResults",
    IterationsPerMethod = 100,
    Seed = 42,
    EnableStaticAnalysis = true,
    EnableSymbolicExecution = true,
    SymbolicExecutionDepth = 50,
    SymbolicExecutionPaths = 20,
    GasLimit = 10_000_000
};

// Create a fuzzing controller
var controller = new FuzzingController(config);

// Start the fuzzing process
controller.Start();

// Wait for the fuzzing to complete
await controller.WaitForCompletion();

// Get the fuzzing status
var status = controller.GetStatus();

// Print the results
Console.WriteLine($"Total Executions: {status.TotalExecutions}");
Console.WriteLine($"Successful Executions: {status.SuccessfulExecutions}");
Console.WriteLine($"Failed Executions: {status.FailedExecutions}");
Console.WriteLine($"Issues Found: {status.IssuesFound}");
Console.WriteLine($"Code Coverage: {status.CodeCoverage:P2}");
```

## Understanding the Results

After running the fuzzer, you'll find the results in the output directory (default: `FuzzingResults`). The results include:

### Summary Report

The summary report provides an overview of the fuzzing results, including:
- Contract information
- Fuzzing statistics
- Issues found
- Code coverage

### Issue Reports

For each issue found, the fuzzer generates a detailed report that includes:
- Issue type
- Severity
- Description
- Method where the issue was found
- Remediation suggestions

### Execution Traces

For each issue found, the fuzzer also generates an execution trace that shows the steps that led to the issue. This can help you understand and reproduce the issue.

### Code Coverage Report

If code coverage is enabled, the fuzzer generates a code coverage report that shows which parts of the contract were executed during fuzzing.

## Interpreting Vulnerability Reports

The fuzzer reports vulnerabilities with the following severity levels:

- **Critical**: Vulnerabilities that can lead to loss of funds or complete contract compromise.
- **High**: Vulnerabilities that can lead to significant contract malfunction or partial compromise.
- **Medium**: Vulnerabilities that can lead to minor contract malfunction or information disclosure.
- **Low**: Vulnerabilities that have minimal impact on the contract's security or functionality.

For each vulnerability, the report includes:
- A description of the vulnerability
- The method where the vulnerability was found
- Suggestions for fixing the vulnerability

## Best Practices

To get the most out of the Neo Smart Contract Fuzzer, follow these best practices:

1. **Start with Basic Fuzzing**: Begin with basic fuzzing to identify obvious issues.
2. **Enable Static Analysis**: Enable static analysis to find issues that may not be detected during execution.
3. **Enable Symbolic Execution**: Enable symbolic execution to find deeper issues that may be difficult to detect with traditional fuzzing.
4. **Use a Fixed Seed**: Use a fixed seed for reproducible results.
5. **Increase Iterations for Complex Contracts**: For complex contracts, increase the number of iterations to ensure thorough testing.
6. **Focus on Critical Methods**: Focus on methods that handle assets or sensitive operations.
7. **Review All Issues**: Review all reported issues, even those with low severity, as they may indicate deeper problems.
8. **Fix Issues Incrementally**: Fix issues one at a time and re-run the fuzzer to ensure the fix is effective.

## Troubleshooting

### Common Issues

- **Out of Memory**: If the fuzzer runs out of memory, try reducing the number of iterations or the symbolic execution depth.
- **Timeout**: If the fuzzer times out, try increasing the timeout or reducing the number of iterations.
- **No Issues Found**: If the fuzzer doesn't find any issues, try increasing the number of iterations or enabling symbolic execution.

### Error Messages

- **Failed to load contract**: Ensure the NEF and manifest files are valid and accessible.
- **Method not found**: Ensure the method name is correct and exists in the contract.
- **Execution failed**: Check the execution trace for details on why the execution failed.

## Conclusion

The Neo Smart Contract Fuzzer is a powerful tool for finding vulnerabilities in Neo smart contracts. By following this guide, you can effectively use the fuzzer to improve the security and reliability of your contracts.
