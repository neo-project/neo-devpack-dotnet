using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all type operations in the symbolic virtual machine.
    /// This class is responsible for operations related to type checking and conversion.
    /// </summary>
    public class TypeOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public TypeOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes a type operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.ISNULL:
                    return HandleIsNull();
                case OpCode.ISTYPE:
                    return HandleIsType();
                case OpCode.CONVERT:
                    return HandleConvert();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the ISNULL operation, which checks if an item is null.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleIsNull()
        {
            // Pop the item from the stack
            var item = _engine.CurrentState.Pop();

            if (item == null)
            {
                LogDebug("ISNULL: Stack underflow");
                return false;
            }

            // Check if the item is null
            bool isNull = item is ConcreteValue<object> concreteObj && concreteObj.Value == null;
            _engine.CurrentState.Push(new ConcreteValue<bool>(isNull));
            LogDebug($"Checked if item is null: {isNull}");

            return true;
        }

        /// <summary>
        /// Handles the ISTYPE operation, which checks if an item is of a specific type.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleIsType()
        {
            // Get the type from the instruction operand
            var instruction = _engine.CurrentState.CurrentInstruction(_script);
            if (instruction == null || instruction.Operand.IsEmpty)
            {
                LogDebug("ISTYPE: Invalid instruction or operand");
                return false;
            }

            // The operand is a single byte representing the type
            var type = (VM.Types.StackItemType)instruction.Operand.Span[0];

            // Pop the item from the stack
            var item = _engine.CurrentState.Pop();

            if (item == null)
            {
                LogDebug("ISTYPE: Stack underflow");
                return false;
            }

            // Check if the item is of the specified type
            bool isType = false;
            if (item is SymbolicValue symbolicValue)
            {
                isType = symbolicValue.Type == type;
            }

            _engine.CurrentState.Push(new ConcreteValue<bool>(isType));
            LogDebug($"Checked if item is of type {type}: {isType}");

            return true;
        }

        /// <summary>
        /// Handles the CONVERT operation, which converts an item to a specific type.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleConvert()
        {
            // Get the type from the instruction operand
            var instruction = _engine.CurrentState.CurrentInstruction(_script);
            if (instruction == null || instruction.Operand.IsEmpty)
            {
                LogDebug("CONVERT: Invalid instruction or operand");
                return false;
            }

            // The operand is a single byte representing the type
            var type = (VM.Types.StackItemType)instruction.Operand.Span[0];

            // Pop the item from the stack
            var item = _engine.CurrentState.Pop();

            if (item == null)
            {
                LogDebug("CONVERT: Stack underflow");
                return false;
            }

            // Convert the item to the specified type
            switch (type)
            {
                case VM.Types.StackItemType.Boolean:
                    if (item is ConcreteValue<bool> boolValue)
                    {
                        _engine.CurrentState.Push(boolValue);
                        LogDebug("Converted item to boolean (already boolean)");
                    }
                    else if (item is ConcreteValue<int> intValue)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<bool>(intValue.Value != 0));
                        LogDebug($"Converted integer {intValue.Value} to boolean: {intValue.Value != 0}");
                    }
                    else if (item is ConcreteValue<BigInteger> bigIntValue)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<bool>(bigIntValue.Value != BigInteger.Zero));
                        LogDebug($"Converted big integer {bigIntValue.Value} to boolean: {bigIntValue.Value != BigInteger.Zero}");
                    }
                    else if (item is ConcreteValue<string> stringValue)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<bool>(!string.IsNullOrEmpty(stringValue.Value)));
                        LogDebug($"Converted string \"{stringValue.Value}\" to boolean: {!string.IsNullOrEmpty(stringValue.Value)}");
                    }
                    else
                    {
                        var symbolicResult = new SymbolicVariable($"Convert_To_Boolean_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Boolean);
                        _engine.CurrentState.Push(symbolicResult);
                        LogDebug($"Created symbolic result for conversion to boolean: {symbolicResult}");
                    }
                    break;

                case VM.Types.StackItemType.Integer:
                    if (item is ConcreteValue<int> intValue2)
                    {
                        _engine.CurrentState.Push(intValue2);
                        LogDebug("Converted item to integer (already integer)");
                    }
                    else if (item is ConcreteValue<BigInteger> bigIntValue2)
                    {
                        _engine.CurrentState.Push(bigIntValue2);
                        LogDebug("Converted item to integer (already big integer)");
                    }
                    else if (item is ConcreteValue<bool> boolValue2)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<int>(boolValue2.Value ? 1 : 0));
                        LogDebug($"Converted boolean {boolValue2.Value} to integer: {(boolValue2.Value ? 1 : 0)}");
                    }
                    else if (item is ConcreteValue<string> stringValue2)
                    {
                        if (int.TryParse(stringValue2.Value, out int result))
                        {
                            _engine.CurrentState.Push(new ConcreteValue<int>(result));
                            LogDebug($"Converted string \"{stringValue2.Value}\" to integer: {result}");
                        }
                        else if (BigInteger.TryParse(stringValue2.Value, out BigInteger bigResult))
                        {
                            _engine.CurrentState.Push(new ConcreteValue<BigInteger>(bigResult));
                            LogDebug($"Converted string \"{stringValue2.Value}\" to big integer: {bigResult}");
                        }
                        else
                        {
                            var symbolicResult = new SymbolicVariable($"Convert_String_To_Integer_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                            _engine.CurrentState.Push(symbolicResult);
                            LogDebug($"Created symbolic result for conversion of string to integer: {symbolicResult}");
                        }
                    }
                    else
                    {
                        var symbolicResult = new SymbolicVariable($"Convert_To_Integer_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                        _engine.CurrentState.Push(symbolicResult);
                        LogDebug($"Created symbolic result for conversion to integer: {symbolicResult}");
                    }
                    break;

                case VM.Types.StackItemType.ByteString:
                    if (item is ConcreteValue<string> stringValue3)
                    {
                        _engine.CurrentState.Push(stringValue3);
                        LogDebug("Converted item to byte string (already string)");
                    }
                    else if (item is SymbolicBuffer buffer)
                    {
                        var size = buffer.GetSize();
                        if (size.HasValue)
                        {
                            var str = System.Text.Encoding.UTF8.GetString(buffer.GetData(), 0, size.Value);
                            _engine.CurrentState.Push(new ConcreteValue<string>(str));
                            LogDebug($"Converted buffer to string: \"{str}\"");
                        }
                        else
                        {
                            var symbolicResult = new SymbolicVariable($"Convert_Buffer_To_String_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                            _engine.CurrentState.Push(symbolicResult);
                            LogDebug($"Created symbolic result for conversion of buffer to string: {symbolicResult}");
                        }
                    }
                    else if (item is ConcreteValue<int> intValue3)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<string>(intValue3.Value.ToString()));
                        LogDebug($"Converted integer {intValue3.Value} to string: \"{intValue3.Value}\"");
                    }
                    else if (item is ConcreteValue<BigInteger> bigIntValue3)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<string>(bigIntValue3.Value.ToString()));
                        LogDebug($"Converted big integer {bigIntValue3.Value} to string: \"{bigIntValue3.Value}\"");
                    }
                    else if (item is ConcreteValue<bool> boolValue3)
                    {
                        _engine.CurrentState.Push(new ConcreteValue<string>(boolValue3.Value.ToString()));
                        LogDebug($"Converted boolean {boolValue3.Value} to string: \"{boolValue3.Value}\"");
                    }
                    else
                    {
                        var symbolicResult = new SymbolicVariable($"Convert_To_String_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                        _engine.CurrentState.Push(symbolicResult);
                        LogDebug($"Created symbolic result for conversion to string: {symbolicResult}");
                    }
                    break;

                case VM.Types.StackItemType.Buffer:
                    if (item is SymbolicBuffer buffer2)
                    {
                        _engine.CurrentState.Push(buffer2);
                        LogDebug("Converted item to buffer (already buffer)");
                    }
                    else if (item is ConcreteValue<string> stringValue4)
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(stringValue4.Value);
                        _engine.CurrentState.Push(new SymbolicBuffer(bytes));
                        LogDebug($"Converted string \"{stringValue4.Value}\" to buffer");
                    }
                    else
                    {
                        var symbolicResult = new SymbolicBuffer();
                        _engine.CurrentState.Push(symbolicResult);
                        LogDebug($"Created symbolic buffer for conversion to buffer: {symbolicResult}");
                    }
                    break;

                default:
                    var symbolicVariable = new SymbolicVariable($"Convert_To_{type}_{_engine.CurrentState.InstructionPointer}", type);
                    _engine.CurrentState.Push(symbolicVariable);
                    LogDebug($"Created symbolic result for conversion to {type}: {symbolicVariable}");
                    break;
            }

            return true;
        }
    }
}
