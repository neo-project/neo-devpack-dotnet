// Copyright (C) 2015-2026 The Neo Project.
//
// UIntTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using CryptoECPoint = Neo.Cryptography.ECC.ECPoint;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest : DebugAndTestBase<Contract_UInt>
    {
        [TestMethod]
        public void TestStringAdd()
        {
            Assert.IsTrue(Contract.IsZeroUInt256(UInt256.Zero));
            AssertGasConsumed(1047480);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt256(UInt256.Zero));
            AssertGasConsumed(1065390);
            Assert.IsTrue(Contract.IsZeroUInt160(UInt160.Zero));
            AssertGasConsumed(1047480);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt160(UInt160.Zero));
            AssertGasConsumed(1065390);
            Assert.IsFalse(Contract.IsZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertGasConsumed(1047480);
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertGasConsumed(1065390);
            Assert.IsFalse(Contract.IsZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(1047480);
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(1065390);
            Assert.AreEqual("Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt", Contract.ToAddress(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(4592370);
        }

        [TestMethod]
        public void TestParseUInt160()
        {
            const string hex = "01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4";
            const string address = "Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt";
            var expected = UInt160.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt160(hex));
            AssertGasConsumed(7747290);
            Assert.AreEqual(expected, Contract.ParseUInt160("0x" + hex));
            AssertGasConsumed(7745100);
            Assert.AreEqual(expected, Contract.ParseUInt160(address));
            AssertGasConsumed(4921740);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160("0x1234"));
            AssertGasConsumed(1994340);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(null));
            AssertGasConsumed(1080330);
        }

        [TestMethod]
        public void TestParseUInt160InvalidAddressVersion()
        {
            var hash = UInt160.Parse("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4");
            var data = new byte[21];
            data[0] = (byte)(ProtocolSettings.Default.AddressVersion + 1);
            hash.GetSpan().ToArray().CopyTo(data, 1);
            var wrongVersionAddress = Base58.Base58CheckEncode(data);

            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(wrongVersionAddress));
            AssertGasConsumed(4629030);
            Assert.IsFalse(Contract.TryParseUInt160(wrongVersionAddress));
            AssertGasConsumed(4629660);

            var wrongLengthAddress = Base58.Base58CheckEncode(new byte[10]);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(wrongLengthAddress));
            AssertGasConsumed(3168660);
            Assert.IsFalse(Contract.TryParseUInt160(wrongLengthAddress));
            AssertGasConsumed(3169290);
        }

        [TestMethod]
        public void TestParseUInt256()
        {
            const string hex = "0x000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f";
            var expected = UInt256.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt256(hex));
            AssertGasConsumed(11245860);
            Assert.AreEqual(expected, Contract.ParseUInt256(hex.Substring(2)));
            AssertGasConsumed(11248050);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256("abcd"));
            AssertGasConsumed(1980570);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256("0xzz" + new string('0', 62)));
            AssertGasConsumed(1399350);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256(null));
            AssertGasConsumed(1080210);
        }

        [TestMethod]
        public void TestParseECPoint()
        {
            const string publicKey = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";
            var expected = CryptoECPoint.Parse(publicKey, ECCurve.Secp256r1);
            Assert.AreEqual(expected, Contract.ParseECPoint(publicKey));
            AssertGasConsumed(11301840);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseECPoint("03deadbeef"));
            AssertGasConsumed(2852610);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseECPoint(null));
            AssertGasConsumed(1080210);
        }

        [TestMethod]
        public void TestTryParseHelpers()
        {
            const string validHash = "01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4";
            const string validAddress = "Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt";
            const string validUInt256 = "0x000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f";
            const string validECPoint = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";
            const string invalidHex160 = "0xgg00000000000000000000000000000000000000";

            Assert.IsTrue(Contract.TryParseUInt160(validHash));
            AssertGasConsumed(7747740);
            Assert.IsTrue(Contract.TryParseUInt160(validAddress));
            AssertGasConsumed(4922190);
            Assert.IsTrue(Contract.TryParseUInt256(validUInt256));
            AssertGasConsumed(11246310);
            Assert.IsTrue(Contract.TryParseECPoint(validECPoint));
            AssertGasConsumed(11302290);

            Assert.IsFalse(Contract.TryParseUInt160(invalidHex160));
            AssertGasConsumed(1416690);
            Assert.IsFalse(Contract.TryParseUInt256("1234"));
            AssertGasConsumed(1975920);
            Assert.IsFalse(Contract.TryParseUInt256("0xzz" + new string('0', 62)));
            AssertGasConsumed(1399980);
            Assert.IsFalse(Contract.TryParseECPoint("1234"));
            AssertGasConsumed(1960560);
        }

        [TestMethod]
        public void TestIsValidChecksNull()
        {
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt160(null));
            AssertGasConsumed(1064790);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt256(null));
            AssertGasConsumed(1064790);
        }

        [TestMethod]
        public void TestIsValidChecks()
        {
            Assert.IsTrue(Contract.IsValidUInt160(UInt160.Zero));
            AssertGasConsumed(1047810);
            Assert.IsTrue(Contract.IsValidUInt256(UInt256.Zero));
            AssertGasConsumed(1047810);

            Assert.IsFalse(Contract.IsValidUInt160(null));
            AssertGasConsumed(1047300);
            Assert.IsFalse(Contract.IsValidUInt256(null));
            AssertGasConsumed(1047300);
        }

        [TestMethod]
        public void TestToUInt160()
        {
            Assert.IsTrue(Contract.ToUInt160(new byte[20]));
            AssertGasConsumed(1293900);
            Assert.IsFalse(Contract.ToUInt160(new byte[19]));
            AssertGasConsumed(1309440);
            Assert.IsFalse(Contract.ToUInt160(new byte[21]));
            AssertGasConsumed(1309440);
            Assert.IsFalse(Contract.ToUInt160(null));
            AssertGasConsumed(1063200);
        }

        [TestMethod]
        public void TestToUInt256()
        {
            Assert.IsTrue(Contract.ToUInt256(new byte[32]));
            AssertGasConsumed(1293900);
            Assert.IsFalse(Contract.ToUInt256(new byte[31]));
            AssertGasConsumed(1309440);
            Assert.IsFalse(Contract.ToUInt256(new byte[33]));
            AssertGasConsumed(1309440);
            Assert.IsFalse(Contract.ToUInt256(null));
            AssertGasConsumed(1063200);
        }

        [TestMethod]
        public void TestNotZeroUInt256()
        {
            Assert.IsFalse(Contract.NotZeroUInt256(UInt256.Zero));
            AssertGasConsumed(1047360);

            Assert.IsTrue(Contract.NotZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void TestNotZeroUInt160()
        {
            Assert.IsFalse(Contract.NotZeroUInt160(UInt160.Zero));
            AssertGasConsumed(1047360);

            Assert.IsTrue(Contract.NotZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(1047360);
        }
    }
}
