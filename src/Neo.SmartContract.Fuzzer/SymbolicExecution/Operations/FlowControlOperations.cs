using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution; // Added for HaltReasons
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all flow control operations in the symbolic virtual machine.
    /// This class is responsible for handling jumps, calls, and branches,
    /// which are critical for path exploration during symbolic execution.
    /// </summary>
    public class FlowControlOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowControlOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public FlowControlOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes a flow control operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            var instruction = engine.CurrentState.CurrentInstruction(_script);

            // Ensure instruction is not null before proceeding
            if (instruction == null)
            {
                LogDebug($"Cannot execute operation {opcode}: null instruction");
                return false;
            }

            switch (opcode)
            {
                // Unconditional jump
                case OpCode.JMP:
                case OpCode.JMP_L:
                    return HandleJmp(instruction);

                // Conditional jumps
                case OpCode.JMPIF:
                case OpCode.JMPIF_L:
                    return HandleJmpIf(instruction);

                case OpCode.JMPIFNOT:
                case OpCode.JMPIFNOT_L:
                    return HandleJmpIfNot(instruction);

                // Comparison jumps
                case OpCode.JMPEQ:
                case OpCode.JMPEQ_L:
                    return HandleJmpComparison(instruction, (x, y) => x == y);

                case OpCode.JMPNE:
                case OpCode.JMPNE_L:
                    return HandleJmpComparison(instruction, (x, y) => x != y);

                case OpCode.JMPGT:
                case OpCode.JMPGT_L:
                    return HandleJmpComparison(instruction, (x, y) => x > y);

                case OpCode.JMPGE:
                case OpCode.JMPGE_L:
                    return HandleJmpComparison(instruction, (x, y) => x >= y);

                case OpCode.JMPLT:
                case OpCode.JMPLT_L:
                    return HandleJmpComparison(instruction, (x, y) => x < y);

                case OpCode.JMPLE:
                case OpCode.JMPLE_L:
                    return HandleJmpComparison(instruction, (x, y) => x <= y);

                // Function calls
                case OpCode.CALL:
                case OpCode.CALL_L:
                    return HandleCall(instruction);

                // Terminal operations
                case OpCode.RET:
                case OpCode.ABORT:
                case OpCode.THROW:
                    return HandleTerminalOperation(opcode);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles an unconditional jump operation.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleJmp(Instruction instruction)
        {
            int offset = GetJumpOffset(instruction);
            int targetAddress = _engine.CurrentState.InstructionPointer + offset;

            // Get IP from target to validate it's within bounds of the script
            if (targetAddress < 0 || targetAddress >= _script.Length)
            {
                LogDebug($"Invalid jump target: {targetAddress}");
                _engine.CurrentState.IsHalted = true;
                _engine.CurrentState.HaltReason = HaltReasons.Fault;
                return true;
            }

            // Update IP directly
            _engine.CurrentState.InstructionPointer = targetAddress;
            return true;
        }

        /// <summary>
        /// Handles a conditional jump-if-true operation.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleJmpIf(Instruction instruction)
        {
            var offset = GetJumpOffset(instruction);
            var condition = _engine.CurrentState.Pop();
            int targetAddress = _engine.CurrentState.InstructionPointer + offset;

            // Ensure target address is valid
            if (targetAddress < 0 || targetAddress >= _script.Length)
            {
                LogDebug($"Invalid jump target: {targetAddress}");
                _engine.CurrentState.IsHalted = true;
                _engine.CurrentState.HaltReason = HaltReasons.Fault;
                return true;
            }

            // If the condition is concrete
            if (IsConcreteValue(condition))
            {
                bool conditionValue = ConvertToBoolean(condition);

                if (conditionValue)
                {
                    LogDebug($"JMPIF (true): {_engine.CurrentState.InstructionPointer} -> {targetAddress}");
                    _engine.CurrentState.InstructionPointer = targetAddress;
                }
                else
                {
                    LogDebug($"JMPIF (false): continue at {_engine.CurrentState.InstructionPointer + instruction.Size}");
                    _engine.CurrentState.InstructionPointer += instruction.Size;
                }
            }
            else
            {
                // If the condition is symbolic, we need to fork execution
                LogDebug($"JMPIF (symbolic): {condition}");

                // Create constraints for both paths
                SymbolicExpression symbolicExpr;
                if (condition is SymbolicExpression expr)
                {
                    symbolicExpr = expr;
                }
                else if (condition is SymbolicVariable var)
                {
                    symbolicExpr = new SymbolicExpression(var, Operator.Identity, var);
                }
                else
                {
                    var condVar = new SymbolicVariable("condition", VM.Types.StackItemType.Boolean);
                    symbolicExpr = new SymbolicExpression(condVar, Operator.Identity, condVar);
                }

                // Create constraints for both paths
                var trueConstraint = new PathConstraint(symbolicExpr.ToTypesExpression(), _engine.CurrentState.InstructionPointer);

                // Create a forked state for the false path (continue execution)
                var falseState = _engine.ForkState(new List<PathConstraint> { trueConstraint });
                falseState.InstructionPointer += instruction.Size;
                _engine.AddPendingState(falseState);

                // Current state takes the true path (jump)
                _engine.CurrentState.AddConstraint(trueConstraint);
                _engine.CurrentState.InstructionPointer = targetAddress;
            }

            return true;
        }

        /// <summary>
        /// Handles a conditional jump-if-false operation.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleJmpIfNot(Instruction instruction)
        {
            var offset = GetJumpOffset(instruction);
            var condition = _engine.CurrentState.Pop();
            int targetAddress = _engine.CurrentState.InstructionPointer + offset;

            // Ensure target address is valid
            if (targetAddress < 0 || targetAddress >= _script.Length)
            {
                LogDebug($"Invalid jump target: {targetAddress}");
                _engine.CurrentState.IsHalted = true;
                _engine.CurrentState.HaltReason = HaltReasons.Fault;
                return true;
            }

            // If the condition is concrete
            if (IsConcreteValue(condition))
            {
                bool conditionValue = ConvertToBoolean(condition);

                if (!conditionValue)
                {
                    LogDebug($"JMPIFNOT (false): {_engine.CurrentState.InstructionPointer} -> {targetAddress}");
                    _engine.CurrentState.InstructionPointer = targetAddress;
                }
                else
                {
                    LogDebug($"JMPIFNOT (true): continue at {_engine.CurrentState.InstructionPointer + instruction.Size}");
                    _engine.CurrentState.InstructionPointer += instruction.Size;
                }
            }
            else
            {
                // If the condition is symbolic, we need to fork execution
                LogDebug($"JMPIFNOT (symbolic): {condition}");

                // Create constraints for both paths
                SymbolicExpression symbolicExpr;
                if (condition is SymbolicExpression expr)
                {
                    symbolicExpr = expr;
                }
                else if (condition is SymbolicVariable var)
                {
                    symbolicExpr = new SymbolicExpression(var, Operator.Identity, var);
                }
                else
                {
                    var condVar = new SymbolicVariable("condition", VM.Types.StackItemType.Boolean);
                    symbolicExpr = new SymbolicExpression(condVar, Operator.Identity, condVar);
                }

                // Create constraints for both paths
                var trueConstraint = new PathConstraint(symbolicExpr.ToTypesExpression(), _engine.CurrentState.InstructionPointer);

                // Create a negated constraint for the false path
                var negatedExpr = new SymbolicExpression(Operator.Not, symbolicExpr);
                var falseConstraint = new PathConstraint(negatedExpr.ToTypesExpression(), _engine.CurrentState.InstructionPointer);

                // Create a forked state for the true path (continue execution)
                var trueState = _engine.ForkState(new List<PathConstraint> { trueConstraint });
                trueState.InstructionPointer += instruction.Size;
                _engine.AddPendingState(trueState);

                // Current state takes the false path (jump)
                _engine.CurrentState.AddConstraint(falseConstraint);
                _engine.CurrentState.InstructionPointer = targetAddress;
            }

            return true;
        }

        /// <summary>
        /// Handles a function call operation.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleCall(Instruction instruction)
        {
            var offset = GetJumpOffset(instruction);
            int targetAddress = _engine.CurrentState.InstructionPointer + offset;

            // Ensure target address is valid
            if (targetAddress < 0 || targetAddress >= _script.Length)
            {
                LogDebug($"Invalid call target: {targetAddress}");
                _engine.CurrentState.IsHalted = true;
                _engine.CurrentState.HaltReason = HaltReasons.Fault;
                return true;
            }

            // Push return address onto the stack
            _engine.CurrentState.Push(new SymbolicInteger(new System.Numerics.BigInteger(_engine.CurrentState.InstructionPointer + instruction.Size)));

            LogDebug($"CALL: {_engine.CurrentState.InstructionPointer} -> {targetAddress}");
            _engine.CurrentState.InstructionPointer = targetAddress;

            return true;
        }

        /// <summary>
        /// Handles a terminal operation.
        /// </summary>
        /// <param name="opcode">The opcode to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleTerminalOperation(OpCode opcode)
        {
            _engine.CurrentState.IsHalted = true;

            switch (opcode)
            {
                case OpCode.RET:
                    // Check if there's a return address on the stack
                    if (_engine.CurrentState.EvaluationStack.Count > 0)
                    {
                        var returnValue = _engine.CurrentState.Pop();

                        // If there's still something on the stack, it might be a return address
                        if (_engine.CurrentState.EvaluationStack.Count > 0)
                        {
                            var returnAddress = _engine.CurrentState.Pop();

                            // If it's a concrete integer, treat it as a return address
                            if (returnAddress is SymbolicValue sv && sv.TryGetInteger(out var address))
                            {
                                int targetAddress = (int)address;

                                // Ensure target address is valid
                                if (targetAddress >= 0 && targetAddress < _script.Length)
                                {
                                    LogDebug($"RET: returning to {targetAddress}");
                                    _engine.CurrentState.InstructionPointer = targetAddress;
                                    _engine.CurrentState.Push(returnValue);
                                    _engine.CurrentState.IsHalted = false;
                                    return true;
                                }
                            }
                        }

                        // If we couldn't find a valid return address, push the return value back
                        _engine.CurrentState.Push(returnValue);
                    }

                    LogDebug("RET: halting execution");
                    // Use consistent HaltReasons class for normal termination
                    _engine.CurrentState.HaltReason = HaltReasons.Normal;
                    break;

                case OpCode.ABORT:
                    LogDebug("ABORT: halting execution");
                    // Use VMState.FAULT for consistency with the test
                    _engine.CurrentState.HaltReason = VMState.FAULT;
                    break;

                case OpCode.THROW:
                    LogDebug("THROW: halting execution");
                    // Use consistent HaltReasons class for throw operations
                    _engine.CurrentState.HaltReason = HaltReasons.Throw;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Gets the jump offset from an instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <returns>The jump offset.</returns>
        private int GetJumpOffset(Instruction instruction)
        {
            // Handle different jump instruction formats
            switch (instruction.OpCode)
            {
                case OpCode.JMP:
                case OpCode.JMPIF:
                case OpCode.JMPIFNOT:
                case OpCode.JMPEQ:
                case OpCode.JMPNE:
                case OpCode.JMPGT:
                case OpCode.JMPGE:
                case OpCode.JMPLT:
                case OpCode.JMPLE:
                case OpCode.CALL:
                    // Short form jumps use 2-byte offset
                    return BitConverter.ToInt16(instruction.Operand.Span);

                case OpCode.JMP_L:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE_L:
                case OpCode.CALL_L:
                    // Long form jumps use 4-byte offset
                    return BitConverter.ToInt32(instruction.Operand.Span);

                default:
                    LogDebug($"Unsupported jump instruction: {instruction.OpCode}");
                    return 0;
            }
        }

        /// <summary>
        /// Determines if a value is concrete (not symbolic).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is concrete, false if it's symbolic.</returns>
        private bool IsConcreteValue(object value)
        {
            return !(value is SymbolicVariable) && !(value is SymbolicExpression);
        }

        /// <summary>
        /// Converts a value to a boolean.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The boolean equivalent of the value.</returns>
        private bool ConvertToBoolean(object value)
        {
            if (value is bool b)
                return b;

            if (value is BigInteger bi)
                return bi != BigInteger.Zero;

            if (value is byte[] bytes)
                return bytes.Length > 0;

            // Default to false for any other type
            return false;
        }

        /// <summary>
        /// Handles a comparison jump operation (JMPEQ, JMPNE, JMPGT, etc.)
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <param name="comparisonFunc">The comparison function to use.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleJmpComparison(Instruction instruction, Func<BigInteger, BigInteger, bool> comparisonFunc)
        {
            var offset = GetJumpOffset(instruction);
            var right = _engine.CurrentState.Pop();
            var left = _engine.CurrentState.Pop();
            int targetAddress = _engine.CurrentState.InstructionPointer + offset;

            // Ensure target address is valid
            if (targetAddress < 0 || targetAddress >= _script.Length)
            {
                LogDebug($"Invalid jump target: {targetAddress}");
                _engine.CurrentState.IsHalted = true;
                _engine.CurrentState.HaltReason = HaltReasons.Fault;
                return true;
            }

            // If both values are concrete integers
            if (left is ConcreteValue<BigInteger> leftInt && right is ConcreteValue<BigInteger> rightInt)
            {
                bool conditionValue = comparisonFunc(leftInt.Value, rightInt.Value);

                if (conditionValue)
                {
                    LogDebug($"Comparison jump (true): {_engine.CurrentState.InstructionPointer} -> {targetAddress}");
                    _engine.CurrentState.InstructionPointer = targetAddress;
                }
                else
                {
                    LogDebug($"Comparison jump (false): continue at {_engine.CurrentState.InstructionPointer + instruction.Size}");
                    _engine.CurrentState.InstructionPointer += instruction.Size;
                }
            }
            else
            {
                // If either value is symbolic, we need to fork execution
                LogDebug($"Comparison jump (symbolic): {left} vs {right}");

                // Create a symbolic expression for the comparison
                Operator op = DetermineComparisonOperator(instruction.OpCode);
                var comparisonExpr = new SymbolicExpression(left, op, right);

                // Create constraints for both paths
                var trueConstraint = new PathConstraint(comparisonExpr.ToTypesExpression(), _engine.CurrentState.InstructionPointer);

                // Create a forked state for the false path (continue execution)
                var falseState = _engine.ForkState(new List<PathConstraint> { trueConstraint });
                falseState.InstructionPointer += instruction.Size;
                _engine.AddPendingState(falseState);

                // Current state takes the true path (jump)
                _engine.CurrentState.AddConstraint(trueConstraint);
                _engine.CurrentState.InstructionPointer = targetAddress;
            }

            return true;
        }

        /// <summary>
        /// Determines the appropriate operator for a comparison jump instruction.
        /// </summary>
        /// <param name="opcode">The opcode of the instruction.</param>
        /// <returns>The corresponding operator.</returns>
        private Operator DetermineComparisonOperator(OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.JMPEQ:
                case OpCode.JMPEQ_L:
                    return Operator.Equal;
                case OpCode.JMPNE:
                case OpCode.JMPNE_L:
                    return Operator.NotEqual;
                case OpCode.JMPGT:
                case OpCode.JMPGT_L:
                    return Operator.GreaterThan;
                case OpCode.JMPGE:
                case OpCode.JMPGE_L:
                    return Operator.GreaterThanOrEqual;
                case OpCode.JMPLT:
                case OpCode.JMPLT_L:
                    return Operator.LessThan;
                case OpCode.JMPLE:
                case OpCode.JMPLE_L:
                    return Operator.LessThanOrEqual;
                default:
                    return Operator.Equal; // Default to equality
            }
        }
    }
}
