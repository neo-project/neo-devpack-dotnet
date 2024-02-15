using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    /// <summary>
    /// You need to build the solution to resolve Nep17Contract class.
    /// </summary>
    [TestClass]
    public class Nep17ContractTests : Nep17Tests<Nep17Contract>
    {
        #region Expected values in base tests

        public override BigInteger ExpectedTotalSupply => 0;
        public override string ExpectedSymbol => "EXAMPLE";
        public override byte ExpectedDecimals => 8;

        #endregion

        /// <summary>
        /// Initialize Test
        /// </summary>
        public Nep17ContractTests() :
            base(
                "templates/neocontractnep17/Artifacts/Nep17Contract.nef",
                "templates/neocontractnep17/Artifacts/Nep17Contract.manifest.json"
                )
        { }

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
        public override void TestTransfer()
        {
            Engine.SetTransactionSigners(Alice);

            // Test mint

            Assert.AreEqual(0, Nep17.TotalSupply);

            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            Nep17.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            AssertTransferEvent(null, Alice.Account, 10);

            // Transfer is done between alice balance to bob

            base.TestTransfer();

            // Test Burn

            Engine.SetTransactionSigners(Alice);

            Nep17.Burn(Alice.Account, Nep17.BalanceOf(Alice.Account));
            Nep17.Burn(Bob.Account, Nep17.BalanceOf(Bob.Account));

            Assert.AreEqual(0, Nep17.TotalSupply);
        }

        [TestMethod]
        public void TestMintAndBurn()
        {
            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            // Test mint -1

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Mint(Alice.Account, -1));

            // Test mint 0

            Nep17.Mint(Alice.Account, 0);

            Assert.AreEqual(0, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Nep17.TotalSupply);
            AssertNoTransferEvent();

            // test mint

            Nep17.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            AssertTransferEvent(null, Alice.Account, 10);

            // Test burn -1

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Alice.Account, -1));

            // Test burn 0

            Nep17.Burn(Alice.Account, 0);

            Assert.AreEqual(10, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Nep17.TotalSupply);
            AssertNoTransferEvent();

            // Test burn

            Nep17.Burn(Alice.Account, 10);

            Assert.AreEqual(0, Nep17.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Nep17.TotalSupply);
            AssertTransferEvent(Alice.Account, null, 10);

            // Can't burn more than the BalanceOf

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Alice.Account, 1));
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Bob.Account, 1));

            // Now check with Bob

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Mint(Alice.Account, 10));
            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Burn(Alice.Account, 10));

            // Clean

            Assert.AreEqual(0, Nep17.TotalSupply);
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            Assert.ThrowsException<VMUnhandledException>(() => Nep17.Update(NefFile, Manifest));

            Engine.SetTransactionSigners(Alice);

            // Test Update with the same script

            Nep17.Update(NefFile, Manifest);

            // Ensure that it works with the same script

            TestTotalSupply();
        }

        [TestMethod]
        public void TestDeployWithOwner()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Test SetOwner notification

            UInt160? previousOwnerRaised = null;
            UInt160? newOwnerRaised = null;

            var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
            var check = Engine.FromHash<Nep17Contract>(expectedHash, false);
            check.OnSetOwner += (previous, newOwner) =>
            {
                previousOwnerRaised = previous;
                newOwnerRaised = newOwner;
            };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var nep17 = Engine.Deploy<Nep17Contract>(NefFile, Manifest, rand);
            Assert.AreEqual(check.Hash, nep17.Hash);

            Coverage?.Join(nep17.GetCoverage());

            Assert.AreEqual(rand, nep17.Owner);
            Assert.IsNull(previousOwnerRaised);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, rand);
        }
    }
}
