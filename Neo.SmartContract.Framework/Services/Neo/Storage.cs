using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Storage
    {
        public static extern StorageContext CurrentContext
        {
            [Syscall("Neo.Storage.GetContext")]
            get;
        }

        [Syscall("Neo.Storage.Get")]
        public static extern byte[] Get(StorageContext context, byte[] key);

        [Syscall("Neo.Storage.Get")]
        public static extern byte[] Get(StorageContext context, string key);

        [Syscall("Neo.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, byte[] value);

        [Syscall("Neo.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, BigInteger value);

        [Syscall("Neo.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, string value);

        [Syscall("Neo.Storage.Put")]
        public static extern void Put(StorageContext context, string key, byte[] value);

        [Syscall("Neo.Storage.Put")]
        public static extern void Put(StorageContext context, string key, BigInteger value);

        [Syscall("Neo.Storage.Put")]
        public static extern void Put(StorageContext context, string key, string value);

        [Syscall("Neo.Storage.Delete")]
        public static extern void Delete(StorageContext context, byte[] key);

        [Syscall("Neo.Storage.Delete")]
        public static extern void Delete(StorageContext context, string key);

        [Syscall("Neo.Storage.Find")]
        public static extern Iterator<byte[], byte[]> Find(StorageContext context, byte[] prefix);

        [Syscall("Neo.Storage.Find")]
        public static extern Iterator<string, byte[]> Find(StorageContext context, string prefix);
        
        public static byte[] Get(byte[] key)
        {
            return Get(CurrentContext, key);
        }

        public static byte[] Get(string key)
        {
            return Get(CurrentContext, key);
        }
        
        public static void Put(byte[] key, byte[] value)
        {
            Put(CurrentContext, key, value);
        }

        public static void Put(byte[] key, BigInteger value)
        {
            Put(CurrentContext, key, value);
        }

        public static void Put(byte[] key, string value)
        {
            Put(CurrentContext, key, value);
        }

        public static void Put(string key, byte[] value)
        {
            Put(CurrentContext, key, value);
        }

        public static void Put(string key, BigInteger value)
        {
            Put(CurrentContext, key, value);
        }

        public static void Put(string key, string value)
        {
            Put(CurrentContext, key, value);
        }

        public static void Delete(byte[] key)
        {
            Delete(CurrentContext, key);
        }

        public static void Delete(string key)
        {
            Delete(CurrentContext, key);
        }
        
        public static Iterator<byte[], byte[]> Find(byte[] prefix)
        {
            return Find(CurrentContext, prefix);
        }

        public static Iterator<string, byte[]> Find(string prefix)
        {
            return Find(CurrentContext, prefix);
        }
    }
}
