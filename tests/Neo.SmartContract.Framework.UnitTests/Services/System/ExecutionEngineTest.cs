using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.VM;
using Neo.VM.Types;
using System.IO;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.System
{
    [TestClass]
    public class ExecutionEngineTest
    {
        class DummyVerificable : IVerifiable
        {
            public Witness[] Witnesses { get; set; }

            public int Size => 0;

            public void Deserialize(BinaryReader reader)
            {
            }

            public void DeserializeUnsigned(BinaryReader reader)
            {
            }

            public UInt160[] GetScriptHashesForVerifying(StoreView snapshot)
            {
                throw new global::System.NotImplementedException();
            }

            public void Serialize(BinaryWriter writer)
            {
            }

            public void SerializeUnsigned(BinaryWriter writer)
            {
            }
        }

        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(SmartContract.TriggerType.Application, new DummyVerificable());
            _engine.AddEntryScript("./TestClasses/Contract_ExecutionEngine.cs");
        }

        [TestMethod]
        public void CallingScriptHashTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("CallingScriptHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("3632c01ec5cc1961ba49d8033798e469f6a6f697", item.GetSpan().ToHexString());
        }

        [TestMethod]
        public void EntryScriptHashTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("EntryScriptHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("3632c01ec5cc1961ba49d8033798e469f6a6f697", item.GetSpan().ToHexString());
        }

        [TestMethod]
        public void ExecutingScriptHashTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("ExecutingScriptHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("3632c01ec5cc1961ba49d8033798e469f6a6f697", item.GetSpan().ToHexString());
        }

        [TestMethod]
        public void ScriptContainerTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("ScriptContainer");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(InteropInterface<IVerifiable>));
            Assert.AreEqual(_engine.ScriptContainer, ((InteropInterface<IVerifiable>)item).GetInterface<IVerifiable>());
        }
    }
}
