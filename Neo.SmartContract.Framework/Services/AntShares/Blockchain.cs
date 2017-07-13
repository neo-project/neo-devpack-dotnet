namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Blockchain
    {
        [Syscall("Neo.Blockchain.GetHeight")]
        public static extern uint GetHeight();

        [Syscall("Neo.Blockchain.GetHeader")]
        public static extern Header GetHeader(uint height);

        [Syscall("Neo.Blockchain.GetHeader")]
        public static extern Header GetHeader(byte[] hash);

        [Syscall("Neo.Blockchain.GetBlock")]
        public static extern Block GetBlock(uint height);

        [Syscall("Neo.Blockchain.GetBlock")]
        public static extern Block GetBlock(byte[] hash);

        [Syscall("Neo.Blockchain.GetTransaction")]
        public static extern Transaction GetTransaction(byte[] hash);

        [Syscall("Neo.Blockchain.GetAccount")]
        public static extern Account GetAccount(byte[] script_hash);

        [Syscall("Neo.Blockchain.GetValidators")]
        public static extern byte[][] GetValidators();

        [Syscall("Neo.Blockchain.GetAsset")]
        public static extern Asset GetAsset(byte[] asset_id);

        [Syscall("Neo.Blockchain.GetContract")]
        public static extern Contract GetContract(byte[] script_hash);
    }
}
