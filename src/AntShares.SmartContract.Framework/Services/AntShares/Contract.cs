namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Contract
    {
        public extern byte[] Script
        {
            [Syscall("AntShares.Contract.GetScript")]
            get;
        }

        public extern StorageContext StorageContext
        {
            [Syscall("AntShares.Contract.GetStorageContext")]
            get;
        }

        [Syscall("AntShares.Contract.Create")]
        public static extern Contract Create(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("AntShares.Contract.Migrate")]
        public static extern Contract Migrate(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("AntShares.Contract.Destroy")]
        public static extern void Destroy();
    }
}
