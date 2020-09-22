namespace Neo.SmartContract.Framework.Services.Neo
{
    public class BlockTransactions
    {
        public extern int Length { [Syscall("System.BlockTransactions.GetLength")] get; }
        public extern Transaction this[int index] { [Syscall("System.BlockTransactions.GetTransaction")] get; }
    }
}
