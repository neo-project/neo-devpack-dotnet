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
            // Create the engine without initialize the native contracts

            var engine = new TestEngine(false);

            Assert.AreEqual(0, engine.Storage.Store.Seek(System.Array.Empty<byte>(), Persistence.SeekDirection.Forward).Count());

            // Initialize native contracts

            engine.Native.Initialize(false);

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.AreEqual(engine.Native.NEO.TotalSupply, engine.Native.NEO.BalanceOf(engine.ValidatorsAddress));
        }

        [TestMethod]
        public void TestTransfer()
        {
            // Create and initialize TestEngine

            var engine = new TestEngine(true);

            // Fake signature of BFTAddress

            engine.Transaction.Signers = new Network.P2P.Payloads.Signer[]
            {
                new Network.P2P.Payloads.Signer()
                {
                     Account = engine.ValidatorsAddress,
                     Scopes = Network.P2P.Payloads.WitnessScope.Global
                }
            };

            // Define address to transfer funds

            UInt160 addressTo = UInt160.Parse("0x1230000000000000000000000000000000000000");

            // Attach to Transfer event

            var raisedEvent = false;

            engine.Native.NEO.OnTransfer += (UInt160 from, UInt160 to, BigInteger amount) =>
            {
                Assert.AreEqual(engine.Transaction.Sender, from);
                Assert.AreEqual(addressTo, to);
                Assert.AreEqual(123, amount);

                // If the event is raised, the variable will be changed
                raisedEvent = true;
            };


            Assert.AreEqual(0, engine.Native.NEO.BalanceOf(addressTo));

            // Transfer funds

            Assert.IsTrue(engine.Native.NEO.Transfer(engine.Transaction.Sender, addressTo, 123, null));

            // Ensure that we have balance and the event was raised

            Assert.IsTrue(raisedEvent);
            Assert.AreEqual(123, engine.Native.NEO.BalanceOf(addressTo));
        }

        [TestMethod]
        public void TestSignature()
        {
            // Initialize out TestEngine

            var engine = new TestEngine(true);

            // Check initial value of getRegisterPrice

            Assert.AreEqual(100000000000, engine.Native.NEO.RegisterPrice);

            // Fake Committee Signature

            engine.Transaction.Signers = new Network.P2P.Payloads.Signer[]
            {
                new Network.P2P.Payloads.Signer()
                {
                     Account = engine.CommitteeAddress,
                     Scopes = Network.P2P.Payloads.WitnessScope.Global
                }
            };

            // Change RegisterPrice to 123

            engine.Native.NEO.RegisterPrice = 123;

            Assert.AreEqual(123, engine.Native.NEO.RegisterPrice);

            // Now test it without this signature

            engine.Transaction.Signers[0].Scopes = Network.P2P.Payloads.WitnessScope.None;

            Assert.ThrowsException<TargetInvocationException>(() => engine.Native.NEO.RegisterPrice = 123);
        }
    }
}
