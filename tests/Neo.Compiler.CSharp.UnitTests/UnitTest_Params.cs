using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Params : TestBase<Contract_Params>
    {
        public UnitTest_Params() : base(Contract_Params.Nef, Contract_Params.Manifest) { }

        [TestMethod]
        public void Test_Params()
        {
            Assert.AreEqual(15, Contract.Test());
            Assert.AreEqual(1002336070, Engine.FeeConsumed.Value);
        }
    }
}
