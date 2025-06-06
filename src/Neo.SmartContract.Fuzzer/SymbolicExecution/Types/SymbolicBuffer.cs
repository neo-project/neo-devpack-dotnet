using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic buffer in the symbolic execution engine.
    /// </summary>
    public class SymbolicBuffer : SymbolicValue
    {
        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => StackItemType.Buffer;

        /// <summary>
        /// Converts this symbolic buffer to a concrete stack item.
        /// </summary>
        /// <returns>A concrete stack item representing this buffer.</returns>
        public override StackItem ToStackItem()
        {
            return new VM.Types.Buffer(_data);
        }

        private readonly byte[] _data;
        private readonly int? _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicBuffer"/> class with a specific size.
        /// </summary>
        /// <param name="size">The size of the buffer.</param>
        public SymbolicBuffer(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Buffer size cannot be negative.");

            _size = size;
            _data = new byte[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicBuffer"/> class with specific data.
        /// </summary>
        /// <param name="data">The data to initialize the buffer with.</param>
        public SymbolicBuffer(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _size = _data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicBuffer"/> class with unknown size.
        /// </summary>
        public SymbolicBuffer()
        {
            _data = System.Array.Empty<byte>();
            _size = null; // Unknown size
        }

        /// <summary>
        /// Gets the size of the buffer.
        /// </summary>
        /// <returns>The size of the buffer, or null if the size is unknown.</returns>
        public int? GetSize()
        {
            return _size;
        }

        /// <summary>
        /// Gets the data in the buffer.
        /// </summary>
        /// <returns>The data in the buffer.</returns>
        public byte[] GetData()
        {
            return _data;
        }

        /// <summary>
        /// Gets a portion of the buffer.
        /// </summary>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="count">The number of bytes to get.</param>
        /// <returns>A new buffer containing the specified portion, or a symbolic buffer if the parameters are out of range.</returns>
        public SymbolicBuffer GetRange(int offset, int count)
        {
            if (_size.HasValue && offset >= 0 && count >= 0 && offset + count <= _size.Value)
            {
                byte[] result = new byte[count];
                System.Array.Copy(_data, offset, result, 0, count);
                return new SymbolicBuffer(result);
            }

            // If the parameters are out of range or the buffer has unknown size, return a symbolic buffer
            return new SymbolicBuffer();
        }

        /// <summary>
        /// Sets a portion of the buffer.
        /// </summary>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="data">The data to set.</param>
        /// <returns>True if the data was set, false otherwise.</returns>
        public bool SetRange(int offset, byte[] data)
        {
            if (_size.HasValue && offset >= 0 && data != null && offset + data.Length <= _size.Value)
            {
                System.Array.Copy(data, 0, _data, offset, data.Length);
                return true;
            }

            // If the parameters are out of range or the buffer has unknown size, we can't set the data
            return false;
        }

        /// <summary>
        /// Concatenates this buffer with another buffer.
        /// </summary>
        /// <param name="other">The buffer to concatenate with.</param>
        /// <returns>A new buffer containing the concatenated data, or a symbolic buffer if either buffer has unknown size.</returns>
        public SymbolicBuffer Concat(SymbolicBuffer other)
        {
            if (_size.HasValue && other._size.HasValue)
            {
                byte[] result = new byte[_size.Value + other._size.Value];
                System.Array.Copy(_data, 0, result, 0, _size.Value);
                System.Array.Copy(other._data, 0, result, _size.Value, other._size.Value);
                return new SymbolicBuffer(result);
            }

            // If either buffer has unknown size, return a symbolic buffer
            return new SymbolicBuffer();
        }

        /// <summary>
        /// Returns a string representation of the buffer.
        /// </summary>
        /// <returns>A string representation of the buffer.</returns>
        public override string ToString()
        {
            if (_size.HasValue)
            {
                return $"SymbolicBuffer[{_size.Value}]";
            }
            else
            {
                return "SymbolicBuffer[?]";
            }
        }
    }
}
