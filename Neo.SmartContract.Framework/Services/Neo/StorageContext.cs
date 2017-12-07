namespace Neo.SmartContract.Framework.Services.Neo
{
    public class StorageContext
    {
        public byte[] this[byte[] key]
        {
            [Syscall("Neo.Storage.Get")]
            get
            {
                return Storage.Get(this, key);
            }
            [Syscall("Neo.Storage.Put")]
            set
            {
                Storage.Put(this, key, value);
            }
        }


        public byte[] this[string key]
        {
            [Syscall("Neo.Storage.Get")]
            get
            {
                return Storage.Get(this, key);
            }
            [Syscall("Neo.Storage.Put")]
            set
            {
                Storage.Put(this, key, value);
            }
        }
    }
}
