# Code Coverage in Neo Smart Contract Fuzzer

This document describes the code coverage feature in the Neo Smart Contract Fuzzer, which is used to measure how much of the contract code is executed during fuzzing.

## Overview

Code coverage is a measure of how much of the contract code is executed during fuzzing. It helps identify areas of the contract that are not being tested, which may contain undiscovered vulnerabilities.

The Neo Smart Contract Fuzzer includes a comprehensive code coverage system that tracks which instructions, methods, and branches are executed during fuzzing. This information is used to guide the fuzzing process and to generate detailed code coverage reports.

## Coverage Types

The fuzzer tracks the following types of coverage:

### Instruction Coverage

Instruction coverage measures which Neo VM instructions are executed during fuzzing. It provides the most detailed view of code coverage, showing exactly which instructions are executed and which are not.

### Method Coverage

Method coverage measures which methods in the contract are executed during fuzzing. It provides a higher-level view of code coverage, showing which methods are tested and which are not.

### Branch Coverage

Branch coverage measures which branches (e.g., if statements, loops) are taken during fuzzing. It helps identify conditional logic that is not being tested.

### Line Coverage

Line coverage measures which lines of source code are executed during fuzzing. It requires the source code to be available and is the most user-friendly view of code coverage.

## Coverage Tracking

The fuzzer tracks code coverage using the `CoverageTracker` class, which records which instructions, methods, and branches are executed during fuzzing. The coverage information is used to:

1. Guide the fuzzing process by prioritizing inputs that increase coverage
2. Generate detailed code coverage reports
3. Identify areas of the contract that are not being tested

## Coverage Reports

The fuzzer can generate code coverage reports in the following formats:

- **HTML**: Interactive HTML reports that show which instructions, methods, and lines are covered
- **XML**: XML reports that can be imported into other tools
- **JSON**: JSON reports that can be processed programmatically
- **Cobertura**: Cobertura-compatible XML reports that can be used with CI/CD systems

The reports include:
- Overall coverage percentage
- Coverage by method
- Coverage by instruction
- Coverage by branch
- Coverage by line (if source code is available)

## Using Code Coverage

To enable code coverage, use the `--enable-coverage` option when running the fuzzer:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --enable-coverage
```

You can also specify the coverage report format using the `--coverage-format` option:

```bash
dotnet run --project src/Neo.SmartContract.Fuzzer -- --nef MyContract.nef --manifest MyContract.manifest.json --enable-coverage --coverage-format Html
```

Supported formats are:
- `Html` (default)
- `Xml`
- `Json`
- `Cobertura`

## Programmatic Usage

You can also enable code coverage programmatically:

```csharp
var config = new FuzzerConfiguration
{
    NefPath = "MyContract.nef",
    ManifestPath = "MyContract.manifest.json",
    OutputDirectory = "FuzzingResults",
    EnableCoverage = true,
    CoverageFormat = CoverageFormat.Html
};

var controller = new FuzzingController(config);
controller.Start();
```

## Coverage-Guided Fuzzing

The fuzzer uses code coverage information to guide the fuzzing process. It prioritizes inputs that increase coverage, which helps find more vulnerabilities more efficiently.

The coverage-guided fuzzing process works as follows:

1. The fuzzer generates random inputs for the contract methods
2. It executes the contract with these inputs and tracks which instructions are executed
3. Inputs that execute previously uncovered instructions are saved to the corpus
4. The fuzzer generates new inputs by mutating the inputs in the corpus
5. The process repeats, gradually increasing code coverage

This approach is more efficient than purely random fuzzing because it focuses on inputs that explore new parts of the contract.

## Interpreting Coverage Reports

When interpreting coverage reports, consider the following:

- **High Coverage**: High coverage (e.g., >80%) indicates that most of the contract code is being tested.
- **Low Coverage**: Low coverage (e.g., <50%) indicates that significant parts of the contract are not being tested.
- **Uncovered Methods**: Methods that are not covered may contain undiscovered vulnerabilities.
- **Uncovered Branches**: Branches that are not covered may contain logic errors or vulnerabilities.
- **Uncovered Lines**: Lines that are not covered may contain bugs or vulnerabilities.

To improve coverage:
1. Increase the number of iterations
2. Enable symbolic execution
3. Focus on methods with low coverage
4. Add custom inputs that target specific branches

## Limitations

The code coverage feature has the following limitations:

1. **Source Code Availability**: Line coverage requires the source code to be available.
2. **Complex Contracts**: For complex contracts, achieving high coverage may require a large number of iterations.
3. **Private Methods**: Private methods that are not called by public methods may be difficult to cover.
4. **Conditional Logic**: Complex conditional logic may be difficult to cover completely.

Despite these limitations, code coverage is a valuable tool for assessing the thoroughness of the fuzzing process and identifying areas of the contract that need more testing.

## Conclusion

The code coverage feature in the Neo Smart Contract Fuzzer helps ensure that the contract is thoroughly tested. By tracking which parts of the contract are executed during fuzzing and guiding the fuzzing process to increase coverage, it helps find more vulnerabilities and improve the security of Neo smart contracts.
