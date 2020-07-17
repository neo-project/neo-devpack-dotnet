using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class NativeTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();

            // Deploy native contracts

            ((TestSnapshot)_engine.Snapshot).SetPersistingBlock(new Network.P2P.Payloads.Block()
            {
                Index = 0,
                ConsensusData = new Network.P2P.Payloads.ConsensusData(),
                Transactions = new Network.P2P.Payloads.Transaction[0],
                Witness = new Network.P2P.Payloads.Witness()
                {
                    InvocationScript = new byte[0],
                    VerificationScript = new byte[0]
                },
                NextConsensus = UInt160.Zero,
                MerkleRoot = UInt256.Zero,
                PrevHash = UInt256.Zero
            });

            using (var script = new ScriptBuilder())
            {
                script.EmitSysCall(TestEngine.Native_Deploy);
                _engine.LoadScript(script.ToArray());
                Assert.AreEqual(VMState.HALT, _engine.Execute());
            }

            _engine.Reset();
            _engine.AddEntryScript("./TestClasses/Contract_Native.cs");
        }

        [TestMethod]
        public void Test_NEO()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("NEO_Decimals");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("NEO_Name");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("NEO", item.GetString());

            _engine.Reset();
            var account = new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };
            result = _engine.ExecuteTestCaseStandard("NEO_BalanceOf", account);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetInteger());
        }

        [TestMethod]
        public void Test_GAS()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("GAS_Decimals");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(8, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("GAS_Name");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("GAS", item.GetString());
        }

        [TestMethod]
        public void Test_Policy()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("policy_GetFeePerByte");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(1000L, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("policy_GetMaxTransactionsPerBlock");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(512, item.GetInteger());
        }
    }
}
