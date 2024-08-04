using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleA : TestBase2<Contract_MultipleA>
    {
        public UnitTest_MultipleA() : base(Contract_MultipleA.Nef, Contract_MultipleA.Manifest) { }

        [TestMethod]
        public void Test()
        {
            Assert.IsTrue(Contract.Test());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
