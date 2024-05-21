using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Safe
    {
        [TestMethod]
        public void UnitTest_TestSafe()
        {
            Assert.IsFalse(Contract_ABISafe.Manifest.Abi.Methods[0].Safe);
            Assert.IsTrue(Contract_ABISafe.Manifest.Abi.Methods[1].Safe);
            Assert.IsFalse(Contract_ABISafe.Manifest.Abi.Methods[2].Safe);
        }
    }
}
