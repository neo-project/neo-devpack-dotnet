using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
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
        private string scriptHash;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(TriggerType.Application, new DummyVerificable());
            _engine.AddEntryScript("./TestClasses/Contract_ExecutionEngine.cs");
            scriptHash = _engine.ScriptEntry.finalNEFScript.ToScriptHash().ToArray().ToHexString();
        }

        [TestMethod]
        public void CallingScriptHashTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("callingScriptHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));
        }

        [TestMethod]
        public void EntryScriptHashTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("entryScriptHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Buffer));
            //test by this way is bad idea? how to sure got a fix hash always?
            var gothash = item.GetSpan().ToHexString();
            Assert.AreEqual(scriptHash, gothash);
        }

        [TestMethod]
        public void ExecutingScriptHashTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("executingScriptHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Buffer));
            //test by this way is bad idea? how to sure got a fix hash always? 
            var gothash = item.GetSpan().ToHexString();
            Assert.AreEqual(scriptHash, gothash);
        }

        [TestMethod]
        public void ScriptContainerTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("scriptContainer");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop() as InteropInterface;
            Assert.AreEqual(null, item);
        }
    }
}
