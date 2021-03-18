using System.Numerics;

namespace Neo.SmartContract.Framework.Services
{
    public static class Storage
    {
        /// <summary>
        /// Returns current StorageContext
        /// </summary>
        public static extern StorageContext CurrentContext
        {
            [Syscall("System.Storage.GetContext")]
            get;
        }

        /// <summary>
        /// Returns current read only StorageContext
        /// </summary>
        public static extern StorageContext CurrentReadOnlyContext
        {
            [Syscall("System.Storage.GetReadOnlyContext")]
            get;
        }

        /// <summary>
        /// Returns the value corresponding to the given key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern ByteString Get(StorageContext context, ByteString key);

        /// <summary>
        /// Returns the value corresponding to the given key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern ByteString Get(StorageContext context, byte[] key);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, ByteString key, ByteString value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, ByteString value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, byte[] value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, ByteString key, BigInteger value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, BigInteger value);

        /// <summary>
        /// Deletes the entry from the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(StorageContext context, ByteString key);

        /// <summary>
        /// Deletes the entry from the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(StorageContext context, byte[] key);

        /// <summary>
        /// Returns a byte[] to byte[] iterator for a byte[] prefix on a given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Find")]
        public static extern Iterator Find(StorageContext context, ByteString prefix, FindOptions options = FindOptions.None);

        /// <summary>
        /// Returns a byte[] to byte[] iterator for a byte[] prefix on a given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Find")]
        public static extern Iterator Find(StorageContext context, byte[] prefix, FindOptions options = FindOptions.None);
    }
}
