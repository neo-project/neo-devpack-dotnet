using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Partial : TestBase<Contract_Partial>
    {
        [TestMethod]
        public void Test_Partial()
        {
            Assert.AreEqual(1, Contract.Test1());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
            Assert.AreEqual(2, Contract.Test2());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
