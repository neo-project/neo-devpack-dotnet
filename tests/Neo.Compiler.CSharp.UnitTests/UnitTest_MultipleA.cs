using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleA : DebugAndTestBase<Contract_MultipleA>
    {
        [TestMethod]
        public void Test()
        {
            Assert.IsTrue(Contract.Test());
            AssertGasConsumed(984060);
        }
    }
}
