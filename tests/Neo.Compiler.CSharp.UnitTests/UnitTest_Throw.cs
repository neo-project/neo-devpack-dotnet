using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Throw : DebugAndTestBase<Contract_Throw>
    {
        [TestMethod]
        public void Test_Throw()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestMain([]));
            AssertGasConsumed(1063530);
            Assert.IsTrue(exception.Message.Contains("Please supply at least one argument."));
        }

        [TestMethod]
        public void Test_NotThrow()
        {
            Contract.TestMain(["test"]);
            AssertGasConsumed(1111290);
        }
    }
}
