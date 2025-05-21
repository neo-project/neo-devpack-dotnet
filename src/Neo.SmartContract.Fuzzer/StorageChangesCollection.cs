using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// A collection of storage changes.
    /// </summary>
    public class StorageChangesCollection : ICollection<StorageChangeEventArgs>
    {
        private readonly List<StorageChangeEventArgs> _changes = new List<StorageChangeEventArgs>();

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _changes.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the keys in the collection.
        /// </summary>
        public IEnumerable<byte[]> Keys => _changes.Select(c => c.Key);

        /// <summary>
        /// Gets the values in the collection.
        /// </summary>
        public IEnumerable<byte[]> Values => _changes.Select(c => c.NewValue).Where(v => v != null);

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(StorageChangeEventArgs item)
        {
            _changes.Add(item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            _changes.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>true if the item is found in the collection; otherwise, false.</returns>
        public bool Contains(StorageChangeEventArgs item)
        {
            return _changes.Contains(item);
        }

        /// <summary>
        /// Determines whether the collection contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the key is found in the collection; otherwise, false.</returns>
        public bool ContainsKey(byte[] key)
        {
            return _changes.Any(c => c.Key.SequenceEqual(key));
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The array that is the destination of the elements copied from the collection.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(StorageChangeEventArgs[] array, int arrayIndex)
        {
            _changes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<StorageChangeEventArgs> GetEnumerator()
        {
            return _changes.GetEnumerator();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; otherwise, false.</returns>
        public bool Remove(StorageChangeEventArgs item)
        {
            return _changes.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
