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
            Assert.AreEqual(0, result.Count);


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
        public void Test_CreateStandardAccount()
        {
            // Wrong pubKey
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("createStandardAccount", new byte[] { 0x01, 0x02 });
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            _engine.Reset();

            // Good pubKey (compressed)
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("createStandardAccount", new byte[] { 0x02, 0x48, 0x6f, 0xd1, 0x57, 0x02, 0xc4, 0x49, 0x0a, 0x26, 0x70, 0x31, 0x12, 0xa5, 0xcc, 0x1d, 0x09, 0x23, 0xfd, 0x69, 0x7a, 0x33, 0x40, 0x6b, 0xd5, 0xa1, 0xc0, 0x0e, 0x00, 0x13, 0xb0, 0x9a, 0x70 });
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsTrue(item.Type == StackItemType.ByteString);
            Assert.AreEqual("e30262c57431d82b8295174f762fa36a5be973fd", item.GetSpan().ToHexString());

            // Good pubKey (uncompressed)
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("createStandardAccount", new byte[] { 0x04, 0x48, 0x6f, 0xd1, 0x57, 0x02, 0xc4, 0x49, 0x0a, 0x26, 0x70, 0x31, 0x12, 0xa5, 0xcc, 0x1d, 0x09, 0x23, 0xfd, 0x69, 0x7a, 0x33, 0x40, 0x6b, 0xd5, 0xa1, 0xc0, 0x0e, 0x00, 0x13, 0xb0, 0x9a, 0x70, 0x05, 0x43, 0x6c, 0x08, 0x2c, 0x2c, 0x88, 0x08, 0x5b, 0x4b, 0x53, 0xd5, 0x4c, 0x55, 0x66, 0xba, 0x44, 0x8d, 0x5c, 0x3e, 0x2a, 0x2a, 0x5c, 0x3a, 0x3e, 0xa5, 0x00, 0xe1, 0x40, 0x77, 0x55, 0x9c });
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsTrue(item.Type == StackItemType.ByteString);
            Assert.AreEqual("e30262c57431d82b8295174f762fa36a5be973fd", item.GetSpan().ToHexString());
        }

        [TestMethod]
        public void Test_GetCallFlags()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getCallFlags").Pop();
            StackItem wantResult = 0b00000111;
            Assert.AreEqual(wantResult, result);
        }
    }
}
