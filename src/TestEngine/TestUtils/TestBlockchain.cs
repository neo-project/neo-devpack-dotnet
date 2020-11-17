using Neo.Ledger;

namespace Neo.TestingEngine
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
