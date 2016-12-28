namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Block : Header
    {
        [Syscall("AntShares.Header.GetTransactionCount")]
        public extern int GetTransactionCount();

        [Syscall("AntShares.Header.GetTransactions")]
        public extern Transaction[] GetTransactions();

        [Syscall("AntShares.Header.GetTransaction")]
        public extern Transaction GetTransaction(int index);
    }
}
