using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class Contract_NativeContracts
    {
        private readonly TestSnapshot snapshot = new TestSnapshot();

        [TestInitialize]
        public void Test_Init()
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall(ApplicationEngine.Neo_Native_Deploy);
                script = sb.ToArray();
            }

            // Fake native deploy

            typeof(TestSnapshot).GetProperty("PersistingBlock").SetValue(snapshot, new Network.P2P.Payloads.Block() { Index = 0 });
            var testengine = new TestEngine(TriggerType.System, null, snapshot);
            testengine.LoadScript(script);
            Assert.AreEqual(VMState.HALT, testengine.Execute());
        }

        [TestMethod]
        public void TestHashes()
        {
            // var attr = typeof(Oracle).GetCustomAttribute<ContractAttribute>();
            Assert.AreEqual(NativeContract.Designate.Hash.ToString(), "0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc");
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0x3c05b488bf4cf699d0631bf80190896ebbf38c3b");
            Assert.AreEqual(NativeContract.NEO.Hash.ToString(), "0xde5f57d430d3dece511cf975a8d37848cb9e0525");
            Assert.AreEqual(NativeContract.GAS.Hash.ToString(), "0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc");
        }

        [TestMethod]
        public void Test_Oracle()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            // Name

            var result = testengine.ExecuteTestCaseStandard("oracleName");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("Oracle", entry.GetString());
        }

        [TestMethod]
        public void Test_Designate()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            // Name

            var result = testengine.ExecuteTestCaseStandard("designateName");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("Designation", entry.GetString());

            // getOracleNodes

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("getOracleNodes");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            entry = result.Pop();

            Assert.AreEqual(0, (entry as VM.Types.Array).Count);
        }

        [TestMethod]
        public void Test_NEO()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("nEOName");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("NEO", entry.GetString());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("nEOSymbol");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            entry = result.Pop();

            Assert.AreEqual("neo", entry.GetString());
        }

        [TestMethod]
        public void Test_GAS()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("gASName");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("GAS", entry.GetString());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("gASSymbol");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            entry = result.Pop();

            Assert.AreEqual("gas", entry.GetString());
        }
    }
}
