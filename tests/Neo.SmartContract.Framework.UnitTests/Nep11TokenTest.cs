// Copyright (C) 2015-2026 The Neo Project.
//
// Nep11TokenTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class Nep11TokenTest : DebugAndTestBase<Contract_SupportedStandard11Enum>
    {
        [TestMethod]
        public void Test_Nep11_Standard_Metadata_And_Queries()
        {
            var validOwner = UInt160.Parse("0x0000000000000000000000000000000000000001");

            Assert.AreEqual("EXAMPLE", Contract.Symbol);
            Assert.AreEqual(BigInteger.Zero, Contract.Decimals);
            Assert.AreEqual(BigInteger.Zero, Contract.TotalSupply);
            Assert.AreEqual(BigInteger.Zero, Contract.BalanceOf(validOwner));
            Assert.IsNotNull(Contract.Tokens);
            Assert.IsNotNull(Contract.TokensOf(validOwner));
            Assert.IsTrue(Contract.TestStandard());
        }

        [TestMethod]
        public void Test_Nep11_OwnerOf_And_Properties_Validate_TokenId_Length_And_Existence()
        {
            byte[] tooLongTokenId = new byte[65];
            byte[] validLengthTokenId = new byte[64];

            var ownerOf = Assert.ThrowsException<TestException>(() => Contract.OwnerOf(tooLongTokenId));
            Assert.IsTrue(ownerOf.Message.Contains("64 or less bytes long."));

            var properties = Assert.ThrowsException<TestException>(() => Contract.Properties(tooLongTokenId));
            Assert.IsTrue(properties.Message.Contains("64 or less bytes long."));

            ownerOf = Assert.ThrowsException<TestException>(() => Contract.OwnerOf(validLengthTokenId));
            Assert.IsTrue(ownerOf.Message.Contains("does not exist."));

            properties = Assert.ThrowsException<TestException>(() => Contract.Properties(validLengthTokenId));
            Assert.IsTrue(properties.Message.Contains("does not exist."));
        }

        [TestMethod]
        public void Test_Nep11_BalanceOf_And_TokensOf_Validate_Owner()
        {
            UInt160? invalidOwner = null;

            var balanceOf = Assert.ThrowsException<TestException>(() => Contract.BalanceOf(invalidOwner));
            Assert.IsTrue(balanceOf.Message.Contains("owner"));

            var tokensOf = Assert.ThrowsException<TestException>(() => Contract.TokensOf(invalidOwner));
            Assert.IsTrue(tokensOf.Message.Contains("owner"));
        }

        [TestMethod]
        public void Test_Transfer_Validates_To_And_TokenId_Length_And_Existence()
        {
            var validTo = UInt160.Parse("0x0000000000000000000000000000000000000002");
            byte[] tooLongTokenId = new byte[65];
            byte[] validLengthTokenId = new byte[64];

            var ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(null, validLengthTokenId, null));
            Assert.IsTrue(ex.Message.Contains("argument \"to\""));

            ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(validTo, tooLongTokenId, null));
            Assert.IsTrue(ex.Message.Contains("64 or less bytes long."));

            ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(validTo, validLengthTokenId, null));
            Assert.IsTrue(ex.Message.Contains("does not exist."));
        }

        [TestMethod]
        public void Test_OnNEP11Payment_Allows_ByteString_TokenId()
        {
            var validFrom = UInt160.Parse("0x0000000000000000000000000000000000000001");

            Contract.OnNEP11Payment(validFrom, BigInteger.One, new byte[] { 0x01 }, null);
        }
    }
}
