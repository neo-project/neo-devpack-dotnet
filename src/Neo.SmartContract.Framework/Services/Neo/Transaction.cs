namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Transaction
    {
        public readonly byte[] Hash;
        public readonly byte Version;
        public readonly uint Nonce;
        public readonly byte[] Sender;
        public readonly long SystemFee;
        public readonly long NetworkFee;
        public readonly uint ValidUntilBlock;
        public readonly byte[] Script;

        // TODO: Wait for merge https://github.com/neo-project/neo/pull/1081
    }
}
