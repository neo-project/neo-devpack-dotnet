using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    [TestClass]
    public class Nep17ContractTests
    {
        private const string NefFilePath = "templates/neocontractnep17/UtArtifacts/Nep17Contract.nef";
        private const string ManifestPath = "templates/neocontractnep17/UtArtifacts/Nep17Contract.manifest.json";

        private static readonly Signer Alice = TestEngine.GetNewSigner();
        private static readonly Signer Bob = TestEngine.GetNewSigner();
        private static readonly TestEngine Engine;
        private static readonly Nep17Contract Nep17;
        private static readonly CoveredContract Coverage;

        static Nep17ContractTests()
        {
            Engine = new TestEngine(true);
            Engine.SetTransactionSigners(Alice);

            var nef = File.ReadAllBytes(NefFilePath);
            var manifest = File.ReadAllText(ManifestPath);

            Nep17 = Engine.Deploy<Nep17Contract>(nef, manifest, null);

            // Get coverage bag, we will join the coverage here

            Coverage = Nep17.GetCoverage()!;
            Assert.IsNotNull(Coverage);
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            // Ennsure that the coverage is more than 90% at the end of the tests

            Console.WriteLine(Coverage.Dump());
            Assert.IsTrue(Coverage.CoveredPercentage > 90.0, "Coverage is less than 90%");
        }

        [TestCleanup]
        public void IncreaseCoverage()
        {
            // Join the coverage of the current engine into the static bag

            Coverage.Join(Nep17?.GetCoverage());
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
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.BalanceOf(null));
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

            Engine.SetTransactionSigners(Alice);

            Nep17.Burn(Bob.Account, 6);
            Nep17.Burn(Alice.Account, 4);

            Assert.AreEqual(0, Nep17.TotalSupply);
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

            // Test mint -1

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Mint(Alice.Account, -1));

            // Test mint 0

            Nep17.Mint(Alice.Account, 0);

            Assert.AreEqual(0, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Nep17.TotalSupply);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(null, raisedTo);
            Assert.AreEqual(null, raisedAmount);

            // test mint

            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            Nep17.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(Alice.Account, raisedTo);
            Assert.AreEqual(10, raisedAmount);

            // Test burn -1

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Alice.Account, -1));

            // Test burn 0

            raisedFrom = null;
            raisedTo = null;
            raisedAmount = null;

            Nep17.Burn(Alice.Account, 0);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            Assert.AreEqual(null, raisedFrom);
            Assert.AreEqual(null, raisedTo);
            Assert.AreEqual(null, raisedAmount);

            // Test burn

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

            // Clean

            Assert.AreEqual(0, Nep17.TotalSupply);

            Nep17.OnTransfer -= onTransfer;
        }

        [TestMethod]
        public void TestDefaultOwner()
        {
            Assert.AreEqual(Engine.Sender, Nep17.Owner);
        }

        [TestMethod]
        public void TestSetGetOwner()
        {
            // Alice is the deployer

            Assert.AreEqual(Alice.Account, Nep17.Owner);
            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Owner = Bob.Account);

            Engine.SetTransactionSigners(Alice);
            Assert.ThrowsException<Exception>(() => Nep17.Owner = UInt160.Zero);

            Nep17.Owner = Bob.Account;
            Assert.AreEqual(Bob.Account, Nep17.Owner);
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Owner = Bob.Account);

            Engine.SetTransactionSigners(Bob);

            Nep17.Owner = Alice.Account;
            Assert.AreEqual(Alice.Account, Nep17.Owner);
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            var nef = File.ReadAllBytes(NefFilePath);
            var manifest = File.ReadAllText(ManifestPath);

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Update(nef, manifest));

            Engine.SetTransactionSigners(Alice);

            // Test Update with the same script

            Nep17.Update(nef, manifest);

            // Ensure that it works with the same script

            TestTotalSupply();
        }

        [TestMethod]
        public void TestDeploy()
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
            Coverage.Join(nep17.GetCoverage());

            Assert.AreEqual(rand, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
        }
    }
}
