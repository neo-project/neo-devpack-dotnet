using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Offset
    {
        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_ABIOffset.cs", true, false);
            var abi = buildScript.finalABI;

            var methodsABI = abi["methods"] as JArray;
            Assert.AreEqual("0", methodsABI[0]["offset"].AsString());
            Assert.AreEqual("7", methodsABI[1]["offset"].AsString());
            Assert.AreEqual("13", methodsABI[2]["offset"].AsString());
            // _initialize()
            Assert.AreEqual("45", methodsABI[3]["offset"].AsString());
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithOptimizer()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_ABIOffset.cs", true, true);
            var abi = buildScript.finalABI;

            var methodsABI = abi["methods"] as JArray;
            Assert.AreEqual("0", methodsABI[0]["offset"].AsString());
            Assert.AreEqual("5", methodsABI[1]["offset"].AsString());
            Assert.AreEqual("10", methodsABI[2]["offset"].AsString());
            // _initialize()
            Assert.AreEqual("35", methodsABI[3]["offset"].AsString());
        }

        [TestMethod]
        public void Test_UnitTest_001()
        {
            var _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs");
            var result = _engine.GetMethod("unitTest_001").Run();

            StackItem wantResult = 3;
            Assert.AreEqual(wantResult.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }
    }
}
