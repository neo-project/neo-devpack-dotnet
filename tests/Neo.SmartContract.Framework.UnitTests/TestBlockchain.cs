using Neo.Ledger;
using Neo.Persistence;

namespace Neo.SmartContract.Framework.UnitTests
{
    public static class TestBlockchain
    {
        private static NeoSystem TheNeoSystem;
        private static IStore _Store;

        public static IStore GetStore()
        {
            if (_Store == null) InitializeMockNeoSystem();
            return _Store;
        }

        static TestBlockchain()
        {
            InitializeMockNeoSystem();
            GetStore();
        }

        public static NeoSystem InitializeMockNeoSystem()
        {
            if (TheNeoSystem == null)
            {
                _Store = new MemoryStore();
                TheNeoSystem = new NeoSystem(null);

                // Ensure that blockchain is loaded

                var blockchain = Blockchain.Singleton;
            }

            return TheNeoSystem;
        }
    }
}
