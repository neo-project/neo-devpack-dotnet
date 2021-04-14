namespace Neo.SmartContract.Framework.Services
{
    public class Transaction
    {
        public readonly UInt256 Hash;
        public readonly byte Version;
        public readonly uint Nonce;
        public readonly UInt160 Sender;
        public readonly long SystemFee;
        public readonly long NetworkFee;
        public readonly uint ValidUntilBlock;
        public readonly ByteString Script;
    }
}
