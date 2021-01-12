using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class DeletegateTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Delegate.cs");
        }

        [TestMethod]
        public void TestDelegateCall()
        {
            _engine.Reset();

            byte[] token = NeonTestTool.HexString2Bytes("0x4961bf0ab79370b23dc45cde29f568d0e0fa6e93"); // NEO token
            var result = _engine.ExecuteTestCaseStandard("testDynamicCall", token, "symbol");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("NEO", item.GetString());
        }
    }
}
