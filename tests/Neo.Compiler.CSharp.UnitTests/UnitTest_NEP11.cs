using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NEP11 : TestBase<Contract_NEP11>
    {
        [TestMethod]
        public void UnitTest_Symbol()
        {
            Assert.AreEqual("TEST", Contract.Symbol);
        }

        [TestMethod]
        public void UnitTest_Decimals()
        {
            Assert.AreEqual(0, Contract.Decimals);
        }
    }
}
