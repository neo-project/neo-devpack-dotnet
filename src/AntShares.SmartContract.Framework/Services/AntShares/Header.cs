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

        public extern ulong Nonce
        {
            [Syscall("AntShares.Header.GetNonce")]
            get;
        }

        public extern byte[] NextMiner
        {
            [Syscall("AntShares.Header.GetNextMiner")]
            get;
        }
    }
}
