using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic array in the symbolic execution engine.
    /// </summary>
    public class SymbolicArray : SymbolicValue
    {
        private readonly StackItemType _type = StackItemType.Array;

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => _type;

        /// <summary>
        /// Converts this symbolic array to a concrete stack item.
        /// </summary>
        /// <returns>A concrete stack item representing this array.</returns>
        public override StackItem ToStackItem()
        {
            var array = new VM.Types.Array();

            foreach (var item in _items)
            {
                try
                {
                    array.Add(item.ToStackItem());
                }
                catch (InvalidOperationException)
                {
                    // If we can't convert a symbolic item, use a placeholder
                    array.Add(new ByteString(new byte[0]));
                }
            }

            return array;
        }

        private readonly List<SymbolicValue> _items;
        private readonly int? _size;
        private readonly StackItemType _itemType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicArray"/> class with a specific size.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        public SymbolicArray(int size)
            : this(size, StackItemType.Any)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicArray"/> class with a specific size and item type.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        /// <param name="itemType">The type of items in the array.</param>
        public SymbolicArray(int size, StackItemType itemType)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Array size cannot be negative.");

            _size = size;
            _itemType = itemType;
            _items = new List<SymbolicValue>(size);

            // Initialize array with symbolic variables
            for (int i = 0; i < size; i++)
            {
                _items.Add(new SymbolicVariable($"Array_Item_{i}", itemType));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicArray"/> class with specific items.
        /// </summary>
        /// <param name="items">The items to initialize the array with.</param>
        public SymbolicArray(IEnumerable<SymbolicValue> items)
        {
            _items = items?.ToList() ?? new List<SymbolicValue>();
            _size = _items.Count;
            _itemType = StackItemType.Any;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicArray"/> class with a specific item type.
        /// </summary>
        /// <param name="itemType">The type of items in the array.</param>
        public SymbolicArray(StackItemType itemType)
        {
            _items = new List<SymbolicValue>();
            _size = null; // Unknown size
            _itemType = itemType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicArray"/> class with unknown size.
        /// </summary>
        public SymbolicArray()
        {
            _items = new List<SymbolicValue>();
            _size = null; // Unknown size
            _itemType = StackItemType.Any;
        }

        /// <summary>
        /// Gets the size of the array.
        /// </summary>
        /// <returns>The size of the array, or null if the size is unknown.</returns>
        public int? GetSize()
        {
            return _size;
        }

        /// <summary>
        /// Gets the items in the array.
        /// </summary>
        /// <returns>The items in the array.</returns>
        public IReadOnlyList<SymbolicValue> GetItems()
        {
            return _items.AsReadOnly();
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <returns>The item at the specified index, or a new symbolic variable if the index is out of range or the array has unknown size.</returns>
        public SymbolicValue GetItem(int index)
        {
            if (_size.HasValue && index >= 0 && index < _size.Value)
            {
                return _items[index];
            }

            // If the index is out of range or the array has unknown size, return a symbolic variable
            return new SymbolicVariable($"Array_Item_{index}", _itemType);
        }

        /// <summary>
        /// Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the item was set, false otherwise.</returns>
        public bool SetItem(int index, SymbolicValue value)
        {
            if (_size.HasValue && index >= 0 && index < _size.Value)
            {
                _items[index] = value;
                return true;
            }

            // If the index is out of range or the array has unknown size, we can't set the item
            return false;
        }

        /// <summary>
        /// Appends an item to the array.
        /// </summary>
        /// <param name="item">The item to append.</param>
        public void Append(SymbolicValue item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        public bool RemoveAt(int index)
        {
            if (_size.HasValue && index >= 0 && index < _size.Value)
            {
                _items.RemoveAt(index);
                return true;
            }

            // If the index is out of range or the array has unknown size, we can't remove the item
            return false;
        }

        /// <summary>
        /// Clears all items from the array.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Reverses the order of items in the array.
        /// </summary>
        public void Reverse()
        {
            _items.Reverse();
        }

        /// <summary>
        /// Pops the last item from the array.
        /// </summary>
        /// <returns>The last item in the array, or a new symbolic variable if the array is empty or has unknown size.</returns>
        public SymbolicValue Pop()
        {
            if (_size.HasValue && _size.Value > 0 && _items.Count > 0)
            {
                var item = _items[_items.Count - 1];
                _items.RemoveAt(_items.Count - 1);
                return item;
            }

            // If the array is empty or has unknown size, return a symbolic variable
            return new SymbolicVariable($"Array_Pop_Result", _itemType);
        }

        /// <summary>
        /// Returns a string representation of the array.
        /// </summary>
        /// <returns>A string representation of the array.</returns>
        public override string ToString()
        {
            if (_size.HasValue)
            {
                return $"SymbolicArray[{_size.Value}]";
            }
            else
            {
                return "SymbolicArray[?]";
            }
        }
    }
}
