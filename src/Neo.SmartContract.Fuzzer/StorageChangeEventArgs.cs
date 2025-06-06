using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Event arguments for storage changes
    /// </summary>
    public class StorageChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The script hash of the contract
        /// </summary>
        public UInt160 ScriptHash { get; }

        /// <summary>
        /// The key of the storage item
        /// </summary>
        public byte[] Key { get; }

        /// <summary>
        /// Gets the length of the key.
        /// </summary>
        public int Length => Key.Length;

        /// <summary>
        /// The original value of the storage item
        /// </summary>
        public byte[]? OriginalValue { get; }

        /// <summary>
        /// The new value of the storage item
        /// </summary>
        public byte[]? NewValue { get; }

        /// <summary>
        /// The timestamp of the storage change (relative to execution start)
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets the value of the storage change (for compatibility with detectors)
        /// </summary>
        public StorageChangeValue Value => new StorageChangeValue(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageChangeEventArgs"/> class.
        /// </summary>
        /// <param name="scriptHash">The script hash of the contract</param>
        /// <param name="key">The key of the storage item</param>
        /// <param name="originalValue">The original value of the storage item</param>
        /// <param name="newValue">The new value of the storage item</param>
        public StorageChangeEventArgs(UInt160 scriptHash, byte[] key, byte[]? originalValue, byte[]? newValue)
        {
            ScriptHash = scriptHash;
            Key = key;
            OriginalValue = originalValue;
            NewValue = newValue;
        }
    }
}
