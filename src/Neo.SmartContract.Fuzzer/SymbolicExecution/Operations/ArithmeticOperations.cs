using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Helper class to execute arithmetic operations during symbolic execution.
    /// Handles both concrete and symbolic values.
    /// </summary>
    public class ArithmeticOperations : IOperationHandler
    {
        private readonly ISymbolicExecutionEngine _engine;
        private readonly IEvaluationService _evaluationService;

        public ArithmeticOperations(ISymbolicExecutionEngine engine, IEvaluationService evaluationService)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
        }

        // --- Implementation of IOperationHandler ---
        public bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            // Ensure the engine passed is the one this instance was created with
            if (engine != _engine)
                throw new ArgumentException("Engine mismatch in ExecuteOperation.");

            switch (opcode)
            {
                // Unary
                case OpCode.INC: return HandleInc();
                case OpCode.DEC: return HandleDec();
                case OpCode.SIGN: return HandleSign();
                case OpCode.NEGATE: return HandleNegate();
                case OpCode.ABS: return HandleAbs();
                case OpCode.NOT: return HandleNot();

                // Binary
                case OpCode.ADD: return HandleAdd();
                case OpCode.SUB: return HandleSub();
                case OpCode.MUL: return HandleMul();
                case OpCode.DIV: return HandleDiv();
                case OpCode.MOD: return HandleMod();
                case OpCode.SHL: return HandleShl();
                case OpCode.SHR: return HandleShr();
                case OpCode.BOOLAND: return HandleBoolAnd();
                case OpCode.BOOLOR: return HandleBoolOr();
                case OpCode.MIN: return HandleMin();
                case OpCode.MAX: return HandleMax();

                // Ternary
                case OpCode.WITHIN: return HandleWithin();

                default: return false; // Opcode not handled by this class
            }
        }

        // --- Unary Operations ---

        private bool HandleInc()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during INC.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var value = state.Pop();
            var result = _evaluationService.Add(value, SymbolicInteger.FromLong(1));
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleDec()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during DEC.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var value = state.Pop();
            var result = _evaluationService.Subtract(value, SymbolicInteger.FromLong(1));
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleSign()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during SIGN.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var value = state.Pop();
            var result = _evaluationService.Sign(value);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleNegate()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during NEGATE.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var value = state.Pop();
            var result = _evaluationService.Negate(value);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleAbs()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during ABS.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var value = state.Pop();
            var result = _evaluationService.Abs(value);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleNot()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during NOT.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var value = state.Pop();
            var result = _evaluationService.Not(value);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        // --- Binary Operations ---

        private bool HandleAdd()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during ADD.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();
            var result = _evaluationService.Add(left, right);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleSub()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during SUB.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();
            var result = _evaluationService.Subtract(left, right);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleMul()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during MUL.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();

            // Check for concrete zero multiplication for path pruning potential
            if ((IsConcreteZero(left) && IsConcrete(right)) || (IsConcrete(left) && IsConcreteZero(right)))
            {
                // If one is concrete zero and the other is concrete, result is zero
                state.Push(SymbolicInteger.FromLong(0)); // Push concrete zero
            }
            else
            {
                var result = _evaluationService.Multiply(left, right);
                state.Push(result);
            }
            state.InstructionPointer++;
            return true;
        }

        private bool HandleDiv()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during DIV.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();

            // Check for division by zero
            if (IsConcreteZero(right))
            {
                // Division by concrete zero
                _engine.LogDebug("Division by zero detected.");
                state.Halt(VMState.FAULT); // Halt on division by zero
                return false; // Operation failed due to division by zero
            }

            var result = _evaluationService.Divide(left, right);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleMod()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during MOD.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();

            // Check for modulo by zero
            if (IsConcreteZero(right))
            {
                // Modulo by concrete zero
                _engine.LogDebug("Modulo by zero detected.");
                state.Halt(VMState.FAULT); // Halt on modulo by zero
                return false; // Operation failed
            }

            var result = _evaluationService.Modulo(left, right);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleShl()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during SHL.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var shift = state.Pop();
            var value = state.Pop();

            // Ensure shift amount is within valid range if concrete
            if (shift is SymbolicInteger concreteShift && (concreteShift.Value < 0 || concreteShift.Value > 256))
            {
                _engine.LogDebug($"Invalid shift amount {concreteShift.Value} for SHL.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var result = _evaluationService.ShiftLeft(value, shift);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleShr()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during SHR.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var shift = state.Pop();
            var value = state.Pop();

            // Ensure shift amount is within valid range if concrete
            if (shift is SymbolicInteger concreteShift && (concreteShift.Value < 0 || concreteShift.Value > 256))
            {
                _engine.LogDebug($"Invalid shift amount {concreteShift.Value} for SHR.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var result = _evaluationService.ShiftRight(value, shift);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleMin()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during MIN.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();
            var result = _evaluationService.Min(left, right);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleMax()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during MAX.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();
            var result = _evaluationService.Max(left, right);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleBoolAnd()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during BOOLAND.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();

            // Perform boolean logic directly
            bool leftBool = ConvertToBool(left);
            bool rightBool = ConvertToBool(right);
            var result = new ConcreteValue<bool>(leftBool && rightBool);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        private bool HandleBoolOr()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during BOOLOR.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var right = state.Pop();
            var left = state.Pop();

            // Perform boolean logic directly
            bool leftBool = ConvertToBool(left);
            bool rightBool = ConvertToBool(right);
            var result = new ConcreteValue<bool>(leftBool || rightBool);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        // --- Ternary Operations ---

        private bool HandleWithin()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            if (state.EvaluationStack.Count < 3)
            {
                _engine.LogDebug("Error: Stack underflow during WITHIN.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var upper = state.Pop();
            var lower = state.Pop();
            var value = state.Pop();

            // Check for potential type errors before evaluation
            if (!(value is SymbolicValue) ||
                !(lower is SymbolicValue) ||
                !(upper is SymbolicValue))
            {
                _engine.LogDebug("Invalid operand types for WITHIN.");
                state.Halt(VMState.FAULT);
                return false;
            }

            var result = _evaluationService.Within(value, lower, upper);
            state.Push(result);
            state.InstructionPointer++;
            return true;
        }

        // --- Helper Methods ---

        private bool IsConcrete(SymbolicValue value)
        {
            // Simplified check: assumes SymbolicInteger and SymbolicBoolean are primary concrete types
            return value is SymbolicInteger || value is SymbolicBoolean; // Add other concrete types if needed
        }

        private bool IsConcreteZero(SymbolicValue value)
        {
            return value is SymbolicInteger integer && integer.Value == BigInteger.Zero;
        }

        private bool ConvertToBool(SymbolicValue value)
        {
            if (value is SymbolicBoolean sb) return sb.Value;
            if (value is SymbolicInteger si) return !si.Value.IsZero;
            // Handle other types or throw/log error for non-boolean convertible types
            _engine.LogDebug($"Warning: Attempted to convert non-boolean symbolic value {value.GetType().Name} to bool in BoolAnd/Or.");
            // Defaulting to false, but this might need refinement based on VM semantics
            return false;
        }
    }
}
