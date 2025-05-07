using Neo.VM;
using System;

namespace Neo.SmartContract.Fuzzer.IntegrationTests
{
    /// <summary>
    /// Helper class for creating a test blockchain for integration tests.
    /// </summary>
    public static class TestBlockchain
    {
        /// <summary>
        /// Creates an application engine for testing.
        /// </summary>
        /// <returns>An application engine for testing.</returns>
        public static ApplicationEngine CreateEngine()
        {
            // Create a standalone engine for testing
            return ApplicationEngine.Create(TriggerType.Application, null, null, 0, true);
        }
    }
}
