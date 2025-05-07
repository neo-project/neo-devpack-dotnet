# Neo Smart Contract Fuzzer: API Reference

This document provides a comprehensive reference for the Neo Smart Contract Fuzzer API, allowing developers to integrate and extend the fuzzer programmatically.

## Core Classes

### `FuzzingController`

The main class that orchestrates the fuzzing process.

```csharp
namespace Neo.SmartContract.Fuzzer.Controller
{
    public class FuzzingController
    {
        // Constructors
        public FuzzingController(FuzzerConfiguration config);

        // Methods
        public Task StartAsync(CancellationToken cancellationToken = default);
        public Task WaitForCompletionAsync(CancellationToken cancellationToken = default);
        public void Stop();
        public FuzzingStatus GetStatus();
        public List<IssueReport> GetIssues();
        public void RegisterVulnerabilityDetector(IVulnerabilityDetector detector);
        public void RegisterStaticAnalyzer(IStaticAnalyzer analyzer);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `config` | `FuzzerConfiguration` | Configuration for the fuzzer |

#### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `StartAsync(CancellationToken cancellationToken = default)` | `Task` | Starts the fuzzing process asynchronously |
| `WaitForCompletionAsync(CancellationToken cancellationToken = default)` | `Task` | Waits for the fuzzing process to complete |
| `Stop()` | `void` | Stops the fuzzing process |
| `GetStatus()` | `FuzzingStatus` | Gets the current status of the fuzzing process |
| `GetIssues()` | `List<IssueReport>` | Gets the issues found during fuzzing |
| `RegisterVulnerabilityDetector(IVulnerabilityDetector detector)` | `void` | Registers a custom vulnerability detector |
| `RegisterStaticAnalyzer(IStaticAnalyzer analyzer)` | `void` | Registers a custom static analyzer |

### `FuzzerConfiguration`

Configuration class for the fuzzer.

```csharp
namespace Neo.SmartContract.Fuzzer
{
    public class FuzzerConfiguration
    {
        // Properties
        public string NefPath { get; set; }
        public string ManifestPath { get; set; }
        public string OutputDirectory { get; set; }
        public int IterationsPerMethod { get; set; }
        public long GasLimit { get; set; }
        public int? Seed { get; set; }
        public bool EnableFeedbackGuidedFuzzing { get; set; }
        public bool EnableTestCaseMinimization { get; set; }
        public bool EnableStaticAnalysis { get; set; }
        public bool EnableSymbolicExecution { get; set; }
        public int SymbolicExecutionDepth { get; set; }
        public int SymbolicExecutionPaths { get; set; }
        public string[] MethodsToFuzz { get; set; }
        public string[] MethodsToExclude { get; set; }
        public string ExecutionEngine { get; set; }
        public string RpcUrl { get; set; }
        public bool PersistStateBetweenCalls { get; set; }
        public bool SaveFailingInputsOnly { get; set; }
        public List<string> ReportFormats { get; set; }

