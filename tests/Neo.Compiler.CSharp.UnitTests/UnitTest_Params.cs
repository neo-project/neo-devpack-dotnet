using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Params : DebugAndTestBase<Contract_Params>
    {
        [TestMethod]
        public void Test_Params()
        {
            Assert.AreEqual(15, Contract.Test());
            AssertGasConsumed(1259370);
        }
    }
}
