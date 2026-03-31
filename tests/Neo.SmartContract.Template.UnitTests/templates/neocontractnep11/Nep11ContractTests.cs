// Copyright (C) 2015-2026 The Neo Project.
//
// Nep11ContractTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Json;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep11
{
    /// <summary>
    /// You need to build the solution to resolve Nep11Contract class.
    /// </summary>
    [TestClass]
    public class Nep11ContractTests : OwnableTests<Nep11ContractTemplate>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public Nep11ContractTests() : base(Nep11ContractTemplate.Nef, Nep11ContractTemplate.Manifest) { }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
        }

        [TestMethod]
        public void TestMintAndTransfer()
        {
            Engine.SetTransactionSigners(Alice);

            var tokenId = Contract.Mint(Bob.Account, "Sword", "Rare sword", "ipfs://sword");
            Assert.IsNotNull(tokenId);

            Assert.AreEqual("EXAMPLE", Contract.Symbol);
            Assert.AreEqual(BigInteger.Zero, Contract.Decimals);
            Assert.IsNotNull(Contract.Tokens);
            Assert.IsNotNull(Contract.TokensOf(Bob.Account));

            CollectionAssert.AreEqual(Bob.Account.ToArray(), Contract.OwnerOf(tokenId)!.ToArray());
            Assert.AreEqual(BigInteger.One, Contract.BalanceOf(Bob.Account));
            Assert.AreEqual(BigInteger.Zero, Contract.BalanceOf(Alice.Account));

            var properties = Contract.Properties(tokenId);
            Assert.IsNotNull(properties);
            var values = properties!.Values.Select(v => v?.ToString()).ToArray();
            Assert.IsTrue(values.Any(v => v is not null && v.Contains("Sword", StringComparison.Ordinal)));
            Assert.IsTrue(values.Any(v => v is not null && v.Contains("Rare sword", StringComparison.Ordinal)));
            Assert.IsTrue(values.Any(v => v is not null && v.Contains("ipfs://sword", StringComparison.Ordinal)));

            Engine.SetTransactionSigners(Bob);
            Assert.IsTrue(Contract.Transfer(Alice.Account, tokenId, null)!.Value);

            CollectionAssert.AreEqual(Alice.Account.ToArray(), Contract.OwnerOf(tokenId)!.ToArray());
            Assert.AreEqual(BigInteger.Zero, Contract.BalanceOf(Bob.Account));
            Assert.AreEqual(BigInteger.One, Contract.BalanceOf(Alice.Account));
            Assert.IsNotNull(Contract.TokensOf(Alice.Account));
        }

        [TestMethod]
        public void TestMintRequiresOwnerAndValidRecipient()
        {
            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<TestException>(() => Contract.Mint(Bob.Account, "Sword", "Rare sword", "ipfs://sword"));

            Engine.SetTransactionSigners(Alice);
            Assert.ThrowsException<TestException>(() => Contract.Mint(UInt160.Zero, "Sword", "Rare sword", "ipfs://sword"));
        }

        [TestMethod]
        public void TestUpdate()
        {
            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<TestException>(() => Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString()));

            Engine.SetTransactionSigners(Alice);
            Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString());

            Assert.AreEqual("EXAMPLE", Contract.Symbol);
        }
    }
}
