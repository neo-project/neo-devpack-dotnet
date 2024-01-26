using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract;
using Neo.SmartContract.TestEngine;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ContractCall
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var snapshot = new TestDataCache();
            var hash = UInt160.Parse("0102030405060708090A0102030405060708090A");
            _engine = new TestEngine(snapshot: snapshot);
            _engine.AddEntryScript(typeof(Contract1));
            snapshot.ContractAdd(new ContractState()
            {
                Hash = hash,
                Nef = _engine.Nef,
                Manifest = _engine.Manifest,
            });

            // will ContractCall 0102030405060708090A0102030405060708090A
            _engine.AddEntryScript(typeof(Contract_ContractCall));
        }

        [TestMethod]
        public void Test_ContractCall()
        {
            var engine = _engine.ExecuteTestCaseStandard("testContractCall");
            Assert.AreEqual(VMState.HALT, _engine.State);
            var result = engine.Pop();
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };
            Assert.AreEqual(wantresult, result.ConvertTo(StackItemType.ByteString));
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
