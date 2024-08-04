using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_PostfixUnary() : TestBase2<Contract_PostfixUnary>(Contract_PostfixUnary.Nef, Contract_PostfixUnary.Manifest)
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("John", Contract.Test());
        }
    }
}
