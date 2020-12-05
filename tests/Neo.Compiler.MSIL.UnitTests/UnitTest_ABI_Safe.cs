using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Safe
    {
        [TestMethod]
        public void UnitTest_TestSafe()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_ABIOSafe.cs", true, false);
            var abi = buildScript.finalABI;

            var methodsABI = abi["methods"] as JArray;
            Assert.IsFalse(methodsABI[0]["safe"].AsBoolean());
            Assert.IsTrue(methodsABI[1]["safe"].AsBoolean());
            Assert.IsFalse(methodsABI[2]["safe"].AsBoolean());
        }
    }
}
