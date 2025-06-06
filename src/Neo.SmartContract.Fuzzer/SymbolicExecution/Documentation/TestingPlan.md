# Testing Plan for Refactored Symbolic Virtual Machine

## Overview

This document outlines a comprehensive testing strategy to ensure that the refactored SymbolicVirtualMachine implementation behaves identically to the original implementation while improving code maintainability and organization.

## Test Categories

### 1. Unit Tests for Individual Components

#### Stack Operations Tests
- Test pushing and popping different value types
- Test stack manipulation operations (DUP, SWAP)
- Test proper IP advancement
- Test error cases (stack underflow)

#### Arithmetic Operations Tests
- Test basic arithmetic (ADD, SUB, MUL, DIV)
- Test concrete value operations
- Test symbolic value operations
- Test mixed concrete/symbolic operations

#### Comparison Operations Tests
- Test equality and inequality operations
- Test relational operators (LT, GT, etc.)
- Test ByteString comparison with symbolic variables
- Test constraint generation for comparisons

#### Flow Control Operations Tests
- Test unconditional jumps (JMP)
- Test conditional jumps (JMPIF, JMPIFNOT)
- Test path constraint generation
- Test path feasibility checking

#### Terminal Operations Tests
- Test RET, ABORT, and THROW operations
- Verify correct halt reasons
- Ensure ABORT is handled properly according to our previous fix

#### Syscall Operations Tests
- Test common syscall handling
- Test symbolic result generation
- Test constraint tracking for syscalls

### 2. Integration Tests

#### Behavioral Equivalence Tests
- Execute identical scripts with both implementations
- Compare execution paths and results
- Verify constraint generation matches

#### End-to-End Test Cases
- Test all operations combined in realistic scripts
- Compare path exploration and branching
- Verify concrete results match expected values

#### Edge Case Tests
- Test error handling and recovery
- Test large scripts and complex control flow
- Test maximum stack depth scenarios

### 3. Compatibility Tests

#### SymbolicExecutionEngine Compatibility
- Verify SymbolicExecutionEngine works with refactored VM
- Test all functionality exposed through the engine
- Ensure no regression in existing tests

#### Detector Compatibility
- Test all vulnerability detectors with the refactored VM
- Verify detection results match original implementation
- Ensure constraint solving integration works correctly

#### Performance Tests
- Compare execution time for both implementations
- Measure memory usage patterns
- Identify and optimize critical paths

## Test Implementation Strategy

### 1. Test Fixtures

Create test fixtures for different components:
- `StackOperationsTests`
- `ArithmeticOperationsTests`
- `ComparisonOperationsTests`
- `FlowControlOperationsTests`
- `SyscallOperationsTests`
- `SymbolicVirtualMachineTests` (for integration testing)

### 2. Comparison Test Framework

Develop a framework to compare the behavior of both implementations:
```csharp
public void CompareImplementations(byte[] script, SymbolicState initialState = null)
{
    // Create both VMs
    var originalVM = new SymbolicVirtualMachine(script, _solver, initialState?.Clone());
    var refactoredVM = new RefactoredSymbolicVirtualMachine(script, _solver, initialState?.Clone());
    
    // Execute both
    var originalResult = originalVM.Execute();
    var refactoredResult = refactoredVM.Execute();
    
    // Compare results
    Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
    
    // Compare each path
    for (int i = 0; i < originalResult.ExecutionPaths.Count; i++)
    {
        CompareExecutionPaths(originalResult.ExecutionPaths[i], refactoredResult.ExecutionPaths[i]);
    }
}

private void CompareExecutionPaths(ExecutionPath original, ExecutionPath refactored)
{
    Assert.AreEqual(original.HaltReason, refactored.HaltReason);
    CompareSymbolicStates(original.FinalState, refactored.FinalState);
}

private void CompareSymbolicStates(SymbolicState original, SymbolicState refactored)
{
    // Compare stack contents
    Assert.AreEqual(original.Stack.Count, refactored.Stack.Count);
    
    // Compare path constraints
    Assert.AreEqual(original.PathConstraints.Count, refactored.PathConstraints.Count);
    
    // Compare other state properties
    // ...
}
```

### 3. Critical Test Cases

Focus on these high-priority test cases:

1. **TestSimpleBranchAndAbort** - ensures correct handling of ABORT opcode and path constraints
2. **TestHash160Comparison** - verifies ByteString comparison with symbolic variables
3. **TestSyscallHandling** - checks correct handling of system calls
4. **TestComplexControlFlow** - tests multiple paths with various constraints
5. **TestStackOperations** - validates correct stack manipulation
6. **TestArithmeticOperations** - ensures correct arithmetic behavior

### 4. Test Data Generation

Create scripts programmatically to test specific behaviors:
```csharp
private byte[] CreateComparisonScript(OpCode comparisonOp)
{
    return new byte[]
    {
        (byte)OpCode.PUSH1,           // Push 1
        (byte)OpCode.PUSH2,           // Push 2
        (byte)comparisonOp,           // Compare
        (byte)OpCode.JMPIF,           // Jump if true
        0x03,                         // Jump offset
        (byte)OpCode.THROW,           // Throw if false
        (byte)OpCode.RET              // Return if true
    };
}
```

## Acceptance Criteria

Tests are considered successful when:

1. All unit tests for individual components pass
2. All integration tests with both implementations produce identical results
3. All existing SymbolicExecutionEngineIntegrationTests pass with the refactored VM
4. Performance is within 10% of the original implementation
5. No regressions in vulnerability detection logic

## Special Cases to Test

1. **ABORT opcode handling** - Ensure we properly set HaltReason.Abort for ABORT operations, as per our previous fix
2. **ByteString comparison** - Verify both equal and not-equal paths are explored when comparing ByteString with symbolic variables
3. **Constraint solver integration** - Confirm constraint solver is called for path feasibility checks
4. **Complex data type handling** - Test interactions between different data types (int, bool, ByteString)
5. **Error cases** - Verify proper error handling and propagation

## Test Execution Plan

1. Implement unit tests for each component
2. Create comparison framework for integration testing
3. Add specific test cases for known edge cases and past bugs
4. Run full test suite with both implementations
5. Analyze and fix any discrepancies
6. Measure and optimize performance
7. Document test results and findings

## Reporting

For each test run, record:
- Number of tests passed/failed
- Any discrepancies between implementations
- Performance metrics
- Coverage statistics

Include this information in merge requests when integrating new components.