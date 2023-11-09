using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Safe
    {
        [TestMethod]
        public void UnitTest_TestSafe()
        {
            var testEngine = new TestEngine();
            testEngine.AddEntryScript("./TestClasses/Contract_ABISafe.cs");

            Assert.IsFalse(testEngine.Manifest.Abi.Methods[0].Safe);
            Assert.IsTrue(testEngine.Manifest.Abi.Methods[1].Safe);
            Assert.IsFalse(testEngine.Manifest.Abi.Methods[2].Safe);
        }
    }
}
