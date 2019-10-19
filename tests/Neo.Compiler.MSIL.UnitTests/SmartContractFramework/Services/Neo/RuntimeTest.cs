using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.Neo
{
    [TestClass]
    public class RuntimeTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(TriggerType.Application);
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
                Script = _engine.InvocationStack.Peek(0).Script,
                Manifest = new SmartContract.Manifest.ContractManifest()
                {
                }
            });

            _engine.InvocationStack.Clear();

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                // First
                sb.EmitAppCall(contract, "GetInvocationCounter");
                // Second
                sb.EmitAppCall(contract, "GetInvocationCounter");

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
        public void Test_Time()
        {
            _engine.Snapshot.GetType().GetProperty("PersistingBlock").SetValue(_engine.Snapshot, new Block()
            {
                Timestamp = 123
            });

            var result = _engine.ExecuteTestCaseStandard("GetTime");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_Platform()
        {
            var result = _engine.ExecuteTestCaseStandard("GetPlatform");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("NEO", item.GetString());
        }

        [TestMethod]
        public void Test_Trigger()
        {
            var result = _engine.ExecuteTestCaseStandard("GetTrigger");
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
            var result = _engine.ExecuteTestCaseStandard("Log", new ByteArray(Encoding.UTF8.GetBytes("LogTest")));
            ApplicationEngine.Log -= method;

            Assert.AreEqual(1, list.Count);

            var item = list[0];
            Assert.AreEqual("LogTest", item.Message);
        }

        [TestMethod]
        public void Test_Notify()
        {
            var list = new List<NotifyEventArgs>();
            var method = new EventHandler<NotifyEventArgs>((s, e) => list.Add(e));

            ApplicationEngine.Notify += method;
            var result = _engine.ExecuteTestCaseStandard("Notify", new ByteArray(Encoding.UTF8.GetBytes("NotifyTest")));
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
            var result = _engine.ExecuteTestCaseStandard("GetNotificationsCount", new ByteArray(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("GetNotificationsCount", new ByteArray(new byte[0]));
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_GetNotifications()
        {
            var result = _engine.ExecuteTestCaseStandard("GetNotifications", new ByteArray(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("GetNotifications", new ByteArray(new byte[0]));
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x03, item.GetBigInteger());
        }
    }
}
