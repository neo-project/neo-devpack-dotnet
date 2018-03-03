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
        public static extern StorageIterator Find(StorageContext context, byte[] prefix);

        [Syscall("Neo.Storage.Find")]
        public static extern StorageIterator Find(StorageContext context, string prefix);
    }
}
