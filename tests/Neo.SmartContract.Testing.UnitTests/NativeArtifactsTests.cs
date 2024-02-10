using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class NativeArtifactsTests
    {
        [TestMethod]
        public void TestInitialize()
        {
            var engine = new TestEngine();

            Assert.AreEqual(0, engine.Storage.Store.Seek(System.Array.Empty<byte>(), Persistence.SeekDirection.Forward).Count());

            engine.Native.Initialize(true);

            // Ensure that the main address contains the totalSupply

            var addr = Contract.GetBFTAddress(engine.ProtocolSettings.StandbyValidators);

            Assert.AreEqual(100_000_000, engine.Native.NEO.totalSupply());
            Assert.AreEqual(engine.Native.NEO.totalSupply(), engine.Native.NEO.balanceOf(addr));

            // Test set

            Assert.AreEqual(500000000, engine.Native.NEO.getGasPerBlock());

            // Fake signature

            engine.Transaction.Signers = new Network.P2P.Payloads.Signer[]
            {
                new Network.P2P.Payloads.Signer()
                {
                     Account = engine.Native.GetCommitteeAddress(),
                     Scopes = Network.P2P.Payloads.WitnessScope.Global
                }
            };
            engine.Native.NEO.setGasPerBlock(123);

            Assert.AreEqual(123, engine.Native.NEO.getGasPerBlock());

            // Invalid signature

            engine.Transaction.Signers[0].Scopes = Network.P2P.Payloads.WitnessScope.None;

            Assert.ThrowsException<TargetInvocationException>(() => engine.Native.NEO.setGasPerBlock(123));
        }
    }
}