        // Methods
        public static FuzzerConfiguration LoadFromFile(string path);
        public static FuzzerConfiguration ParseCommandLineArgs(string[] args);
    }
}
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NefPath` | `string` | `""` | Path to the NEF file |
| `ManifestPath` | `string` | `""` | Path to the manifest file |
| `OutputDirectory` | `string` | `"fuzzer-output"` | Directory to save execution results |
| `IterationsPerMethod` | `int` | `1000` | Number of iterations per method |
| `GasLimit` | `long` | `20_000_000` | Gas limit per execution |
| `Seed` | `int?` | `null` | Random seed for reproducibility (null uses current timestamp) |
| `EnableFeedbackGuidedFuzzing` | `bool` | `true` | Whether to enable feedback-guided fuzzing |
| `EnableTestCaseMinimization` | `bool` | `true` | Whether to enable test case minimization |
| `EnableStaticAnalysis` | `bool` | `true` | Whether to enable static analysis |
| `EnableSymbolicExecution` | `bool` | `false` | Whether to enable symbolic execution |
| `SymbolicExecutionDepth` | `int` | `100` | Maximum depth for symbolic execution |
| `SymbolicExecutionPaths` | `int` | `1000` | Maximum number of paths to explore |
| `MethodsToFuzz` | `string[]` | `null` | Array of methods to fuzz (null means all public methods) |
| `MethodsToExclude` | `string[]` | `null` | Array of methods to exclude from fuzzing |
| `ExecutionEngine` | `string` | `"neo-express"` | Execution engine to use (neo-express, rpc, in-memory) |
| `RpcUrl` | `string` | `"http://localhost:10332"` | RPC URL for RPC execution engine |
| `PersistStateBetweenCalls` | `bool` | `false` | Whether to persist state between method calls |
| `SaveFailingInputsOnly` | `bool` | `false` | Whether to save only failing inputs |
| `ReportFormats` | `List<string>` | `["json"]` | List of report formats to generate |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `LoadFromFile` | `string path` | `FuzzerConfiguration` | Loads configuration from a JSON file |
| `ParseCommandLineArgs` | `string[] args` | `FuzzerConfiguration` | Parses command-line arguments into a configuration |

### `IExecutionEngine`

Interface for executing contracts.

```csharp
namespace Neo.SmartContract.Fuzzer.Execution
{
    public interface IExecutionEngine
    {
        // Methods
        ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iteration);
        void Reset();
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `ExecuteMethod` | `ContractMethodDescriptor method, StackItem[] parameters, int iteration` | `ExecutionResult` | Executes a contract method with the given parameters |
| `Reset` | None | `void` | Resets the execution engine state |

### `NeoExpressExecutionEngine`

Implementation of IExecutionEngine that uses Neo Express for execution.

```csharp
namespace Neo.SmartContract.Fuzzer.Execution
{
    public class NeoExpressExecutionEngine : IExecutionEngine
    {
        // Constructors
        public NeoExpressExecutionEngine(byte[] nefBytes, ContractManifest manifest, FuzzerConfiguration config);

        // Methods
        public ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iteration);
        public void Reset();
    }
}
```

### `RpcExecutionEngine`

Implementation of IExecutionEngine that uses a Neo RPC node for execution.

```csharp
namespace Neo.SmartContract.Fuzzer.Execution
{
    public class RpcExecutionEngine : IExecutionEngine
    {
        // Constructors
        public RpcExecutionEngine(byte[] nefBytes, ContractManifest manifest, FuzzerConfiguration config);

        // Methods
        public ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iteration);
        public void Reset();
    }
}
```

### `InMemoryExecutionEngine`

Implementation of IExecutionEngine that executes in-memory for faster testing.

```csharp
namespace Neo.SmartContract.Fuzzer.Execution
{
    public class InMemoryExecutionEngine : IExecutionEngine
    {
        // Constructors
        public InMemoryExecutionEngine(byte[] nefBytes, ContractManifest manifest, FuzzerConfiguration config);

        // Methods
        public ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iteration);
        public void Reset();
    }
}
```

### `InputGenerator`

Generates inputs for contract methods.

```csharp
namespace Neo.SmartContract.Fuzzer.InputGeneration
{
    public class InputGenerator
    {
        // Constructors
        public InputGenerator(FeedbackAggregator feedbackAggregator, int seed);

        // Methods
        public TestCase GenerateTestCase(string methodName);
        public void AddToCorpus(TestCase testCase, bool isInteresting);
        public void SetParameterGenerator(string parameterType, IParameterGenerator generator);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `feedbackAggregator` | `FeedbackAggregator` | Feedback aggregator for guided fuzzing |
| `seed` | `int` | Random seed for reproducibility |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `GenerateTestCase` | `string methodName` | `TestCase` | Generates a test case for the specified method |
| `AddToCorpus` | `TestCase testCase, bool isInteresting` | `void` | Adds a test case to the corpus if it's interesting |
| `SetParameterGenerator` | `string parameterType, IParameterGenerator generator` | `void` | Sets a custom parameter generator for a specific type |

### `IParameterGenerator`

Interface for custom parameter generators.

```csharp
namespace Neo.SmartContract.Fuzzer.InputGeneration
{
    public interface IParameterGenerator
    {
        StackItem Generate(Random random);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Generate` | `Random random` | `StackItem` | Generates a parameter value |

### `FeedbackAggregator`

Collects and prioritizes feedback to guide the fuzzing process.

```csharp
namespace Neo.SmartContract.Fuzzer.Feedback
{
    public class FeedbackAggregator
    {
        // Constructors
        public FeedbackAggregator();

