using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Extensions : DebugAndTestBase<Contract_Extensions>
    {
        [TestMethod]
        public void TestSum()
        {
            Assert.AreEqual(5, Contract.TestSum(3, 2));
            AssertGasConsumed(1065060);
        }
    }
}
