using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_PostfixUnary : TestBase<Contract_PostfixUnary>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("John", Contract.Test());
        }
    }
}
