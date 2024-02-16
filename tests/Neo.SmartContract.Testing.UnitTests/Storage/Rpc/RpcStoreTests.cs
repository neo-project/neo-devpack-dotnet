using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing.Storage;
using Neo.SmartContract.Testing.Storage.Rpc;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class RpcStoreTests
    {
        public abstract class DummyContract : SmartContract
        {
            public abstract BigInteger GetCandidateVote(ECPoint point);
            protected DummyContract(SmartContractInitialize initialize) : base(initialize) { }
        }

        [TestMethod]
        public void TestRpcStore()
        {
            var engine = new TestEngine(false)
            {
                Storage = new EngineStorage(new RpcStore("http://seed2.neo.org:10332"))
            };

            // check network values

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.IsTrue(engine.Native.Ledger.CurrentIndex > 4_905_187);

            // check with Seek (RPC doesn't support Backward)

            // Assert.IsTrue(engine.Native.NEO.GasPerBlock == 5);

            // check deploy

            var node = ECPoint.Parse("03d9e8b16bd9b22d3345d6d4cde31be1c3e1d161532e3d0ccecb95ece2eb58336e", ECCurve.Secp256k1);
            var state = engine.Native.ContractManagement.GetContract(engine.Native.NEO.Hash);
            var contract = engine.Deploy<DummyContract>(state.Nef, state.Manifest, null,
                c => c.Setup(s => s.GetCandidateVote(It.IsAny<ECPoint>())).Returns(() => engine.Native.NEO.GetCandidateVote(node)));

            var votes = contract.GetCandidateVote(node);
            Assert.IsTrue(votes > 5_000_000, $"Votes: {votes}");
        }
    }
}
