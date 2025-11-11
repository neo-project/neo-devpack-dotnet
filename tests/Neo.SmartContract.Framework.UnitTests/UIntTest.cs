// Copyright (C) 2015-2025 The Neo Project.
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
using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

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
        public void ParseUIntTypesFromContract()
        {
            var u160Hex = "0xd2a4cff31913016155e38e474a2c06d08be276cf";
            var expected160 = Neo.UInt160.Parse(u160Hex).GetSpan().ToArray();
            CollectionAssert.AreEqual(expected160, Contract.ParseUInt160Bytes(expected160));

            var u256Hex = "0xedcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";
            var expected256 = Neo.UInt256.Parse(u256Hex).GetSpan().ToArray();
            CollectionAssert.AreEqual(expected256, Contract.ParseUInt256Bytes(expected256));

            var expectedEc = ECCurve.Secp256r1.G.EncodePoint(true);
            CollectionAssert.AreEqual(expectedEc, Contract.ParseECPointBytes(expectedEc));
        }

        [TestMethod]
        public void ParseUIntTypesInvalid_Throws()
        {
            Assert.ThrowsException<TestException>(() => Contract.ParseUInt160Bytes(new byte[] { 0x01, 0x02 }));
            Assert.ThrowsException<TestException>(() => Contract.ParseUInt256Bytes(new byte[] { 0x01, 0x02 }));
            Assert.ThrowsException<TestException>(() => Contract.ParseECPointBytes(new byte[] { 0x02 }));
        }
    }
}
