# Refactored Symbolic Virtual Machine

## Overview

The refactored `SymbolicVirtualMachine.cs` serves as the main entry point and coordinator for symbolic execution, while delegating specific operations to specialized classes. This document provides a high-level view of how the new structure works together.

## Class Structure

```
SymbolicVirtualMachine
├── Properties
│   ├── CurrentState
│   ├── PendingStates
│   └── Script
├── Fields
│   ├── _script
│   ├── _solver
│   ├── _stackOperations
│   ├── _arithmeticOperations
│   ├── _comparisonOperations
│   ├── _flowControlOperations
│   ├── _syscallOperations
│   ├── _concreteEvaluator
│   └── _symbolicEvaluator
└── Methods
    ├── Constructor
    ├── Execute
    ├── ExecuteStep
    ├── ExecuteTerminalOperation
    └── LogDebug
```

## Execution Flow

1. **Initialization**: When a `SymbolicVirtualMachine` is created, it initializes all necessary operations classes and evaluators.

2. **Execute Method**: The main entry point that drives the symbolic execution process, handling state queues and path exploration.

3. **ExecuteStep Method**: Dispatches the current instruction to the appropriate operation handler based on the opcode.

4. **Operation Handlers**: Specialized classes that implement the `ISymbolicOperation` interface handle specific types of operations:
   - `StackOperations`: Handles stack manipulations (PUSH, POP, etc.)
   - `ArithmeticOperations`: Handles arithmetic operations (ADD, SUB, etc.)
   - `ComparisonOperations`: Handles comparison operations (EQUAL, LT, etc.)
   - `FlowControlOperations`: Handles flow control operations (JMP, JMPIF, etc.)
   - `SyscallOperations`: Handles system calls

5. **Evaluation Services**: Specialized classes that implement the `IEvaluationService` interface handle evaluation of expressions:
   - `ConcreteEvaluation`: Evaluates operations on concrete values
   - `SymbolicEvaluation`: Creates and manipulates symbolic expressions

6. **Utilities**: Helper classes provide common functionality:
   - `ByteStringOperations`: Utilities for working with ByteString values

## Key Improvements

1. **Better Separation of Concerns**: Each class has a clear and focused responsibility, making the code more maintainable.

2. **Improved Testability**: Individual components can be tested in isolation, leading to more robust code.

3. **Enhanced Extensibility**: New operations or evaluators can be added without modifying the core VM.

4. **Reduced Complexity**: The main VM class is significantly smaller and easier to understand.

5. **More Consistent Error Handling**: Centralized error handling in each component.

6. **Better Code Organization**: Related functionality is grouped together logically.

## Migration Plan

1. First create a wrapper around the original SymbolicVirtualMachine that delegates to the original implementation.

2. Incrementally replace functionality in the wrapper with calls to the new components.

3. Once all functionality has been migrated, replace the original implementation with the new one.

4. Update tests to use the new components as needed.

## Code Size Metrics

| File                           | Line Count |
|--------------------------------|------------|
| SymbolicVirtualMachine.cs      | ~250       |
| StackOperations.cs             | ~150       |
| ArithmeticOperations.cs        | ~150       |
| ComparisonOperations.cs        | ~150       |
| FlowControlOperations.cs       | ~200       |
| SyscallOperations.cs           | ~100       |
| ConcreteEvaluation.cs          | ~150       |
| SymbolicEvaluation.cs          | ~100       |
| ByteStringOperations.cs        | ~50        |
| Interfaces & Base Classes      | ~50        |
| **Total**                      | ~1350      |

This represents a modest increase in overall line count compared to the original monolithic implementation (~1000 lines), but with significantly improved maintainability, readability, and testability.