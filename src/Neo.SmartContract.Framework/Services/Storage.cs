// Copyright (C) 2015-2025 The Neo Project.
//
// Storage.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
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
        public static extern ByteString? Get(StorageContext context, ByteString key);

        /// <summary>
        /// Returns the value corresponding to the given key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern ByteString? Get(StorageContext context, byte[] key);

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

        #region Interface with default Context
        public static ByteString? Get(ByteString key) => Get(CurrentReadOnlyContext, key);
        public static ByteString? Get(byte[] key) => Get(CurrentReadOnlyContext, key);
        public static void Put(ByteString key, ByteString value) => Put(CurrentContext, key, value);
        public static void Put(byte[] key, ByteString value) => Put(CurrentContext, key, value);
        public static void Put(byte[] key, byte[] value) => Put(CurrentContext, key, value);
        public static void Put(ByteString key, BigInteger value) => Put(CurrentContext, key, value);
        public static void Put(byte[] key, BigInteger value) => Put(CurrentContext, key, value);
        public static void Delete(ByteString key) => Delete(CurrentContext, key);
        public static void Delete(byte[] key) => Delete(CurrentContext, key);
        public static Iterator Find(ByteString prefix, FindOptions options = FindOptions.None) => Find(CurrentReadOnlyContext, prefix, options);
        public static Iterator Find(byte[] prefix, FindOptions options = FindOptions.None) => Find(CurrentReadOnlyContext, prefix, options);
        #endregion
    }
}
