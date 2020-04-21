using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Offset
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            _engine.Reset();
            _engine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs", true, false);
            var abi = _engine.ScriptEntry.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("7", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("13", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("45", methodsABI[2].GetDictItem("offset").ToString());
            // _initialize()
            Assert.AreEqual("0", methodsABI[3].GetDictItem("offset").ToString());
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithOptimizer()
        {
            _engine.Reset();
            _engine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs", true, true);
            var abi = _engine.ScriptEntry.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("7", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("13", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("39", methodsABI[2].GetDictItem("offset").ToString());
            // _initialize()
            Assert.AreEqual("0", methodsABI[3].GetDictItem("offset").ToString());
        }

        [TestMethod]
        public void Test_UnitTest_001()
        {
            _engine.Reset();
            _engine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs");
            var result = _engine.GetMethod("unitTest_001").Run();

            StackItem wantResult = 3;
            Assert.AreEqual(wantResult.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }
    }
}
