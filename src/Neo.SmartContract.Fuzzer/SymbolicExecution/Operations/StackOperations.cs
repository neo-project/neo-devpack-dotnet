using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all stack manipulation operations in the symbolic virtual machine.
    /// </summary>
    public class StackOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly ReadOnlyMemory<byte> _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public StackOperations(ISymbolicExecutionEngine engine, ReadOnlyMemory<byte> script) : base(engine)
        {
            // ReadOnlyMemory<T> is a value type, so we can't use the null-coalescing operator
            // Instead, check if the memory is empty or default
            if (script.IsEmpty)
            {
                throw new ArgumentException("Script cannot be empty", nameof(script));
            }
            _script = script;
        }

        /// <summary>
        /// Executes a stack operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            var state = engine.CurrentState;
            var instruction = state.CurrentInstruction(_script);

            switch (opcode)
            {
                // Push operations
                case OpCode.PUSHINT8:
                    HandlePushInt8(instruction);
                    return true;

                case OpCode.PUSHINT16:
                    HandlePushInt16(instruction);
                    return true;

                case OpCode.PUSHINT32:
                    HandlePushInt32(instruction);
                    return true;

                case OpCode.PUSHINT64:
                    HandlePushInt64(instruction);
                    return true;

                case OpCode.PUSHINT128:
                    HandlePushInt128(instruction);
                    return true;

                case OpCode.PUSHINT256:
                    HandlePushInt256(instruction);
                    return true;

                case OpCode.PUSH0:
                    // Create a concrete value from BigInteger.Zero
                    state.Push(new ConcreteValue<BigInteger>(BigInteger.Zero));
                    // Check if instruction is null before accessing its properties
                    state.InstructionPointer += instruction?.Size ?? 1; // Default to 1 if instruction is null
                    return true;

                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                    // Add null check for instruction before calling HandlePushData
                    if (instruction == null)
                    {
                        state.InstructionPointer++; // Skip the instruction
                        return true;
                    }
                    HandlePushData(instruction);
                    return true;

                case OpCode.PUSH1:
                    // Create a concrete value from BigInteger.One
                    state.Push(new ConcreteValue<BigInteger>(BigInteger.One));
                    // Check if instruction is null before accessing its properties
                    state.InstructionPointer += instruction?.Size ?? 1; // Default to 1 if instruction is null
                    return true;

                case OpCode.PUSH2:
                case OpCode.PUSH3:
                case OpCode.PUSH4:
                case OpCode.PUSH5:
                case OpCode.PUSH6:
                case OpCode.PUSH7:
                case OpCode.PUSH8:
                case OpCode.PUSH9:
                case OpCode.PUSH10:
                case OpCode.PUSH11:
                case OpCode.PUSH12:
                case OpCode.PUSH13:
                case OpCode.PUSH14:
                case OpCode.PUSH15:
                case OpCode.PUSH16:
                    // Convert opcode to integer value (PUSH2 = 2, PUSH3 = 3, etc.)
                    var value = (byte)opcode - (byte)OpCode.PUSH1 + 1;
                    // Create a concrete value from the BigInteger
                    state.Push(new ConcreteValue<BigInteger>(new BigInteger(value)));
                    // Use null conditional operator to avoid null reference
                    state.InstructionPointer += instruction?.Size ?? 1;
                    return true;

                // Stack manipulation operations
                case OpCode.NOP:
                    state.InstructionPointer += instruction?.Size ?? 1;
                    return true;

                case OpCode.DUP:
                    HandleDup();
                    return true;

                case OpCode.SWAP:
                    HandleSwap();
                    return true;

                case OpCode.TUCK:
                    HandleTuck();
                    return true;

                case OpCode.OVER:
                    HandleOver();
                    return true;

                case OpCode.ROT:
                    HandleRot();
                    return true;

                case OpCode.DEPTH:
                    HandleDepth();
                    return true;

                case OpCode.DROP:
                    HandleDrop();
                    return true;

                case OpCode.PICK:
                    HandlePick();
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the PUSHDATA operations.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        private bool HandlePushData(Instruction instruction)
        {
            var operand = instruction.Operand;
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (operand.Length > _engine.Limits.MaxItemSize)
            {
                _engine.LogDebug("Error: PUSHDATA size exceeds limits.");
                state.Halt(VMState.FAULT);
                return false;
            }
            // Convert ReadOnlyMemory<byte> to byte[] using Span.ToArray()
            var operandBytes = operand.Span.ToArray();
            state.Push(new SymbolicByteArray(operandBytes));
            state.InstructionPointer++;
            _engine.LogDebug($"PUSHDATA: {BitConverter.ToString(operandBytes).Replace("-", "")}");
            return true;
        }

        /// <summary>
        /// Handles the DUP operation.
        /// </summary>
        private bool HandleDup()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during DUP.");
                state.Halt(VMState.FAULT);
                return false;
            }
            var item = state.Peek();
            state.Push(item);
            state.InstructionPointer++;

            _engine.LogDebug($"DUP: {item}");
            return true;
        }

        /// <summary>
        /// Handles the SWAP operation.
        /// </summary>
        private bool HandleSwap()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during SWAP.");
                state.Halt(VMState.FAULT);
                return false;
            }
            var a = state.Pop();
            var b = state.Pop();
            state.Push(a);
            state.Push(b);
            state.InstructionPointer++;

            _engine.LogDebug($"SWAP: {a} <-> {b}");
            return true;
        }

        /// <summary>
        /// Handles the TUCK operation.
        /// </summary>
        private bool HandleTuck()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during TUCK.");
                state.Halt(VMState.FAULT);
                return false;
            }
            var a = state.Pop();
            var b = state.Pop();
            state.Push(a);
            state.Push(b);
            state.Push(a);
            state.InstructionPointer++;

            _engine.LogDebug($"TUCK: {a}, {b} -> {a}, {b}, {a}");
            return true;
        }

        /// <summary>
        /// Handles the OVER operation.
        /// </summary>
        private bool HandleOver()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (state.EvaluationStack.Count < 2)
            {
                _engine.LogDebug("Error: Stack underflow during OVER.");
                state.Halt(VMState.FAULT);
                return false;
            }
            var a = state.Pop();
            var b = state.Pop();
            state.Push(b);
            state.Push(a);
            state.Push(b);
            state.InstructionPointer++;

            _engine.LogDebug($"OVER: {a}, {b} -> {b}, {a}, {b}");
            return true;
        }

        /// <summary>
        /// Handles the ROT operation.
        /// </summary>
        private bool HandleRot()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (state.EvaluationStack.Count < 3)
            {
                _engine.LogDebug("Error: Stack underflow during ROT.");
                state.Halt(VMState.FAULT);
                return false;
            }
            var a = state.Pop();
            var b = state.Pop();
            var c = state.Pop();
            state.Push(b);
            state.Push(a);
            state.Push(c);
            state.InstructionPointer++;

            _engine.LogDebug($"ROT: {a}, {b}, {c} -> {b}, {a}, {c}");
            return true;
        }

        /// <summary>
        /// Handles the DEPTH operation.
        /// </summary>
        private bool HandleDepth()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            try
            {
                int depth = state.EvaluationStack.Count;
                state.Push(new ConcreteValue<BigInteger>(new BigInteger(depth)));
                state.InstructionPointer++;

                _engine.LogDebug($"DEPTH: {depth}");
                return true;
            }
            catch (Exception ex)
            {
                _engine.LogDebug($"Error in DEPTH operation: {ex.Message}");
                // Push a symbolic value instead of failing
                state.Push(new SymbolicVariable("stack_depth", VM.Types.StackItemType.Integer));
                state.InstructionPointer++;
                return true;
            }
        }

        /// <summary>
        /// Handles the DROP operation.
        /// </summary>
        private bool HandleDrop()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (state.EvaluationStack.Count < 1)
            {
                _engine.LogDebug("Error: Stack underflow during DROP.");
                state.Halt(VMState.FAULT);
                return false;
            }
            state.Pop();
            state.InstructionPointer++;

            _engine.LogDebug($"DROP");
            return true;
        }

        /// <summary>
        /// Handles the PUSHINT8 operation.
        /// </summary>
        private bool HandlePushInt8(Instruction instruction)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (instruction == null || instruction.Operand.IsEmpty)
            {
                _engine.LogDebug("Error: PUSHINT8 instruction or operand is invalid.");
                state.Halt(VMState.FAULT);
                return false;
            }

            // Get the 1-byte signed integer from the operand
            var value = new BigInteger(instruction.Operand.Span[0]);
            if (instruction.Operand.Span[0] >= 128) // Handle negative numbers (two's complement)
            {
                value -= 256; // Convert to negative
            }

            // Push the value onto the stack
            state.Push(new ConcreteValue<BigInteger>(value));
            state.InstructionPointer += instruction.Size;

            _engine.LogDebug($"PUSHINT8: {value}");
            return true;
        }

        /// <summary>
        /// Handles the PUSHINT16 operation.
        /// </summary>
        private bool HandlePushInt16(Instruction instruction)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (instruction == null || instruction.Operand.IsEmpty || instruction.Operand.Length < 2)
            {
                _engine.LogDebug("Error: PUSHINT16 instruction or operand is invalid.");
                state.Halt(VMState.FAULT);
                return false;
            }

            // Get the 2-byte signed integer from the operand (little-endian)
            var value = BitConverter.ToInt16(instruction.Operand.Span);

            // Push the value onto the stack
            state.Push(new ConcreteValue<BigInteger>(value));
            state.InstructionPointer += instruction.Size;

            _engine.LogDebug($"PUSHINT16: {value}");
            return true;
        }

        /// <summary>
        /// Handles the PUSHINT32 operation.
        /// </summary>
        private bool HandlePushInt32(Instruction instruction)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (instruction == null || instruction.Operand.IsEmpty || instruction.Operand.Length < 4)
            {
                _engine.LogDebug("Error: PUSHINT32 instruction or operand is invalid.");
                state.Halt(VMState.FAULT);
                return false;
            }

            // Get the 4-byte signed integer from the operand (little-endian)
            var value = BitConverter.ToInt32(instruction.Operand.Span);

            // Push the value onto the stack
            state.Push(new ConcreteValue<BigInteger>(value));
            state.InstructionPointer += instruction.Size;

            _engine.LogDebug($"PUSHINT32: {value}");
            return true;
        }

        /// <summary>
        /// Handles the PUSHINT64 operation.
        /// </summary>
        private bool HandlePushInt64(Instruction instruction)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (instruction == null || instruction.Operand.IsEmpty || instruction.Operand.Length < 8)
            {
                _engine.LogDebug("Error: PUSHINT64 instruction or operand is invalid.");
                state.Halt(VMState.FAULT);
                return false;
            }

            // Get the 8-byte signed integer from the operand (little-endian)
            var value = BitConverter.ToInt64(instruction.Operand.Span);

            // Push the value onto the stack
            state.Push(new ConcreteValue<BigInteger>(value));
            state.InstructionPointer += instruction.Size;

            _engine.LogDebug($"PUSHINT64: {value}");
            return true;
        }

        /// <summary>
        /// Handles the PUSHINT128 operation.
        /// </summary>
        private bool HandlePushInt128(Instruction instruction)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (instruction == null || instruction.Operand.IsEmpty || instruction.Operand.Length < 16)
            {
                _engine.LogDebug("Error: PUSHINT128 instruction or operand is invalid.");
                state.Halt(VMState.FAULT);
                return false;
            }

            // Create a BigInteger from the 16-byte array (little-endian)
            var value = new BigInteger(instruction.Operand.Span, isUnsigned: false, isBigEndian: false);

            // Push the value onto the stack
            state.Push(new ConcreteValue<BigInteger>(value));
            state.InstructionPointer += instruction.Size;

            _engine.LogDebug($"PUSHINT128: {value}");
            return true;
        }

        /// <summary>
        /// Handles the PUSHINT256 operation.
        /// </summary>
        private bool HandlePushInt256(Instruction instruction)
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }
            if (instruction == null || instruction.Operand.IsEmpty || instruction.Operand.Length < 32)
            {
                _engine.LogDebug("Error: PUSHINT256 instruction or operand is invalid.");
                state.Halt(VMState.FAULT);
                return false;
            }

            // Create a BigInteger from the 32-byte array (little-endian)
            var value = new BigInteger(instruction.Operand.Span, isUnsigned: false, isBigEndian: false);

            // Push the value onto the stack
            state.Push(new ConcreteValue<BigInteger>(value));
            state.InstructionPointer += instruction.Size;

            _engine.LogDebug($"PUSHINT256: {value}");
            return true;
        }

        /// <summary>
        /// Handles the PICK operation.
        /// </summary>
        private bool HandlePick()
        {
            if (_engine.CurrentState is not SymbolicState state)
            {
                _engine.LogDebug("Error: CurrentState is not SymbolicState.");
                return false;
            }

            try
            {
                if (state.EvaluationStack.Count < 1)
                {
                    _engine.LogDebug("Error: Stack underflow trying to read index for PICK.");
                    // Create a symbolic variable instead of failing
                    state.Push(new SymbolicVariable("pick_result", VM.Types.StackItemType.Any));
                    state.InstructionPointer++;
                    return true;
                }

                var nValue = state.Pop();
                int index;

                // Handle different types of values for the index
                if (nValue is SymbolicInteger indexValue)
                {
                    index = (int)indexValue.Value;
                }
                else if (nValue is ConcreteValue<BigInteger> concreteValue)
                {
                    index = (int)concreteValue.Value;
                }
                else if (nValue is ConcreteValue<int> concreteIntValue)
                {
                    index = concreteIntValue.Value;
                }
                else
                {
                    _engine.LogDebug($"PICK index is not a concrete integer: {nValue}. Using symbolic result.");
                    state.Push(new SymbolicVariable("pick_result", VM.Types.StackItemType.Any));
                    state.InstructionPointer++;
                    return true;
                }

                if (index < 0)
                {
                    _engine.LogDebug("Error: Negative index for PICK.");
                    state.Push(new SymbolicVariable("pick_result_negative_index", VM.Types.StackItemType.Any));
                    state.InstructionPointer++;
                    return true;
                }

                if (index >= state.EvaluationStack.Count)
                {
                    _engine.LogDebug($"Error: PICK index {index} out of bounds.");
                    state.Push(new SymbolicVariable("pick_result_out_of_bounds", VM.Types.StackItemType.Any));
                    state.InstructionPointer++;
                    return true;
                }

                var item = state.Peek(index);
                state.Push(item);
                state.InstructionPointer++;

                _engine.LogDebug($"PICK: {index} -> {item}");
                return true;
            }
            catch (Exception ex)
            {
                _engine.LogDebug($"Error in PICK operation: {ex.Message}");
                // Push a symbolic value instead of failing
                state.Push(new SymbolicVariable("pick_result_exception", VM.Types.StackItemType.Any));
                state.InstructionPointer++;
                return true;
            }
        }
    }
}
