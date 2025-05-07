# Neo Smart Contract Fuzzer: Configuration Guide

This document provides detailed information about the configuration options available for the Neo Smart Contract Fuzzer.

## Configuration Methods

The fuzzer can be configured in three ways:

1. **Command-line arguments**: Passed directly when running the fuzzer
2. **Configuration file**: A JSON file containing configuration options
3. **Programmatic configuration**: When using the fuzzer as a library

## Command-line Arguments

```bash
# Using the convenience script
run-fuzzer.bat --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --iterations 1000 \
  --gas-limit 20000000 \
  --seed 42 \
  --coverage \
  --coverage-format html \
  --methods transfer,balanceOf \
  --exclude destroy \
  --feedback \
  --static-analysis \
  --symbolic \
  --engine neo-express

# Or directly with dotnet
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --iterations 1000 \
  --gas-limit 20000000 \
  --seed 42 \
  --coverage \
  --coverage-format html \
  --methods transfer,balanceOf \
  --exclude destroy \
  --feedback \
  --static-analysis \
  --symbolic \
  --engine neo-express
```

## Configuration File

You can create a JSON configuration file with all the options:

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
  "EnableSymbolicExecution": true,
  "SymbolicExecutionDepth": 100,
  "SymbolicExecutionPaths": 1000,
  "EnableTestCaseMinimization": true,
  "ExecutionEngine": "neo-express",
  "RpcUrl": "http://localhost:10332",
  "ReportFormats": ["json", "html", "markdown"]
}
```

Then run the fuzzer with:

```bash
# Using the convenience script
run-fuzzer.bat --config path/to/config.json

# Or directly with dotnet
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --config path/to/config.json
```

## Configuration Options

### Basic Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| NEF Path | `--nef` | `NefPath` | - | Path to the NEF file of your smart contract |
| Manifest Path | `--manifest` | `ManifestPath` | - | Path to the manifest file of your smart contract |
| Output Directory | `--output` | `OutputDirectory` | `fuzzer-output` | Directory to save execution results |
| Iterations | `--iterations` | `IterationsPerMethod` | 1000 | Number of fuzzing iterations per method |
| Gas Limit | `--gas-limit` | `GasLimit` | 20000000 | Gas limit per execution (20 GAS) |
| Random Seed | `--seed` | `Seed` | Current timestamp | Random seed for reproducibility |

### Method Selection

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Methods to Include | `--methods` | `MethodsToFuzz` | All public methods | Comma-separated list of methods to include in fuzzing |
| Methods to Exclude | `--exclude` | `MethodsToExclude` | None | Comma-separated list of methods to exclude from fuzzing |

### Coverage Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Coverage | `--coverage` | `EnableCoverage` | `false` | Enable code coverage measurement |
| Coverage Format | `--coverage-format` | `CoverageFormat` | `html` | Coverage report format (`html`, `json`, `text`) |

### Execution Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Execution Engine | `--engine` | `ExecutionEngine` | `neo-express` | Execution engine to use (`neo-express`, `rpc`, `in-memory`) |
| RPC URL | `--rpc-url` | `RpcUrl` | `http://localhost:10332` | RPC URL for RPC execution engine |
| Persist State | `--persist-state` | `PersistStateBetweenCalls` | `false` | Persist contract storage state between method calls |
| Save Failing Only | `--save-failing-only` | `SaveFailingInputsOnly` | `false` | Only save inputs that cause exceptions |
| Replay Input | `--replay` | - | - | Replay a specific input from a result file |

### Fuzzing Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Feedback | `--feedback` | `EnableFeedbackGuidedFuzzing` | `true` | Enable feedback-guided fuzzing |
| Max Corpus Size | `--max-corpus-size` | `MaxCorpusSize` | 1000 | Maximum number of test cases in corpus |
| Enable Test Case Minimization | `--minimize` | `EnableTestCaseMinimization` | `true` | Enable test case minimization |

