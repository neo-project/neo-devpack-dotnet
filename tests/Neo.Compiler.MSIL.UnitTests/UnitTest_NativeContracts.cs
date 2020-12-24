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
            Assert.AreEqual(NativeContract.ContractManagement.Hash.ToString(), "0x081514120c7894779309255b7fb18b376cec731a");
            Assert.AreEqual(NativeContract.RoleManagement.Hash.ToString(), "0x136ec44854ad9a714901eb7d714714f1791203f2");
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0xb1c37d5847c2ae36bdde31d0cc833a7ad9667f8f");
            Assert.AreEqual(NativeContract.NEO.Hash.ToString(), "0x0a46e2e37c9987f570b4af253fb77e7eef0f72b6");
            Assert.AreEqual(NativeContract.GAS.Hash.ToString(), "0xa6a6c15dcdc9b997dac448b6926522d22efeedfb");
            Assert.AreEqual(NativeContract.Policy.Hash.ToString(), "0xdde31084c0fdbebc7f5ed5f53a38905305ccee14");
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
            Assert.AreEqual("NEO", entry.GetString());

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

            Assert.AreEqual("GAS", entry.GetString());
        }
    }
}
