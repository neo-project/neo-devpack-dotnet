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
    /// Handles all splice operations in the symbolic virtual machine.
    /// This class is responsible for operations on strings and buffers.
    /// </summary>
    public class SpliceOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpliceOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public SpliceOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes a splice operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.NEWBUFFER:
                    return HandleNewBuffer();
                case OpCode.MEMCPY:
                    return HandleMemCpy();
                case OpCode.CAT:
                    return HandleCat();
                case OpCode.SUBSTR:
                    return HandleSubStr();
                case OpCode.LEFT:
                    return HandleLeft();
                case OpCode.RIGHT:
                    return HandleRight();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the NEWBUFFER operation, which creates a new buffer of the specified size.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewBuffer()
        {
            // Pop the size from the stack
            var size = _engine.CurrentState.Pop();

            if (size == null)
            {
                LogDebug("NEWBUFFER: Stack underflow");
                return false;
            }

            // If size is a concrete value, create a buffer of that size
            if (size is ConcreteValue<int> intSize)
            {
                var symbolicBuffer = new SymbolicBuffer(intSize.Value);
                _engine.CurrentState.Push(symbolicBuffer);
                LogDebug($"Created buffer of size {intSize.Value}");
            }
            else if (size is ConcreteValue<BigInteger> bigIntSize)
            {
                // Ensure the size is within reasonable bounds
                if (bigIntSize.Value > int.MaxValue || bigIntSize.Value < 0)
                {
                    LogDebug($"NEWBUFFER: Invalid size {bigIntSize.Value}");
                    return false;
                }

                var symbolicBuffer = new SymbolicBuffer((int)bigIntSize.Value);
                _engine.CurrentState.Push(symbolicBuffer);
                LogDebug($"Created buffer of size {bigIntSize.Value}");
            }
            else
            {
                // If size is symbolic, create a symbolic buffer with unknown size
                var symbolicBuffer = new SymbolicBuffer();
                _engine.CurrentState.Push(symbolicBuffer);
                LogDebug($"Created buffer of symbolic size {size}");
            }

            return true;
        }

        /// <summary>
        /// Handles the MEMCPY operation, which copies data from one buffer to another.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleMemCpy()
        {
            // Pop the count, source offset, destination offset, source, and destination from the stack
            var count = _engine.CurrentState.Pop();
            var srcOffset = _engine.CurrentState.Pop();
            var dstOffset = _engine.CurrentState.Pop();
            var src = _engine.CurrentState.Pop();
            var dst = _engine.CurrentState.Pop();

            if (count == null || srcOffset == null || dstOffset == null || src == null || dst == null)
            {
                LogDebug("MEMCPY: Stack underflow");
                return false;
            }

            // If all parameters are concrete values and the source and destination are buffers, copy the data
            if (count is ConcreteValue<int> intCount &&
                srcOffset is ConcreteValue<int> intSrcOffset &&
                dstOffset is ConcreteValue<int> intDstOffset &&
                src is SymbolicBuffer srcBuffer &&
                dst is SymbolicBuffer dstBuffer)
            {
                var srcSize = srcBuffer.GetSize();
                var dstSize = dstBuffer.GetSize();

                if (srcSize.HasValue && dstSize.HasValue &&
                    intSrcOffset.Value >= 0 && intDstOffset.Value >= 0 && intCount.Value >= 0 &&
                    intSrcOffset.Value + intCount.Value <= srcSize.Value &&
                    intDstOffset.Value + intCount.Value <= dstSize.Value)
                {
                    byte[] data = new byte[intCount.Value];
                    Array.Copy(srcBuffer.GetData(), intSrcOffset.Value, data, 0, intCount.Value);
                    dstBuffer.SetRange(intDstOffset.Value, data);
                    LogDebug($"Copied {intCount.Value} bytes from source offset {intSrcOffset.Value} to destination offset {intDstOffset.Value}");
                }
                else
                {
                    LogDebug("MEMCPY: Invalid parameters");
                    return false;
                }
            }
            else
            {
                LogDebug("MEMCPY: Non-concrete parameters or non-buffer types");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the CAT operation, which concatenates two strings or buffers.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleCat()
        {
            // Pop the two items to concatenate from the stack
            var item2 = _engine.CurrentState.Pop();
            var item1 = _engine.CurrentState.Pop();

            if (item1 == null || item2 == null)
            {
                LogDebug("CAT: Stack underflow");
                return false;
            }

            // If both items are buffers, concatenate them
            if (item1 is SymbolicBuffer buffer1 && item2 is SymbolicBuffer buffer2)
            {
                var result = buffer1.Concat(buffer2);
                _engine.CurrentState.Push(result);
                LogDebug("Concatenated two buffers");
            }
            // If both items are strings, concatenate them
            else if (item1 is ConcreteValue<string> str1 && item2 is ConcreteValue<string> str2)
            {
                var result = new ConcreteValue<string>(str1.Value + str2.Value);
                _engine.CurrentState.Push(result);
                LogDebug("Concatenated two strings");
            }
            // If one item is a string and the other is a buffer, convert the buffer to a string and concatenate
            else if (item1 is ConcreteValue<string> str && item2 is SymbolicBuffer buffer)
            {
                var size = buffer.GetSize();
                if (size.HasValue)
                {
                    var bufferStr = Encoding.UTF8.GetString(buffer.GetData(), 0, size.Value);
                    var result = new ConcreteValue<string>(str.Value + bufferStr);
                    _engine.CurrentState.Push(result);
                    LogDebug("Concatenated string and buffer");
                }
                else
                {
                    var symbolicResult = new SymbolicVariable($"Cat_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                    _engine.CurrentState.Push(symbolicResult);
                    LogDebug($"Created symbolic result for concatenation of string and symbolic buffer: {symbolicResult}");
                }
            }
            else if (item1 is SymbolicBuffer buffer3 && item2 is ConcreteValue<string> str3)
            {
                var size = buffer3.GetSize();
                if (size.HasValue)
                {
                    var bufferStr = Encoding.UTF8.GetString(buffer3.GetData(), 0, size.Value);
                    var result = new ConcreteValue<string>(bufferStr + str3.Value);
                    _engine.CurrentState.Push(result);
                    LogDebug("Concatenated buffer and string");
                }
                else
                {
                    var symbolicResult = new SymbolicVariable($"Cat_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                    _engine.CurrentState.Push(symbolicResult);
                    LogDebug($"Created symbolic result for concatenation of symbolic buffer and string: {symbolicResult}");
                }
            }
            else
            {
                // If the items are not strings or buffers, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"Cat_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for concatenation of non-string/buffer types: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the SUBSTR operation, which gets a substring of a string or buffer.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleSubStr()
        {
            // Pop the count, offset, and string/buffer from the stack
            var count = _engine.CurrentState.Pop();
            var offset = _engine.CurrentState.Pop();
            var str = _engine.CurrentState.Pop();

            if (count == null || offset == null || str == null)
            {
                LogDebug("SUBSTR: Stack underflow");
                return false;
            }

            // If all parameters are concrete values and the string is a concrete string, get the substring
            if (count is ConcreteValue<int> intCount &&
                offset is ConcreteValue<int> intOffset &&
                str is ConcreteValue<string> concreteStr)
            {
                if (intOffset.Value >= 0 && intCount.Value >= 0 && intOffset.Value + intCount.Value <= concreteStr.Value.Length)
                {
                    var result = new ConcreteValue<string>(concreteStr.Value.Substring(intOffset.Value, intCount.Value));
                    _engine.CurrentState.Push(result);
                    LogDebug($"Got substring from offset {intOffset.Value} with length {intCount.Value}");
                }
                else
                {
                    LogDebug("SUBSTR: Invalid parameters for string");
                    return false;
                }
            }
            // If all parameters are concrete values and the string is a buffer, get the subbuffer
            else if (count is ConcreteValue<int> intCount2 &&
                     offset is ConcreteValue<int> intOffset2 &&
                     str is SymbolicBuffer buffer)
            {
                var result = buffer.GetRange(intOffset2.Value, intCount2.Value);
                _engine.CurrentState.Push(result);
                LogDebug($"Got subbuffer from offset {intOffset2.Value} with length {intCount2.Value}");
            }
            else
            {
                // If the parameters are not concrete values or the string is not a concrete string or buffer, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"SubStr_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for substring: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the LEFT operation, which gets the leftmost characters of a string or buffer.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleLeft()
        {
            // Pop the count and string/buffer from the stack
            var count = _engine.CurrentState.Pop();
            var str = _engine.CurrentState.Pop();

            if (count == null || str == null)
            {
                LogDebug("LEFT: Stack underflow");
                return false;
            }

            // If count is a concrete value and the string is a concrete string, get the leftmost characters
            if (count is ConcreteValue<int> intCount && str is ConcreteValue<string> concreteStr)
            {
                if (intCount.Value >= 0 && intCount.Value <= concreteStr.Value.Length)
                {
                    var result = new ConcreteValue<string>(concreteStr.Value.Substring(0, intCount.Value));
                    _engine.CurrentState.Push(result);
                    LogDebug($"Got leftmost {intCount.Value} characters");
                }
                else
                {
                    LogDebug("LEFT: Invalid count for string");
                    return false;
                }
            }
            // If count is a concrete value and the string is a buffer, get the leftmost bytes
            else if (count is ConcreteValue<int> intCount2 && str is SymbolicBuffer buffer)
            {
                var result = buffer.GetRange(0, intCount2.Value);
                _engine.CurrentState.Push(result);
                LogDebug($"Got leftmost {intCount2.Value} bytes");
            }
            else
            {
                // If count is not a concrete value or the string is not a concrete string or buffer, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"Left_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for left: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the RIGHT operation, which gets the rightmost characters of a string or buffer.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleRight()
        {
            // Pop the count and string/buffer from the stack
            var count = _engine.CurrentState.Pop();
            var str = _engine.CurrentState.Pop();

            if (count == null || str == null)
            {
                LogDebug("RIGHT: Stack underflow");
                return false;
            }

            // If count is a concrete value and the string is a concrete string, get the rightmost characters
            if (count is ConcreteValue<int> intCount && str is ConcreteValue<string> concreteStr)
            {
                if (intCount.Value >= 0 && intCount.Value <= concreteStr.Value.Length)
                {
                    var result = new ConcreteValue<string>(concreteStr.Value.Substring(concreteStr.Value.Length - intCount.Value, intCount.Value));
                    _engine.CurrentState.Push(result);
                    LogDebug($"Got rightmost {intCount.Value} characters");
                }
                else
                {
                    LogDebug("RIGHT: Invalid count for string");
                    return false;
                }
            }
            // If count is a concrete value and the string is a buffer, get the rightmost bytes
            else if (count is ConcreteValue<int> intCount2 && str is SymbolicBuffer buffer)
            {
                var size = buffer.GetSize();
                if (size.HasValue && intCount2.Value >= 0 && intCount2.Value <= size.Value)
                {
                    var result = buffer.GetRange(size.Value - intCount2.Value, intCount2.Value);
                    _engine.CurrentState.Push(result);
                    LogDebug($"Got rightmost {intCount2.Value} bytes");
                }
                else
                {
                    LogDebug("RIGHT: Invalid count for buffer");
                    return false;
                }
            }
            else
            {
                // If count is not a concrete value or the string is not a concrete string or buffer, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"Right_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.ByteString);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for right: {symbolicResult}");
            }

            return true;
        }
    }
}
