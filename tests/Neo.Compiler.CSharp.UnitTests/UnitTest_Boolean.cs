using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Boolean() : DebugAndTestBase<Contract_Boolean>
    {
        [TestMethod]
        public void Test_BooleanOr()
        {
            Assert.AreEqual(true, Contract.TestBooleanOr());
            AssertGasConsumed(984060);
        }
    }
}
