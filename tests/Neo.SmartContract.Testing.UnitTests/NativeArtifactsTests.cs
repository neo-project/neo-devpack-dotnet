using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing.Exceptions;
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
            // Create the engine without initialize the native contracts

            var engine = new TestEngine(false);

            Assert.AreEqual(0, engine.Storage.Store.Seek(System.Array.Empty<byte>(), Persistence.SeekDirection.Forward).Count());

            // Initialize native contracts

            engine.Native.Initialize(false);

            // Check symbols

            using var fee = engine.CreateGasWatcher();
            Assert.AreEqual("NEO", engine.Native.NEO.Symbol);
            Assert.AreEqual(984060L, fee.Value);

            using var gas = engine.CreateGasWatcher();
            {
                Assert.AreEqual("GAS", engine.Native.GAS.Symbol);
                Assert.AreEqual(984060L, gas);
            }

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.AreEqual(engine.Native.NEO.TotalSupply, engine.Native.NEO.BalanceOf(engine.ValidatorsAddress));

            // Check coverage

            Assert.AreEqual(1M, engine.Native.NEO.GetCoverage(o => o.Symbol)!.CoveredLinesPercentage);
            Assert.AreEqual(1M, engine.Native.NEO.GetCoverage(o => o.TotalSupply)!.CoveredLinesPercentage);
            Assert.AreEqual(1M, engine.Native.NEO.GetCoverage(o => o.BalanceOf(It.IsAny<UInt160>()))!.CoveredLinesPercentage);
        }

        [TestMethod]
        public void TestCandidate()
        {
            var engine = new TestEngine(true) { Fee = 1001_0000_0000 };

            // Check initial value

            Assert.AreEqual(0, engine.Native.NEO.Candidates?.Length);

            // Register

            var candidate = ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.Secp256r1);
            engine.SetTransactionSigners(Contract.CreateSignatureContract(candidate).ScriptHash);
            Assert.IsTrue(engine.Native.NEO.RegisterCandidate(candidate));

            // Check

            Assert.AreEqual(1, engine.Native.NEO.Candidates?.Length);
            Assert.AreEqual(candidate.ToString(), engine.Native.NEO.Candidates![0].PublicKey!.ToString());
        }

        [TestMethod]
        public void TestTransfer()
        {
            // Create and initialize TestEngine

            var engine = new TestEngine(true);

            // Fake signature of BFTAddress

            engine.SetTransactionSigners(Network.P2P.Payloads.WitnessScope.Global, engine.ValidatorsAddress);

            // Define address to transfer funds

            UInt160 addressTo = UInt160.Parse("0x1230000000000000000000000000000000000000");
            Assert.AreEqual(0, engine.Native.NEO.BalanceOf(addressTo));

            // Attach to Transfer event

            var raisedEvent = false;
            engine.Native.NEO.OnTransfer += (from, to, amount) =>
                {
                    Assert.AreEqual(engine.Transaction.Sender, from);
                    Assert.AreEqual(addressTo, to);
                    Assert.AreEqual(123, amount);

                    // If the event is raised, the variable will be changed
                    raisedEvent = true;
                };

            // Transfer funds

            Assert.IsTrue(engine.Native.NEO.Transfer(engine.Transaction.Sender, addressTo, 123, null));

            // Ensure that we have balance and the event was raised

            Assert.IsTrue(raisedEvent);
            Assert.AreEqual(123, engine.Native.NEO.BalanceOf(addressTo));
        }

        [TestMethod]
        public void TestSignature()
        {
            // Create and initialize TestEngine

            var engine = new TestEngine(true);

            // Check initial value of getRegisterPrice

            Assert.AreEqual(100000000000, engine.Native.NEO.RegisterPrice);

            // Fake Committee Signature

            engine.SetTransactionSigners(new Network.P2P.Payloads.Signer()
            {
                Account = engine.CommitteeAddress,
                Scopes = Network.P2P.Payloads.WitnessScope.Global
            });

            // Change RegisterPrice to 123

            engine.Native.NEO.RegisterPrice = 123;

            Assert.AreEqual(123, engine.Native.NEO.RegisterPrice);

            // Now test it without this signature

            engine.SetTransactionSigners(TestEngine.GetNewSigner());

            var exception = Assert.ThrowsException<TestException>(() => engine.Native.NEO.RegisterPrice = 123);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }
    }
}
