// Copyright (C) 2015-2025 The Neo Project.
//
// Nep17ContractTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.InvalidTypes;
using System.Numerics;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    /// <summary>
    /// You need to build the solution to resolve Nep17Contract class.
    /// </summary>
    [TestClass]
    public class Nep17ContractTests : Nep17Tests<Nep17ContractTemplate>
    {
        #region Expected values in base tests

        public override BigInteger ExpectedTotalSupply => 0;
        public override string ExpectedSymbol => "EXAMPLE";
        public override byte ExpectedDecimals => 8;

        #endregion

        /// <summary>
        /// Initialize Test
        /// </summary>
        public Nep17ContractTests() : base(Nep17ContractTemplate.Nef, Nep17ContractTemplate.Manifest) { }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
        }

        [TestMethod]
        public override void TestTransfer()
        {
            Engine.SetTransactionSigners(Alice);

            // Test mint

            Assert.AreEqual(0, Contract.TotalSupply);

            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            Contract.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Contract.TotalSupply);
            AssertTransferEvent(null, Alice.Account, 10);

            // Transfer is done between alice balance to bob

            base.TestTransfer();

            // Test Burn

            Engine.SetTransactionSigners(Alice);

            Contract.Burn(Alice.Account, Contract.BalanceOf(Alice.Account));
            Contract.Burn(Bob.Account, Contract.BalanceOf(Bob.Account));

            Assert.AreEqual(0, Contract.TotalSupply);
        }

        [TestMethod]
        public void TestMintAndBurn()
        {
            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            // Test mint -1

            Assert.ThrowsException<TestException>(() => Contract.Mint(Alice.Account, -1));

            // Test mint 0

            Contract.Mint(Alice.Account, 0);

            Assert.AreEqual(0, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Contract.TotalSupply);
            AssertNoTransferEvent();

            // test mint

            Contract.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Contract.TotalSupply);
            AssertTransferEvent(null, Alice.Account, 10);

            // Test burn -1

            Assert.ThrowsException<TestException>(() => Contract.Burn(Alice.Account, -1));

            // Test burn 0

            Contract.Burn(Alice.Account, 0);

            Assert.AreEqual(10, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Contract.TotalSupply);
            AssertNoTransferEvent();

            // Test burn

            Contract.Burn(Alice.Account, 10);

            Assert.AreEqual(0, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Contract.TotalSupply);
            AssertTransferEvent(Alice.Account, null, 10);

            // Can't burn more than the BalanceOf

            Assert.ThrowsException<TestException>(() => Contract.Burn(Alice.Account, 1));
            Assert.ThrowsException<TestException>(() => Contract.Burn(Bob.Account, 1));

            // Now check with Bob

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<TestException>(() => Contract.Mint(Alice.Account, 10));
            Assert.ThrowsException<TestException>(() => Contract.Burn(Alice.Account, 10));

            // Clean

            Assert.AreEqual(0, Contract.TotalSupply);
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            Assert.ThrowsException<TestException>(() => Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString()));

            Engine.SetTransactionSigners(Alice);

            // Test Update with the same script

            Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString());

            // Ensure that it works with the same script

            TestTotalSupply();
        }

        [TestMethod]
        public void TestDeployWithOwner()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Try with invalid owners

            Assert.ThrowsException<TestException>(() => Engine.Deploy<Nep17ContractTemplate>(NefFile, Manifest, UInt160.Zero));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<Nep17ContractTemplate>(NefFile, Manifest, InvalidUInt160.InvalidLength));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<Nep17ContractTemplate>(NefFile, Manifest, InvalidUInt160.InvalidType));

            // Test SetOwner notification

            UInt160? previousOwnerRaised = null;
            UInt160? newOwnerRaised = null;

            var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
            var check = Engine.FromHash<Nep17ContractTemplate>(expectedHash, false);
            check.OnSetOwner += (previous, newOwner) =>
            {
                previousOwnerRaised = previous;
                newOwnerRaised = newOwner;
            };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var nep17 = Engine.Deploy<Nep17ContractTemplate>(NefFile, Manifest, rand);
            Assert.AreEqual(check.Hash, nep17.Hash);

            Coverage?.Join(nep17.GetCoverage());

            Assert.AreEqual(rand, nep17.Owner);
            Assert.IsNull(previousOwnerRaised);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, rand);
        }
    }
}
