using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic map in the symbolic execution engine.
    /// </summary>
    public class SymbolicMap : SymbolicValue
    {
        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => StackItemType.Map;

        /// <summary>
        /// Converts this symbolic map to a concrete stack item.
        /// </summary>
        /// <returns>A concrete stack item representing this map.</returns>
        public override StackItem ToStackItem()
        {
            var map = new VM.Types.Map();

            foreach (var entry in _entries)
            {
                try
                {
                    var key = entry.Key.ToStackItem();
                    var value = entry.Value.ToStackItem();

                    // Map keys must be PrimitiveType
                    if (key is PrimitiveType primitiveKey)
                    {
                        map[primitiveKey] = value;
                    }
                    else
                    {
                        // If the key is not a primitive type, skip it
                        continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    // If we can't convert a symbolic item, skip it
                    continue;
                }
            }

            return map;
        }

        private readonly Dictionary<SymbolicValue, SymbolicValue> _entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicMap"/> class.
        /// </summary>
        public SymbolicMap()
        {
            _entries = new Dictionary<SymbolicValue, SymbolicValue>(new SymbolicValueComparer());
        }

        /// <summary>
        /// Gets the size of the map.
        /// </summary>
        /// <returns>The size of the map, or null if the size is unknown.</returns>
        public int? GetSize()
        {
            return _entries.Count;
        }

        /// <summary>
        /// Gets the entries in the map.
        /// </summary>
        /// <returns>The entries in the map.</returns>
        public IReadOnlyDictionary<SymbolicValue, SymbolicValue> GetEntries()
        {
            return _entries;
        }

        /// <summary>
        /// Gets the keys in the map.
        /// </summary>
        /// <returns>The keys in the map.</returns>
        public IReadOnlyList<SymbolicValue> GetKeys()
        {
            return _entries.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the values in the map.
        /// </summary>
        /// <returns>The values in the map.</returns>
        public IReadOnlyList<SymbolicValue> GetValues()
        {
            return _entries.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Checks if the map contains the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the map contains the key, false otherwise, or null if unknown.</returns>
        public bool? HasKey(SymbolicValue key)
        {
            if (key is ConcreteValue)
            {
                return _entries.ContainsKey(key);
            }

            // If the key is symbolic, we don't know if it's in the map
            return null;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key, or a new symbolic variable if the key is not found.</returns>
        public SymbolicValue GetItem(SymbolicValue key)
        {
            if (_entries.TryGetValue(key, out var value))
            {
                return value;
            }

            // If the key is not found, return a symbolic variable
            return new SymbolicVariable($"Map_Item_{key}", StackItemType.Any);
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetItem(SymbolicValue key, SymbolicValue value)
        {
            _entries[key] = value;
        }

        /// <summary>
        /// Adds a key-value pair to the map.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void Add(SymbolicValue key, SymbolicValue value)
        {
            _entries[key] = value;
        }

        /// <summary>
        /// Removes the value with the specified key from the map.
        /// </summary>
        /// <param name="key">The key of the value to remove.</param>
        /// <returns>True if the value was removed, false otherwise.</returns>
        public bool Remove(SymbolicValue key)
        {
            return _entries.Remove(key);
        }

        /// <summary>
        /// Clears all entries from the map.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
        }

        /// <summary>
        /// Returns a string representation of the map.
        /// </summary>
        /// <returns>A string representation of the map.</returns>
        public override string ToString()
        {
            return $"SymbolicMap[{_entries.Count}]";
        }

        /// <summary>
        /// Comparer for symbolic values in a dictionary.
        /// </summary>
        private class SymbolicValueComparer : IEqualityComparer<SymbolicValue>
        {
            public bool Equals(SymbolicValue? x, SymbolicValue? y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null)
                    return false;

                return x.ToString() == y.ToString();
            }

            public int GetHashCode(SymbolicValue obj)
            {
                return obj?.ToString()?.GetHashCode() ?? 0;
            }
        }
    }
}
