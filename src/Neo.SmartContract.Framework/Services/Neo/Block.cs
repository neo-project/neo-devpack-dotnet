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
        public readonly int TransactionsCount;

        // TODO: Wait for merge https://github.com/neo-project/neo/pull/1081
    }
}
