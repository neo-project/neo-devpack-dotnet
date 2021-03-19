using Neo.Wallets;

namespace Neo.TestingEngine
{
    class TestAccount : WalletAccount
    {
        public override bool HasKey => this.key != null;

        private byte[] privateKey;
        private UInt160 scriptHash;
        private KeyPair key = null;

        public TestAccount(UInt160 scriptHash, byte[] privKey = null) : base(scriptHash, ProtocolSettings.Default)
        {
            if (privKey != null)
            {
                this.privateKey = privKey;
            }
            else
            {
                this.privateKey = new byte[32];
            }

            this.scriptHash = scriptHash;
            this.key = new KeyPair(this.privateKey);
        }

        public override KeyPair GetKey()
        {
            return this.key;
        }
    }
}
