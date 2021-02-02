using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.TestingEngine;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Delegate
    {
        private TestEngine testengine;
        private TestDataCache snapshot;

        [TestInitialize]
        public void Init()
        {
            snapshot = new TestDataCache();
            testengine = new TestEngine(snapshot: snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_Delegate.cs");
        }

        [TestMethod]
        public void TestFunc()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("sumFunc", 2, 3).Pop();
            Assert.AreEqual(5, result.GetInteger());
        }

        [TestMethod]
        public void TestDelegateCall()
        {
            var token = NativeContract.NEO.Hash; // NEO token

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testDynamicCall", token.ToArray(), "symbol");
            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("NEO", item.GetString());
        }
    }
}
