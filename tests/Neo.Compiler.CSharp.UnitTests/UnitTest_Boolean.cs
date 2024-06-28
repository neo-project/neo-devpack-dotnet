using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Boolean() : TestBase<Contract_Boolean>(Contract_Boolean.Nef, Contract_Boolean.Manifest)
    {
        [TestMethod]
        public void Test_BooleanOr()
        {
            Assert.AreEqual(true, Contract.TestBooleanOr());
        }
    }
}
