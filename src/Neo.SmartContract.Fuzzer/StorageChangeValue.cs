namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Represents a storage change value with timestamp.
    /// </summary>
    public class StorageChangeValue
    {
        /// <summary>
        /// The timestamp of the storage change.
        /// </summary>
        public long Timestamp { get; }

        /// <summary>
        /// The original value of the storage item.
        /// </summary>
        public byte[]? OriginalValue { get; }

        /// <summary>
        /// The new value of the storage item.
        /// </summary>
        public byte[]? NewValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageChangeValue"/> class.
        /// </summary>
        /// <param name="args">The storage change event args.</param>
        public StorageChangeValue(StorageChangeEventArgs args)
        {
            Timestamp = args.Timestamp;
            OriginalValue = args.OriginalValue;
            NewValue = args.NewValue;
        }
    }
}
