namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Block
    {
        public readonly UInt256 Hash;
        public readonly uint Version;
        public readonly UInt256 PrevHash;
        public readonly UInt256 MerkleRoot;
        public readonly ulong Timestamp;
        public readonly uint Index;
        public readonly UInt160 NextConsensus;
        public readonly int TransactionsCount;
    }
}
