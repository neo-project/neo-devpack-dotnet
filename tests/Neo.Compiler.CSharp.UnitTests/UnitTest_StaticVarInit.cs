using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVarInit : TestBase<Contract_StaticVarInit>
    {
        [TestMethod]
        public void Test_StaticVarInit()
        {
            var var1 = Contract.StaticInit();
            Assert.AreEqual(1000470, Engine.FeeConsumed.Value);
            Assert.AreEqual(var1, Contract.Hash);

            var var2 = Contract.DirectGet();
            Assert.AreEqual(985530, Engine.FeeConsumed.Value);
            Assert.AreEqual(var1, var2);
        }
    }
}
