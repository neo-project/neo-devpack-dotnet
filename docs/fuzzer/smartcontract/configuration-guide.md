# Neo Smart Contract Fuzzer: Configuration Guide

This document provides detailed information about the configuration options available for the Neo Smart Contract Fuzzer.

## Configuration Methods

The fuzzer can be configured in three ways:

1. **Command-line arguments**: Passed directly when running the fuzzer
2. **Configuration file**: A JSON file containing configuration options
3. **Programmatic configuration**: When using the fuzzer as a library

## Command-line Arguments

```bash
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
  --persist-state \
  --save-failing-only
```

## Configuration File

You can create a JSON configuration file with all the options:

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
  "methodsToExclude": ["destroy"],
  "symbolicExecution": {
    "enabled": true,
    "maxDepth": 100,
    "maxStates": 1000,
    "timeout": 60
  },
  "vulnerabilityDetection": {
    "enabled": true,
    "detectors": ["integer-overflow", "reentrancy", "unauthorized-access"]
  }
}
```

Then run the fuzzer with:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --config path/to/config.json
```

## Configuration Options

### Basic Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| NEF Path | `--nef` | `nefPath` | - | Path to the NEF file of your smart contract |
| Manifest Path | `--manifest` | `manifestPath` | - | Path to the manifest file of your smart contract |
| Output Directory | `--output` | `outputDirectory` | `fuzzer-output` | Directory to save execution results |
| Iterations | `--iterations` | `iterationsPerMethod` | 1000 | Number of fuzzing iterations per method |
| Gas Limit | `--gas-limit` | `gasLimit` | 20000000 | Gas limit per execution (20 GAS) |
| Random Seed | `--seed` | `seed` | Current timestamp | Random seed for reproducibility |

### Method Selection

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Methods to Include | `--methods` | `methodsToInclude` | All public methods | Comma-separated list of methods to include in fuzzing |
| Methods to Exclude | `--exclude` | `methodsToExclude` | None | Comma-separated list of methods to exclude from fuzzing |

### Coverage Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Coverage | `--coverage` | `enableCoverage` | `false` | Enable code coverage measurement |
| Coverage Format | `--coverage-format` | `coverageFormat` | `coz` | Coverage report format (`coz`, `html`, `json`) |

### Execution Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Persist State | `--persist-state` | `persistStateBetweenCalls` | `false` | Persist contract storage state between method calls |
| Save Failing Only | `--save-failing-only` | `saveFailingInputsOnly` | `false` | Only save inputs that cause exceptions |
| Replay Input | `--replay` | - | - | Replay a specific input from a result file |

### Symbolic Execution Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Symbolic Execution | `--symbolic` | `symbolicExecution.enabled` | `true` | Enable symbolic execution |
| Max Depth | `--max-depth` | `symbolicExecution.maxDepth` | 100 | Maximum execution depth for symbolic execution |
| Max States | `--max-states` | `symbolicExecution.maxStates` | 1000 | Maximum number of states to explore |
| Timeout | `--timeout` | `symbolicExecution.timeout` | 60 | Timeout in seconds for symbolic execution |

### Vulnerability Detection Options

| Option | CLI Flag | Config File Key | Default | Description |
|--------|----------|----------------|---------|-------------|
| Enable Vulnerability Detection | `--detect-vulnerabilities` | `vulnerabilityDetection.enabled` | `true` | Enable vulnerability detection |
| Detectors | `--detectors` | `vulnerabilityDetection.detectors` | All | Comma-separated list of vulnerability detectors to use |

## Advanced Configuration

### Custom Parameter Generators

You can configure custom parameter generators for specific parameter types:

```json
{
  "parameterGenerators": {
    "UInt160": {
      "type": "predefined",
      "values": [
        "0x0000000000000000000000000000000000000001",
        "0x0000000000000000000000000000000000000002"
      ]
    },
    "String": {
      "type": "regex",
      "pattern": "[a-zA-Z0-9]{5,10}"
    },
    "Integer": {
      "type": "range",
      "min": 0,
      "max": 1000
    }
  }
}
```

### Custom Constraint Solver

You can configure a custom constraint solver for symbolic execution:

```json
{
  "symbolicExecution": {
    "constraintSolver": {
      "type": "z3",
      "timeout": 5000,
      "memoryLimit": 1024
    }
  }
}
```

### Custom Vulnerability Detectors

You can configure custom vulnerability detectors:

```json
{
  "vulnerabilityDetection": {
    "detectors": [
      {
        "name": "integer-overflow",
        "enabled": true,
        "sensitivity": "high"
      },
      {
        "name": "reentrancy",
        "enabled": true,
        "checkDepth": 3
      }
    ]
  }
}
```

## Environment Variables

The fuzzer also respects the following environment variables:

- `NEO_FUZZER_CONFIG`: Path to a configuration file
- `NEO_FUZZER_OUTPUT`: Directory to save execution results
- `NEO_FUZZER_GAS_LIMIT`: Gas limit per execution
- `NEO_FUZZER_SEED`: Random seed for reproducibility
- `NEO_FUZZER_ITERATIONS`: Number of fuzzing iterations per method

## Configuration Precedence

Configuration options are applied in the following order of precedence (highest to lowest):

1. Command-line arguments
2. Configuration file specified with `--config`
3. Environment variables
4. Default values

This means that command-line arguments will override options specified in the configuration file, which will override environment variables, which will override default values.
