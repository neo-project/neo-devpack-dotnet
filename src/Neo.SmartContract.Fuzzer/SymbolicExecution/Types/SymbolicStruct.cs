using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic struct in the symbolic execution engine.
    /// </summary>
    public class SymbolicStruct : SymbolicValue
    {
        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => StackItemType.Struct;

        /// <summary>
        /// Converts this symbolic struct to a concrete stack item.
        /// </summary>
        /// <returns>A concrete stack item representing this struct.</returns>
        public override StackItem ToStackItem()
        {
            var structItem = new VM.Types.Struct();

            foreach (var field in _fields)
            {
                try
                {
                    structItem.Add(field.ToStackItem());
                }
                catch (InvalidOperationException)
                {
                    // If we can't convert a symbolic item, use a placeholder
                    structItem.Add(new ByteString(new byte[0]));
                }
            }

            return structItem;
        }

        private readonly List<SymbolicValue> _fields;
        private readonly int? _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicStruct"/> class with a specific size.
        /// </summary>
        /// <param name="size">The size of the struct.</param>
        public SymbolicStruct(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Struct size cannot be negative.");

            _size = size;
            _fields = new List<SymbolicValue>(size);

            // Initialize struct with symbolic variables
            for (int i = 0; i < size; i++)
            {
                _fields.Add(new SymbolicVariable($"Struct_Field_{i}", StackItemType.Any));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicStruct"/> class with specific fields.
        /// </summary>
        /// <param name="fields">The fields to initialize the struct with.</param>
        public SymbolicStruct(IEnumerable<SymbolicValue> fields)
        {
            _fields = fields?.ToList() ?? new List<SymbolicValue>();
            _size = _fields.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicStruct"/> class with unknown size.
        /// </summary>
        public SymbolicStruct()
        {
            _fields = new List<SymbolicValue>();
            _size = null; // Unknown size
        }

        /// <summary>
        /// Gets the size of the struct.
        /// </summary>
        /// <returns>The size of the struct, or null if the size is unknown.</returns>
        public int? GetSize()
        {
            return _size;
        }

        /// <summary>
        /// Gets the fields in the struct.
        /// </summary>
        /// <returns>The fields in the struct.</returns>
        public IReadOnlyList<SymbolicValue> GetFields()
        {
            return _fields.AsReadOnly();
        }

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <param name="index">The index of the field to get.</param>
        /// <returns>The field at the specified index, or a new symbolic variable if the index is out of range or the struct has unknown size.</returns>
        public SymbolicValue GetField(int index)
        {
            if (_size.HasValue && index >= 0 && index < _size.Value)
            {
                return _fields[index];
            }

            // If the index is out of range or the struct has unknown size, return a symbolic variable
            return new SymbolicVariable($"Struct_Field_{index}", StackItemType.Any);
        }

        /// <summary>
        /// Sets the field at the specified index.
        /// </summary>
        /// <param name="index">The index of the field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the field was set, false otherwise.</returns>
        public bool SetField(int index, SymbolicValue value)
        {
            if (_size.HasValue && index >= 0 && index < _size.Value)
            {
                _fields[index] = value;
                return true;
            }

            // If the index is out of range or the struct has unknown size, we can't set the field
            return false;
        }

        /// <summary>
        /// Appends a field to the struct.
        /// </summary>
        /// <param name="field">The field to append.</param>
        public void Append(SymbolicValue field)
        {
            _fields.Add(field);
        }

        /// <summary>
        /// Removes the field at the specified index.
        /// </summary>
        /// <param name="index">The index of the field to remove.</param>
        /// <returns>True if the field was removed, false otherwise.</returns>
        public bool RemoveAt(int index)
        {
            if (_size.HasValue && index >= 0 && index < _size.Value)
            {
                _fields.RemoveAt(index);
                return true;
            }

            // If the index is out of range or the struct has unknown size, we can't remove the field
            return false;
        }

        /// <summary>
        /// Clears all fields from the struct.
        /// </summary>
        public void Clear()
        {
            _fields.Clear();
        }

        /// <summary>
        /// Reverses the order of fields in the struct.
        /// </summary>
        public void Reverse()
        {
            _fields.Reverse();
        }

        /// <summary>
        /// Returns a string representation of the struct.
        /// </summary>
        /// <returns>A string representation of the struct.</returns>
        public override string ToString()
        {
            if (_size.HasValue)
            {
                return $"SymbolicStruct[{_size.Value}]";
            }
            else
            {
                return "SymbolicStruct[?]";
            }
        }
    }
}
