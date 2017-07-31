namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Contract
    {
        public extern byte[] Script
        {
            [Syscall("Neo.Contract.GetScript")]
            get;
        }

        public extern StorageContext StorageContext
        {
            [Syscall("Neo.Contract.GetStorageContext")]
            get;
        }

        [Syscall("Neo.Contract.Create")]
        public static extern Contract Create(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("Neo.Contract.Migrate")]
        public static extern Contract Migrate(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("Neo.Contract.Destroy")]
        public static extern void Destroy();
    }
}
