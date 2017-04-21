namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public static class Blockchain
    {
        [Syscall("AntShares.Blockchain.GetHeight")]
        public static extern uint GetHeight();

        [Syscall("AntShares.Blockchain.GetHeader")]
        public static extern Header GetHeader(uint height);

        [Syscall("AntShares.Blockchain.GetHeader")]
        public static extern Header GetHeader(byte[] hash);

        [Syscall("AntShares.Blockchain.GetBlock")]
        public static extern Block GetBlock(uint height);

        [Syscall("AntShares.Blockchain.GetBlock")]
        public static extern Block GetBlock(byte[] hash);

        [Syscall("AntShares.Blockchain.GetTransaction")]
        public static extern Transaction GetTransaction(byte[] hash);

        [Syscall("AntShares.Blockchain.GetAccount")]
        public static extern Account GetAccount(byte[] script_hash);

        [Syscall("AntShares.Blockchain.GetAsset")]
        public static extern Asset GetAsset(byte[] asset_id);
    }
}
