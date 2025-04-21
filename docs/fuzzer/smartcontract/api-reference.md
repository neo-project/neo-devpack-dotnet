# Neo Smart Contract Fuzzer: API Reference

This document provides a comprehensive reference for the Neo Smart Contract Fuzzer API, allowing developers to integrate and extend the fuzzer programmatically.

## Core Classes

### `SmartContractFuzzer`

The main class that orchestrates the fuzzing process.

```csharp
namespace Neo.SmartContract.Fuzzer
{
    public class SmartContractFuzzer
    {
        // Constructors
        public SmartContractFuzzer(FuzzerConfiguration config);
        
        // Methods
        public void Run();
        public void DetectVulnerabilities();
        
        // Properties
        public FuzzerConfiguration Configuration { get; }
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
| `Run()` | `void` | Runs the fuzzing process |
| `DetectVulnerabilities()` | `void` | Runs vulnerability detection on the contract |

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
        public int Iterations { get; set; } // Alias for IterationsPerMethod
        public long GasLimit { get; set; }
        public int Seed { get; set; }
        public bool EnableCoverage { get; set; }
        public string CoverageFormat { get; set; }
        public bool PersistStateBetweenCalls { get; set; }
        public bool SaveFailingInputsOnly { get; set; }
        public List<string> MethodsToFuzz { get; set; }
        public List<string> MethodsToInclude { get; set; }
        public List<string> MethodsToExclude { get; set; }
        public int MaxSteps { get; set; }
        
        // Methods
        public static FuzzerConfiguration LoadFromFile(string path);
        public void SaveToFile(string path);
    }
}
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NefPath` | `string` | `""` | Path to the NEF file |
| `ManifestPath` | `string` | `""` | Path to the manifest file |
| `OutputDirectory` | `string` | `"fuzzer-output"` | Directory to save execution results |
| `IterationsPerMethod` | `int` | `10` | Number of iterations per method |
| `Iterations` | `int` | `10` | Alias for IterationsPerMethod |
| `GasLimit` | `long` | `20_000_000` | Gas limit per execution |
| `Seed` | `int` | Current timestamp | Random seed for reproducibility |
| `EnableCoverage` | `bool` | `true` | Whether to enable coverage tracking |
| `CoverageFormat` | `string` | `"html"` | Format for coverage reports |
| `PersistStateBetweenCalls` | `bool` | `false` | Whether to persist state between method calls |
| `SaveFailingInputsOnly` | `bool` | `false` | Whether to save only failing inputs |
| `MethodsToFuzz` | `List<string>` | `[]` | List of methods to fuzz |
| `MethodsToInclude` | `List<string>` | `[]` | List of methods to include in vulnerability detection |
| `MethodsToExclude` | `List<string>` | `[]` | List of methods to exclude from fuzzing |
| `MaxSteps` | `int` | `10000` | Maximum number of steps to execute |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `LoadFromFile` | `string path` | `FuzzerConfiguration` | Loads configuration from a JSON file |
| `SaveToFile` | `string path` | `void` | Saves configuration to a JSON file |

### `ContractExecutor`

Executes the contract with generated inputs.

```csharp
namespace Neo.SmartContract.Fuzzer
{
    public class ContractExecutor
    {
        // Constructors
        public ContractExecutor(byte[] nefBytes, ContractManifest manifest, FuzzerConfiguration config);
        
        // Methods
        public ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iteration);
        
        // Static Methods
        public static object ConvertStackItemToJson(StackItem item);
        public static StackItem ConvertJsonElementToStackItem(JsonElement element, ContractParameterType expectedType);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `nefBytes` | `byte[]` | NEF file bytes |
| `manifest` | `ContractManifest` | Contract manifest |
| `config` | `FuzzerConfiguration` | Fuzzer configuration |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `ExecuteMethod` | `ContractMethodDescriptor method, StackItem[] parameters, int iteration` | `ExecutionResult` | Executes a contract method with the given parameters |

#### Static Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `ConvertStackItemToJson` | `StackItem item` | `object` | Converts a StackItem to a JSON-serializable object |
| `ConvertJsonElementToStackItem` | `JsonElement element, ContractParameterType expectedType` | `StackItem` | Converts a JSON element to a StackItem |

### `ParameterGenerator`

Generates parameters for contract methods.

```csharp
namespace Neo.SmartContract.Fuzzer
{
    public class ParameterGenerator
    {
        // Constructors
        public ParameterGenerator(int seed);
        
