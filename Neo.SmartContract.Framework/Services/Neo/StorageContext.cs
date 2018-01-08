namespace Neo.SmartContract.Framework.Services.Neo
{
    public class StorageContext
    {
        public extern byte[] this[byte[] key]
        {
            [Syscall("Neo.Storage.Get")]
            get;
            [Syscall("Neo.Storage.Put")]
            set;
        }


        public extern byte[] this[string key]
        {
            [Syscall("Neo.Storage.Get")]
            get;
            [Syscall("Neo.Storage.Put")]
            set;
        }
    }
}
