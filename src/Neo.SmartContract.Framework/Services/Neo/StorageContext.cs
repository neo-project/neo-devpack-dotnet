namespace Neo.SmartContract.Framework.Services.Neo
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
