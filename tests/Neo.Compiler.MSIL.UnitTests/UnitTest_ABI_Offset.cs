using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Offset
    {
        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_ABIOffset.cs", true, false);
            var abi = buildScript.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("0", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("6", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("38", methodsABI[2].GetDictItem("offset").ToString());
            // _initialize()
            Assert.AreEqual("49", methodsABI[3].GetDictItem("offset").ToString());
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithOptimizer()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_ABIOffset.cs", true, true);
            var abi = buildScript.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("0", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("5", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("30", methodsABI[2].GetDictItem("offset").ToString());
            // _initialize()
            Assert.AreEqual("36", methodsABI[3].GetDictItem("offset").ToString());
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
