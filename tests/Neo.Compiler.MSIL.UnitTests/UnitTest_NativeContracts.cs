using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
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
            // Fake native deploy
            snapshot.SetPersistingBlock(new Network.P2P.Payloads.Block() { Index = 0 });
            snapshot.DeployNativeContracts();
        }

        [TestMethod]
        public void TestHashes()
        {
            // var attr = typeof(Oracle).GetCustomAttribute<ContractAttribute>();
            Assert.AreEqual(NativeContract.Designate.Hash.ToString(), "0x7ab39c37afd995f2f947a7ecbf40e91307058595");
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0x479f134275b36d64f31fdbdc557cd2aa2f32d8d1");
            Assert.AreEqual(NativeContract.NEO.Hash.ToString(), "0x74c21a1ca66b7a190bf2a65db83ba6fe550cea64");
            Assert.AreEqual(NativeContract.GAS.Hash.ToString(), "0xb399c051778cf37a1e4ef88509b2e054d0420a32");
            Assert.AreEqual(NativeContract.Policy.Hash.ToString(), "0xe8ff1989c19526f4d8102f226e2c6c993b63efe9");
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
