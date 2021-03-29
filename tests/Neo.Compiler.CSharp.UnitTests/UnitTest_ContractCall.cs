using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ContractCall
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var hash = UInt160.Parse("0102030405060708090A0102030405060708090A");
            _engine = new TestEngine(snapshot: new TestDataCache());
            _engine.AddEntryScript("./TestClasses/Contract1.cs");
            _engine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = hash,
                Nef = _engine.ScriptEntry.nef,
                Manifest = ContractManifest.FromJson(_engine.ScriptEntry.manifest),
            });

            // will ContractCall 0102030405060708090A0102030405060708090A
            _engine.AddEntryScript("./TestClasses/Contract_ContractCall.cs");
        }

        [TestMethod]
        public void Test_ContractCall()
        {
            var result = _engine.ExecuteTestCaseStandard("testContractCall");
            Assert.AreEqual(VMState.HALT, _engine.State);

            StackItem wantresult = new byte[] { 1, 2, 3, 4 };
            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_ContractCall_Void()
        {
            var result = _engine.ExecuteTestCaseStandard("testContractCallVoid");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(0, result.Count);
        }
    }
}
