using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using Neo.VM.Types;
using System.Text;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.Neo
{
    [TestClass]
    public class JsonTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Json.cs");
        }

        [TestMethod]
        public void Test_SerializeDeserialize()
        {
            // Empty Serialize

            var result = _engine.ExecuteTestCaseStandard("Serialize");
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Empty Serialize

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("Deserialize");
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Serialize

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("Serialize", new Array(new StackItem[]{
                 StackItem.Null, new Boolean(true), new ByteArray(Encoding.ASCII.GetBytes("asd"))
            }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("[null,true,\"asd\"]", item.GetString());

            // Deserialize

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("Deserialize", new ByteArray(Encoding.ASCII.GetBytes("[null,true,\"asd\"]")));
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
            Assert.IsInstanceOfType(entry, typeof(ByteArray));
            Assert.AreEqual("asd", entry.GetString());
        }
    }
}
