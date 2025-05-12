# Neo Smart Contract Fuzzer: Advanced Usage

This document covers advanced usage scenarios and techniques for the Neo Smart Contract Fuzzer, helping you get the most out of the tool for complex contracts and specific testing needs.

## Feedback-Guided Fuzzing

Feedback-guided fuzzing uses execution feedback to guide the generation of new test cases. This can significantly improve the efficiency of fuzzing by focusing on inputs that explore new code paths.

### Enabling Feedback-Guided Fuzzing

Feedback-guided fuzzing is enabled by default. You can explicitly enable or disable it:

```bash
# Enable feedback-guided fuzzing
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --feedback

# Disable feedback-guided fuzzing
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --no-feedback
```

### Corpus Management

The fuzzer maintains a corpus of interesting test cases that trigger unique behaviors:

```bash
# Specify a directory for the corpus
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --corpus-dir path/to/corpus

# Limit the corpus size
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --max-corpus-size 1000
```

### Feedback Metrics

The fuzzer uses various metrics to determine if a test case is interesting:

- **Code Coverage**: Does the test case cover new instructions or branches?
- **Storage Access**: Does the test case access new storage keys?
- **Event Emission**: Does the test case trigger new events?
- **Exception Types**: Does the test case cause new types of exceptions?
- **Gas Consumption**: Does the test case consume gas in a unique pattern?

## Symbolic Execution

Symbolic execution explores multiple execution paths by treating inputs as symbolic values. This can help find vulnerabilities that are difficult to discover with random fuzzing.

### Enabling Symbolic Execution

Symbolic execution is disabled by default due to its computational cost. You can enable it:

```bash
# Enable symbolic execution
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --symbolic

# Configure symbolic execution depth
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --symbolic \
  --symbolic-depth 100 \
  --symbolic-paths 1000
```

### Constraint Solving

The symbolic execution engine uses a constraint solver to generate concrete inputs that satisfy path constraints:

```bash
# Specify a timeout for constraint solving
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --symbolic \
  --solver-timeout 5000
```

## Test Case Minimization

Test case minimization reduces the complexity of test cases that trigger issues, making it easier to understand and fix the underlying problems.

### Enabling Test Case Minimization

Test case minimization is enabled by default. You can explicitly enable or disable it:

```bash
# Enable test case minimization
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --minimize

# Disable test case minimization
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --no-minimize
```

### Minimization Strategies

The fuzzer uses several strategies to minimize test cases:

- **Parameter Removal**: Removing parameters that don't affect the issue
- **Value Simplification**: Simplifying complex values (e.g., large integers, long strings)
- **Delta Debugging**: Systematically reducing the test case while preserving the issue

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

You can integrate the fuzzer into your CI/CD pipeline to automatically test contracts during development:

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
          --coverage \
          --report-format junit,json
    - name: Upload Results
      uses: actions/upload-artifact@v2
      with:
        name: fuzzer-results
        path: fuzzer-results
    - name: Publish Test Results
      uses: EnricoMi/publish-unit-test-result-action@v1
      if: always()
      with:
        files: fuzzer-results/junit-report.xml
```

### Continuous Fuzzing

For long-running fuzzing campaigns, you can set up continuous fuzzing:

```bash
# Run the fuzzer continuously, saving results periodically
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --continuous \
  --save-interval 3600 \
  --max-runtime 86400
```

This will run the fuzzer for 24 hours, saving results every hour.

### Regression Testing

You can use the fuzzer for regression testing to ensure that new changes don't introduce vulnerabilities:

```bash
# Run the fuzzer with a fixed seed for reproducibility
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --seed 42 \
  --iterations 1000 \
  --report-format junit
