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

        public extern byte[] Sender
        {
            [Syscall("Neo.Transaction.GetSender")]
            get;
        }

        public extern uint Nonce
        {
            [Syscall("Neo.Transaction.GetNonce")]
            get;
        }

        [Syscall("Neo.Transaction.GetAttributes")]
        public extern TransactionAttribute[] GetAttributes();
    }
}
