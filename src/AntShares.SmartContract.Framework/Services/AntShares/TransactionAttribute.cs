namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class TransactionAttribute : IApiInterface
    {
        public extern byte Usage
        {
            [Syscall("AntShares.Attribute.GetUsage")]
            get;
        }

        public extern byte[] Data
        {
            [Syscall("AntShares.Attribute.GetData")]
            get;
        }
    }
}
