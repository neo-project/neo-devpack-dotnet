using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_PostfixUnary : DebugAndTestBase<Contract_PostfixUnary>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("John", Contract.Test());
        }

        [TestMethod]
        public void TestUndefinedCase()
        {
            Contract.TestUndefinedCase();
        }

        [TestMethod]
        public void TestInvert()
        {
            Contract.TestInvert();
        }
    }
}
