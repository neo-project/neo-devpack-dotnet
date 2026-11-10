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
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt256(UInt256.Zero));
            Assert.IsTrue(Contract.IsZeroUInt160(UInt160.Zero));
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt160(UInt160.Zero));
            Assert.IsFalse(Contract.IsZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            Assert.IsFalse(Contract.IsZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.AreEqual("Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt", Contract.ToAddress(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
        }

        [TestMethod]
        public void TestParseUInt160()
        {
            const string hex = "01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4";
            const string address = "Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt";
            var expected = UInt160.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt160(hex));
            Assert.AreEqual(expected, Contract.ParseUInt160("0x" + hex));
            Assert.AreEqual(expected, Contract.ParseUInt160(address));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160("0x1234"));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(null));
        }

        [TestMethod]
        public void TestParseHexByteEquivalenceForAllBytes()
        {
            for (int b = 0; b < 256; b++)
            {
                var c = (char)b;
                bool isHex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
                var value = "0x" + c + "0" + new string('0', 38);
                if (b >= 0x80)
                    continue;
                if (isHex)
                {
                    var expected = UInt160.Parse(value);
                    Assert.AreEqual(expected, Contract.ParseUInt160(value), $"byte {b}");
                    Assert.IsTrue(Contract.TryParseUInt160(value), $"byte {b}");
                }
                else
                {
                    Assert.IsFalse(Contract.TryParseUInt160(value), $"byte {b}");
                }
            }
        }

        [TestMethod]
        public void TestParseHexVariants()
        {
            const string hex = "01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4";
            var expected = UInt160.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt160("0X" + hex));
            Assert.AreEqual(expected, Contract.ParseUInt160(hex.ToUpperInvariant()));
            Assert.AreEqual(expected, Contract.ParseUInt160("0x" + hex.ToUpperInvariant()));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(hex + "0"));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160("0x" + hex.Substring(0, 38) + "zz"));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160("0x"));
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
            Assert.IsFalse(Contract.TryParseUInt160(wrongVersionAddress));

            var wrongLengthAddress = Base58.Base58CheckEncode(new byte[10]);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(wrongLengthAddress));
            Assert.IsFalse(Contract.TryParseUInt160(wrongLengthAddress));
        }

        [TestMethod]
        public void TestParseUInt256()
        {
            const string hex = "0x000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f";
            var expected = UInt256.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt256(hex));
            Assert.AreEqual(expected, Contract.ParseUInt256(hex.Substring(2)));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256("abcd"));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256("0xzz" + new string('0', 62)));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256(null));
        }

        [TestMethod]
        public void TestParseECPoint()
        {
            const string publicKey = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";
            var expected = CryptoECPoint.Parse(publicKey, ECCurve.Secp256r1);
            Assert.AreEqual(expected, Contract.ParseECPoint(publicKey));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseECPoint("03deadbeef"));
            Assert.ThrowsExactly<TestException>(() => Contract.ParseECPoint(null));
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
            Assert.IsTrue(Contract.TryParseUInt160(validAddress));
            Assert.IsTrue(Contract.TryParseUInt256(validUInt256));
            Assert.IsTrue(Contract.TryParseECPoint(validECPoint));

            Assert.IsFalse(Contract.TryParseUInt160(invalidHex160));
            Assert.IsFalse(Contract.TryParseUInt256("1234"));
            Assert.IsFalse(Contract.TryParseUInt256("0xzz" + new string('0', 62)));
            Assert.IsFalse(Contract.TryParseECPoint("1234"));
        }

        [TestMethod]
        public void TestIsValidChecksNull()
        {
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt160(null));
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt256(null));
        }

        [TestMethod]
        public void TestIsValidChecks()
        {
            Assert.IsTrue(Contract.IsValidUInt160(UInt160.Zero));
            Assert.IsTrue(Contract.IsValidUInt256(UInt256.Zero));

            Assert.IsFalse(Contract.IsValidUInt160(null));
            Assert.IsFalse(Contract.IsValidUInt256(null));
        }

        [TestMethod]
        public void TestToUInt160()
        {
            Assert.IsTrue(Contract.ToUInt160(new byte[20]));
            Assert.IsFalse(Contract.ToUInt160(new byte[19]));
            Assert.IsFalse(Contract.ToUInt160(new byte[21]));
            Assert.IsFalse(Contract.ToUInt160(null));
        }

        [TestMethod]
        public void TestToUInt256()
        {
            Assert.IsTrue(Contract.ToUInt256(new byte[32]));
            Assert.IsFalse(Contract.ToUInt256(new byte[31]));
            Assert.IsFalse(Contract.ToUInt256(new byte[33]));
            Assert.IsFalse(Contract.ToUInt256(null));
        }

        [TestMethod]
        public void TestNotZeroUInt256()
        {
            Assert.IsFalse(Contract.NotZeroUInt256(UInt256.Zero));

            Assert.IsTrue(Contract.NotZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
        }

        [TestMethod]
        public void TestNotZeroUInt160()
        {
            Assert.IsFalse(Contract.NotZeroUInt160(UInt160.Zero));

            Assert.IsTrue(Contract.NotZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
        }
    }
}
