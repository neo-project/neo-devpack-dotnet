using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Offset
    {
        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            var testEngine = new TestEngine();
            var buildScript = testEngine.Build("./TestClasses/Contract_ABIOffset.cs");
            var manifest = buildScript.manifest;
            var abi = manifest["abi"];

            var methodsABI = abi["methods"] as JArray;
            Assert.AreEqual("0", methodsABI[0]["offset"].AsString());
            Assert.AreEqual("7", methodsABI[1]["offset"].AsString());
            Assert.AreEqual("12", methodsABI[2]["offset"].AsString());
            // _initialize()
            Assert.AreEqual("43", methodsABI[3]["offset"].AsString());
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithOptimizer()
        {
            var testEngine = new TestEngine();
            var buildScript = testEngine.Build("./TestClasses/Contract_ABIOffset.cs");
            var manifest = buildScript.manifest;
            var abi = manifest["abi"];

            var methodsABI = abi["methods"] as JArray;
            Assert.AreEqual("0", methodsABI[0]["offset"].AsString());
            Assert.AreEqual("5", methodsABI[1]["offset"].AsString());
            Assert.AreEqual("9", methodsABI[2]["offset"].AsString());
            // _initialize()
            Assert.AreEqual("33", methodsABI[3]["offset"].AsString());
        }

        [TestMethod]
        public void Test_UnitTest_001()
        {
            var _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs");
            var result = _engine.ExecuteTestCaseStandard("unitTest_001");

            StackItem wantResult = 3;
            Assert.AreEqual(wantResult.ConvertTo(StackItemType.ByteString), result);
        }
    }
}
