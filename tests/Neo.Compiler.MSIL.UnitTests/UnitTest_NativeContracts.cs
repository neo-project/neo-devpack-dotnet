using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class Contract_NativeContracts
    {
        private readonly TestDataCache snapshot = new TestDataCache();

        [TestInitialize]
        public void Test_Init()
        {
            // Fake native deploy
            snapshot.DeployNativeContracts();
        }

        [TestMethod]
        public void TestHashes()
        {
            Assert.AreEqual(NativeContract.ContractManagement.Hash.ToString(), "0xa501d7d7d10983673b61b7a2d3a813b36f9f0e43");
            Assert.AreEqual(NativeContract.RoleManagement.Hash.ToString(), "0x597b1471bbce497b7809e2c8f10db67050008b02");
            Assert.AreEqual(NativeContract.NameService.Hash.ToString(), "0xa2b524b68dfe43a9d56af84f443c6b9843b8028c");
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0x8dc0e742cbdfdeda51ff8a8b78d46829144c80ee");
            Assert.AreEqual(NativeContract.NEO.Hash.ToString(), "0xf61eebf573ea36593fd43aa150c055ad7906ab83");
            Assert.AreEqual(NativeContract.GAS.Hash.ToString(), "0x70e2301955bf1e74cbb31d18c2f96972abadb328");
            Assert.AreEqual(NativeContract.Policy.Hash.ToString(), "0x79bcd398505eb779df6e67e4be6c14cded08e2f2");
            Assert.AreEqual(NativeContract.Ledger.Hash.ToString(), "0x971d69c6dd10ce88e7dfffec1dc603c6125a8764");
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

        [TestMethod]
        public void Test_Ledger()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot, persistingBlock: Blockchain.GenesisBlock);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("ledgerHash");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();
            Assert.IsTrue(entry is VM.Types.ByteString);
            var hash = new UInt160((VM.Types.ByteString)entry);
            Assert.AreEqual(NativeContract.Ledger.Hash.ToString(), hash.ToString());

            testengine.Reset();

            result = testengine.ExecuteTestCaseStandard("ledgerCurrentIndex");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            entry = result.Pop();

            Assert.AreEqual(0, entry.GetInteger());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("ledgerCurrentHash");

            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            entry = result.Pop();
            Assert.IsTrue(entry is VM.Types.ByteString);
            var blockHash = new UInt256((VM.Types.ByteString)entry);
            Assert.AreEqual(Blockchain.GenesisBlock.Hash.ToString(), blockHash.ToString());
        }
    }
}
