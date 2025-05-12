# Neo Smart Contract Fuzzer Tests

This directory contains the test suite for the Neo Smart Contract Fuzzer, a comprehensive fuzzing and vulnerability detection tool for Neo N3 smart contracts.

## Test Structure

The tests are organized into the following categories:

### Core Tests

Located in the `Core` directory, these tests focus on the core functionality of the fuzzer:

- `FuzzerConfigurationTests.cs`: Tests the configuration system
- `ParameterGenerationTests.cs`: Tests the parameter generation system
- `SmartContractFuzzerTests.cs`: Tests the main fuzzer component

### Integration Tests

Located in the root directory, these tests verify the end-to-end functionality of the fuzzer:

- `IntegrationTests.cs`: Tests the complete fuzzing process with symbolic execution and vulnerability detection

### Symbolic Execution Tests

Located in the `SymbolicExecution` directory, these tests focus on the symbolic execution engine:

- `SymbolicExecutionTests.cs`: Tests the core symbolic execution components
- `PathVisualizationTests.cs`: Tests the path visualization component
- `PathVisualizationIntegrationTests.cs`: Tests the integration of path visualization with the fuzzer

### Reporting Tests

Located in the `Reporting` directory, these tests focus on the reporting system:

- `EnhancedReportGeneratorTests.cs`: Tests the enhanced report generation
- `MetricsCollectorTests.cs`: Tests the metrics collection system
- `ReportFormatterTests.cs`: Tests the report formatting system

### Vulnerability Tests

Located in the `Vulnerabilities` directory, these tests focus on vulnerability detection:

- `VulnerabilityAnalyzerTests.cs`: Tests the vulnerability analysis system
- `VulnerabilityDetectorTests.cs`: Tests the vulnerability detection system

## Running the Tests

To run the tests, use the following command from the root of the repository:

```bash
dotnet test tests/Neo.SmartContract.Fuzzer.Tests
```

To run a specific test:

```bash
dotnet test tests/Neo.SmartContract.Fuzzer.Tests --filter "FullyQualifiedName=Neo.SmartContract.Fuzzer.Tests.IntegrationTests"
```

## Test Data

Test data files are stored in the `TestData` directory and are automatically copied to the output directory during the build process.

## Adding New Tests

When adding new tests:

1. Follow the existing structure and naming conventions
2. Create unit tests for individual components in their respective directories
3. Create integration tests for end-to-end functionality
4. Ensure all tests are properly documented with XML comments

## Test Coverage

The test suite aims to provide comprehensive coverage of the Neo Smart Contract Fuzzer's functionality, including:

- Symbolic execution engine
- Vulnerability detection
- Path visualization
- Constraint solving
- Report generation

The tests use a combination of real and synthetic smart contracts to verify the fuzzer's behavior under various conditions.