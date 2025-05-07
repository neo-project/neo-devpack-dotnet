using Neo.Persistence;
using Neo.Persistence.Providers;
using System;

namespace Neo.SmartContract.Fuzzer.Tests
{
    /// <summary>
    /// Provides a simple blockchain environment for testing
    /// </summary>
    public static class TestBlockchain
    {
        private static readonly MemoryStoreProvider _provider = new MemoryStoreProvider();

        /// <summary>
        /// Get a snapshot of the blockchain
        /// </summary>
        /// <returns>A snapshot of the blockchain</returns>
        public static DataCache GetSnapshot()
        {
            // Create a new snapshot for testing
            return new MemoryStoreProvider().GetStore().GetSnapshot();
        }
    }
}
