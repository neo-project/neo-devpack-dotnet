using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Partial : DebugAndTestBase<Contract_Partial>
    {
        [TestMethod]
        public void Test_Partial()
        {
            Assert.AreEqual(1, Contract.Test1());
            AssertGasConsumed(984060);
            Assert.AreEqual(2, Contract.Test2());
            AssertGasConsumed(984060);
        }
    }
}
