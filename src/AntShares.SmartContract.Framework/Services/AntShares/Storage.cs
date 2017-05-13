namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Storage
    {
        [Syscall("AntShares.Storage.Get")]
        public extern byte[] Get(StorageContext context, byte[] key);

        [Syscall("AntShares.Storage.Get")]
        public extern byte[] Get(StorageContext context, string key);

        [Syscall("AntShares.Storage.Put")]
        public extern void Put(StorageContext context, byte[] key, byte[] value);

        [Syscall("AntShares.Storage.Put")]
        public extern void Put(StorageContext context, byte[] key, string value);

        [Syscall("AntShares.Storage.Put")]
        public extern void Put(StorageContext context, string key, byte[] value);

        [Syscall("AntShares.Storage.Put")]
        public extern void Put(StorageContext context, string key, string value);

        [Syscall("AntShares.Storage.Delete")]
        public extern void Delete(StorageContext context, byte[] key);

        [Syscall("AntShares.Storage.Delete")]
        public extern void Delete(StorageContext context, string key);
    }
}
