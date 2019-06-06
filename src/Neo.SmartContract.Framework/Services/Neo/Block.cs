namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Block : Header
    {
        [Syscall("Neo.Block.GetTransactionCount")]
        public extern int GetTransactionCount();

        [Syscall("Neo.Block.GetTransactions")]
        public extern Transaction[] GetTransactions();

        [Syscall("Neo.Block.GetTransaction")]
        public extern Transaction GetTransaction(int index);
    }
}
