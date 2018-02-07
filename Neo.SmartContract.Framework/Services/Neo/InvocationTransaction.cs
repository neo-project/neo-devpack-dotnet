namespace Neo.SmartContract.Framework.Services.Neo
{
    public class InvocationTransaction : Transaction
    {
        public extern byte[] Script
        {
            [Syscall("Neo.InvocationTransaction.GetScript")]
            get;
        }
    }
}
