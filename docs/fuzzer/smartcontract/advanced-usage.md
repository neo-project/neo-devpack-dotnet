# Neo Smart Contract Fuzzer: Advanced Usage

This document covers advanced usage scenarios and techniques for the Neo Smart Contract Fuzzer, helping you get the most out of the tool for complex contracts and specific testing needs.

## Custom Parameter Generation

### Defining Custom Generators

You can define custom parameter generators for specific parameter types:

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
    },
    "ByteArray": {
      "type": "random",
      "minLength": 10,
      "maxLength": 100
    }
  }
}
```

### Method-Specific Parameter Generators

You can also define parameter generators for specific methods:

```json
{
  "methodParameterGenerators": {
    "transfer": {
      "from": {
        "type": "predefined",
        "values": [
          "0x0000000000000000000000000000000000000001",
          "0x0000000000000000000000000000000000000002"
        ]
      },
      "to": {
        "type": "predefined",
        "values": [
          "0x0000000000000000000000000000000000000003",
          "0x0000000000000000000000000000000000000004"
        ]
      },
      "amount": {
        "type": "range",
        "min": 1,
        "max": 1000
      }
    }
  }
}
```

### Implementing Custom Generator Classes

For more complex parameter generation, you can implement custom generator classes:

```csharp
public class CustomUInt160Generator : IParameterGenerator<UInt160>
{
    public UInt160 Generate(Random random)
    {
        // Custom logic to generate UInt160 values
        byte[] bytes = new byte[20];
        random.NextBytes(bytes);
        return new UInt160(bytes);
    }
}
```

Register your custom generator in the configuration:

```json
{
  "customGenerators": {
    "UInt160": "MyNamespace.CustomUInt160Generator, MyAssembly"
  }
}
```

## Contract State Manipulation

### Initial Contract State

You can define the initial state of the contract storage:

```json
{
  "initialState": {
    "storage": {
      "admin": "0x0000000000000000000000000000000000000001",
      "totalSupply": 1000000,
      "balances": {
        "0x0000000000000000000000000000000000000001": 500000,
        "0x0000000000000000000000000000000000000002": 500000
      }
    }
  }
}
```

### State Snapshots

You can save and load contract state snapshots:

```bash
# Save a state snapshot after running the fuzzer
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --save-state path/to/state.json

# Load a state snapshot when running the fuzzer
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --load-state path/to/state.json
```

### State Mutation

You can define state mutation strategies for more thorough testing:

```json
{
  "stateMutation": {
    "enabled": true,
    "strategies": [
      {
        "type": "random-key",
        "probability": 0.1
      },
      {
        "type": "random-value",
        "probability": 0.1
      },
      {
        "type": "delete-key",
        "probability": 0.05
      }
    ]
  }
}
```

## Method Sequence Testing

### Defining Method Sequences

You can define sequences of method calls to test interactions between methods:

```json
{
  "methodSequences": [
    {
      "name": "transfer-sequence",
      "sequence": [
        {
          "method": "mint",
          "parameters": {
            "to": "0x0000000000000000000000000000000000000001",
            "amount": 1000
          }
        },
        {
          "method": "transfer",
          "parameters": {
            "from": "0x0000000000000000000000000000000000000001",
            "to": "0x0000000000000000000000000000000000000002",
            "amount": 500
          }
        },
        {
          "method": "balanceOf",
          "parameters": {
            "account": "0x0000000000000000000000000000000000000002"
          },
          "expectedResult": 500
        }
      ]
    }
  ]
}
```

### Random Method Sequences

You can also generate random method sequences:

```json
{
  "randomMethodSequences": {
    "enabled": true,
    "minLength": 3,
    "maxLength": 10,
    "count": 100
  }
}
```

## Blockchain Environment Simulation

### Block Information

You can simulate different blockchain environments:

```json
{
  "blockchainSimulation": {
    "blockHeight": 1000000,
    "timestamp": 1625097600,
    "previousHash": "0x0000000000000000000000000000000000000000000000000000000000000000"
  }
}
```

### Transaction Context

You can simulate different transaction contexts:

```json
{
  "transactionContext": {
    "hash": "0x0000000000000000000000000000000000000000000000000000000000000000",
    "sender": "0x0000000000000000000000000000000000000001",
    "signers": [
      {
        "account": "0x0000000000000000000000000000000000000001",
        "scopes": "CalledByEntry"
      },
      {
        "account": "0x0000000000000000000000000000000000000002",
        "scopes": "Global"
      }
    ]
  }
}
```

### Native Contract Simulation

You can simulate interactions with native contracts:

```json
{
  "nativeContractSimulation": {
    "NEO": {
      "balances": {
        "0x0000000000000000000000000000000000000001": 1000,
        "0x0000000000000000000000000000000000000002": 2000
      }
    },
    "GAS": {
      "balances": {
        "0x0000000000000000000000000000000000000001": 5000,
        "0x0000000000000000000000000000000000000002": 10000
      }
    }
  }
}
```

## Targeted Fuzzing

### Focusing on Specific Code Paths

You can focus fuzzing on specific code paths:

```json
{
  "targetedFuzzing": {
    "targetMethods": ["transfer", "withdraw"],
    "targetInstructions": [
      {
        "offset": 42,
        "opcode": "SYSCALL",
        "operand": "System.Storage.Put"
      }
    ]
  }
}
```

### Guided Fuzzing

You can use coverage information to guide the fuzzing process:

```json
{
  "guidedFuzzing": {
    "enabled": true,
    "strategy": "coverage-maximization",
    "weightedMethods": {
      "transfer": 2.0,
      "withdraw": 1.5
    }
  }
}
```

## Performance Optimization

### Parallel Fuzzing

You can run fuzzing in parallel to improve performance:

```json
{
  "parallelFuzzing": {
    "enabled": true,
    "threads": 8,
    "batchSize": 100
  }
}
```

### Caching

You can enable caching to avoid redundant computations:

```json
{
  "caching": {
    "enabled": true,
    "maxSize": 1000,
    "persistCache": true,
    "cacheDirectory": "fuzzer-cache"
  }
}
```

### Resource Limits

You can set resource limits to prevent excessive resource usage:

```json
{
  "resourceLimits": {
    "maxMemory": 4096,
    "maxExecutionTime": 60,
    "maxStorageSize": 1024
  }
}
```

## Integration with Other Tools

### Exporting Results

You can export fuzzing results in various formats:

```bash
# Export results in JSON format
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --export-results path/to/results.json \
  --export-format json

