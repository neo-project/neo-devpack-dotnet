using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Safe
    {
        [TestMethod]
        public void UnitTest_TestSafe()
        {
            var testEngine = new TestEngine();
            var buildScript = testEngine.Build("./TestClasses/Contract_ABISafe.cs");
            var manifest = buildScript.manifest;
            var abi = manifest["abi"];

            var methodsABI = abi["methods"] as JArray;
            Assert.IsFalse(methodsABI[1]["safe"].AsBoolean());
            Assert.IsTrue(methodsABI[2]["safe"].AsBoolean());
            Assert.IsFalse(methodsABI[3]["safe"].AsBoolean());
        }
    }
}
