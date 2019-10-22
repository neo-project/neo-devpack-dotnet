using Moq;
using Neo.Compiler.MSIL.Utils;
using Neo.Ledger;
using Neo.Persistence;

namespace Neo.SmartContract.Framework.UnitTests
{
    public static class TestBlockchain
    {
        private static NeoSystem TheNeoSystem;
        private static Mock<Store> _Store;

        public static Store GetStore()
        {
            if (_Store == null) InitializeMockNeoSystem();
            return _Store.Object;
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
                var mockSnapshot = new TestSnapshot();

                _Store = new Mock<Store>();

                _Store.Setup(p => p.GetBlocks()).Returns(mockSnapshot.Blocks);
                _Store.Setup(p => p.GetTransactions()).Returns(mockSnapshot.Transactions);
                _Store.Setup(p => p.GetContracts()).Returns(mockSnapshot.Contracts);
                _Store.Setup(p => p.GetStorages()).Returns(mockSnapshot.Storages);
                _Store.Setup(p => p.GetHeaderHashList()).Returns(mockSnapshot.HeaderHashList);
                _Store.Setup(p => p.GetBlockHashIndex()).Returns(mockSnapshot.BlockHashIndex);
                _Store.Setup(p => p.GetHeaderHashIndex()).Returns(mockSnapshot.HeaderHashIndex);
                _Store.Setup(p => p.GetSnapshot()).Returns(mockSnapshot.Clone());

                TheNeoSystem = new NeoSystem(_Store.Object);

                // Ensure that blockchain is loaded

                var blockchain = Blockchain.Singleton;
            }

            return TheNeoSystem;
        }
    }
}
