using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all slot operations in the symbolic virtual machine.
    /// This class is responsible for operations on local variables, arguments, and static fields.
    /// </summary>
    public class SlotOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlotOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public SlotOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes a slot operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.INITSSLOT:
                    return HandleInitSSlot();
                case OpCode.INITSLOT:
                    return HandleInitSlot();
                case OpCode.LDSFLD0:
                    return HandleLoadStaticField(0);
                case OpCode.LDSFLD1:
                    return HandleLoadStaticField(1);
                case OpCode.LDSFLD2:
                    return HandleLoadStaticField(2);
                case OpCode.LDSFLD3:
                    return HandleLoadStaticField(3);
                case OpCode.LDSFLD4:
                    return HandleLoadStaticField(4);
                case OpCode.LDSFLD5:
                    return HandleLoadStaticField(5);
                case OpCode.LDSFLD6:
                    return HandleLoadStaticField(6);
                case OpCode.LDSFLD:
                    return HandleLoadStaticField();
                case OpCode.STSFLD0:
                    return HandleStoreStaticField(0);
                case OpCode.STSFLD1:
                    return HandleStoreStaticField(1);
                case OpCode.STSFLD2:
                    return HandleStoreStaticField(2);
                case OpCode.STSFLD3:
                    return HandleStoreStaticField(3);
                case OpCode.STSFLD4:
                    return HandleStoreStaticField(4);
                case OpCode.STSFLD5:
                    return HandleStoreStaticField(5);
                case OpCode.STSFLD6:
                    return HandleStoreStaticField(6);
                case OpCode.STSFLD:
                    return HandleStoreStaticField();
                case OpCode.LDLOC0:
                    return HandleLoadLocalVariable(0);
                case OpCode.LDLOC1:
                    return HandleLoadLocalVariable(1);
                case OpCode.LDLOC2:
                    return HandleLoadLocalVariable(2);
                case OpCode.LDLOC3:
                    return HandleLoadLocalVariable(3);
                case OpCode.LDLOC4:
                    return HandleLoadLocalVariable(4);
                case OpCode.LDLOC5:
                    return HandleLoadLocalVariable(5);
                case OpCode.LDLOC6:
                    return HandleLoadLocalVariable(6);
                case OpCode.LDLOC:
                    return HandleLoadLocalVariable();
                case OpCode.STLOC0:
                    return HandleStoreLocalVariable(0);
                case OpCode.STLOC1:
                    return HandleStoreLocalVariable(1);
                case OpCode.STLOC2:
                    return HandleStoreLocalVariable(2);
                case OpCode.STLOC3:
                    return HandleStoreLocalVariable(3);
                case OpCode.STLOC4:
                    return HandleStoreLocalVariable(4);
                case OpCode.STLOC5:
                    return HandleStoreLocalVariable(5);
                case OpCode.STLOC6:
                    return HandleStoreLocalVariable(6);
                case OpCode.STLOC:
                    return HandleStoreLocalVariable();
                case OpCode.LDARG0:
                    return HandleLoadArgument(0);
                case OpCode.LDARG1:
                    return HandleLoadArgument(1);
                case OpCode.LDARG2:
                    return HandleLoadArgument(2);
                case OpCode.LDARG3:
                    return HandleLoadArgument(3);
                case OpCode.LDARG4:
                    return HandleLoadArgument(4);
                case OpCode.LDARG5:
                    return HandleLoadArgument(5);
                case OpCode.LDARG6:
                    return HandleLoadArgument(6);
                case OpCode.LDARG:
                    return HandleLoadArgument();
                case OpCode.STARG0:
                    return HandleStoreArgument(0);
                case OpCode.STARG1:
                    return HandleStoreArgument(1);
                case OpCode.STARG2:
                    return HandleStoreArgument(2);
                case OpCode.STARG3:
                    return HandleStoreArgument(3);
                case OpCode.STARG4:
                    return HandleStoreArgument(4);
                case OpCode.STARG5:
                    return HandleStoreArgument(5);
                case OpCode.STARG6:
                    return HandleStoreArgument(6);
                case OpCode.STARG:
                    return HandleStoreArgument();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the INITSSLOT operation, which initializes the static field slot.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleInitSSlot()
        {
            // Get the number of static fields from the instruction operand
            var instruction = _engine.CurrentState.CurrentInstruction(_script);
            if (instruction == null || instruction.Operand.IsEmpty)
            {
                LogDebug("INITSSLOT: Invalid instruction or operand");
                return false;
            }

            // The operand is a single byte representing the number of static fields
            var staticFieldCount = instruction.Operand.Span[0];

            // Get the current state
            var state = _engine.CurrentState as SymbolicState;
            if (state == null)
            {
                LogDebug("INITSSLOT: Current state is not a SymbolicState");
                return false;
            }

            // Create a new slot with the specified number of static fields
            var slot = new SymbolicSlot(staticFieldCount, 0, 0);

            // Set the slot in the state using reflection
            var slotField = typeof(SymbolicState).GetField("_slot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (slotField == null)
            {
                LogDebug("INITSSLOT: Could not find _slot field in SymbolicState");
                return false;
            }

            slotField.SetValue(state, slot);

            LogDebug($"Initialized static field slot with {staticFieldCount} fields");
            return true;
        }

        /// <summary>
        /// Handles the INITSLOT operation, which initializes the local variable and argument slots.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleInitSlot()
        {
            // Get the number of local variables and arguments from the instruction operand
            var instruction = _engine.CurrentState.CurrentInstruction(_script);
            if (instruction == null || instruction.Operand.IsEmpty || instruction.Operand.Length < 2)
            {
                LogDebug("INITSLOT: Invalid instruction or operand");
                return false;
            }

            // The operand consists of two bytes: the first is the number of local variables, the second is the number of arguments
            var localVariableCount = instruction.Operand.Span[0];
            var argumentCount = instruction.Operand.Span[1];

            // Get the current state
            var state = _engine.CurrentState as SymbolicState;
            if (state == null)
            {
                LogDebug("INITSLOT: Current state is not a SymbolicState");
                return false;
            }

            // Create a new slot with the specified number of local variables and arguments
            var staticFieldCount = state.Slot?.StaticFieldCount ?? 0;
            var slot = new SymbolicSlot(staticFieldCount, localVariableCount, argumentCount);

            // Set the slot in the state using reflection
            var slotField = typeof(SymbolicState).GetField("_slot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (slotField == null)
            {
                LogDebug("INITSLOT: Could not find _slot field in SymbolicState");
                return false;
            }

            slotField.SetValue(state, slot);

            LogDebug($"Initialized slot with {localVariableCount} local variables and {argumentCount} arguments");
            return true;
        }

        /// <summary>
        /// Handles the LDSFLD operation, which loads a static field onto the stack.
        /// </summary>
        /// <param name="index">The index of the static field to load, or null to get the index from the instruction operand.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleLoadStaticField(int? index = null)
        {
            // Get the index of the static field
            int fieldIndex;
            if (index.HasValue)
            {
                fieldIndex = index.Value;
            }
            else
            {
                // Get the index from the instruction operand
                var instruction = _engine.CurrentState.CurrentInstruction(_script);
                if (instruction == null || instruction.Operand.IsEmpty)
                {
                    LogDebug("LDSFLD: Invalid instruction or operand");
                    return false;
                }

                fieldIndex = instruction.Operand.Span[0];
            }

            // Get the static field from the slot
            var state = _engine.CurrentState as SymbolicState;
            if (state == null || state.Slot == null)
            {
                LogDebug("LDSFLD: Current state is not a SymbolicState or slot is not initialized");
                return false;
            }

            var field = state.Slot.GetStaticField(fieldIndex);
            if (field == null)
            {
                LogDebug($"LDSFLD: Static field at index {fieldIndex} not found");

                // Create a new symbolic value for the static field
                field = new SymbolicVariable($"staticField{fieldIndex}", VM.Types.StackItemType.Any);

                // Store it in the slot for future reference
                state.Slot.SetStaticField(fieldIndex, field);
            }

            // Push the field onto the stack
            _engine.CurrentState.Push(field);
            LogDebug($"Loaded static field at index {fieldIndex}");

            return true;
        }

        /// <summary>
        /// Handles the STSFLD operation, which stores a value from the stack into a static field.
        /// </summary>
        /// <param name="index">The index of the static field to store, or null to get the index from the instruction operand.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleStoreStaticField(int? index = null)
        {
            // Get the index of the static field
            int fieldIndex;
            if (index.HasValue)
            {
                fieldIndex = index.Value;
            }
            else
            {
                // Get the index from the instruction operand
                var instruction = _engine.CurrentState.CurrentInstruction(_script);
                if (instruction == null || instruction.Operand.IsEmpty)
                {
                    LogDebug("STSFLD: Invalid instruction or operand");
                    return false;
                }

                fieldIndex = instruction.Operand.Span[0];
            }

            // Pop the value from the stack
            var value = _engine.CurrentState.Pop();
            if (value == null)
            {
                LogDebug("STSFLD: Stack underflow");
                return false;
            }

            // Store the value in the static field
            var state = _engine.CurrentState as SymbolicState;
            if (state == null || state.Slot == null)
            {
                LogDebug("STSFLD: Current state is not a SymbolicState or slot is not initialized");
                return false;
            }

            var success = state.Slot.SetStaticField(fieldIndex, value);
            if (!success)
            {
                LogDebug($"STSFLD: Failed to set static field at index {fieldIndex}");
                return false;
            }

            LogDebug($"Stored value in static field at index {fieldIndex}");
            return true;
        }

        /// <summary>
        /// Handles the LDLOC operation, which loads a local variable onto the stack.
        /// </summary>
        /// <param name="index">The index of the local variable to load, or null to get the index from the instruction operand.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleLoadLocalVariable(int? index = null)
        {
            // Get the index of the local variable
            int variableIndex;
            if (index.HasValue)
            {
                variableIndex = index.Value;
            }
            else
            {
                // Get the index from the instruction operand
                var instruction = _engine.CurrentState.CurrentInstruction(_script);
                if (instruction == null || instruction.Operand.IsEmpty)
                {
                    LogDebug("LDLOC: Invalid instruction or operand");
                    return false;
                }

                variableIndex = instruction.Operand.Span[0];
            }

            // Get the local variable from the slot
            var state = _engine.CurrentState as SymbolicState;
            if (state == null || state.Slot == null)
            {
                LogDebug("LDLOC: Current state is not a SymbolicState or slot is not initialized");
                return false;
            }

            var variable = state.Slot.GetLocalVariable(variableIndex);
            if (variable == null)
            {
                LogDebug($"LDLOC: Local variable at index {variableIndex} not found");
                return false;
            }

            // Push the variable onto the stack
            _engine.CurrentState.Push(variable);
            LogDebug($"Loaded local variable at index {variableIndex}");

            return true;
        }

        /// <summary>
        /// Handles the STLOC operation, which stores a value from the stack into a local variable.
        /// </summary>
        /// <param name="index">The index of the local variable to store, or null to get the index from the instruction operand.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleStoreLocalVariable(int? index = null)
        {
            // Get the index of the local variable
            int variableIndex;
            if (index.HasValue)
            {
                variableIndex = index.Value;
            }
            else
            {
                // Get the index from the instruction operand
                var instruction = _engine.CurrentState.CurrentInstruction(_script);
                if (instruction == null || instruction.Operand.IsEmpty)
                {
                    LogDebug("STLOC: Invalid instruction or operand");
                    return false;
                }

                variableIndex = instruction.Operand.Span[0];
            }

            // Pop the value from the stack
            var value = _engine.CurrentState.Pop();
            if (value == null)
            {
                LogDebug("STLOC: Stack underflow");
                return false;
            }

            // Store the value in the local variable
            var state = _engine.CurrentState as SymbolicState;
            if (state == null || state.Slot == null)
            {
                LogDebug("STLOC: Current state is not a SymbolicState or slot is not initialized");
                return false;
            }

            var success = state.Slot.SetLocalVariable(variableIndex, value);
            if (!success)
            {
                LogDebug($"STLOC: Failed to set local variable at index {variableIndex}");
                return false;
            }

            LogDebug($"Stored value in local variable at index {variableIndex}");
            return true;
        }

        /// <summary>
        /// Handles the LDARG operation, which loads an argument onto the stack.
        /// </summary>
        /// <param name="index">The index of the argument to load, or null to get the index from the instruction operand.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleLoadArgument(int? index = null)
        {
            // Get the index of the argument
            int argumentIndex;
            if (index.HasValue)
            {
                argumentIndex = index.Value;
            }
            else
            {
                // Get the index from the instruction operand
                var instruction = _engine.CurrentState.CurrentInstruction(_script);
                if (instruction == null || instruction.Operand.IsEmpty)
                {
                    LogDebug("LDARG: Invalid instruction or operand");
                    return false;
                }

                argumentIndex = instruction.Operand.Span[0];
            }

            // Get the argument from the slot
            var state = _engine.CurrentState as SymbolicState;
            if (state == null || state.Slot == null)
            {
                LogDebug("LDARG: Current state is not a SymbolicState or slot is not initialized");
                return false;
            }

            var argument = state.Slot.GetArgument(argumentIndex);
            if (argument == null)
            {
                LogDebug($"LDARG: Argument at index {argumentIndex} not found");
                return false;
            }

            // Push the argument onto the stack
            _engine.CurrentState.Push(argument);
            LogDebug($"Loaded argument at index {argumentIndex}");

            return true;
        }

        /// <summary>
        /// Handles the STARG operation, which stores a value from the stack into an argument.
        /// </summary>
        /// <param name="index">The index of the argument to store, or null to get the index from the instruction operand.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleStoreArgument(int? index = null)
        {
            // Get the index of the argument
            int argumentIndex;
            if (index.HasValue)
            {
                argumentIndex = index.Value;
            }
            else
            {
                // Get the index from the instruction operand
                var instruction = _engine.CurrentState.CurrentInstruction(_script);
                if (instruction == null || instruction.Operand.IsEmpty)
                {
                    LogDebug("STARG: Invalid instruction or operand");
                    return false;
                }

                argumentIndex = instruction.Operand.Span[0];
            }

            // Pop the value from the stack
            var value = _engine.CurrentState.Pop();
            if (value == null)
            {
                LogDebug("STARG: Stack underflow");
                return false;
            }

            // Store the value in the argument
            var state = _engine.CurrentState as SymbolicState;
            if (state == null || state.Slot == null)
            {
                LogDebug("STARG: Current state is not a SymbolicState or slot is not initialized");
                return false;
            }

            var success = state.Slot.SetArgument(argumentIndex, value);
            if (!success)
            {
                LogDebug($"STARG: Failed to set argument at index {argumentIndex}");
                return false;
            }

            LogDebug($"Stored value in argument at index {argumentIndex}");
            return true;
        }
    }
}
