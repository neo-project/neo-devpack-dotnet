# Neo Smart Contract Fuzzer Integration Tests

This project contains integration tests for the Neo Smart Contract Fuzzer. These tests verify that the fuzzer can detect vulnerabilities in real Neo smart contracts and measure its performance and effectiveness.

## Test Contracts

The `TestContracts` directory contains vulnerable Neo smart contracts that are used to test the fuzzer:

- `IntegerOverflowContract.cs`: Contains integer overflow vulnerabilities
- `DivisionByZeroContract.cs`: Contains division by zero vulnerabilities
- `UnauthorizedAccessContract.cs`: Contains unauthorized access vulnerabilities
- `StorageExhaustionContract.cs`: Contains storage exhaustion vulnerabilities

Each contract contains both vulnerable and safe versions of the same functionality to test the fuzzer's ability to distinguish between them.

## Integration Tests

The `FuzzerIntegrationTests.cs` file contains integration tests that verify the fuzzer's ability to detect vulnerabilities in the test contracts. These tests run the fuzzer on each contract and check that it finds the expected vulnerabilities.

## Performance Benchmarks

The `PerformanceBenchmarkTests.cs` file contains benchmarks that measure the performance of the fuzzer. These benchmarks test different configurations of the fuzzer and measure metrics such as execution rate, success rate, and issue detection rate.

## Tool Comparison

The `ToolComparisonTests.cs` file contains tests that compare the effectiveness and efficiency of different vulnerability detection approaches:

- Static analysis only
- Feedback-guided fuzzing only
- Symbolic execution only
- Combined approach (static analysis + feedback-guided fuzzing + symbolic execution)

## Running the Tests

To run the integration tests, use the following command:

```
dotnet test tests/Neo.SmartContract.Fuzzer.IntegrationTests
```

To run a specific test, use the following command:

```
dotnet test tests/Neo.SmartContract.Fuzzer.IntegrationTests --filter "FullyQualifiedName=Neo.SmartContract.Fuzzer.IntegrationTests.FuzzerIntegrationTests.IntegerOverflowContract_DetectsVulnerabilities"
```

## Test Results

The test results are stored in the `TestResults` directory. Each test generates a report file that contains information about the vulnerabilities found, the execution statistics, and the performance metrics.

## Interpreting the Results

The integration tests verify that the fuzzer can detect vulnerabilities in Neo smart contracts. The performance benchmarks measure the fuzzer's efficiency and effectiveness. The tool comparison tests compare the fuzzer with other vulnerability detection approaches.

When interpreting the results, consider the following metrics:

- **Issues Found**: The number of vulnerabilities found by the fuzzer
- **Execution Rate**: The number of executions per second
- **Success Rate**: The percentage of executions that completed successfully
- **Code Coverage**: The percentage of code covered by the fuzzer
- **Efficiency**: The number of issues found per second

A higher number of issues found, execution rate, success rate, code coverage, and efficiency indicate better performance.
