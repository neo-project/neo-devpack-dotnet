# Neo Smart Contract Fuzzer

The Neo Smart Contract Fuzzer is a comprehensive testing tool designed to help developers identify bugs, vulnerabilities, and edge cases in Neo N3 smart contracts. By automatically generating diverse inputs and executing contracts under various conditions, the fuzzer helps ensure contract reliability and security before deployment to the Neo blockchain.

## Documentation Index

- [Getting Started](./getting-started.md) - Quick start guide for using the fuzzer
- [Technical Overview](./technical-overview.md) - Architecture and components of the fuzzer
- [Configuration Guide](./configuration-guide.md) - Detailed configuration options
- [Vulnerability Detection](./vulnerability-detection.md) - How the fuzzer detects vulnerabilities
- [Symbolic Execution](./symbolic-execution.md) - Understanding the symbolic execution engine
- [Advanced Usage](./advanced-usage.md) - Advanced features and techniques
- [API Reference](./api-reference.md) - Reference for programmatic usage
- [Development Plan](./development-plan.md) - Future development roadmap

## Key Features

- **Dynamic Input Generation**: Automatically generates diverse inputs for contract methods
- **Symbolic Execution**: Uses symbolic execution to explore multiple execution paths
- **Vulnerability Detection**: Identifies common smart contract vulnerabilities
- **Code Coverage Tracking**: Measures and reports code coverage during fuzzing
- **Detailed Reporting**: Generates comprehensive reports of findings
- **Configurable**: Highly configurable to suit different testing needs
- **Feedback-Guided Fuzzing**: Uses execution feedback to guide input generation
- **Test Case Minimization**: Reduces complex test cases to minimal reproducible examples
- **Reproducible Testing**: Supports fixed random seeds for reproducible tests

## Use Cases

- **Pre-deployment Testing**: Thoroughly test contracts before deployment
- **Security Auditing**: Identify potential security vulnerabilities
- **Regression Testing**: Ensure new changes don't introduce bugs
- **Edge Case Discovery**: Find edge cases that might not be covered by unit tests
- **Performance Analysis**: Identify gas-intensive operations and optimization opportunities
- **Continuous Integration**: Integrate into CI/CD pipelines for automated testing

## Quick Example

```bash
# Using the convenience script
./run-fuzzer.bat --nef path/to/contract.nef --manifest path/to/contract.manifest.json

# Or directly with dotnet
dotnet run --project src/Neo.SmartContract.Fuzzer/Neo.SmartContract.Fuzzer.csproj \
  --nef path/to/contract.nef \
  --manifest path/to/contract.manifest.json \
  --output fuzzer-results \
  --iterations 1000
```

## Understanding Results

The fuzzer generates detailed reports in the output directory:

```
fuzzer-results/
├── SampleContract/
│   ├── issues/        # Detailed issue reports
│   └── report.json    # Summary report
└── README.md          # Explanation of results
```

For more detailed usage instructions, see the [Getting Started](./getting-started.md) guide.
