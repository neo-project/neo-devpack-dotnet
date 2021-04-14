namespace Neo.SmartContract.Framework.Services
{
    public class StorageContext : IApiInterface
    {
        /// <summary>
        /// Returns current StorageContext as ReadOnly
        /// </summary>
        public extern StorageContext AsReadOnly
        {
            [Syscall("System.Storage.AsReadOnly")]
            get;
        }
    }
}
