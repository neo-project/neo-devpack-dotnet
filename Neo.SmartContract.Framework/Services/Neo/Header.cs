namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Header : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("Neo.Header.GetHash")]
            get;
        }

        public extern uint Version
        {
            [Syscall("Neo.Header.GetVersion")]
            get;
        }

        public extern byte[] PrevHash
        {
            [Syscall("Neo.Header.GetPrevHash")]
            get;
        }

        public extern byte[] MerkleRoot
        {
            [Syscall("Neo.Header.GetMerkleRoot")]
            get;
        }

        public extern uint Timestamp
        {
            [Syscall("Neo.Header.GetTimestamp")]
            get;
        }

        public extern ulong ConsensusData
        {
            [Syscall("Neo.Header.GetConsensusData")]
            get;
        }

        public extern byte[] NextConsensus
        {
            [Syscall("Neo.Header.GetNextConsensus")]
            get;
        }
    }
}
