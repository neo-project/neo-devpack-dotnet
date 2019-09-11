namespace Neo.SmartContract.Framework.Services.Neo
{
    public class StorageContext
    {
        /// <summary>
        /// Returns current StorageContext as ReadOnly
        /// </summary>
        public static extern StorageContext AsReadOnly
        {
            [Syscall("System.StorageContext.AsReadOnly")]
            get;
        }
    }
}
