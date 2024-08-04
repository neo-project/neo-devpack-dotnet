using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Throw : TestBase2<Contract_Throw>
    {
        public UnitTest_Throw() : base(Contract_Throw.Nef, Contract_Throw.Manifest) { }

        [TestMethod]
        public void Test_Throw()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestMain([]));
            Assert.AreEqual(1063530, Engine.FeeConsumed.Value);
            Assert.IsTrue(exception.Message.Contains("Please supply at least one argument."));
        }

        [TestMethod]
        public void Test_NotThrow()
        {
            Contract.TestMain(["test"]);
            Assert.AreEqual(1111290, Engine.FeeConsumed.Value);
        }
    }
}
