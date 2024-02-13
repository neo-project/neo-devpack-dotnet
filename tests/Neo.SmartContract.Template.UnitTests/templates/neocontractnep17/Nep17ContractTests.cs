using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    [TestClass]
    public class Nep17ContractTests
    {
        private const string NefFilePath = "templates/neocontractnep17/UtArtifacts/Nep17Contract.nef";
        private const string ManifestPath = "templates/neocontractnep17/UtArtifacts/Nep17Contract.manifest.json";

        private readonly TestEngine Engine;
        private readonly Nep17Contract Nep17;
        private readonly Signer Alice = TestEngine.GetNewSigner();
        private readonly Signer Bob = TestEngine.GetNewSigner();

        public Nep17ContractTests()
        {
            Engine = new TestEngine(true);
            Engine.SetTransactionSigners(Alice);

            var nef = File.ReadAllBytes(NefFilePath);
            var manifest = File.ReadAllText(ManifestPath);

            Nep17 = Engine.Deploy<Nep17Contract>(nef, manifest, null);
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

            int raisedTimes = 0;
            UInt160? raisedFrom = null;
            UInt160? raisedTo = null;
            BigInteger? raisedAmount = null;

            void onTransfer(UInt160 from, UInt160 to, BigInteger amount)
            {
                raisedFrom = from;
                raisedTo = to;
                raisedAmount = amount;
                raisedTimes++;
            }

            Nep17.OnTransfer += onTransfer;

            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            Nep17.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(1, raisedTimes);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(Alice.Account, raisedTo);
            Assert.AreEqual(10, raisedAmount);

            raisedTimes = 0;
            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            // Invoke transfer

            Assert.IsTrue(Nep17.Transfer(Alice.Account, Bob.Account, 6));

            Assert.AreEqual(4, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(6, Nep17.BalanceOf(Bob.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(1, raisedTimes);
            Assert.AreEqual(Alice.Account, raisedFrom);
            Assert.AreEqual(Bob.Account, raisedTo);
            Assert.AreEqual(6, raisedAmount);

            // Check with more balance

            raisedTimes = 0;
            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            Assert.IsFalse(Nep17.Transfer(Alice.Account, Bob.Account, 6));
            Assert.AreEqual(0, raisedTimes);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(null, raisedTo);
            Assert.AreEqual(null, raisedAmount);

            // Check with not signed

            raisedTimes = 0;
            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            Engine.SetTransactionSigners(Bob);
            Assert.IsFalse(Nep17.Transfer(Alice.Account, Bob.Account, 0));
            Assert.AreEqual(0, raisedTimes);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(null, raisedTo);
            Assert.AreEqual(null, raisedAmount);

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

            var nef = File.ReadAllBytes(NefFilePath).AsSerializable<NefFile>();
            var manifest = ContractManifest.Parse(File.ReadAllText(ManifestPath));
            UInt160 expectedHash = Helper.GetContractHash(Bob.Account, nef.CheckSum, manifest.Name);

            UInt160? newOwnerRaised = null;
            var check = Engine.FromHash<Nep17Contract>(expectedHash, false);
            check.OnSetOwner += (newOwner) => { newOwnerRaised = newOwner; };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var nep17 = Engine.Deploy<Nep17Contract>(nef, manifest, rand);

            Assert.AreEqual(rand, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
        }
    }
}