```

Add this to your CI pipeline to catch regressions early.

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
using Neo.SmartContract.Fuzzer.Controller;

// Create a fuzzer configuration
var config = new FuzzerConfiguration
{
    NefPath = "path/to/contract.nef",
    ManifestPath = "path/to/contract.manifest.json",
    OutputDirectory = "fuzzer-results",
    IterationsPerMethod = 1000,
    GasLimit = 20_000_000, // 20 GAS
    Seed = 42,
    EnableFeedbackGuidedFuzzing = true,
    EnableTestCaseMinimization = true,
    EnableStaticAnalysis = true,
    EnableSymbolicExecution = false,
    ReportFormats = new List<string> { "json", "html" }
};

// Create a fuzzing controller
var controller = new FuzzingController(config);

// Start the fuzzing process
await controller.StartAsync();

// Get the fuzzing status
var status = controller.GetStatus();

// Print the results
Console.WriteLine($"Total Executions: {status.TotalExecutions}");
Console.WriteLine($"Successful Executions: {status.SuccessfulExecutions}");
Console.WriteLine($"Failed Executions: {status.FailedExecutions}");
Console.WriteLine($"Issues Found: {status.IssuesFound}");
Console.WriteLine($"Code Coverage: {status.CodeCoverage:P2}");
Console.WriteLine($"Execution Time: {status.ElapsedTime}");

// Access the issues
var issues = controller.GetIssues();
foreach (var issue in issues)
{
    Console.WriteLine($"Issue Type: {issue.IssueType}");
    Console.WriteLine($"Description: {issue.Description}");
    Console.WriteLine($"Method: {issue.Method}");
    Console.WriteLine($"Severity: {issue.Severity}");

    // Access the test case that triggered the issue
    var testCase = issue.TestCase;
    Console.WriteLine($"Parameters: {string.Join(", ", testCase.Parameters)}");

    // Access the minimized test case
    var minimizedTestCase = issue.MinimizedTestCase;
    if (minimizedTestCase != null)
    {
        Console.WriteLine($"Minimized Parameters: {string.Join(", ", minimizedTestCase.Parameters)}");
    }
}
```

### Custom Vulnerability Detectors

You can implement custom vulnerability detectors to find specific issues in your contracts:

```csharp
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.Models;

public class CustomVulnerabilityDetector : IVulnerabilityDetector
{
    public string Name => "CustomDetector";

    public List<IssueReport> DetectVulnerabilities(TestCase testCase, ExecutionResult result)
    {
        var issues = new List<IssueReport>();

        // Example: Detect if a method consumes more than 10 GAS
        if (result.FeeConsumed > 10_000_000)
        {
            issues.Add(new IssueReport
            {
                IssueType = "High Gas Consumption",
                Description = $"Method consumed {result.FeeConsumed / 100000000.0} GAS",
                Severity = "Medium",
                Method = testCase.MethodName,
                TestCase = testCase,
                GasConsumed = result.FeeConsumed
            });
        }

        // Example: Detect if a method accesses a specific storage key
        if (result.StorageChanges?.Any(sc => sc.Key.ToHexString() == "0123456789abcdef") == true)
        {
            issues.Add(new IssueReport
            {
                IssueType = "Sensitive Storage Access",
                Description = "Method accesses a sensitive storage key",
                Severity = "High",
                Method = testCase.MethodName,
                TestCase = testCase
            });
        }

        return issues;
    }
```

To register your custom detector, add it to the fuzzing controller:

```csharp
// Create a fuzzing controller
var controller = new FuzzingController(config);

// Register custom vulnerability detector
controller.RegisterVulnerabilityDetector(new CustomVulnerabilityDetector());

// Start the fuzzing process
await controller.StartAsync();
```

Or specify it in the configuration file:

```json
{
  "VulnerabilityDetectors": [
    {
      "Name": "CustomDetector",
      "Type": "MyNamespace.CustomVulnerabilityDetector, MyAssembly",
      "Enabled": true
    }
  ]
}
```

## Conclusion

These advanced usage scenarios demonstrate the flexibility and power of the Neo Smart Contract Fuzzer. By leveraging these features, you can create comprehensive testing strategies for even the most complex smart contracts, helping to ensure their reliability and security before deployment to the Neo blockchain.
