using Neo.Ledger;

namespace Neo.SmartContract.Framework.UnitTests
{
    public static class TestBlockchain
    {
        public static readonly NeoSystem TheNeoSystem;

        static TestBlockchain()
        {
            TheNeoSystem = new NeoSystem();

            // Ensure that blockchain is loaded

            var _ = Blockchain.Singleton;
        }
    }
}