### Static Analysis Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Static Analysis | `--static-analysis` | `EnableStaticAnalysis` | `true` | Enable static analysis |
| Source Path | `--source` | `SourcePath` | - | Path to the source code file (optional) |

### Symbolic Execution Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Symbolic Execution | `--symbolic` | `EnableSymbolicExecution` | `false` | Enable symbolic execution |
| Symbolic Depth | `--symbolic-depth` | `SymbolicExecutionDepth` | 100 | Maximum depth for symbolic execution |
| Symbolic Paths | `--symbolic-paths` | `SymbolicExecutionPaths` | 1000 | Maximum number of paths to explore |

### Reporting Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Report Format | `--report-format` | `ReportFormats` | `json` | Report format(s) (`json`, `html`, `markdown`, `text`) |

## Advanced Configuration

### Custom Parameter Generators

You can configure custom parameter generators for specific parameter types in your configuration file:

```json
{
  "ParameterGenerators": {
    "UInt160": {
      "Type": "predefined",
      "Values": [
        "0x0000000000000000000000000000000000000001",
        "0x0000000000000000000000000000000000000002"
      ]
    },
    "String": {
      "Type": "regex",
      "Pattern": "[a-zA-Z0-9]{5,10}"
    },
    "Integer": {
      "Type": "range",
      "Min": 0,
      "Max": 1000
    }
  }
}
```

### Custom Constraint Solver

For symbolic execution, you can configure a custom constraint solver:

```json
{
  "ConstraintSolver": {
    "Type": "z3",
    "Timeout": 5000,
    "MemoryLimit": 1024
  }
}
```

### Custom Vulnerability Detectors

You can enable or disable specific vulnerability detectors:

```json
{
  "VulnerabilityDetectors": [
    {
      "Name": "CrashDetector",
      "Enabled": true
    },
    {
      "Name": "GasConsumptionDetector",
      "Enabled": true,
      "Threshold": 10000000
    },
    {
      "Name": "StorageVulnerabilityDetector",
      "Enabled": true
    },
    {
      "Name": "IntegerVulnerabilityDetector",
      "Enabled": true
    }
  ]
}
```

### Logging Configuration

You can configure the logging level and output:

```json
{
  "Logging": {
    "LogLevel": "Info",
    "LogFile": "fuzzer.log",
    "Verbose": false
  }
}
```

Alternatively, you can set the `NEO_FUZZER_VERBOSE` environment variable to enable verbose logging:

```bash
# Windows
set NEO_FUZZER_VERBOSE=1

# Linux/macOS
export NEO_FUZZER_VERBOSE=1
```

## Environment Variables

The fuzzer respects the following environment variables:

- `NEO_FUZZER_CONFIG`: Path to a configuration file
- `NEO_FUZZER_OUTPUT`: Directory to save execution results
- `NEO_FUZZER_GAS_LIMIT`: Gas limit per execution
- `NEO_FUZZER_SEED`: Random seed for reproducibility
- `NEO_FUZZER_ITERATIONS`: Number of fuzzing iterations per method
- `NEO_FUZZER_VERBOSE`: Enable verbose logging (set to "1" or "true")

## Configuration Precedence

Configuration options are applied in the following order of precedence (highest to lowest):

1. Command-line arguments
2. Configuration file specified with `--config`
3. Environment variables
4. Default values

This means that command-line arguments will override options specified in the configuration file, which will override environment variables, which will override default values.

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
    OutputDirectory = "FuzzerResults",
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
await controller.WaitForCompletionAsync();

// Get the fuzzing status
var status = controller.GetStatus();

// Print the results
Console.WriteLine($"Total Executions: {status.TotalExecutions}");
Console.WriteLine($"Successful Executions: {status.SuccessfulExecutions}");
Console.WriteLine($"Failed Executions: {status.FailedExecutions}");
Console.WriteLine($"Issues Found: {status.IssuesFound}");
Console.WriteLine($"Code Coverage: {status.CodeCoverage:P2}");
```
