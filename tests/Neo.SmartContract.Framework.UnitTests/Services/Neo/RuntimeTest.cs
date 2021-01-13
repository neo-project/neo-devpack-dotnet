using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.IO;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class RuntimeTest
    {
        class DummyVerificable : IVerifiable
        {
            public Witness[] Witnesses { get; set; }

            public int Size => 0;

            public void Deserialize(BinaryReader reader) { }

            public void DeserializeUnsigned(BinaryReader reader) { }

            public UInt160[] GetScriptHashesForVerifying(StoreView snapshot)
            {
                return new UInt160[]
                {
                    UInt160.Parse("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                };
            }

            public void Serialize(BinaryWriter writer) { }

            public void SerializeUnsigned(BinaryWriter writer) { }
        }

        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(TriggerType.Application, new DummyVerificable(), persistingBlock: new Block()
            {
                Index = 123,
                Timestamp = 1234,
                ConsensusData = new ConsensusData(),
                Transactions = new Transaction[0],
                Witness = new Witness()
                {
                    InvocationScript = new byte[0],
                    VerificationScript = new byte[0]
                },
                NextConsensus = UInt160.Zero,
                MerkleRoot = UInt256.Zero,
                PrevHash = UInt256.Zero
            });
            _engine.AddEntryScript("./TestClasses/Contract_Runtime.cs");
        }

        [TestMethod]
        public void Test_InvocationCounter()
        {
            // We need a new TestEngine because invocationCounter it's shared between them

            var contract = _engine.EntryScriptHash;
            var engine = new TestEngine(TriggerType.Application, new DummyVerificable());
            engine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = contract,
                Nef = _engine.ScriptEntry.nefFile,
                Manifest = ContractManifest.FromJson(JObject.Parse(_engine.Build("./TestClasses/Contract_Runtime.cs").finalManifest)),
            });

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                // First
                sb.EmitDynamicCall(contract, "getInvocationCounter");
                // Second
                sb.EmitDynamicCall(contract, "getInvocationCounter");
                engine.LoadScript(sb.ToArray());
            }

            // Check

            Assert.AreEqual(VMState.HALT, engine.Execute());
            Assert.AreEqual(2, engine.ResultStack.Count);

            var item = engine.ResultStack.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetInteger());

            item = engine.ResultStack.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetInteger());
        }

        [TestMethod]
        public void Test_Time()
        {
            var result = _engine.ExecuteTestCaseStandard("getTime");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(1234, item.GetInteger());
        }

        [TestMethod]
        public void Test_Platform()
        {
            var result = _engine.ExecuteTestCaseStandard("getPlatform");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            Assert.AreEqual("NEO", item.GetString());
        }

        [TestMethod]
        public void Test_Trigger()
        {
            var result = _engine.ExecuteTestCaseStandard("getTrigger");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual((byte)TriggerType.Application, item.GetInteger());
        }

        [TestMethod]
        public void Test_GasLeft()
        {
            var result = _engine.ExecuteTestCaseStandard("getGasLeft");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(TestEngine.TestGas - 2400, item.GetInteger());
        }

        [TestMethod]
        public void Test_Log()
        {
            var list = new System.Collections.Generic.List<LogEventArgs>();
            var method = new EventHandler<LogEventArgs>((s, e) => list.Add(e));

            ApplicationEngine.Log += method;
            var result = _engine.ExecuteTestCaseStandard("log", new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("LogTest")));
            ApplicationEngine.Log -= method;

            Assert.AreEqual(1, list.Count);

            var item = list[0];
            Assert.AreEqual("LogTest", item.Message);
        }

        [TestMethod]
        public void Test_CheckWitness()
        {
            // True

            var result = _engine.ExecuteTestCaseStandard("checkWitness", new VM.Types.ByteString(
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, }
                ));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Boolean));
            Assert.IsTrue(item.GetBoolean());

            // False

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("checkWitness", new VM.Types.ByteString(
                new byte[] { 0xFA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, }
                ));
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Boolean));
            Assert.IsFalse(item.GetBoolean());
        }

        [TestMethod]
        public void Test_GetNotificationsCount()
        {
            _engine.ClearNotifications();
            _engine.SendTestNotification(UInt160.Zero, "", new VM.Types.Array(new StackItem[] { new Integer(0x01) }));
            _engine.SendTestNotification(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), "", new VM.Types.Array(new StackItem[] { new Integer(0x02) }));

            var result = _engine.ExecuteTestCaseStandard("getNotificationsCount", new VM.Types.ByteString(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getNotificationsCount", StackItem.Null);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetInteger());
        }

        [TestMethod]
        public void Test_GetNotifications()
        {
            _engine.ClearNotifications();
            _engine.SendTestNotification(UInt160.Zero, "", new VM.Types.Array(new StackItem[] { new Integer(0x01) }));
            _engine.SendTestNotification(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), "", new VM.Types.Array(new StackItem[] { new Integer(0x02) }));

            var result = _engine.ExecuteTestCaseStandard("getNotifications", new VM.Types.ByteString(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getAllNotifications");
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x03, item.GetInteger());
        }
    }
}
