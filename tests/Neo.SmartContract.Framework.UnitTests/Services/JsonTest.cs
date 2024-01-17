using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM;
using Neo.VM.Types;
using System.Text;
using Neo.SmartContract.TestEngine;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class JsonTest
    {
        private TestEngine.TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine.TestEngine(snapshot: new TestDataCache());
            _engine.AddEntryScript("./TestClasses/Contract_Json.cs");
        }

        [TestMethod]
        public void Test_SerializeDeserialize()
        {
            // Empty Serialize

            var result = _engine.ExecuteTestCaseStandard("serialize");
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Empty Serialize

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("deserialize");
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Serialize

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("serialize", new Array(_engine.ReferenceCounter, new StackItem[]{
                 StackItem.Null, StackItem.True, new ByteString(Encoding.ASCII.GetBytes("asd"))
            }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            Assert.AreEqual("[null,true,\"asd\"]", item.GetString());

            // Deserialize

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("deserialize", new ByteString(Encoding.ASCII.GetBytes("[null,true,\"asd\"]")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));

            var entry = ((Array)item)[0];
            Assert.IsInstanceOfType(entry, typeof(Null));
            entry = ((Array)item)[1];
            Assert.IsInstanceOfType(entry, typeof(Boolean));
            Assert.AreEqual(true, entry.GetBoolean());
            entry = ((Array)item)[2];
            Assert.IsInstanceOfType(entry, typeof(VM.Types.ByteString));
            Assert.AreEqual("asd", entry.GetString());
        }
    }
}
