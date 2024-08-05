using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Safe : DebugAndTestBase<Contract_ABISafe>
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

        [TestMethod]
        public void Method3Test()
        {
            Assert.AreEqual(3, Contract.UnitTest_003());
        }
    }
}
