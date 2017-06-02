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

        [Syscall("AntShares.Blockchain.RegisterValidator")]
        public static extern byte[] RegisterValidator(byte[] pubkey);

        [Syscall("AntShares.Blockchain.GetValidators")]
        public static extern byte[][] GetValidators();

        [Syscall("AntShares.Blockchain.CreateAsset")]
        public static extern Asset CreateAsset(byte asset_type, string name, long amount, byte precision, byte[] owner, byte[] admin, byte[] issuer);

        [Syscall("AntShares.Blockchain.GetAsset")]
        public static extern Asset GetAsset(byte[] asset_id);

        [Syscall("AntShares.Blockchain.CreateContract")]
        public static extern Contract CreateContract(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("AntShares.Blockchain.GetContract")]
        public static extern Contract GetContract(byte[] script_hash);
    }
}
