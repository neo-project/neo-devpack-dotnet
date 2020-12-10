using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.TestingEngine;
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
            Assert.AreEqual(NativeContract.Designate.Hash.ToString(), "0x7062149f9377e3a110a343f811b9e406f8ef7824");
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0x35e4fc2e69a4d04d1db4d755c4150c50aff2e9a9");
            Assert.AreEqual(NativeContract.NEO.Hash.ToString(), "0xe22f9134cef8b03e53f71b3f960a20a65cddc972");
            Assert.AreEqual(NativeContract.GAS.Hash.ToString(), "0x36a019d836d964c438c573f78badf79b9e7eebdd");
            Assert.AreEqual(NativeContract.Policy.Hash.ToString(), "0x1ca594b36b6b6b3f05efce8b106c824053d18713");
        }

        [TestMethod]
        public void Test_Oracle()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            // Minimum Response Fee

            var result = testengine.ExecuteTestCaseStandard("oracleMinimumResponseFee");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual(0_10000000u, entry.GetInteger());
        }

        [TestMethod]
        public void Test_Designation()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            // getOracleNodes

            var result = testengine.ExecuteTestCaseStandard("getOracleNodes");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual(0, (entry as VM.Types.Array).Count);
        }

        [TestMethod]
        public void Test_NEO()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            // NeoSymbol

            var result = testengine.ExecuteTestCaseStandard("nEOSymbol");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();
            Assert.AreEqual("neo", entry.GetString());

            // NeoHash

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("nEOHash");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            entry = result.Pop();
            Assert.IsTrue(entry is VM.Types.ByteString);
            var hash = new UInt160((VM.Types.ByteString)entry);
            Assert.AreEqual(NativeContract.NEO.Hash, hash);
        }

        [TestMethod]
        public void Test_GAS()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("gASSymbol");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("gas", entry.GetString());
        }
    }
}
