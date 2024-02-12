using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing;
using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Template.UnitTests.nep17
{
    [TestClass]
    public class Nep17Tests
    {
        private readonly TestEngine Engine;
        private readonly Nep17Contract Nep17;
        private readonly Signer Alice = TestEngine.GetNewSigner();
        private readonly Signer Bob = TestEngine.GetNewSigner();
        private readonly byte[] Nef;

        public Nep17Tests()
        {
            Nef = File.ReadAllBytes("nep17/UtArtifacts/Nep17Contract.nef");
            Engine = new TestEngine(true);
            Engine.SetTransactionSigners(Alice);
            Nep17 = Deploy(null);
        }

        public Nep17Contract Deploy(object? data)
        {
            var manifest = File.ReadAllText("nep17/UtArtifacts/Nep17Contract.manifest.json");

            return Engine.Deploy<Nep17Contract>(Nef, manifest, data);
        }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Nep17.MyMethod());
        }

        [TestMethod]
        public void TestVerify()
        {
            Engine.SetTransactionSigners(Alice);
            Assert.IsTrue(Nep17.Verify);
            Engine.SetTransactionSigners(Bob);
            Assert.IsFalse(Nep17.Verify);
        }

        [TestMethod]
        public void TestDeciamls()
        {
            Assert.AreEqual(8, Nep17.Decimals);
        }

        [TestMethod]
        public void TestSymbol()
        {
            Assert.AreEqual("EXAMPLE", Nep17.Symbol);
        }

        [TestMethod]
        public void TestTotalSupply()
        {
            Assert.AreEqual(0, Nep17.TotalSupply);
        }

        [TestMethod]
        public void TestBalanceOf()
        {
            Assert.AreEqual(0, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Nep17.BalanceOf(Bob.Account));
        }

        [TestMethod]
        public void TestTransfer()
        {
            // Listen OnTransfer

            UInt160? raisedFrom = null;
            UInt160? raisedTo = null;
            BigInteger? raisedAmount = null;

            void onTransfer(UInt160 from, UInt160 to, BigInteger amount)
            {
                raisedFrom = from;
                raisedTo = to;
                raisedAmount = amount;
            }

            Nep17.OnTransfer += onTransfer;

            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            Nep17.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(Alice.Account, raisedTo);
            Assert.AreEqual(10, raisedAmount);

            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            // Invoke transfer

            Assert.IsTrue(Nep17.Transfer(Alice.Account, Bob.Account, 6));

            Assert.AreEqual(4, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(6, Nep17.BalanceOf(Bob.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(Alice.Account, raisedFrom);
            Assert.AreEqual(Bob.Account, raisedTo);
            Assert.AreEqual(6, raisedAmount);

            // Check with more balance

            Assert.IsFalse(Nep17.Transfer(Alice.Account, Bob.Account, 6));

            // Check with not signed

            Engine.SetTransactionSigners(Bob);
            Assert.IsFalse(Nep17.Transfer(Alice.Account, Bob.Account, 0));

            // Clean OnTransfer

            Nep17.OnTransfer -= onTransfer;
        }

        [TestMethod]
        public void TestMintAndBurn()
        {
            // Listen OnTransfer

            UInt160? raisedFrom = null;
            UInt160? raisedTo = null;
            BigInteger? raisedAmount = null;

            void onTransfer(UInt160 from, UInt160 to, BigInteger amount)
            {
                raisedFrom = from;
                raisedTo = to;
                raisedAmount = amount;
            }

            Nep17.OnTransfer += onTransfer;

            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            Nep17.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(Alice.Account, raisedTo);
            Assert.AreEqual(10, raisedAmount);

            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            Nep17.Burn(Alice.Account, 10);

            Assert.AreEqual(0, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Nep17.TotalSupply);
            Assert.AreEqual(Alice.Account, raisedFrom);
            Assert.AreEqual(null, raisedTo);
            Assert.AreEqual(10, raisedAmount);

            // Can't burn more than the BalanceOf

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Alice.Account, 1));
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Bob.Account, 1));

            // Now check with Bob

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Mint(Alice.Account, 10));
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Alice.Account, 10));

            // Clean OnTransfer

            Nep17.OnTransfer -= onTransfer;
        }

        [TestMethod]
        public void TestDefaultOwner()
        {
            Assert.AreEqual(Engine.Sender, Nep17.Owner);
        }

        [TestMethod]
        public void TestSetOwnerDuringDeploy()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Test SetOwner notification

            UInt160 expectedHash = Helper.GetContractHash(Bob.Account, Nef.AsSerializable<NefFile>().CheckSum, "Nep17Contract");

            UInt160? newOwnerRaised = null;
            var check = Engine.FromHash<Nep17Contract>(expectedHash, false);
            check.OnSetOwner += (newOwner) => { newOwnerRaised = newOwner; };

            // Deploy with bob arg, we can use the same storage
            // because the contract hash contains the Sender, and now it's Bob

            var rand = TestEngine.GetNewSigner().Account;
            var nep17 = Deploy(rand);

            Assert.AreEqual(rand, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
        }
    }
}
