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
                Storage = new EngineStorage(new RpcStore("http://seed2t5.neo.org:20332"))
            };

            // check network values

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.IsTrue(engine.Native.Ledger.CurrentIndex > 3_510_270);

            // check with Seek (RPC doesn't support Backward, it could be slow)

            Assert.IsTrue(engine.Native.NEO.GasPerBlock == 500000000);

            // check deploy

            var node = ECPoint.Parse("03009b7540e10f2562e5fd8fac9eaec25166a58b26e412348ff5a86927bfac22a2", ECCurve.Secp256r1);
            var state = engine.Native.ContractManagement.GetContract(engine.Native.NEO.Hash);
            var contract = engine.Deploy<DummyContract>(state.Nef, state.Manifest, null,
                c => c.Setup(s => s.GetCandidateVote(It.IsAny<ECPoint>())).Returns(() => engine.Native.NEO.GetCandidateVote(node)));

            var votes = contract.GetCandidateVote(node);
            Assert.IsTrue(votes > 3_000_000, $"Votes: {votes}");
        }
    }
}
