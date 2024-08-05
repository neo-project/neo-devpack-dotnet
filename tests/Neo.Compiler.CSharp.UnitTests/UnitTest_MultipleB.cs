using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleB : TestBase2<Contract_MultipleB>
    {
        [TestMethod]
        public void Test()
        {
            Assert.IsFalse(Contract.Test());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
