using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.Zero, Contract.TestBigIntegerAdd(null, BigInteger.One));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.Parse("3"), Contract.TestBigIntegerAddNonNullable(BigInteger.One, new BigInteger(2)));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestBigIntegerCompare(new BigInteger(2), BigInteger.One));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestBigIntegerCompare(null, BigInteger.One));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestBigIntegerCompareNonNullable(new BigInteger(2), BigInteger.One));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual(BigInteger.One, Contract.TestBigIntegerDefault(BigInteger.One));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.Zero, Contract.TestBigIntegerDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.One, Contract.TestBigIntegerDefaultNonNullable(BigInteger.One));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestInt()
        {
            Assert.AreEqual(3, Contract.TestIntAdd(1, 2));
            Assert.AreEqual(1048500, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestIntAdd(null, 1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, Contract.TestIntAddNonNullable(1, 2));
            Assert.AreEqual(1047660, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestIntCompare(2, 1));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestIntCompare(null, 1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestIntCompareNonNullable(2, 1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual(1, Contract.TestIntDefault(1));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestIntDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual(1, Contract.TestIntDefaultNonNullable(1));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUInt()
        {
            Assert.AreEqual(3u, Contract.TestUIntAdd(1u, 2u));
            Assert.AreEqual(1048500, Engine.FeeConsumed.Value);
            Assert.AreEqual(0u, Contract.TestUIntAdd(null, 1u));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual(3u, Contract.TestUIntAddNonNullable(1u, 2u));
            Assert.AreEqual(1047660, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestUIntCompare(2u, 1u));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestUIntCompare(null, 1u));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestUIntCompareNonNullable(2u, 1u));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual(1u, Contract.TestUIntDefault(1u));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual(0u, Contract.TestUIntDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual(1u, Contract.TestUIntDefaultNonNullable(1u));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestLong()
        {
            Assert.AreEqual(3L, Contract.TestLongAdd(1L, 2L));
            Assert.AreEqual(1048500, Engine.FeeConsumed.Value);
            Assert.AreEqual(0L, Contract.TestLongAdd(null, 1L));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual(3L, Contract.TestLongAddNonNullable(1L, 2L));
            Assert.AreEqual(1047660, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestLongCompare(2L, 1L));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestLongCompare(null, 1L));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestLongCompareNonNullable(2L, 1L));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual(1L, Contract.TestLongDefault(1L));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual(0L, Contract.TestLongDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual(1L, Contract.TestLongDefaultNonNullable(1L));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestULong()
        {
            Assert.AreEqual(3UL, Contract.TestULongAdd(1UL, 2UL));
            Assert.AreEqual(1048590, Engine.FeeConsumed.Value);
            Assert.AreEqual(0UL, Contract.TestULongAdd(null, 1UL));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual(3UL, Contract.TestULongAddNonNullable(1UL, 2UL));
            Assert.AreEqual(1047750, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestULongCompare(2UL, 1UL));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestULongCompare(null, 1UL));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestULongCompareNonNullable(2UL, 1UL));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual(1UL, Contract.TestULongDefault(1UL));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual(0UL, Contract.TestULongDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual(1UL, Contract.TestULongDefaultNonNullable(1UL));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestShort()
        {
            Assert.AreEqual((short)3, Contract.TestShortAdd((short)1, (short)2));
            Assert.AreEqual(1048800, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)0, Contract.TestShortAdd(null, (short)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)3, Contract.TestShortAddNonNullable((short)1, (short)2));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestShortCompare((short)2, (short)1));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestShortCompare(null, (short)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestShortCompareNonNullable((short)2, (short)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual((short)1, Contract.TestShortDefault((short)1));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)0, Contract.TestShortDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)1, Contract.TestShortDefaultNonNullable((short)1));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUShort()
        {
            Assert.AreEqual((ushort)3, Contract.TestUShortAdd((ushort)1, (ushort)2));
            Assert.AreEqual(1048800, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)0, Contract.TestUShortAdd(null, (ushort)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)3, Contract.TestUShortAddNonNullable((ushort)1, (ushort)2));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestUShortCompare((ushort)2, (ushort)1));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestUShortCompare(null, (ushort)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestUShortCompareNonNullable((ushort)2, (ushort)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual((ushort)1, Contract.TestUShortDefault((ushort)1));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)0, Contract.TestUShortDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)1, Contract.TestUShortDefaultNonNullable((ushort)1));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestSByte()
        {
            Assert.AreEqual((sbyte)3, Contract.TestSByteAdd((sbyte)1, (sbyte)2));
            Assert.AreEqual(1048800, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)0, Contract.TestSByteAdd(null, (sbyte)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)3, Contract.TestSByteAddNonNullable((sbyte)1, (sbyte)2));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestSByteCompare((sbyte)2, (sbyte)1));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestSByteCompare(null, (sbyte)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestSByteCompareNonNullable((sbyte)2, (sbyte)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual((sbyte)1, Contract.TestSByteDefault((sbyte)1));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)0, Contract.TestSByteDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)1, Contract.TestSByteDefaultNonNullable((sbyte)1));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestByte()
        {
            Assert.AreEqual((byte)3, Contract.TestByteAdd((byte)1, (byte)2));
            Assert.AreEqual(1048800, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)0, Contract.TestByteAdd(null, (byte)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)3, Contract.TestByteAddNonNullable((byte)1, (byte)2));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestByteCompare((byte)2, (byte)1));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestByteCompare(null, (byte)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestByteCompareNonNullable((byte)2, (byte)1));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual((byte)1, Contract.TestByteDefault((byte)1));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)0, Contract.TestByteDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)1, Contract.TestByteDefaultNonNullable((byte)1));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestBool()
        {
            Assert.IsTrue(Contract.TestBoolAnd(true, true));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestBoolAnd(null, true));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestBoolAndNonNullable(true, true));
            Assert.AreEqual(1047180, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestBoolOr(true, false));
            Assert.AreEqual(1047810, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestBoolOr(null, false));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestBoolOrNonNullable(true, false));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestBoolDefault(true));
            Assert.AreEqual(1047210, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.TestBoolDefault(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestBoolDefaultNonNullable(true));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUInt160()
        {
            UInt160 smaller = UInt160.Parse("0x0000000000000000000000000000000000000001");
            UInt160 larger = UInt160.Parse("0x0000000000000000000000000000000000000002");

            Assert.AreEqual(smaller, Contract.TestUInt160Default(smaller));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(UInt160.Zero, Contract.TestUInt160Default(null));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.AreEqual(smaller, Contract.TestUInt160DefaultNonNullable(smaller));
            Assert.AreEqual(1047240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUInt256()
        {
            UInt256 smaller = UInt256.Parse("0x0000000000000000000000000000000000000000000000000000000000000001");
            UInt256 larger = UInt256.Parse("0x0000000000000000000000000000000000000000000000000000000000000002");

            Assert.AreEqual(smaller, Contract.TestUInt256Default(smaller));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(UInt256.Zero, Contract.TestUInt256Default(null));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.AreEqual(smaller, Contract.TestUInt256DefaultNonNullable(smaller));
            Assert.AreEqual(1047240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestArrays()
        {
            Assert.AreEqual(1, Contract.TestUInt160ArrayLength([UInt160.Zero]));
            Assert.AreEqual(1109190, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestUInt160ArrayLength(null));
            Assert.AreEqual(1047480, Engine.FeeConsumed.Value);
            Assert.AreEqual(1, Contract.TestUInt160ArrayLengthNonNullable([UInt160.Zero]));
            Assert.AreEqual(1108830, Engine.FeeConsumed.Value);

            Assert.AreEqual(1, Contract.TestUInt256ArrayLength([UInt256.Zero]));
            Assert.AreEqual(1109190, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestUInt256ArrayLength(null));
            Assert.AreEqual(1047480, Engine.FeeConsumed.Value);
            Assert.AreEqual(1, Contract.TestUInt256ArrayLengthNonNullable([UInt256.Zero]));
            Assert.AreEqual(1108830, Engine.FeeConsumed.Value);

            Assert.AreEqual(1, Contract.TestByteArrayLength([1]));
            Assert.AreEqual(1108980, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestByteArrayLength(null));
            Assert.AreEqual(1047480, Engine.FeeConsumed.Value);
            Assert.AreEqual(1, Contract.TestByteArrayLengthNonNullable([1]));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestString()
        {
            Assert.AreEqual(4, Contract.TestStringLength("test"));
            Assert.AreEqual(1047720, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestStringLength(null));
            Assert.AreEqual(1047480, Engine.FeeConsumed.Value);
            Assert.AreEqual(4, Contract.TestStringLengthNonNullable("test"));
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.AreEqual("test", Contract.TestStringDefault("test"));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(string.Empty, Contract.TestStringDefault(null));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.AreEqual("test", Contract.TestStringDefaultNonNullable("test"));
            Assert.AreEqual(1047240, Engine.FeeConsumed.Value);

            Assert.AreEqual("testtest", Contract.TestStringConcat("test", "test"));
            Assert.AreEqual(1355100, Engine.FeeConsumed.Value);
            Assert.AreEqual("test", Contract.TestStringConcat("test", null));
            Assert.AreEqual(1355190, Engine.FeeConsumed.Value);
            Assert.AreEqual("testtest", Contract.TestStringConcatNonNullable("test", "test"));
            Assert.AreEqual(1354740, Engine.FeeConsumed.Value);
        }
    }
}
