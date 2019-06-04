namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Transaction : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("System.Transaction.GetHash")]
            get;
        }

        public extern byte[] Script
        {
            [Syscall("Neo.Transaction.GetScript")]
            get;
        }

        [Syscall("Neo.Transaction.GetAttributes")]
        public extern TransactionAttribute[] GetAttributes();
    }
}
