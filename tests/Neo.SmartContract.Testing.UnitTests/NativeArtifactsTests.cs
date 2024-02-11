using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class NativeArtifactsTests
    {
        [TestMethod]
        public void TestInitialize()
        {
            var engine = new TestEngine(false);

            Assert.AreEqual(0, engine.Storage.Store.Seek(System.Array.Empty<byte>(), Persistence.SeekDirection.Forward).Count());

            engine.Native.Initialize(false);

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(100_000_000, engine.Native.NEO.totalSupply());
            Assert.AreEqual(engine.Native.NEO.totalSupply(), engine.Native.NEO.balanceOf(engine.BFTAddress));
        }

        [TestMethod]
        public void TestTransfer()
        {
            var engine = new TestEngine(true);

            // Test set

            Assert.AreEqual(500000000, engine.Native.NEO.getGasPerBlock());

            // Fake signature

            engine.Transaction.Signers = new Network.P2P.Payloads.Signer[]
            {
                new Network.P2P.Payloads.Signer()
                {
                     Account = engine.BFTAddress,
                     Scopes = Network.P2P.Payloads.WitnessScope.Global
                }
            };

            bool raisedEvent = false;

            engine.Native.NEO.Transfer += (UInt160 from, UInt160 to, BigInteger amount) =>
            {
                raisedEvent = true;
            };

            UInt160 wallet = UInt160.Parse("0x1230000000000000000000000000000000000000");

            Assert.AreEqual(0, engine.Native.NEO.balanceOf(wallet));
            Assert.IsTrue(engine.Native.NEO.transfer(engine.Transaction.Sender, wallet, 123, null));

            Assert.IsTrue(raisedEvent);
            Assert.AreEqual(123, engine.Native.NEO.balanceOf(wallet));
        }

        [TestMethod]
        public void TestSignature()
        {
            var engine = new TestEngine(true);

            // Test set

            Assert.AreEqual(100000000000, engine.Native.NEO.getRegisterPrice());

            // Fake signature

            engine.Transaction.Signers = new Network.P2P.Payloads.Signer[]
            {
                new Network.P2P.Payloads.Signer()
                {
                     Account = engine.Native.GetCommitteeAddress(),
                     Scopes = Network.P2P.Payloads.WitnessScope.Global
                }
            };

            engine.Native.NEO.setRegisterPrice(123);

            Assert.AreEqual(123, engine.Native.NEO.getRegisterPrice());

            // Invalid signature

            engine.Transaction.Signers[0].Scopes = Network.P2P.Payloads.WitnessScope.None;

            Assert.ThrowsException<TargetInvocationException>(() => engine.Native.NEO.setRegisterPrice(123));
        }
    }
}
