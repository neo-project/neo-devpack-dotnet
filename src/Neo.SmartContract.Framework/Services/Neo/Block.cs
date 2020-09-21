namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Block
    {
        public readonly byte[] Hash;
        public readonly uint Version;
        public readonly byte[] PrevHash;
        public readonly byte[] MerkleRoot;
        public readonly ulong Timestamp;
        public readonly uint Index;
        public readonly byte[] NextConsensus;
        public readonly BlockTransactions Transactions;
    }

    public class BlockTransactions
    {
        public extern int Length { [Syscall("System.BlockTransactions.GetLength")] get; }
        public extern Transaction this[int index] { [Syscall("System.BlockTransactions.GetTransaction")] get; }
    }
}
