// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableType.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NullableType : DebugAndTestBase<Contract_NullableType>
    {
        [TestMethod]
        public void TestBigInteger()
        {
            Assert.AreEqual(BigInteger.Parse("3"), Contract.TestBigIntegerAdd(BigInteger.One, new BigInteger(2)));
            AssertGasConsumed(1048200);
            Assert.AreEqual(BigInteger.Zero, Contract.TestBigIntegerAdd(null, BigInteger.One));
            AssertGasConsumed(1047360);
            Assert.AreEqual(BigInteger.Parse("3"), Contract.TestBigIntegerAddNonNullable(BigInteger.One, new BigInteger(2)));
            AssertGasConsumed(1047360);

            Assert.IsTrue(Contract.TestBigIntegerCompare(new BigInteger(2), BigInteger.One));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestBigIntegerCompare(null, BigInteger.One));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestBigIntegerCompareNonNullable(new BigInteger(2), BigInteger.One));
            AssertGasConsumed(1047360);

            Assert.AreEqual(BigInteger.One, Contract.TestBigIntegerDefault(BigInteger.One));
            AssertGasConsumed(1047210);
            Assert.AreEqual(BigInteger.Zero, Contract.TestBigIntegerDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual(BigInteger.One, Contract.TestBigIntegerDefaultNonNullable(BigInteger.One));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestInt()
        {
            Assert.AreEqual(3, Contract.TestIntAdd(1, 2));
            AssertGasConsumed(1048470);
            Assert.AreEqual(0, Contract.TestIntAdd(null, 1));
            AssertGasConsumed(1047360);
            Assert.AreEqual(3, Contract.TestIntAddNonNullable(1, 2));
            AssertGasConsumed(1047630);

            Assert.IsTrue(Contract.TestIntCompare(2, 1));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestIntCompare(null, 1));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestIntCompareNonNullable(2, 1));
            AssertGasConsumed(1047360);

            Assert.AreEqual(1, Contract.TestIntDefault(1));
            AssertGasConsumed(1047210);
            Assert.AreEqual(0, Contract.TestIntDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual(1, Contract.TestIntDefaultNonNullable(1));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestUInt()
        {
            Assert.AreEqual(3u, Contract.TestUIntAdd(1u, 2u));
            AssertGasConsumed(1048470);
            Assert.AreEqual(0u, Contract.TestUIntAdd(null, 1u));
            AssertGasConsumed(1047360);
            Assert.AreEqual(3u, Contract.TestUIntAddNonNullable(1u, 2u));
            AssertGasConsumed(1047630);

            Assert.IsTrue(Contract.TestUIntCompare(2u, 1u));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestUIntCompare(null, 1u));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestUIntCompareNonNullable(2u, 1u));
            AssertGasConsumed(1047360);

            Assert.AreEqual(1u, Contract.TestUIntDefault(1u));
            AssertGasConsumed(1047210);
            Assert.AreEqual(0u, Contract.TestUIntDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual(1u, Contract.TestUIntDefaultNonNullable(1u));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestLong()
        {
            Assert.AreEqual(3L, Contract.TestLongAdd(1L, 2L));
            AssertGasConsumed(1048470);
            Assert.AreEqual(0L, Contract.TestLongAdd(null, 1L));
            AssertGasConsumed(1047360);
            Assert.AreEqual(3L, Contract.TestLongAddNonNullable(1L, 2L));
            AssertGasConsumed(1047630);

            Assert.IsTrue(Contract.TestLongCompare(2L, 1L));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestLongCompare(null, 1L));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestLongCompareNonNullable(2L, 1L));
            AssertGasConsumed(1047360);

            Assert.AreEqual(1L, Contract.TestLongDefault(1L));
            AssertGasConsumed(1047210);
            Assert.AreEqual(0L, Contract.TestLongDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual(1L, Contract.TestLongDefaultNonNullable(1L));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestULong()
        {
            Assert.AreEqual(3UL, Contract.TestULongAdd(1UL, 2UL));
            AssertGasConsumed(1048560);
            Assert.AreEqual(0UL, Contract.TestULongAdd(null, 1UL));
            AssertGasConsumed(1047360);
            Assert.AreEqual(3UL, Contract.TestULongAddNonNullable(1UL, 2UL));
            AssertGasConsumed(1047720);

            Assert.IsTrue(Contract.TestULongCompare(2UL, 1UL));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestULongCompare(null, 1UL));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestULongCompareNonNullable(2UL, 1UL));
            AssertGasConsumed(1047360);

            Assert.AreEqual(1UL, Contract.TestULongDefault(1UL));
            AssertGasConsumed(1047210);
            Assert.AreEqual(0UL, Contract.TestULongDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual(1UL, Contract.TestULongDefaultNonNullable(1UL));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestShort()
        {
            Assert.AreEqual((short)3, Contract.TestShortAdd((short)1, (short)2));
            AssertGasConsumed(1048740);
            Assert.AreEqual((short)0, Contract.TestShortAdd(null, (short)1));
            AssertGasConsumed(1047360);
            Assert.AreEqual((short)3, Contract.TestShortAddNonNullable((short)1, (short)2));
            AssertGasConsumed(1047900);

            Assert.IsTrue(Contract.TestShortCompare((short)2, (short)1));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestShortCompare(null, (short)1));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestShortCompareNonNullable((short)2, (short)1));
            AssertGasConsumed(1047360);

            Assert.AreEqual((short)1, Contract.TestShortDefault((short)1));
            AssertGasConsumed(1047210);
            Assert.AreEqual((short)0, Contract.TestShortDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual((short)1, Contract.TestShortDefaultNonNullable((short)1));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestUShort()
        {
            Assert.AreEqual((ushort)3, Contract.TestUShortAdd((ushort)1, (ushort)2));
            AssertGasConsumed(1048740);
            Assert.AreEqual((ushort)0, Contract.TestUShortAdd(null, (ushort)1));
            AssertGasConsumed(1047360);
            Assert.AreEqual((ushort)3, Contract.TestUShortAddNonNullable((ushort)1, (ushort)2));
            AssertGasConsumed(1047900);

            Assert.IsTrue(Contract.TestUShortCompare((ushort)2, (ushort)1));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestUShortCompare(null, (ushort)1));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestUShortCompareNonNullable((ushort)2, (ushort)1));
            AssertGasConsumed(1047360);

            Assert.AreEqual((ushort)1, Contract.TestUShortDefault((ushort)1));
            AssertGasConsumed(1047210);
            Assert.AreEqual((ushort)0, Contract.TestUShortDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual((ushort)1, Contract.TestUShortDefaultNonNullable((ushort)1));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestSByte()
        {
            Assert.AreEqual((sbyte)3, Contract.TestSByteAdd((sbyte)1, (sbyte)2));
            AssertGasConsumed(1048740);
            Assert.AreEqual((sbyte)0, Contract.TestSByteAdd(null, (sbyte)1));
            AssertGasConsumed(1047360);
            Assert.AreEqual((sbyte)3, Contract.TestSByteAddNonNullable((sbyte)1, (sbyte)2));
            AssertGasConsumed(1047900);

            Assert.IsTrue(Contract.TestSByteCompare((sbyte)2, (sbyte)1));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestSByteCompare(null, (sbyte)1));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestSByteCompareNonNullable((sbyte)2, (sbyte)1));
            AssertGasConsumed(1047360);

            Assert.AreEqual((sbyte)1, Contract.TestSByteDefault((sbyte)1));
            AssertGasConsumed(1047210);
            Assert.AreEqual((sbyte)0, Contract.TestSByteDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual((sbyte)1, Contract.TestSByteDefaultNonNullable((sbyte)1));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestByte()
        {
            Assert.AreEqual((byte)3, Contract.TestByteAdd((byte)1, (byte)2));
            AssertGasConsumed(1048740);
            Assert.AreEqual((byte)0, Contract.TestByteAdd(null, (byte)1));
            AssertGasConsumed(1047360);
            Assert.AreEqual((byte)3, Contract.TestByteAddNonNullable((byte)1, (byte)2));
            AssertGasConsumed(1047900);

            Assert.IsTrue(Contract.TestByteCompare((byte)2, (byte)1));
            AssertGasConsumed(1048200);
            Assert.IsFalse(Contract.TestByteCompare(null, (byte)1));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestByteCompareNonNullable((byte)2, (byte)1));
            AssertGasConsumed(1047360);

            Assert.AreEqual((byte)1, Contract.TestByteDefault((byte)1));
            AssertGasConsumed(1047210);
            Assert.AreEqual((byte)0, Contract.TestByteDefault(null));
            AssertGasConsumed(1047300);
            Assert.AreEqual((byte)1, Contract.TestByteDefaultNonNullable((byte)1));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestBool()
        {
            Assert.IsTrue(Contract.TestBoolAnd(true, true));
            AssertGasConsumed(1048020);
            Assert.IsFalse(Contract.TestBoolAnd(null, true));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestBoolAndNonNullable(true, true));
            AssertGasConsumed(1047180);

            Assert.IsTrue(Contract.TestBoolOr(true, false));
            AssertGasConsumed(1047810);
            Assert.IsFalse(Contract.TestBoolOr(null, false));
            AssertGasConsumed(1047360);
            Assert.IsTrue(Contract.TestBoolOrNonNullable(true, false));
            AssertGasConsumed(1047150);

            Assert.IsTrue(Contract.TestBoolDefault(true));
            AssertGasConsumed(1047210);
            Assert.IsFalse(Contract.TestBoolDefault(null));
            AssertGasConsumed(1047300);
            Assert.IsTrue(Contract.TestBoolDefaultNonNullable(true));
            AssertGasConsumed(1047030);
        }

        [TestMethod]
        public void TestUInt160()
        {
            UInt160 smaller = UInt160.Parse("0x0000000000000000000000000000000000000001");
            UInt160 larger = UInt160.Parse("0x0000000000000000000000000000000000000002");

            Assert.AreEqual(smaller, Contract.TestUInt160Default(smaller));
            AssertGasConsumed(1047420);
            Assert.AreEqual(UInt160.Zero, Contract.TestUInt160Default(null));
            AssertGasConsumed(1047510);
            Assert.AreEqual(smaller, Contract.TestUInt160DefaultNonNullable(smaller));
            AssertGasConsumed(1047240);
        }

        [TestMethod]
        public void TestUInt256()
        {
            UInt256 smaller = UInt256.Parse("0x0000000000000000000000000000000000000000000000000000000000000001");
            UInt256 larger = UInt256.Parse("0x0000000000000000000000000000000000000000000000000000000000000002");

            Assert.AreEqual(smaller, Contract.TestUInt256Default(smaller));
            AssertGasConsumed(1047420);
            Assert.AreEqual(UInt256.Zero, Contract.TestUInt256Default(null));
            AssertGasConsumed(1047510);
            Assert.AreEqual(smaller, Contract.TestUInt256DefaultNonNullable(smaller));
            AssertGasConsumed(1047240);
        }

        [TestMethod]
        public void TestArrays()
        {
            Assert.AreEqual(1, Contract.TestUInt160ArrayLength([UInt160.Zero]));
            AssertGasConsumed(1109190);
            Assert.AreEqual(0, Contract.TestUInt160ArrayLength(null));
            AssertGasConsumed(1047480);
            Assert.AreEqual(1, Contract.TestUInt160ArrayLengthNonNullable([UInt160.Zero]));
            AssertGasConsumed(1108830);

            Assert.AreEqual(1, Contract.TestUInt256ArrayLength([UInt256.Zero]));
            AssertGasConsumed(1109190);
            Assert.AreEqual(0, Contract.TestUInt256ArrayLength(null));
            AssertGasConsumed(1047480);
            Assert.AreEqual(1, Contract.TestUInt256ArrayLengthNonNullable([UInt256.Zero]));
            AssertGasConsumed(1108830);

            Assert.AreEqual(1, Contract.TestByteArrayLength([1]));
            AssertGasConsumed(1047720);
            Assert.AreEqual(0, Contract.TestByteArrayLength(null));
            AssertGasConsumed(1047480);
            Assert.AreEqual(1, Contract.TestByteArrayLengthNonNullable([1]));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void TestNullableByteArrayAbi()
        {
            var method = Contract_NullableType.Manifest.Abi.GetMethod("testByteArrayLength", 1);
            Assert.IsNotNull(method);
            Assert.AreEqual(ContractParameterType.ByteArray, method!.Parameters[0].Type);

            var parameter = typeof(Contract_NullableType)
                .GetMethod(nameof(Contract_NullableType.TestByteArrayLength))!
                .GetParameters()[0];
            Assert.AreEqual(typeof(byte[]), parameter.ParameterType);
        }

        [TestMethod]
        public void TestString()
        {
            Assert.AreEqual(4, Contract.TestStringLength("test"));
            AssertGasConsumed(1047720);
            Assert.AreEqual(0, Contract.TestStringLength(null));
            AssertGasConsumed(1047480);
            Assert.AreEqual(4, Contract.TestStringLengthNonNullable("test"));
            AssertGasConsumed(1047360);

            Assert.AreEqual("test", Contract.TestStringDefault("test"));
            AssertGasConsumed(1047420);
            Assert.AreEqual(string.Empty, Contract.TestStringDefault(null));
            AssertGasConsumed(1047510);
            Assert.AreEqual("test", Contract.TestStringDefaultNonNullable("test"));
            AssertGasConsumed(1047240);

            Assert.AreEqual("testtest", Contract.TestStringConcat("test", "test"));
            AssertGasConsumed(1355100);
            Assert.AreEqual("test", Contract.TestStringConcat("test", null));
            AssertGasConsumed(1355190);
            Assert.AreEqual("testtest", Contract.TestStringConcatNonNullable("test", "test"));
            AssertGasConsumed(1354740);
        }
    }
}
