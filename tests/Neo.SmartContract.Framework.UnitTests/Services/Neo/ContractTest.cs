using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
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
            _engine = new TestEngine(verificable: new Transaction()
            {
                Signers = new Signer[] { new Signer() { Account = UInt160.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff01") } }
            });
            _engine.AddEntryScript("./TestClasses/Contract_Contract.cs");
            _engine.Snapshot.DeployNativeContracts(Blockchain.GenesisBlock);
        }

        [TestMethod]
        public void Test_CreateCallDestroy()
        {
            // Create

            var script = _engine.Build("./TestClasses/Contract_Create.cs");
            var manifest = ContractManifest.FromJson(JObject.Parse(script.finalManifest));
            var nef = new NefFile() { Script = script.finalNEFScript, Compiler = "unit-test", Version = "1.0", Tokens = System.Array.Empty<MethodToken>() };
            var hash = Helper.GetContractHash((_engine.ScriptContainer as Transaction).Sender, nef.Script);
            nef.CheckSum = NefFile.ComputeChecksum(nef);

            // Create

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("create", nef.ToArray(), manifest.ToJson().ToString());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));
            var itemArray = item as Array;
            Assert.AreEqual(1, itemArray[0].GetInteger()); // Id
            Assert.AreEqual(0, itemArray[1].GetInteger()); // UpdateCounter
            Assert.AreEqual(hash.ToArray(), itemArray[2]); // Hash
            Assert.AreEqual(nef.ToJson().AsString(), itemArray[3].GetSpan().AsSerializable<NefFile>().ToJson().AsString()); // Nef
            Assert.AreEqual(manifest.ToString(), itemArray[4].GetString()); // Manifest

            // Call

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", hash.ToArray(), "oldContract", (byte)CallFlags.All, new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetInteger());

            // Destroy

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("destroy");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Check again for failures

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", hash.ToArray());
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_Update()
        {
            // Create

            var script = _engine.Build("./TestClasses/Contract_CreateAndUpdate.cs");
            var manifest = ContractManifest.FromJson(JObject.Parse(script.finalManifest));
            var nef = new NefFile() { Script = script.finalNEFScript, Compiler = "unit-test", Version = "1.0", Tokens = System.Array.Empty<MethodToken>() };
            var hash = Helper.GetContractHash((_engine.ScriptContainer as Transaction).Sender, nef.Script);
            nef.CheckSum = NefFile.ComputeChecksum(nef);

            var scriptUpdate = _engine.Build("./TestClasses/Contract_Update.cs");
            var manifestUpdate = ContractManifest.FromJson(JObject.Parse(scriptUpdate.finalManifest));

            // Create

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("create", nef.ToArray(), manifest.ToJson().ToString());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));
            var itemArray = item as Array;
            Assert.AreEqual(1, itemArray[0].GetInteger()); // Id
            Assert.AreEqual(0, itemArray[1].GetInteger()); // UpdateCounter
            Assert.AreEqual(hash.ToArray(), itemArray[2]); // Hash
            Assert.AreEqual(nef.ToJson().AsString(), itemArray[3].GetSpan().AsSerializable<NefFile>().ToJson().AsString()); // Nef
            Assert.AreEqual(manifest.ToString(), itemArray[4].GetString()); // Manifest

            // Call & Update

            _engine.Reset();
            nef.Script = scriptUpdate.finalNEFScript;
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            result = _engine.ExecuteTestCaseStandard("call", hash.ToArray(), "oldContract", (byte)CallFlags.All,
                new Array(new StackItem[] { nef.ToArray(), manifestUpdate.ToJson().ToString() }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetInteger());

            // Call Again

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", hash.ToArray(), "newContract", (byte)CallFlags.All, new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(124, item.GetInteger());

            // Check again for failures

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", hash.ToArray(), "oldContract", new Array());
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
            Assert.IsTrue(item.Type == VM.Types.StackItemType.ByteString);
            Assert.AreEqual("3ae15fc83b48d9bb5c327e578e2f1d2100ba1b89", item.GetSpan().ToHexString());

            // Good pubKey (uncompressed)

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("createStandardAccount", new byte[] { 0x04, 0x48, 0x6f, 0xd1, 0x57, 0x02, 0xc4, 0x49, 0x0a, 0x26, 0x70, 0x31, 0x12, 0xa5, 0xcc, 0x1d, 0x09, 0x23, 0xfd, 0x69, 0x7a, 0x33, 0x40, 0x6b, 0xd5, 0xa1, 0xc0, 0x0e, 0x00, 0x13, 0xb0, 0x9a, 0x70, 0x05, 0x43, 0x6c, 0x08, 0x2c, 0x2c, 0x88, 0x08, 0x5b, 0x4b, 0x53, 0xd5, 0x4c, 0x55, 0x66, 0xba, 0x44, 0x8d, 0x5c, 0x3e, 0x2a, 0x2a, 0x5c, 0x3a, 0x3e, 0xa5, 0x00, 0xe1, 0x40, 0x77, 0x55, 0x9c });
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsTrue(item.Type == VM.Types.StackItemType.ByteString);
            Assert.AreEqual("3ae15fc83b48d9bb5c327e578e2f1d2100ba1b89", item.GetSpan().ToHexString());
        }

        [TestMethod]
        public void Test_GetCallFlags()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getCallFlags").Pop();
            StackItem wantResult = 0b00001111;
            Assert.AreEqual(wantResult, result);
        }
    }
}
