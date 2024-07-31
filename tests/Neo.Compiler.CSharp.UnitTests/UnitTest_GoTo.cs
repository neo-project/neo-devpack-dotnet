using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_GoTo : TestBase<Contract_GoTo>
    {
        public UnitTest_GoTo() : base(Contract_GoTo.Nef, Contract_GoTo.Manifest) { }

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(3, Contract.Test());
            Assert.AreEqual(989760, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, Contract.TestTry());
            Assert.AreEqual(990180, Engine.FeeConsumed.Value);
        }
    }
}
