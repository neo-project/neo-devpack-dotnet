using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Boolean() : TestBase2<Contract_Boolean>(Contract_Boolean.Nef, Contract_Boolean.Manifest)
    {
        [TestMethod]
        public void Test_BooleanOr()
        {
            Assert.AreEqual(true, Contract.TestBooleanOr());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
