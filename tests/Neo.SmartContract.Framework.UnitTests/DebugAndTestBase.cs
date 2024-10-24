using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests;

public class DebugAndTestBase<T> : TestBase<T>
    where T : SmartContract.Testing.SmartContract, IContractInfo
{

    // allowing specific derived class to enable/disable Gas test
    protected virtual bool TestGasConsume { set; get; } = true;

    static DebugAndTestBase()
    {
        TestCleanup.TestInitialize(typeof(T));
    }

    protected void AssertGasConsumed(long gasConsumed)
    {
        if (TestGasConsume)
            Assert.AreEqual(gasConsumed, Engine.FeeConsumed.Value);
    }
}
