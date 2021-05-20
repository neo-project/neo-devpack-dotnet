using Neo.Wallets;

namespace Neo.TestingEngine
{
    class TestAccount : WalletAccount
    {
        public override bool HasKey => this.key != null;
        private KeyPair key = null;

        public TestAccount(UInt160 scriptHash, byte[] privKey = null) : base(scriptHash, ProtocolSettings.Default)
        {
            if (privKey != null)
            {
                key = new KeyPair(privKey);
            }
        }

        public override KeyPair GetKey()
        {
            return this.key;
        }
    }
}
