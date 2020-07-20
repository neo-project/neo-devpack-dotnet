using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Cryptography.ECC;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class NativeTest
    {
        private TestEngine _engine;
        private readonly byte[] pubKey = NeonTestTool.HexString2Bytes("03ea01cb94bdaf0cd1c01b159d474f9604f4af35a3e2196f6bdfdb33b2aa4961fa");

        [TestInitialize]
        public void Init()
        {
            // Deploy native contracts
            var block = new Network.P2P.Payloads.Block()
            {
                Index = 0,
                ConsensusData = new Network.P2P.Payloads.ConsensusData(),
                Transactions = new Network.P2P.Payloads.Transaction[0],
                Witness = new Network.P2P.Payloads.Witness()
                {
                    InvocationScript = new byte[0],
                    VerificationScript = Contract.CreateSignatureRedeemScript(ECPoint.FromBytes(pubKey, ECCurve.Secp256k1))
                },
                NextConsensus = UInt160.Zero,
                MerkleRoot = UInt256.Zero,
                PrevHash = UInt256.Zero
            };

            _engine = new TestEngine(TriggerType.Application, block);
            ((TestSnapshot)_engine.Snapshot).SetPersistingBlock(block);

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


            // Before RegisterCandidate
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("NEO_GetCandidates");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(21, ((Array)item).Count);

            // RegisterCandidate
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("NEO_RegisterCandidate", pubKey);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(true, item.GetBoolean());

            // After RegisterCandidate
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("NEO_GetCandidates");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(22, ((Array)item).Count);
            var candidate = ((Array)item)[21];
            Assert.IsInstanceOfType(candidate, typeof(Struct));
            var candidatePubKey = ((Struct)candidate)[0];
            var candidateVotes = ((Struct)candidate)[1];
            Assert.IsInstanceOfType(candidatePubKey, typeof(ByteString));
            Assert.AreEqual(true, candidatePubKey.Equals((ByteString)pubKey));
            Assert.IsInstanceOfType(candidateVotes, typeof(Integer));
            Assert.AreEqual(0, candidateVotes.GetInteger());
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

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("policy_GetBlockedAccounts");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(0, ((Array)item).Count);
        }
    }
}
