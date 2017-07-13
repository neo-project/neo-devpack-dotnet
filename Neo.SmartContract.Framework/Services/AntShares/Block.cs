namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Block : Header
    {
        [Syscall("Neo.Header.GetTransactionCount")]
        public extern int GetTransactionCount();

        [Syscall("Neo.Header.GetTransactions")]
        public extern Transaction[] GetTransactions();

        [Syscall("Neo.Header.GetTransaction")]
        public extern Transaction GetTransaction(int index);
    }
}