        // Methods
        public StackItem GenerateParameter(ContractParameterType type, int depth);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `seed` | `int` | Random seed for reproducibility |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `GenerateParameter` | `ContractParameterType type, int depth` | `StackItem` | Generates a parameter of the specified type |

### `CoverageTracker`

Tracks code coverage during fuzzing.

```csharp
namespace Neo.SmartContract.Fuzzer.Coverage
{
    public class CoverageTracker
    {
        // Constructors
        public CoverageTracker(byte[] nefBytes, ContractManifest manifest, string outputDirectory);
        
        // Methods
        public void TrackExecutionCoverage(ExecutionResult result);
        public void GenerateReport(string format);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `nefBytes` | `byte[]` | NEF file bytes |
| `manifest` | `ContractManifest` | Contract manifest |
| `outputDirectory` | `string` | Directory to save coverage reports |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `TrackExecutionCoverage` | `ExecutionResult result` | `void` | Tracks coverage for an execution result |
| `GenerateReport` | `string format` | `void` | Generates a coverage report in the specified format |

## Symbolic Execution

### `SymbolicExecutionEngine`

The main class for symbolic execution.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public class SymbolicExecutionEngine
    {
        // Constructors
        public SymbolicExecutionEngine(byte[] nefBytes, IConstraintSolver solver, IEnumerable<IVulnerabilityDetector> detectors, IEnumerable<SymbolicVariable> initialVariables, int maxSteps);
        
        // Methods
        public SymbolicExecutionResult Execute();
        
        // Static Methods
        public static List<SymbolicVariable> CreateSymbolicArgumentsForMethod(List<string> paramTypes);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `nefBytes` | `byte[]` | NEF file bytes |
| `solver` | `IConstraintSolver` | Constraint solver |
| `detectors` | `IEnumerable<IVulnerabilityDetector>` | Vulnerability detectors |
| `initialVariables` | `IEnumerable<SymbolicVariable>` | Initial symbolic variables |
| `maxSteps` | `int` | Maximum number of steps to execute |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `Execute` | None | `SymbolicExecutionResult` | Executes the contract symbolically |

#### Static Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `CreateSymbolicArgumentsForMethod` | `List<string> paramTypes` | `List<SymbolicVariable>` | Creates symbolic arguments for a method |

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
        List<VulnerabilityRecord> DetectVulnerabilities(SymbolicState state, ExecutionPath path);
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
| `DetectVulnerabilities` | `SymbolicState state, ExecutionPath path` | `List<VulnerabilityRecord>` | Detects vulnerabilities in an execution path |

### `VulnerabilityRecord`

Represents a detected vulnerability.

```csharp
namespace Neo.SmartContract.Fuzzer.Detectors
{
    public class VulnerabilityRecord
    {
        // Properties
        public string Type { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Severity Severity { get; set; }
        public string Recommendation { get; set; }
    }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `string` | Type of vulnerability |
| `Description` | `string` | Description of the vulnerability |
| `Location` | `string` | Location of the vulnerability |
| `Severity` | `Severity` | Severity of the vulnerability |
| `Recommendation` | `string` | Recommendation for fixing the vulnerability |

### `Severity`

Enum for vulnerability severity levels.

```csharp
namespace Neo.SmartContract.Fuzzer.Detectors
{
    public enum Severity
    {
        Low,
        Medium,
        High,
        Critical
    }
}
```

## Constraint Solving

### `IConstraintSolver`

Interface for constraint solvers.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
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

### `SimpleConstraintSolver`

A simple constraint solver implementation.

```csharp
namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public class SimpleConstraintSolver : IConstraintSolver
    {
        // Constructors
        public SimpleConstraintSolver(int seed);
        
        // Methods
        public bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints);
        public Dictionary<string, object> GetModel(IEnumerable<SymbolicExpression> constraints);
    }
}
```

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `seed` | `int` | Random seed for reproducibility |

#### Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `IsSatisfiable` | `IEnumerable<SymbolicExpression> constraints` | `bool` | Checks if the constraints are satisfiable |
| `GetModel` | `IEnumerable<SymbolicExpression> constraints` | `Dictionary<string, object>` | Gets a model that satisfies the constraints |

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

## Conclusion

This API reference provides a comprehensive overview of the Neo Smart Contract Fuzzer's programmatic interface. By leveraging these classes and interfaces, developers can integrate the fuzzer into their own tools and workflows, extend its functionality with custom components, and create advanced testing strategies for Neo smart contracts.
