using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Instance : DebugAndTestBase<Contract_Instance>
    {
        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(3, Contract.Sum(2));
            AssertGasConsumed(1640160);
            Assert.AreEqual(4, Contract.Sum(3));
            AssertGasConsumed(1640160);
            Assert.AreEqual(8, Contract.Sum2(3));
            AssertGasConsumed(1678110);
        }
    }
}