# Export results in CSV format
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --export-results path/to/results.csv \
  --export-format csv
```

### Integration with CI/CD

You can integrate the fuzzer into your CI/CD pipeline:

```yaml
# Example GitHub Actions workflow
name: Smart Contract Fuzzing

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  fuzz:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 9.0.x
    - name: Compile Contract
      run: dotnet build src/MyContract/MyContract.csproj
    - name: Run Fuzzer
      run: |
        dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
          --nef src/MyContract/bin/Debug/net9.0/MyContract.nef \
          --manifest src/MyContract/bin/Debug/net9.0/MyContract.manifest.json \
          --output fuzzer-results \
          --iterations 1000 \
          --coverage
    - name: Upload Results
      uses: actions/upload-artifact@v2
      with:
        name: fuzzer-results
        path: fuzzer-results
```

### Integration with Security Scanners

You can integrate the fuzzer with other security scanners:

```bash
# Run the fuzzer and export results for further analysis
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --export-results path/to/results.json \
  --export-format json

# Run a security scanner on the results
security-scanner --input path/to/results.json --output security-report.json
```

## Programmatic Usage

### Using the Fuzzer as a Library

You can use the fuzzer programmatically in your own code:

```csharp
using Neo.SmartContract.Fuzzer;

// Create a configuration
var config = new FuzzerConfiguration
{
    NefPath = "path/to/contract.nef",
    ManifestPath = "path/to/contract.manifest.json",
    OutputDirectory = "fuzzer-results",
    IterationsPerMethod = 1000,
    GasLimit = 20000000,
    Seed = 42,
    EnableCoverage = true
};

// Create and run the fuzzer
var fuzzer = new SmartContractFuzzer(config);
var results = fuzzer.Run();

// Process the results
foreach (var methodResult in results.MethodResults)
{
    Console.WriteLine($"Method: {methodResult.Key}");
    Console.WriteLine($"Executions: {methodResult.Value.Count}");
    Console.WriteLine($"Failures: {methodResult.Value.Count(r => r.Exception != null)}");
}

// Access coverage information
var coverage = results.Coverage;
Console.WriteLine($"Instruction coverage: {coverage.InstructionCoverage}%");
Console.WriteLine($"Branch coverage: {coverage.BranchCoverage}%");

// Access vulnerability information
var vulnerabilities = results.Vulnerabilities;
foreach (var vulnerability in vulnerabilities)
{
    Console.WriteLine($"Vulnerability: {vulnerability.Type}");
    Console.WriteLine($"Severity: {vulnerability.Severity}");
    Console.WriteLine($"Location: {vulnerability.Location}");
}
```

### Custom Vulnerability Detectors

You can implement custom vulnerability detectors:

```csharp
public class CustomVulnerabilityDetector : IVulnerabilityDetector
{
    public string Name => "custom-detector";
    
    public IEnumerable<Vulnerability> DetectVulnerabilities(ExecutionTrace trace)
    {
        // Custom logic to detect vulnerabilities
        // ...
        
        yield return new Vulnerability
        {
            Type = "custom-vulnerability",
            Severity = Severity.High,
            Location = "Method: transfer, Offset: 42",
            Description = "Custom vulnerability description",
            Recommendation = "Custom vulnerability recommendation"
        };
    }
}
```

Register your custom detector in the configuration:

```json
{
  "customDetectors": {
    "custom-detector": "MyNamespace.CustomVulnerabilityDetector, MyAssembly"
  }
}
```

## Conclusion

These advanced usage scenarios demonstrate the flexibility and power of the Neo Smart Contract Fuzzer. By leveraging these features, you can create comprehensive testing strategies for even the most complex smart contracts, helping to ensure their reliability and security before deployment to the Neo blockchain.
