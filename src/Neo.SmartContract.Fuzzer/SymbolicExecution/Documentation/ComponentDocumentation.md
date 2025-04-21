# Symbolic Virtual Machine Component Documentation

This document defines the purpose, responsibilities, and structure of each component in the refactored symbolic execution framework.

## Core Class Documentation

### SymbolicVirtualMachine.cs

**Purpose:** Serve as the main entry point and coordinator for symbolic execution.

**Responsibilities:**
- Initialize and manage execution state
- Coordinate the overall execution flow
- Maintain execution queues and path exploration
- Provide public API for executing code symbolically
- Log execution progress and debug information

**Key Elements:**
- Fields for script data, constraint solver, and execution state
- Properties for accessing current state and pending states
- Constructor for initializing the virtual machine
- Execute method to start symbolic execution
- ExecuteStep method to process one instruction
- ExecuteTerminalOperation method for handling terminal opcodes
- Basic logging infrastructure

**Dependencies:**
- Operations components (delegated execution of specific operations)
- Evaluation components (evaluation of expressions)
- Utility components (helper functions)

## Operation Classes Documentation

### Operations/StackOperations.cs

**Purpose:** Handle all stack-related operations in the virtual machine.

**Responsibilities:**
- Implement methods for push operations
- Implement methods for pop operations
- Implement methods for duplication and other stack manipulations
- Manage stack state during operations

**Key Elements:**
- ExecuteStackOperation method
- ExecutePush method for different push opcodes
- IsPushOperation helper method
- HandleStackManipulation method

**Dependencies:**
- Core SymbolicVirtualMachine (for state access)

### Operations/ArithmeticOperations.cs

**Purpose:** Handle all arithmetic operations in the virtual machine.

**Responsibilities:**
- Implement methods for basic arithmetic (ADD, SUB, MUL, DIV)
- Implement methods for modulo operations
- Implement methods for increment/decrement operations
- Support both concrete and symbolic arithmetic

**Key Elements:**
- ExecuteArithmeticOperation method
- Support methods for handling different types
- Integration with evaluation components

**Dependencies:**
- Evaluation components (for expression evaluation)
- Core SymbolicVirtualMachine (for state access)

### Operations/ComparisonOperations.cs

**Purpose:** Handle all comparison operations in the virtual machine.

**Responsibilities:**
- Implement methods for equality testing (EQUAL, NOTEQUAL)
- Implement methods for relational operators (LT, GT, LTE, GTE)
- Handle comparisons between different types
- Special handling for ByteString comparisons

**Key Elements:**
- ExecuteComparisonOperation method
- ForkExecutionForByteStringComparison method
- Specialized handlers for different type combinations

**Dependencies:**
- Evaluation components (for expression evaluation)
- ByteString utilities
- Core SymbolicVirtualMachine (for state access)

### Operations/FlowControlOperations.cs

**Purpose:** Handle all flow control operations in the virtual machine.

**Responsibilities:**
- Implement methods for jumps (JMP, JMPIF, JMPIFNOT)
- Manage constraint generation for conditional paths
- Coordinate with constraint solver for feasibility checks
- Handle path forking and exploration

**Key Elements:**
- ExecuteFlowControlOperation method
- ExecuteJump method
- ExecuteConditionalJump method
- Path feasibility checking

**Dependencies:**
- Constraint solver (for feasibility checks)
- Core SymbolicVirtualMachine (for state access)

### Operations/SyscallOperations.cs

**Purpose:** Handle all system call operations in the virtual machine.

**Responsibilities:**
- Implement methods for system calls
- Model effects of system calls on symbolic execution
- Handle Native Contract interactions
- Maintain syscall-specific constraints

**Key Elements:**
- ExecuteSyscall method
- Handlers for different syscall types
- Syscall modeling utilities

**Dependencies:**
- Core SymbolicVirtualMachine (for state access)

## Evaluation Classes Documentation

### Evaluation/ConcreteEvaluation.cs

**Purpose:** Evaluate operations on concrete values.

**Responsibilities:**
- Implement concrete value operations for different types
- Support binary operations on concrete values
- Support unary operations on concrete values
- Handle type conversions and checks

**Key Elements:**
- EvaluateConcreteBinaryOperation method
- EvaluateConcreteUnaryOperation method
- Type-specific evaluation methods

**Dependencies:**
- ByteString utilities (for ByteString operations)

### Evaluation/SymbolicEvaluation.cs

**Purpose:** Create and manipulate symbolic expressions.

**Responsibilities:**
- Create symbolic expressions for operations
- Handle symbolic constraint generation
- Manage interaction with constraint solver
- Support symbolic path explorations

**Key Elements:**
- CreateSymbolicExpression method
- AddConstraint methods
- Simplification utilities

**Dependencies:**
- Constraint solver (for constraint checking)
- ConcreteEvaluation (for mixed symbolic/concrete operations)

## Utility Classes Documentation

### Utils/ByteStringOperations.cs

**Purpose:** Provide utilities for ByteString operations.

**Responsibilities:**
- Implement ByteString-specific operations
- Support ByteString concatenation
- Support ByteString comparison
- Provide string representation utilities

**Key Elements:**
- ByteStringConcat method
- ByteStringEquals method
- ToString helpers for ByteString values

**Dependencies:**
- None (standalone utility)

## Interface Definitions

### ISymbolicOperation

**Purpose:** Define common interface for operation handlers.

**Key Members:**
- ExecuteOperation method

### IEvaluationService

**Purpose:** Define interface for evaluation services.

**Key Members:**
- EvaluateBinaryOperation method
- EvaluateUnaryOperation method