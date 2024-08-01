using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleA : TestBase<Contract_MultipleA>
    {
        public UnitTest_MultipleA() : base(Contract_MultipleA.Nef, Contract_MultipleA.Manifest) { }

        [TestMethod]
        public void Test_NotThrow()
        {
            Assert.IsTrue(Contract.Test());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
