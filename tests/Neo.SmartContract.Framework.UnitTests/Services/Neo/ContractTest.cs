using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.IO.Json;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using Array = Neo.VM.Types.Array;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class ContractTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Contract.cs");
        }

        [TestMethod]
        public void Test_CreateCallDestroy()
        {
            // Create

            var script = _engine.Build("./TestClasses/Contract_Create.cs");
            var manifest = ContractManifest.FromJson(JObject.Parse(script.finalManifest));

            // Check first

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), "oldContract", new Array());
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Create

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("create", script.finalNEF, manifest.ToJson().ToString());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsTrue(item.Type == VM.Types.StackItemType.InteropInterface);
            var ledger = (item as InteropInterface).GetInterface<Ledger.ContractState>();
            Assert.AreEqual(manifest.Hash, ledger.ScriptHash);

            // Call

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), "oldContract", new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetBigInteger());

            // Destroy

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("destroy");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetByteLength());

            // Check again for failures

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray());
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_Update()
        {
            // Create

            var script = _engine.Build("./TestClasses/Contract_CreateAndUpdate.cs");
            var manifest = ContractManifest.FromJson(JObject.Parse(script.finalManifest));

            var scriptUpdate = _engine.Build("./TestClasses/Contract_Update.cs");
            var manifestUpdate = ContractManifest.FromJson(JObject.Parse(scriptUpdate.finalManifest));

            // Check first

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), "oldContract", new Array());
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            _engine.Reset();
            _ = _engine.ExecuteTestCaseStandard("call", manifestUpdate.Hash.ToArray(), "newContract", new Array());
            Assert.AreEqual(VMState.FAULT, _engine.State);

            // Create

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("create", script.finalNEF, manifest.ToJson().ToString());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsTrue(item.Type == VM.Types.StackItemType.InteropInterface);
            var ledger = (item as InteropInterface).GetInterface<Ledger.ContractState>();
            Assert.AreEqual(manifest.Hash, ledger.ScriptHash);

            // Call & Update

            _engine.Reset();
            var args = new Array
            {
                scriptUpdate.finalNEF,
                manifestUpdate.ToJson().ToString()
            };
            result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), "oldContract", args);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetBigInteger());

            // Call Again

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", manifestUpdate.Hash.ToArray(), "newContract", new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(124, item.GetBigInteger());

            // Check again for failures

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), "oldContract", new Array());
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_GetCallFlags()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getCallFlags").Pop();
            StackItem wantResult = 0b00000111;
            Assert.AreEqual(wantResult, result);
        }

        [TestMethod]
        public void Test_CreateStandardAccount()
        {
            _engine.Reset();
            var pubkey = new byte[] { 0x03, 0x2d, 0xf7, 0x2f, 0x6e, 0x05, 0xc8, 0x6a, 0xc9, 0x2a, 0x35, 0x05, 0x53, 0x24, 0x6a, 0x76, 0x65, 0x18, 0x4a, 0x98, 0x9a, 0x3e, 0x8a, 0xe6, 0xaa, 0x69, 0x21, 0x3e, 0x57, 0xbe, 0xc6, 0xbe, 0x53 };
            var result = _engine.ExecuteTestCaseStandard("createStandardAccount", pubkey).Pop();
            StackItem wantResult = new byte[] { 0xFE, 0xCD, 0x5A, 0x2C, 0x4D, 0xE9, 0xD7, 0xFD, 0x32, 0x80, 0xBB, 0x08, 0xF2, 0xB1, 0x6D, 0xAC, 0xA7, 0x0A, 0x59, 0xE3 };
            Assert.AreEqual(wantResult.ConvertTo(StackItemType.ByteString), result);
        }
    }
}
