namespace Neo.SmartContract.Framework.Services.Neo
{
    public class TransactionAttribute : IApiInterface
    {
        public extern byte Usage
        {
            [Syscall("Neo.Attribute.GetUsage")]
            get;
        }

        public extern byte[] Data
        {
            [Syscall("Neo.Attribute.GetData")]
            get;
        }
    }
}
