namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public static class Storage
    {
        public static extern StorageContext CurrentContext
        {
            [Syscall("AntShares.Storage.GetContext")]
            get;
        }

        [Syscall("AntShares.Storage.Get")]
        public static extern byte[] Get(StorageContext context, byte[] key);

        [Syscall("AntShares.Storage.Get")]
        public static extern byte[] Get(StorageContext context, string key);

        [Syscall("AntShares.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, byte[] value);

        [Syscall("AntShares.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, string value);

        [Syscall("AntShares.Storage.Put")]
        public static extern void Put(StorageContext context, string key, byte[] value);

        [Syscall("AntShares.Storage.Put")]
        public static extern void Put(StorageContext context, string key, string value);

        [Syscall("AntShares.Storage.Delete")]
        public static extern void Delete(StorageContext context, byte[] key);

        [Syscall("AntShares.Storage.Delete")]
        public static extern void Delete(StorageContext context, string key);
    }
}