        // Methods
        public bool AddExecutionFeedback(TestCase testCase, ExecutionResult result);
        public void AddStaticAnalysisHint(StaticAnalysisHint hint);
        public Dictionary<string, object> GetCoverageStatistics();
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `AddExecutionFeedback` | `TestCase testCase, ExecutionResult result` | `bool` | Processes execution result and determines if it's interesting |
| `AddStaticAnalysisHint` | `StaticAnalysisHint hint` | `void` | Adds a static analysis hint |
| `GetCoverageStatistics` | None | `Dictionary<string, object>` | Returns coverage statistics |

### `TestCaseMinimizer`

Reduces test cases to their minimal form while preserving the issue.

```csharp
namespace Neo.SmartContract.Fuzzer.Minimization
{
    public class TestCaseMinimizer
    {
        // Constructors
        public TestCaseMinimizer(IExecutionEngine executor, ContractMethodDescriptor method,
                                Predicate<ExecutionResult> predicate, int seed);

        // Methods
        public TestCase Minimize(TestCase testCase);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `executor` | `IExecutionEngine` | Execution engine |
| `method` | `ContractMethodDescriptor` | Method descriptor |
| `predicate` | `Predicate<ExecutionResult>` | Predicate that determines if an execution result exhibits the issue |
| `seed` | `int` | Random seed for reproducibility |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Minimize` | `TestCase testCase` | `TestCase` | Minimizes the test case while preserving the predicate |

## Symbolic Execution

### `ISymbolicExecutionIntegrator`

Interface for symbolic execution integration.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public interface ISymbolicExecutionIntegrator
    {
        // Methods
        List<IssueReport> AnalyzeMethod(ContractMethodDescriptor method);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `AnalyzeMethod` | `ContractMethodDescriptor method` | `List<IssueReport>` | Analyzes a method using symbolic execution |

### `SymbolicExecutionEngine`

The main class for symbolic execution.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public class SymbolicExecutionEngine
    {
        // Constructors
        public SymbolicExecutionEngine(byte[] nefBytes, IConstraintSolver solver, int maxDepth, int maxPaths);

        // Methods
        public SymbolicExecutionResult Execute(ContractMethodDescriptor method);

        // Static Methods
        public static List<SymbolicVariable> CreateSymbolicArgumentsForMethod(ContractMethodDescriptor method);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `nefBytes` | `byte[]` | NEF file bytes |
| `solver` | `IConstraintSolver` | Constraint solver |
| `maxDepth` | `int` | Maximum execution depth |
| `maxPaths` | `int` | Maximum number of paths to explore |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Execute` | `ContractMethodDescriptor method` | `SymbolicExecutionResult` | Executes the method symbolically |

#### Static Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `CreateSymbolicArgumentsForMethod` | `ContractMethodDescriptor method` | `List<SymbolicVariable>` | Creates symbolic arguments for a method |

### `SymbolicExecutionResult`

Represents the result of symbolic execution.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public class SymbolicExecutionResult
    {
        // Properties
        public List<ExecutionPath> ExecutionPaths { get; }
        public int TotalSteps { get; }
        public int TotalStates { get; }
        public TimeSpan ExecutionTime { get; }
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ExecutionPaths` | `List<ExecutionPath>` | List of execution paths |
| `TotalSteps` | `int` | Total number of steps executed |
| `TotalStates` | `int` | Total number of states explored |
| `ExecutionTime` | `TimeSpan` | Total execution time |

### `ExecutionPath`

Represents a single execution path.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    public class ExecutionPath
    {
        // Properties
        public List<ExecutionStep> Steps { get; }
        public List<SymbolicExpression> PathConstraints { get; }
        public VMState HaltReason { get; }
        public SymbolicValue ReturnValue { get; }
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Steps` | `List<ExecutionStep>` | List of execution steps |
| `PathConstraints` | `List<SymbolicExpression>` | List of path constraints |
| `HaltReason` | `VMState` | Reason for halting execution |
| `ReturnValue` | `SymbolicValue` | Return value of the execution |

## Vulnerability Detection

### `IVulnerabilityDetector`

Interface for vulnerability detectors.

```csharp
namespace Neo.SmartContract.Fuzzer.Detectors
{
    public interface IVulnerabilityDetector
    {
        // Properties
        string Name { get; }

        // Methods
        List<IssueReport> DetectVulnerabilities(TestCase testCase, ExecutionResult result);
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Name of the detector |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `DetectVulnerabilities` | `TestCase testCase, ExecutionResult result` | `List<IssueReport>` | Detects vulnerabilities in an execution result |

### `IssueReport`

Represents a detected issue or vulnerability.

```csharp
namespace Neo.SmartContract.Fuzzer.Models
{
    public class IssueReport
    {
        // Properties
        public string IssueType { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Method { get; set; }
        public string Location { get; set; }
        public string Source { get; set; }
        public TestCase TestCase { get; set; }
        public TestCase MinimizedTestCase { get; set; }
        public long GasConsumed { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IssueType` | `string` | Type of issue |
| `Description` | `string` | Description of the issue |
| `Severity` | `string` | Severity of the issue (Low, Medium, High, Critical) |
| `Method` | `string` | Method where the issue was found |
| `Location` | `string` | Location of the issue |
| `Source` | `string` | Source of the issue (Fuzzing, StaticAnalysis, SymbolicExecution) |
| `TestCase` | `TestCase` | Test case that triggered the issue |
| `MinimizedTestCase` | `TestCase` | Minimized test case that still triggers the issue |
| `GasConsumed` | `long` | Gas consumed when the issue was triggered |
| `Metadata` | `Dictionary<string, object>` | Additional metadata about the issue |

### `TestCase`

Represents a test case for a contract method.

```csharp
namespace Neo.SmartContract.Fuzzer.Models
{
    public class TestCase
    {
        // Properties
        public string MethodName { get; set; }
        public StackItem[] Parameters { get; set; }
        public int Iteration { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `MethodName` | `string` | Name of the method |
| `Parameters` | `StackItem[]` | Parameters for the method |
| `Iteration` | `int` | Iteration number |
| `Metadata` | `Dictionary<string, object>` | Additional metadata about the test case |

## Static Analysis

### `IStaticAnalyzer`

Interface for static analyzers.

```csharp
namespace Neo.SmartContract.Fuzzer.StaticAnalysis
{
    public interface IStaticAnalyzer
    {
        // Properties
        string Name { get; }

        // Methods
        List<StaticAnalysisHint> Analyze();
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Name of the analyzer |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Analyze` | None | `List<StaticAnalysisHint>` | Analyzes the contract and returns hints |

### `StaticAnalysisHint`

Represents a hint from static analysis.

```csharp
namespace Neo.SmartContract.Fuzzer.StaticAnalysis
{
    public class StaticAnalysisHint
    {
        // Properties
        public string RiskType { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Location { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `RiskType` | `string` | Type of risk |
| `Description` | `string` | Description of the hint |
| `Severity` | `string` | Severity of the hint (Low, Medium, High, Critical) |
| `Location` | `string` | Location of the hint |
| `Metadata` | `Dictionary<string, object>` | Additional metadata about the hint |

## Constraint Solving

### `IConstraintSolver`

Interface for constraint solvers.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public interface IConstraintSolver
    {
        // Methods
        bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints);
        Dictionary<string, object> GetModel(IEnumerable<SymbolicExpression> constraints);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `IsSatisfiable` | `IEnumerable<SymbolicExpression> constraints` | `bool` | Checks if the constraints are satisfiable |
| `GetModel` | `IEnumerable<SymbolicExpression> constraints` | `Dictionary<string, object>` | Gets a model that satisfies the constraints |

## Utility Classes

### `Logger`

Utility class for logging.

```csharp
namespace Neo.SmartContract.Fuzzer.Logging
{
    public static class Logger
    {
        // Methods
        public static void Debug(string message);
        public static void Info(string message);
        public static void Warning(string message);
        public static void Error(string message);
        public static void Exception(Exception ex, string message = null);
        public static void Verbose(string message);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Debug` | `string message` | `void` | Logs a debug message |
| `Info` | `string message` | `void` | Logs an info message |
| `Warning` | `string message` | `void` | Logs a warning message |
| `Error` | `string message` | `void` | Logs an error message |
| `Exception` | `Exception ex, string message = null` | `void` | Logs an exception with an optional message |
| `Verbose` | `string message` | `void` | Logs a verbose message (only shown when verbose logging is enabled) |

### `MinimizationPredicates`

Utility class with predefined predicates for test case minimization.

```csharp
namespace Neo.SmartContract.Fuzzer.Minimization
{
    public static class MinimizationPredicates
    {
        // Methods
        public static Predicate<ExecutionResult> FailsExecution();
        public static Predicate<ExecutionResult> FailsWithExceptionMessage(string pattern);
        public static Predicate<ExecutionResult> ConsumesMoreGasThan(long threshold);
        public static Predicate<ExecutionResult> AccessesStorage();
        public static Predicate<ExecutionResult> EmitsEvent(string eventName);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `FailsExecution` | None | `Predicate<ExecutionResult>` | Returns a predicate that checks if the execution fails |
| `FailsWithExceptionMessage` | `string pattern` | `Predicate<ExecutionResult>` | Returns a predicate that checks if the execution fails with an exception message matching the pattern |
| `ConsumesMoreGasThan` | `long threshold` | `Predicate<ExecutionResult>` | Returns a predicate that checks if the execution consumes more gas than the threshold |
| `AccessesStorage` | None | `Predicate<ExecutionResult>` | Returns a predicate that checks if the execution accesses storage |
| `EmitsEvent` | `string eventName` | `Predicate<ExecutionResult>` | Returns a predicate that checks if the execution emits an event with the specified name |

## Symbolic Types

### `SymbolicValue`

Base class for symbolic values.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    public abstract class SymbolicValue
    {
        // Properties
        public string Type { get; }

        // Methods
        public abstract SymbolicValue Clone();
        public abstract bool Equals(SymbolicValue other);
        public abstract string ToString();
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `string` | Type of the symbolic value |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Clone` | None | `SymbolicValue` | Creates a clone of the symbolic value |
| `Equals` | `SymbolicValue other` | `bool` | Checks if this value equals another |
| `ToString` | None | `string` | Returns a string representation |

### `SymbolicVariable`

Represents a symbolic variable.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    public class SymbolicVariable : SymbolicValue
    {
        // Constructors
        public SymbolicVariable(string name, string type);

        // Properties
        public string Name { get; }
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `name` | `string` | Name of the variable |
| `type` | `string` | Type of the variable |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Name of the variable |

### `SymbolicExpression`

Represents a symbolic expression.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    public class SymbolicExpression : SymbolicValue
    {
        // Constructors
        public SymbolicExpression(SymbolicValue left, Operator op, SymbolicValue right);

        // Properties
        public SymbolicValue Left { get; }
        public Operator Op { get; }
        public SymbolicValue Right { get; }
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `left` | `SymbolicValue` | Left operand |
| `op` | `Operator` | Operator |
| `right` | `SymbolicValue` | Right operand |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Left` | `SymbolicValue` | Left operand |
| `Op` | `Operator` | Operator |
| `Right` | `SymbolicValue` | Right operand |

### `ConcreteValue<T>`

Represents a concrete value.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    public class ConcreteValue<T> : SymbolicValue
    {
        // Constructors
        public ConcreteValue(T value);

        // Properties
        public T Value { get; }
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `value` | `T` | Concrete value |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `T` | Concrete value |

## Extension Points

### `IOperationHandler`

Interface for operation handlers.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    public interface IOperationHandler
    {
        // Properties
        string Name { get; }

        // Methods
        bool HandleOperation(Instruction instruction);
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Name of the operation handler |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `HandleOperation` | `Instruction instruction` | `bool` | Handles an operation |

### `IEvaluationService`

Interface for evaluation services.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    public interface IEvaluationService
    {
        // Methods
        SymbolicValue EvaluateUnary(Operator op, SymbolicValue operand);
        SymbolicValue EvaluateBinary(SymbolicValue left, Operator op, SymbolicValue right);
        SymbolicValue EvaluateConversion(SymbolicValue value, string targetType);
        bool EvaluateCondition(SymbolicExpression condition);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `EvaluateUnary` | `Operator op, SymbolicValue operand` | `SymbolicValue` | Evaluates a unary operation |
| `EvaluateBinary` | `SymbolicValue left, Operator op, SymbolicValue right` | `SymbolicValue` | Evaluates a binary operation |
| `EvaluateConversion` | `SymbolicValue value, string targetType` | `SymbolicValue` | Evaluates a type conversion |
| `EvaluateCondition` | `SymbolicExpression condition` | `bool` | Evaluates a condition |

## Utility Classes

### `ByteArrayComparer`

Utility class for comparing byte arrays.

```csharp
namespace Neo.SmartContract.Fuzzer
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        // Methods
        public bool Equals(byte[] x, byte[] y);
        public int GetHashCode(byte[] obj);
    }
}
```

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Equals` | `byte[] x, byte[] y` | `bool` | Checks if two byte arrays are equal |
| `GetHashCode` | `byte[] obj` | `int` | Gets the hash code for a byte array |

## Example Usage

Here's a complete example of using the Neo Smart Contract Fuzzer programmatically:

```csharp
using Neo.SmartContract.Fuzzer;
using Neo.SmartContract.Fuzzer.Controller;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Create a configuration
        var config = new FuzzerConfiguration
        {
            NefPath = "MyContract.nef",
            ManifestPath = "MyContract.manifest.json",
            OutputDirectory = "fuzzer-results",
            IterationsPerMethod = 1000,
            Seed = 42,
            EnableFeedbackGuidedFuzzing = true,
            EnableTestCaseMinimization = true,
            EnableStaticAnalysis = true,
            EnableSymbolicExecution = false,
            GasLimit = 20_000_000, // 20 GAS
            ReportFormats = new List<string> { "json", "html" }
        };

        // Create a fuzzing controller
        var controller = new FuzzingController(config);

        // Register a custom vulnerability detector
        controller.RegisterVulnerabilityDetector(new CustomDetector());

        // Start the fuzzing process
        await controller.StartAsync();

        // Wait for completion
        await controller.WaitForCompletionAsync();

        // Get and display the results
        var status = controller.GetStatus();
        Console.WriteLine($"Total Executions: {status.TotalExecutions}");
        Console.WriteLine($"Successful Executions: {status.SuccessfulExecutions}");
        Console.WriteLine($"Failed Executions: {status.FailedExecutions}");
        Console.WriteLine($"Issues Found: {status.IssuesFound}");
        Console.WriteLine($"Code Coverage: {status.CodeCoverage:P2}");
        Console.WriteLine($"Execution Time: {status.ElapsedTime}");

        // Get the issues
        var issues = controller.GetIssues();
        foreach (var issue in issues)
        {
            Console.WriteLine($"Issue: {issue.IssueType} - {issue.Description}");
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
    }
}

// Custom vulnerability detector
class CustomDetector : IVulnerabilityDetector
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

        return issues;
    }
}
```

## Conclusion

This API reference provides a comprehensive overview of the Neo Smart Contract Fuzzer's programmatic interface. By leveraging these classes and interfaces, developers can integrate the fuzzer into their own tools and workflows, extend its functionality with custom components, and create advanced testing strategies for Neo smart contracts.
