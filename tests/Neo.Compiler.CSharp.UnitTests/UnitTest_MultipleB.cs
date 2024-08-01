using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleB : TestBase<Contract_MultipleB>
    {
        public UnitTest_MultipleB() : base(Contract_MultipleB.Nef, Contract_MultipleB.Manifest) { }

        [TestMethod]
        public void Test_NotThrow()
        {
            Assert.IsFalse(Contract.Test());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
