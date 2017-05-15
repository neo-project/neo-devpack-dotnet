namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Header : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("AntShares.Header.GetHash")]
            get;
        }

        public extern uint Version
        {
            [Syscall("AntShares.Header.GetVersion")]
            get;
        }

        public extern byte[] PrevHash
        {
            [Syscall("AntShares.Header.GetPrevHash")]
            get;
        }

        public extern byte[] MerkleRoot
        {
            [Syscall("AntShares.Header.GetMerkleRoot")]
            get;
        }

        public extern uint Timestamp
        {
            [Syscall("AntShares.Header.GetTimestamp")]
            get;
        }

        public extern ulong ConsensusData
        {
            [Syscall("AntShares.Header.GetConsensusData")]
            get;
        }

        public extern byte[] NextConsensus
        {
            [Syscall("AntShares.Header.GetNextConsensus")]
            get;
        }
    }
}
