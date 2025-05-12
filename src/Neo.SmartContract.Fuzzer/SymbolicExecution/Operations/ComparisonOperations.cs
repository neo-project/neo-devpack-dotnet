using Neo.SmartContract.Fuzzer.SymbolicExecution.Evaluation;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.SmartContract.Fuzzer.SymbolicExecution;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all comparison operations in the symbolic virtual machine.
    /// This class is responsible for creating symbolic path constraints and forking
    /// execution states when branches are encountered.
    /// </summary>
    public class ComparisonOperations : BaseOperations
    {
        /// <summary>
        /// Concrete evaluation service.
        /// </summary>
        private readonly IEvaluationService _concreteEvaluation;

        /// <summary>
        /// Symbolic evaluation service.
        /// </summary>
        private readonly IEvaluationService _symbolicEvaluation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        public ComparisonOperations(ISymbolicExecutionEngine engine) : base(engine)
        {
            _concreteEvaluation = new ConcreteEvaluation();
            _symbolicEvaluation = new SymbolicEvaluation();
        }

        /// <summary>
        /// Executes a comparison operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            // Comparison operations
            if (opcode == OpCode.EQUAL || opcode == OpCode.NOTEQUAL)
            {
                return HandleComparison(opcode);
            }

            // Numeric comparison operations - handles both old and new OpCode style
            // New style uses LT, LE, GT, GE directly for all types
            // Old style used NUMLT, NUMLE, NUMGT, NUMGE specifically for numeric types
            if (opcode == OpCode.NUMEQUAL || opcode == OpCode.NUMNOTEQUAL ||
                opcode == OpCode.LT || opcode == OpCode.LE ||
                opcode == OpCode.GT || opcode == OpCode.GE)
            {
                return HandleNumericComparison(opcode);
            }

            return false;
        }

        /// <summary>
        /// Handles general comparison operations.
        /// </summary>
        /// <param name="opcode">The operation code.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleComparison(OpCode opcode)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug($"Error: CurrentState in HandleComparison is not SymbolicState.");
                // Optionally halt: state?.Halt(VMState.FAULT);
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug($"Error: Stack underflow during {opcode}.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();
            object result;

            // Create symbolic expressions for the comparison
            var equalExpr = new Types.SymbolicExpression(left, Types.Operator.Equal, right);
            var notEqualExpr = new Types.SymbolicExpression(left, Types.Operator.NotEqual, right);

            // Create path constraints for both possible outcomes
            var equalConstraint = new PathConstraint(equalExpr, state.InstructionPointer);
            var notEqualConstraint = new PathConstraint(notEqualExpr, state.InstructionPointer);

            // Check if both operands are concrete
            if (IsConcreteValue(left) && IsConcreteValue(right))
            {
                // If both are concrete, evaluate concretely
                try
                {
                    // Use the appropriate comparison method based on the opcode
                    result = opcode switch
                    {
                        OpCode.EQUAL => _concreteEvaluation.Equal(left, right),
                        OpCode.NOTEQUAL => _concreteEvaluation.NotEqual(left, right),
                        _ => throw new NotSupportedException($"Unsupported comparison opcode: {opcode}")
                    };
                    LogDebug($"Concrete comparison: {left} compared to {right} = {result}");

                    // Ensure the result is a concrete boolean value
                    if (result is bool boolResult)
                    {
                        result = new ConcreteValue<bool>(boolResult);
                    }
                    else if (result is SymbolicValue symbolicResult)
                    {
                        // Keep it as is
                        result = symbolicResult;
                    }
                    else
                    {
                        // Convert to a concrete boolean value
                        result = new ConcreteValue<bool>(Convert.ToBoolean(result));
                    }
                }
                catch (Exception ex)
                {
                    // Handle comparison exceptions
                    LogDebug($"Comparison exception: {ex.Message}");
                    // Create a symbolic result for the exception case
                    _engine.LogDebug("Concrete comparison failed due to exception. Creating symbolic variable.");
                    result = new SymbolicVariable($"comparison_exception_{_engine.CurrentState.InstructionPointer}", StackItemType.Boolean);
                }
            }
            else
            {
                // If at least one is symbolic, evaluate symbolically
                // Use the appropriate comparison method based on the opcode
                result = opcode switch
                {
                    OpCode.EQUAL => _symbolicEvaluation.Equal(left, right),
                    OpCode.NOTEQUAL => _symbolicEvaluation.NotEqual(left, right),
                    _ => throw new NotSupportedException($"Unsupported comparison opcode: {opcode}")
                };
                LogDebug($"Symbolic comparison: {left} compared to {right} = {result}");

                // For symbolic execution, we want to create a concrete boolean result
                // that will be used for the current execution path
                if (result is bool boolResult)
                {
                    result = new ConcreteValue<bool>(boolResult);
                }
                else if (!(result is SymbolicValue))
                {
                    // Convert to a concrete boolean value if it's not already a SymbolicValue
                    result = new ConcreteValue<bool>(Convert.ToBoolean(result));
                };
            }

            // Push the result onto the stack
            state.Push(ConvertToSymbolicValue(result));
            state.InstructionPointer++;

            // If we have symbolic values, fork the execution state to explore both paths
            if (!IsConcreteValue(left) || !IsConcreteValue(right))
            {
                // Try to fork the execution state for both true and false outcomes
                // First, create a new state for the equal case
                var equalState = (SymbolicState)state.Clone();
                equalState.AddConstraint(equalConstraint);
                equalState.Pop(); // Remove the result
                equalState.Push(new ConcreteValue<bool>(true)); // Push true
                _engine.PendingStates.Enqueue(equalState);

                // Then, create a new state for the not-equal case
                var notEqualState = (SymbolicState)state.Clone();
                notEqualState.AddConstraint(notEqualConstraint);
                notEqualState.Pop(); // Remove the result
                notEqualState.Push(new ConcreteValue<bool>(false)); // Push false
                _engine.PendingStates.Enqueue(notEqualState);
            }

            return true;
        }

        /// <summary>
        /// Handles numeric comparison operations.
        /// </summary>
        /// <param name="opcode">The operation code.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNumericComparison(OpCode opcode)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug($"Error: CurrentState in HandleNumericComparison is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug($"Error: Stack underflow during {opcode}.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var rightValue = state.Pop();
            var leftValue = state.Pop();
            object result;

            // Map numeric comparison opcodes to regular comparison opcodes
            // In newer Neo.VM versions, some opcodes have been consolidated
            // LT, LE, GT, GE are already the correct opcodes
            OpCode mappedOpcode = opcode switch
            {
                OpCode.NUMEQUAL => OpCode.EQUAL,
                OpCode.NUMNOTEQUAL => OpCode.NOTEQUAL,
                _ => opcode
            };

            // Convert operands to numeric types if they're concrete
            object numericLeft = leftValue;
            object numericRight = rightValue;

            if (IsConcreteValue(leftValue) && !(leftValue is BigInteger))
            {
                try
                {
                    numericLeft = ConvertToNumeric(leftValue);
                }
                catch
                {
                    numericLeft = leftValue;
                }
            }

            if (IsConcreteValue(rightValue) && !(rightValue is BigInteger))
            {
                try
                {
                    numericRight = ConvertToNumeric(rightValue);
                }
                catch
                {
                    numericRight = rightValue;
                }
            }

            // Check if both operands are concrete after conversion
            if (IsConcreteValue(numericLeft) && IsConcreteValue(numericRight))
            {
                // If both are concrete, evaluate concretely
                try
                {
                    // Use the appropriate comparison method based on the opcode
                    result = mappedOpcode switch
                    {
                        OpCode.LT => _concreteEvaluation.LessThan((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                        OpCode.LE => _concreteEvaluation.LessThanOrEqual((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                        OpCode.GT => _concreteEvaluation.GreaterThan((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                        OpCode.GE => _concreteEvaluation.GreaterThanOrEqual((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                        OpCode.NUMEQUAL => _concreteEvaluation.NumericEquals((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                        OpCode.NUMNOTEQUAL => _concreteEvaluation.NumericNotEquals((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                        _ => throw new NotSupportedException($"Unsupported numeric comparison opcode: {mappedOpcode}")
                    };
                    LogDebug($"Concrete numeric comparison: {numericLeft} compared to {numericRight} = {result}");
                }
                catch (Exception ex)
                {
                    // Handle comparison exceptions
                    LogDebug($"Comparison exception: {ex.Message}");
                    // Create a symbolic result for the exception case
                    _engine.LogDebug("Concrete numeric comparison failed due to exception. Creating symbolic variable.");
                    result = new SymbolicVariable($"numeric_comparison_exception_{_engine.CurrentState.InstructionPointer}", StackItemType.Boolean);
                }
            }
            else
            {
                // If at least one is symbolic, evaluate symbolically
                // Use the appropriate comparison method based on the opcode
                result = mappedOpcode switch
                {
                    OpCode.LT => _symbolicEvaluation.LessThan((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                    OpCode.LE => _symbolicEvaluation.LessThanOrEqual((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                    OpCode.GT => _symbolicEvaluation.GreaterThan((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                    OpCode.GE => _symbolicEvaluation.GreaterThanOrEqual((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                    OpCode.NUMEQUAL => _symbolicEvaluation.NumericEquals((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                    OpCode.NUMNOTEQUAL => _symbolicEvaluation.NumericNotEquals((SymbolicValue)numericLeft, (SymbolicValue)numericRight),
                    _ => throw new NotSupportedException($"Unsupported numeric comparison opcode: {mappedOpcode}")
                };
                LogDebug($"Symbolic numeric comparison: {numericLeft} compared to {numericRight} = {result}");
            }

            state.Push(ConvertToSymbolicValue(result));
            state.InstructionPointer++;
            return true;
        }

        /// <summary>
        /// Converts a value to a numeric type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The numeric equivalent of the value.</returns>
        private object ConvertToNumeric(object value)
        {
            if (value is BigInteger)
                return value;

            if (value is byte[] bytes)
            {
                try
                {
                    return new BigInteger(bytes);
                }
                catch
                {
                    // If conversion fails, return the original value
                    return value;
                }
            }

            if (value is bool b)
                return b ? BigInteger.One : BigInteger.Zero;

            // If we can't convert, return the original value
            return value;
        }

        /// <summary>
        /// Determines if a value is concrete (not symbolic).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is concrete, false if it's symbolic.</returns>
        private bool IsConcreteValue(object value)
        {
            return value is not SymbolicValue && value is not SymbolicExpression;
        }

        /// <summary>
        /// Converts a value to a SymbolicValue if it isn't already.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value as a SymbolicValue.</returns>
        private SymbolicValue ConvertToSymbolicValue(object value)
        {
            if (value is SymbolicValue symbolicValue)
            {
                return symbolicValue;
            }
            else if (value is bool boolValue)
            {
                return boolValue ? new SymbolicBoolean(true) : new SymbolicBoolean(false);
            }
            else if (value is BigInteger bigIntValue)
            {
                return new SymbolicInteger(bigIntValue);
            }
            else if (value is int intValue)
            {
                return new SymbolicInteger(new BigInteger(intValue));
            }
            else if (value is long longValue)
            {
                return new SymbolicInteger(new BigInteger(longValue));
            }
            else if (value is byte[] byteArray)
            {
                return new SymbolicByteArray(byteArray);
            }
            else if (value is string stringValue)
            {
                return new SymbolicByteArray(System.Text.Encoding.UTF8.GetBytes(stringValue));
            }
            else
            {
                // Default fallback - create a symbolic variable
                return new SymbolicVariable($"unknown_{Guid.NewGuid():N}", StackItemType.Any);
            }
        }

        /// <summary>
        /// Creates a path constraint from a comparison.
        /// </summary>
        /// <param name="opcode">The operation code.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <param name="instructionPointer">The instruction pointer.</param>
        /// <returns>A path constraint.</returns>
        public PathConstraint CreateConstraint(OpCode opcode, object left, object right, int instructionPointer)
        {
            // Convert operands to symbolic expressions if they're not already
            // Create a symbolic expression for the comparison
            SymbolicExpression expr;

            SymbolicValue leftValue = left as SymbolicValue ?? new SymbolicVariable("left", VM.Types.StackItemType.Any);
            SymbolicValue rightValue = right as SymbolicValue ?? new SymbolicVariable("right", VM.Types.StackItemType.Any);

            switch (opcode)
            {
                case OpCode.EQUAL:
                    expr = new SymbolicExpression(leftValue, Operator.Equal, rightValue);
                    break;
                case OpCode.NOTEQUAL:
                    expr = new SymbolicExpression(leftValue, Operator.NotEqual, rightValue);
                    break;
                case OpCode.LT:
                    expr = new SymbolicExpression(leftValue, Operator.LessThan, rightValue);
                    break;
                case OpCode.LE:
                    expr = new SymbolicExpression(leftValue, Operator.LessThanOrEqual, rightValue);
                    break;
                case OpCode.GT:
                    expr = new SymbolicExpression(leftValue, Operator.GreaterThan, rightValue);
                    break;
                case OpCode.GE:
                    expr = new SymbolicExpression(leftValue, Operator.GreaterThanOrEqual, rightValue);
                    break;
                default:
                    throw new NotSupportedException($"Opcode {opcode} not supported for constraints");
            }

            // Create the constraint
            return new PathConstraint(expr.ToTypesExpression(), instructionPointer);
        }
    }
}
