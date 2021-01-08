using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
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
        /// Returns the byte[] value corresponding to given byte[] key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern byte[] Get(StorageContext context, byte[] key);

        /// <summary>
        /// Returns the byte[] value corresponding to given string key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern byte[] Get(StorageContext context, string key);

        /// <summary>
        /// Writes byte[] value on byte[] key for given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, byte[] value);

        /// <summary>
        /// Writes BigInteger value on byte[] key for given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, BigInteger value);

        /// <summary>
        /// Writes string value on byte[] key for given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, string value);

        /// <summary>
        /// Writes byte[] value on string key for given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, string key, byte[] value);

        /// <summary>
        /// Writes BigInteger value on string key for given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, string key, BigInteger value);

        /// <summary>
        /// Writes string value on string key for given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, string key, string value);

        /// <summary>
        /// Deletes byte[] key from given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(StorageContext context, byte[] key);

        /// <summary>
        /// Deletes string key from given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(StorageContext context, string key);

        /// <summary>
        /// Returns a byte[] to byte[] iterator for a byte[] prefix on a given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Find")]
        public static extern Iterator<byte[], byte[]> Find(StorageContext context, byte[] prefix);

        /// <summary>
        /// Returns a string to byte[] iterator for a string prefix on a given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Find")]
        public static extern Iterator<string, byte[]> Find(StorageContext context, string prefix);

        /// <summary>
        /// Returns the byte[] value corresponding to given byte[] key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Get")]
        public static extern byte[] Get(byte[] key);

        /// <summary>
        /// Returns the byte[] value corresponding to given string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Get")]
        public static extern byte[] Get(string key);

        /// <summary>
        /// Writes byte[] value on byte[] key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Put")]
        public static extern void Put(byte[] key, byte[] value);

        /// <summary>
        /// Writes BigInteger value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.PutEx")]
        public static extern void PutEx(byte[] key, byte[] value, StorageFlags flags);

        /// <summary>
        /// Writes BigInteger value on byte[] key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Put")]
        public static extern void Put(byte[] key, BigInteger value);

        /// <summary>
        /// Writes BigInteger value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.PutEx")]
        public static extern void PutEx(byte[] key, BigInteger value, StorageFlags flags);

        /// <summary>
        /// Writes string value on byte[] key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Put")]
        public static extern void Put(byte[] key, string value);

        /// <summary>
        /// Writes BigInteger value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.PutEx")]
        public static extern void PutEx(byte[] key, string value, StorageFlags flags);

        /// <summary>
        /// Writes byte[] value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Put")]
        public static extern void Put(string key, byte[] value);

        /// <summary>
        /// Writes BigInteger value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.PutEx")]
        public static extern void PutEx(string key, byte[] value, StorageFlags flags);

        /// <summary>
        /// Writes BigInteger value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Put")]
        public static extern void Put(string key, BigInteger value);

        /// <summary>
        /// Writes BigInteger value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.PutEx")]
        public static extern void PutEx(string key, BigInteger value, StorageFlags flags);

        /// <summary>
        /// Writes string value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Put")]
        public static extern void Put(string key, string value);

        /// <summary>
        /// Writes string value on string key for current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.PutEx")]
        public static extern void PutEx(string key, string value, StorageFlags flags);

        /// <summary>
        /// Deletes byte[] key from current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(byte[] key);

        /// <summary>
        /// Deletes string key from given Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(string key);

        /// <summary>
        /// Returns a byte[] to byte[] iterator for a byte[] prefix on current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Find")]
        public static extern Iterator<byte[], byte[]> Find(byte[] prefix, byte findOptions);

        /// <summary>
        /// Returns a string to byte[] iterator for a string prefix on current Storage context
        /// </summary>
        [Syscall("System.Storage.GetContext")]
        [Syscall("System.Storage.Find")]
        public static extern Iterator<string, byte[]> Find(string prefix, byte findOptions);
    }
}
