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
            AssertGasConsumed(4154400);
        }

        [TestMethod]
        public void TestParseUInt160()
        {
            const string hex = "01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4";
            const string address = "Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt";
            var expected = UInt160.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt160(hex));
            AssertGasConsumed(7412760);
            Assert.AreEqual(expected, Contract.ParseUInt160("0x" + hex));
            AssertGasConsumed(7412520);
            Assert.AreEqual(expected, Contract.ParseUInt160(address));
            AssertGasConsumed(4674000);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160("0x1234"));
            AssertGasConsumed(1740690);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(null));
            AssertGasConsumed(1080330);
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
        public void TestParseUInt160InvalidAddressVersion()
        {
            var hash = UInt160.Parse("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4");
            var data = new byte[21];
            data[0] = (byte)(ProtocolSettings.Default.AddressVersion + 1);
            hash.GetSpan().ToArray().CopyTo(data, 1);
            var wrongVersionAddress = Base58.Base58CheckEncode(data);

            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(wrongVersionAddress));
            AssertGasConsumed(4381290);
            Assert.IsFalse(Contract.TryParseUInt160(wrongVersionAddress));
            AssertGasConsumed(4381920);

            var wrongLengthAddress = Base58.Base58CheckEncode(new byte[10]);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt160(wrongLengthAddress));
            AssertGasConsumed(2898600);
            Assert.IsFalse(Contract.TryParseUInt160(wrongLengthAddress));
            AssertGasConsumed(2899230);
        }

        [TestMethod]
        public void TestParseUInt256()
        {
            const string hex = "0x000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f";
            var expected = UInt256.Parse(hex);

            Assert.AreEqual(expected, Contract.ParseUInt256(hex));
            AssertGasConsumed(10873050);
            Assert.AreEqual(expected, Contract.ParseUInt256(hex.Substring(2)));
            AssertGasConsumed(10873290);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256("abcd"));
            AssertGasConsumed(1724130);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256("0xzz" + new string('0', 62)));
            AssertGasConsumed(1151640);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseUInt256(null));
            AssertGasConsumed(1080210);
        }

        [TestMethod]
        public void TestParseECPoint()
        {
            const string publicKey = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";
            var expected = CryptoECPoint.Parse(publicKey, ECCurve.Secp256r1);
            Assert.AreEqual(expected, Contract.ParseECPoint(publicKey));
            AssertGasConsumed(10912650);
            Assert.ThrowsExactly<TestException>(() => Contract.ParseECPoint("03deadbeef"));
            AssertGasConsumed(2580810);
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
            AssertGasConsumed(7413210);
            Assert.IsTrue(Contract.TryParseUInt160(validAddress));
            AssertGasConsumed(4674450);
            Assert.IsTrue(Contract.TryParseUInt256(validUInt256));
            AssertGasConsumed(10873500);
            Assert.IsTrue(Contract.TryParseECPoint(validECPoint));
            AssertGasConsumed(10913100);

            Assert.IsFalse(Contract.TryParseUInt160(invalidHex160));
            AssertGasConsumed(1168980);
            Assert.IsFalse(Contract.TryParseUInt256("1234"));
            AssertGasConsumed(1722000);
            Assert.IsFalse(Contract.TryParseUInt256("0xzz" + new string('0', 62)));
            AssertGasConsumed(1152270);
            Assert.IsFalse(Contract.TryParseECPoint("1234"));
            AssertGasConsumed(1706640);
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
