using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Integer : DebugAndTestBase<Contract_Integer>
    {
        [TestMethod]
        public void divRemByte_test()
        {
            var result = Contract.DivRemByte((byte)10, (byte)4);
            var expected = byte.DivRem(10, 4);
            Assert.AreEqual(expected.Remainder, (byte)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (byte)(BigInteger)result[1]);
            Assert.AreEqual(1110150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemShort_test()
        {
            var result = Contract.DivRemShort((short)10, (short)3);
            var expected = short.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, checked((short)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((short)(BigInteger)result[1]));
            Assert.AreEqual(1110150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemInt_test()
        {
            var result = Contract.DivRemInt(10, 3);
            var expected = int.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (int)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (int)(BigInteger)result[1]);
            Assert.AreEqual(1110150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemLong_test()
        {
            var result = Contract.DivRemLong(10L, 3L);
            var expected = long.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (long)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (long)(BigInteger)result[1]);
            Assert.AreEqual(1110240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemSByte_test()
        {
            var result = Contract.DivRemSbyte((sbyte)10, (sbyte)3);
            var expected = sbyte.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (sbyte)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (sbyte)(BigInteger)result[1]);
            Assert.AreEqual(1110150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemUShort_test()
        {
            var result = Contract.DivRemUshort((ushort)10, (ushort)3);
            var expected = ushort.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (ushort)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ushort)(BigInteger)result[1]);
            Assert.AreEqual(1110150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemUInt_test()
        {
            var result = Contract.DivRemUint((uint)10, (uint)3);
            var expected = uint.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (uint)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (uint)(BigInteger)result[1]);
            Assert.AreEqual(1110150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemULong_test()
        {
            var result = Contract.DivRemUlong((ulong)10, (ulong)3);
            var expected = ulong.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void divRemZeroU_test()
        {
            Assert.ThrowsException<TestException>(() => Contract.DivRemUint((uint)10, (uint)0));
            Assert.AreEqual(1047540, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_Normal()
        {
            var result = Contract.DivRemUlong((ulong)10, (ulong)3);
            var expected = ulong.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, (ulong)2);
            var expected = ulong.DivRem(ulong.MaxValue, 2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_DivideByOne()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, (ulong)1);
            var expected = ulong.DivRem(ulong.MaxValue, 1);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendSmallerThanDivisor()
        {
            var result = Contract.DivRemUlong((ulong)3, (ulong)10);
            var expected = ulong.DivRem(3, 10);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_DivideByMaxValue()
        {
            var result = Contract.DivRemUlong((ulong)10, ulong.MaxValue);
            var expected = ulong.DivRem(10, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public void TestDivRemUlong_DivideByZero()
        {
            Contract.DivRemUlong((ulong)10, (ulong)0);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public void TestDivRemUlong_BothZero()
        {
            Contract.DivRemUlong((ulong)0, (ulong)0);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueDividedByMaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, ulong.MaxValue);
            var expected = ulong.DivRem(ulong.MaxValue, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueMinusOneDividedByMaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue - 1, ulong.MaxValue);
            var expected = ulong.DivRem(ulong.MaxValue - 1, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueDividedByTwo()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, 2UL);
            var expected = ulong.DivRem(ulong.MaxValue, 2UL);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_LargeNumbersDivisibleWithoutRemainder()
        {
            ulong large1 = ulong.MaxValue / 2;
            ulong large2 = 2;
            var result = Contract.DivRemUlong(large1, large2);
            var expected = ulong.DivRem(large1, large2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_ConsecutiveLargeNumbers()
        {
            ulong large1 = ulong.MaxValue - 1;
            ulong large2 = ulong.MaxValue;
            var result = Contract.DivRemUlong(large1, large2);
            var expected = ulong.DivRem(large1, large2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendJustSmallerThanDivisor()
        {
            ulong dividend = (1UL << 63) - 1; // 2^63 - 1
            ulong divisor = 1UL << 63; // 2^63
            var result = Contract.DivRemUlong(dividend, divisor);
            var expected = ulong.DivRem(dividend, divisor);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendJustLargerThanDivisor()
        {
            ulong dividend = (1UL << 63) + 1; // 2^63 + 1
            ulong divisor = 1UL << 63; // 2^63
            var result = Contract.DivRemUlong(dividend, divisor);
            var expected = ulong.DivRem(dividend, divisor);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_LargePrimeNumbers()
        {
            ulong largePrime1 = 18446744073709551557; // Largest prime number smaller than 2^64
            ulong largePrime2 = 18446744073709551533; // Second largest prime number smaller than 2^64
            var result = Contract.DivRemUlong(largePrime1, largePrime2);
            var expected = ulong.DivRem(largePrime1, largePrime2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDivRemUlong_PowersOfTwo()
        {
            for (int i = 1; i < 64; i++)
            {
                ulong dividend = 1UL << i;
                ulong divisor = 1UL << (i - 1);
                var result = Contract.DivRemUlong(dividend, divisor);
                var expected = ulong.DivRem(dividend, divisor);
                Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
                Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            }
        }

        [TestMethod]
        public void TestDivRemUlong_AlternatingBits()
        {
            ulong alternatingBits = 0xAAAAAAAAAAAAAAAA; // 1010...1010
            var result = Contract.DivRemUlong(alternatingBits, 3);
            var expected = ulong.DivRem(alternatingBits, 3);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            Assert.AreEqual(1110330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampByte()
        {
            Assert.AreEqual((byte)5, Contract.ClampByte(5, 0, 10));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.ClampByte(5, 10, 0));
            Assert.AreEqual(1062870, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)5, Contract.ClampByte(0, 5, 10));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)5, Contract.ClampByte(10, 0, 5));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)0, Contract.ClampByte(0, 0, 10));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)10, Contract.ClampByte(10, 0, 10));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)10, Contract.ClampByte(255, 0, 10));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((byte)10, Contract.ClampByte(20, 0, 10));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampSByte()
        {
            Assert.AreEqual((sbyte)0, Contract.ClampSByte(0, -5, 5));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)-5, Contract.ClampSByte(-10, -5, 5));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)5, Contract.ClampSByte(10, -5, 5));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)-5, Contract.ClampSByte(sbyte.MinValue, -5, 5));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual((sbyte)5, Contract.ClampSByte(sbyte.MaxValue, -5, 5));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampShort()
        {
            Assert.AreEqual((short)0, Contract.ClampShort(0, -1000, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)-1000, Contract.ClampShort(-2000, -1000, 1000));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)1000, Contract.ClampShort(2000, -1000, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)-1000, Contract.ClampShort(short.MinValue, -1000, 1000));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual((short)1000, Contract.ClampShort(short.MaxValue, -1000, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampUShort()
        {
            Assert.AreEqual((ushort)500, Contract.ClampUShort(500, 0, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)0, Contract.ClampUShort(0, 0, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)1000, Contract.ClampUShort(1000, 0, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual((ushort)1000, Contract.ClampUShort(ushort.MaxValue, 0, 1000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampInt()
        {
            Assert.AreEqual(0, Contract.ClampInt(0, -1000000, 1000000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1000000, Contract.ClampInt(-2000000, -1000000, 1000000));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000, Contract.ClampInt(2000000, -1000000, 1000000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1000000, Contract.ClampInt(int.MinValue, -1000000, 1000000));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000, Contract.ClampInt(int.MaxValue, -1000000, 1000000));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampUInt()
        {
            Assert.AreEqual(500000U, Contract.ClampUInt(500000U, 0U, 1000000U));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(0U, Contract.ClampUInt(0U, 0U, 1000000U));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000U, Contract.ClampUInt(1000000U, 0U, 1000000U));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000U, Contract.ClampUInt(uint.MaxValue, 0U, 1000000U));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampLong()
        {
            Assert.AreEqual(0L, Contract.ClampLong(0L, -1000000000000L, 1000000000000L));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1000000000000L, Contract.ClampLong(-2000000000000L, -1000000000000L, 1000000000000L));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000000000L, Contract.ClampLong(2000000000000L, -1000000000000L, 1000000000000L));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1000000000000L, Contract.ClampLong(long.MinValue, -1000000000000L, 1000000000000L));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000000000L, Contract.ClampLong(long.MaxValue, -1000000000000L, 1000000000000L));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampULong()
        {
            Assert.AreEqual(500000000000UL, Contract.ClampULong(500000000000UL, 0UL, 1000000000000UL));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(0UL, Contract.ClampULong(0UL, 0UL, 1000000000000UL));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000000000UL, Contract.ClampULong(1000000000000UL, 0UL, 1000000000000UL));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(1000000000000UL, Contract.ClampULong(ulong.MaxValue, 0UL, 1000000000000UL));
            Assert.AreEqual(1048440, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClampBigInteger()
        {
            Assert.AreEqual(BigInteger.Zero, Contract.ClampBigInteger(BigInteger.Zero, BigInteger.MinusOne, BigInteger.One));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.MinusOne, Contract.ClampBigInteger(BigInteger.MinusOne, BigInteger.MinusOne, BigInteger.One));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.One, Contract.ClampBigInteger(BigInteger.One, BigInteger.MinusOne, BigInteger.One));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.ClampBigInteger(BigInteger.MinusOne, BigInteger.MinusOne, BigInteger.MinusOne));
            Assert.AreEqual(1062870, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.One, Contract.ClampBigInteger(BigInteger.One, BigInteger.MinusOne, BigInteger.One));
            Assert.ThrowsException<TestException>(() => Contract.ClampBigInteger(BigInteger.MinusOne, BigInteger.One, BigInteger.MinusOne));
            Assert.AreEqual(1062870, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.ClampBigInteger(BigInteger.One, BigInteger.One, BigInteger.MinusOne));
            Assert.AreEqual(1062870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestMethodIntCopySign()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(int.CopySign(5, 1), Contract.CopySignInt(5, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CopySign(5, -1), Contract.CopySignInt(5, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CopySign(-5, 1), Contract.CopySignInt(-5, 1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CopySign(-5, -1), Contract.CopySignInt(-5, -1));
            Assert.AreEqual(1049280, Engine.FeeConsumed.Value);


            // Test with max values
            Assert.AreEqual(int.CopySign(int.MaxValue, 1), Contract.CopySignInt(int.MaxValue, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CopySign(int.MaxValue, -1), Contract.CopySignInt(int.MaxValue, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.CopySignInt(int.MinValue, 1));
            Assert.AreEqual(1064850, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CopySign(int.MinValue, -1), Contract.CopySignInt(int.MinValue, -1));
            Assert.AreEqual(1049280, Engine.FeeConsumed.Value);

            // Test with min values
            Assert.AreEqual(int.MaxValue, Contract.CopySignInt(int.MaxValue, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(-int.MaxValue, Contract.CopySignInt(int.MaxValue, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.CopySignInt(int.MinValue, 1));
            Assert.AreEqual(1064850, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.MinValue, Contract.CopySignInt(int.MinValue, -1));
            Assert.AreEqual(1049280, Engine.FeeConsumed.Value);

            // Test with zero
            Assert.AreEqual(0, Contract.CopySignInt(0, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.CopySignInt(0, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
        }


        [TestMethod]
        public void TestMethodSByteCopySign()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(sbyte.CopySign(5, 1), Contract.CopySignSbyte(5, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(sbyte.CopySign(5, 0), Contract.CopySignSbyte(5U, 0));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);

            // Test with max values
            Assert.AreEqual(sbyte.CopySign(sbyte.MaxValue, 1), Contract.CopySignSbyte(sbyte.MaxValue, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(sbyte.CopySign(sbyte.MaxValue, 0), Contract.CopySignSbyte(sbyte.MaxValue, 0));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.CopySignSbyte(sbyte.MinValue, 0));
            Assert.AreEqual(1064850, Engine.FeeConsumed.Value);
            Assert.AreEqual(sbyte.CopySign(sbyte.MaxValue, 0), Contract.CopySignSbyte(sbyte.MaxValue, 0));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);

            // Test with zero
            Assert.AreEqual(0U, Contract.CopySignSbyte(0U, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(0U, Contract.CopySignSbyte(0U, 0));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);

            // Test with negative values
            Assert.AreEqual(sbyte.CopySign(5, -1), Contract.CopySignSbyte(5U, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestMethodShortCopySign()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(short.CopySign(5, 1), Contract.CopySignShort(5, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CopySign(5, -1), Contract.CopySignShort(5, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CopySign(-5, 1), Contract.CopySignShort(-5, 1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CopySign(-5, -1), Contract.CopySignShort(-5, -1));
            Assert.AreEqual(1049280, Engine.FeeConsumed.Value);

            // Test with max values
            Assert.AreEqual(short.CopySign(short.MaxValue, 1), Contract.CopySignShort(short.MaxValue, 1));
            Assert.AreEqual(1049460, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CopySign(short.MaxValue, -1), Contract.CopySignShort(short.MaxValue, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.CopySignShort(short.MinValue, 1));
            Assert.AreEqual(1064850, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CopySign(short.MinValue, -1), Contract.CopySignShort(short.MinValue, -1));
            Assert.AreEqual(1049280, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestMethodIntCreateChecked()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(int.CreateChecked(5), Contract.CreateCheckedInt(5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateChecked(0), Contract.CreateCheckedInt(0));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateChecked(-5), Contract.CreateCheckedInt(-5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);

            // Test with max values
            Assert.AreEqual(int.CreateChecked(int.MaxValue), Contract.CreateCheckedInt(int.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateChecked(int.MinValue), Contract.CreateCheckedInt(int.MinValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);

            // Test with min values
            Assert.AreEqual(int.CreateChecked(int.MaxValue), Contract.CreateCheckedInt(int.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateChecked(int.MinValue), Contract.CreateCheckedInt(int.MinValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestMethodShortCreateChecked()
        {
            Assert.AreEqual(short.CreateChecked(5), Contract.CreateCheckedShort(5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CreateChecked(0), Contract.CreateCheckedShort(0));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CreateChecked(-5), Contract.CreateCheckedShort(-5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CreateChecked(short.MaxValue), Contract.CreateCheckedShort(short.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(short.CreateChecked(short.MinValue), Contract.CreateCheckedShort(short.MinValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestMethodIntCreateSaturating()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(int.CreateSaturating(5), Contract.CreateSaturatingInt(5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateSaturating(0), Contract.CreateSaturatingInt(0));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateSaturating(-5), Contract.CreateSaturatingInt(-5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);

            // Test with max values
            Assert.AreEqual(int.MaxValue, Contract.CreateSaturatingInt(int.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.MaxValue, Contract.CreateSaturatingInt(int.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);

            // Test with min values
            Assert.AreEqual(int.MaxValue, Contract.CreateSaturatingInt(int.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.MaxValue, Contract.CreateSaturatingInt(int.MaxValue));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
        }


        [TestMethod]
        public void TestMethodIntIsEvenInteger()
        {
            Assert.AreEqual(int.IsEvenInteger(0), Contract.IsEvenIntegerInt(0));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsEvenInteger(1), Contract.IsEvenIntegerInt(1));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsEvenInteger(2), Contract.IsEvenIntegerInt(2));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsEvenInteger(3), Contract.IsEvenIntegerInt(3));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsEvenInteger(4), Contract.IsEvenIntegerInt(4));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsEvenInteger(5), Contract.IsEvenIntegerInt(5));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestIsOddIntegerInt()
        {
            Assert.AreEqual(false, Contract.IsOddIntegerInt(0));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(true, Contract.IsOddIntegerInt(1));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(false, Contract.IsOddIntegerInt(2));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(true, Contract.IsOddIntegerInt(3));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(false, Contract.IsOddIntegerInt(4));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(true, Contract.IsOddIntegerInt(5));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestIsNegativeInt()
        {
            Assert.AreEqual(int.IsNegative(0), Contract.IsNegativeInt(0));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsNegative(1), Contract.IsNegativeInt(1));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsNegative(-1), Contract.IsNegativeInt(-1));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsNegative(int.MaxValue), Contract.IsNegativeInt(int.MaxValue));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsNegative(int.MinValue), Contract.IsNegativeInt(int.MinValue));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestIsPositiveInt()
        {
            Assert.AreEqual(int.IsPositive(0), Contract.IsPositiveInt(0));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPositive(1), Contract.IsPositiveInt(1));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPositive(-1), Contract.IsPositiveInt(-1));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPositive(int.MaxValue), Contract.IsPositiveInt(int.MaxValue));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPositive(int.MinValue), Contract.IsPositiveInt(int.MinValue));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestIsPow2Int()
        {
            Assert.AreEqual(int.IsPow2(0), Contract.IsPow2Int(0));
            Assert.AreEqual(1047390, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPow2(1), Contract.IsPow2Int(1));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPow2(2), Contract.IsPow2Int(2));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPow2(3), Contract.IsPow2Int(3));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPow2(4), Contract.IsPow2Int(4));
            Assert.AreEqual(1047960, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestMethodsInt()
        {
            Assert.AreEqual(int.CopySign(5, -1), Contract.CopySignInt(5, -1));
            Assert.AreEqual(1049490, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateChecked(5), Contract.CreateCheckedInt(5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.CreateSaturating(5), Contract.CreateSaturatingInt(5));
            Assert.AreEqual(1047030, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsEvenInteger(5), Contract.IsEvenIntegerInt(5));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsOddInteger(5), Contract.IsOddIntegerInt(5));
            Assert.AreEqual(1047570, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsNegative(5), Contract.IsNegativeInt(5));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPositive(5), Contract.IsPositiveInt(5));
            Assert.AreEqual(1047420, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.IsPow2(5), Contract.IsPow2Int(5));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.LeadingZeroCount(5), Contract.LeadingZeroCountInt(5));
            Assert.AreEqual(1049820, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.Log2(5), Contract.Log2Int(5));
            Assert.AreEqual(1049700, Engine.FeeConsumed.Value);
            Assert.AreEqual(int.PopCount(5), Contract.PopCountInt(5));
            Assert.AreEqual(1049070, Engine.FeeConsumed.Value);
        }
    }
}
