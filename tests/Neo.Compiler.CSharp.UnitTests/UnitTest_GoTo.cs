using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_GoTo : DebugAndTestBase<Contract_GoTo>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(3, Contract.Test());
            AssertGasConsumed(989040);
            Assert.AreEqual(3, Contract.TestTry());
            AssertGasConsumed(989580);
        }
    }
}
