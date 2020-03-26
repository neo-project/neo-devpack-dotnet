using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NEOSmartContract = Neo.SmartContract;

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
            _engine = new TestEngine(TriggerType.Application, new DummyVerificable());
            _engine.AddEntryScript("./TestClasses/Contract_Runtime.cs");
        }

        [TestMethod]
        public void Test_InvocationCounter()
        {
            // Build script

            _engine.Reset();

            var contract = _engine.EntryScriptHash;
            _engine.Snapshot.Contracts.Add(contract, new Ledger.ContractState()
            {
                Script = _engine.InvocationStack.Peek().Script,
                Manifest = new NEOSmartContract.Manifest.ContractManifest()
                {
                }
            });

            _engine.InvocationStack.Clear();

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                // First
                sb.EmitAppCall(contract, "getInvocationCounter");
                // Second
                sb.EmitAppCall(contract, "getInvocationCounter");

                _engine.LoadScript(sb.ToArray());
            }

            // Check

            Assert.AreEqual(VMState.HALT, _engine.Execute());
            Assert.AreEqual(2, _engine.ResultStack.Count);

            var item = _engine.ResultStack.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());

            item = _engine.ResultStack.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetBigInteger());
        }
        
        [TestMethod]
        public void Test_GasLeft()
        {
            var result = _engine.ExecuteTestCaseStandard("getGasLeft");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(-1, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_Time()
        {
            ((TestSnapshot)_engine.Snapshot).SetPersistingBlock(new Block()
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

            var result = _engine.ExecuteTestCaseStandard("getTime");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(1234, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_Platform()
        {
            var result = _engine.ExecuteTestCaseStandard("getPlatform");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("NEO", item.GetString());
        }

        [TestMethod]
        public void Test_Trigger()
        {
            var result = _engine.ExecuteTestCaseStandard("getTrigger");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual((byte)TriggerType.Application, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_Log()
        {
            var list = new List<LogEventArgs>();
            var method = new EventHandler<LogEventArgs>((s, e) => list.Add(e));

            ApplicationEngine.Log += method;
            var result = _engine.ExecuteTestCaseStandard("log", new ByteArray(Encoding.UTF8.GetBytes("LogTest")));
            ApplicationEngine.Log -= method;

            Assert.AreEqual(1, list.Count);

            var item = list[0];
            Assert.AreEqual("LogTest", item.Message);
        }

        [TestMethod]
        public void Test_CheckWitness()
        {
            // True

            var result = _engine.ExecuteTestCaseStandard("checkWitness", new ByteArray(
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, }
                ));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("checkWitness", new ByteArray(
                new byte[] { 0xFA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, }
                ));
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Boolean));
            Assert.IsFalse(item.ToBoolean());
        }

        [TestMethod]
        public void Test_Notify()
        {
            var list = new List<NotifyEventArgs>();
            var method = new EventHandler<NotifyEventArgs>((s, e) => list.Add(e));

            ApplicationEngine.Notify += method;
            var result = _engine.ExecuteTestCaseStandard("notify", new ByteArray(Encoding.UTF8.GetBytes("NotifyTest")));
            ApplicationEngine.Notify -= method;

            Assert.AreEqual(1, list.Count);

            var item = list[0];
            var array = item.State;
            Assert.IsInstanceOfType(array, typeof(VM.Types.Array));
            Assert.AreEqual("NotifyTest", ((VM.Types.Array)array)[0].GetString());
        }

        [TestMethod]
        public void Test_GetNotificationsCount()
        {
            var notifications = ((List<NotifyEventArgs>)_engine.Notifications);
            notifications.Clear();
            notifications.AddRange(new NotifyEventArgs[]
            {
                new NotifyEventArgs(null, UInt160.Zero, new Integer(0x01)),
                new NotifyEventArgs(null, UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), new Integer(0x02))
            });

            var result = _engine.ExecuteTestCaseStandard("getNotificationsCount", new ByteArray(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getNotificationsCount", StackItem.Null);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_GetNotifications()
        {
            var notifications = ((List<NotifyEventArgs>)_engine.Notifications);
            notifications.Clear();
            notifications.AddRange(new NotifyEventArgs[]
            {
                new NotifyEventArgs(null, UInt160.Zero, new Integer(0x01)),
                new NotifyEventArgs(null, UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), new Integer(0x02))
            });

            var result = _engine.ExecuteTestCaseStandard("getNotifications", new ByteArray(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getAllNotifications");
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x03, item.GetBigInteger());
        }
    }
}
