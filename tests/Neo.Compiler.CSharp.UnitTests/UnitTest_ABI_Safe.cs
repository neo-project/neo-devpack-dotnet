using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Safe() : TestBase<Contract_ABISafe>(Contract_ABISafe.Nef, Contract_ABISafe.Manifest)
    {
        [TestMethod]
        public void UnitTest_TestSafe()
        {
            Assert.IsFalse(Contract_ABISafe.Manifest.Abi.Methods[0].Safe);
            Assert.IsTrue(Contract_ABISafe.Manifest.Abi.Methods[1].Safe);
            Assert.IsFalse(Contract_ABISafe.Manifest.Abi.Methods[2].Safe);
        }

        [TestMethod]
        public void Method1Test()
        {
            Assert.AreEqual(1, Contract.UnitTest_001());
        }

        // [TestMethod]
        // public void Method2Test()
        // {
        //     Assert.AreEqual(2, Contract.UnitTest_002());
        // }

        [TestMethod]
        public void Method3Test()
        {
            Assert.AreEqual(3, Contract.UnitTest_003());
        }
    }
}
