using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Extensions : TestBase2<Contract_Extensions>
    {
        [TestMethod]
        public void TestSum()
        {
            Assert.AreEqual(5, Contract.TestSum(3, 2));
            Assert.AreEqual(1065060, Engine.FeeConsumed.Value);
        }
    }
}
