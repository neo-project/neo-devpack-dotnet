using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Math : DebugAndTestBase<Contract_Math>
    {
        [TestMethod]
        public void max_test()
        {
            Assert.AreEqual(2, Contract.Max(1, 2));
            AssertGasConsumed(1047360);
            Assert.AreEqual(3, Contract.Max(3, 1));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void min_test()
        {
            Assert.AreEqual(1, Contract.Min(1, 2));
            AssertGasConsumed(1047360);
            Assert.AreEqual(1, Contract.Min(3, 1));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void sign_test()
        {
            Assert.AreEqual(1, Contract.Sign(1));
            AssertGasConsumed(1047150);
            Assert.AreEqual(-1, Contract.Sign(-1));
            AssertGasConsumed(1047150);
            Assert.AreEqual(0, Contract.Sign(0));
            AssertGasConsumed(1047150);
        }

        [TestMethod]
        public void abs_test()
        {
            Assert.AreEqual(1, Contract.Abs(1));
            AssertGasConsumed(1047150);
            Assert.AreEqual(1, Contract.Abs(-1));
            AssertGasConsumed(1047150);
            Assert.AreEqual(0, Contract.Abs(0));
            AssertGasConsumed(1047150);
        }

        [TestMethod]
        public void bigMul_test()
        {
            Assert.AreEqual(((long)int.MaxValue) * int.MaxValue, Contract.BigMul(int.MaxValue, int.MaxValue));
            AssertGasConsumed(1047870);
            Assert.AreEqual(((long)int.MinValue) * int.MinValue, Contract.BigMul(int.MinValue, int.MinValue));
            AssertGasConsumed(1047870);
            Assert.ThrowsException<TestException>(() => Contract.BigMul(long.MaxValue, long.MaxValue));
            AssertGasConsumed(1063230);
        }

        [TestMethod]
        public void divRemByte_test()
        {
            var result = Contract.DivRemByte((byte)10, (byte)4);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((byte)10, (byte)4);
            Assert.AreEqual(expected.Remainder, checked((byte)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((byte)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemShort_test()
        {
            var result = Contract.DivRemShort((short)10, (short)3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((short)10, (short)3);
            Assert.AreEqual(expected.Remainder, checked((short)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((short)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemInt_test()
        {
            var result = Contract.DivRemInt(10, 3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((int)10, (int)3);
            Assert.AreEqual(expected.Remainder, checked((int)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((int)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemLong_test()
        {
            var result = Contract.DivRemLong(10L, 3L);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((long)10, (long)3);
            Assert.AreEqual(expected.Remainder, checked((long)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((long)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemSByte_test()
        {
            var result = Contract.DivRemSbyte((sbyte)10, (sbyte)3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((sbyte)10, (sbyte)3);
            Assert.AreEqual(expected.Remainder, checked((sbyte)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((sbyte)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemUShort_test()
        {
            var result = Contract.DivRemUshort((ushort)10, (ushort)3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((ushort)10, (ushort)3);
            Assert.AreEqual(expected.Remainder, checked((ushort)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ushort)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemUInt_test()
        {
            var result = Contract.DivRemUint((uint)10, (uint)3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((uint)10, (uint)3);
            Assert.AreEqual(expected.Remainder, checked((uint)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((uint)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemULong_test()
        {
            var result = Contract.DivRemUlong((ulong)10, (ulong)3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((ulong)10, (ulong)3);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemZeroU_test()
        {
            Assert.ThrowsException<TestException>(() => Contract.DivRemUint((uint)10, (uint)0));
            AssertGasConsumed(1047510);
        }

        [TestMethod]
        public void TestDivRemUlong_Normal()
        {
            var result = Contract.DivRemUlong((ulong)10, (ulong)3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((ulong)10, (ulong)3);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, (ulong)2);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(ulong.MaxValue, (ulong)2);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_DivideByOne()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, (ulong)1);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(ulong.MaxValue, (ulong)1);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendSmallerThanDivisor()
        {
            var result = Contract.DivRemUlong((ulong)3, (ulong)10);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((ulong)3, (ulong)10);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void TestDivRemUlong_DivideByMaxValue()
        {
            var result = Contract.DivRemUlong((ulong)10, ulong.MaxValue);
            Assert.IsNotNull(result);

            var expected = Math.DivRem((ulong)10, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109370);
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
            Assert.IsNotNull(result);

            var expected = Math.DivRem(ulong.MaxValue, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueMinusOneDividedByMaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue - 1, ulong.MaxValue);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(ulong.MaxValue - 1, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueDividedByTwo()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, 2UL);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(ulong.MaxValue, 2UL);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_LargeNumbersDivisibleWithoutRemainder()
        {
            ulong large1 = ulong.MaxValue / 2;
            ulong large2 = 2;
            var result = Contract.DivRemUlong(large1, large2);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(large1, large2);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void TestDivRemUlong_ConsecutiveLargeNumbers()
        {
            ulong large1 = ulong.MaxValue - 1;
            ulong large2 = ulong.MaxValue;
            var result = Contract.DivRemUlong(large1, large2);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(large1, large2);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendJustSmallerThanDivisor()
        {
            ulong dividend = (1UL << 63) - 1; // 2^63 - 1
            ulong divisor = 1UL << 63; // 2^63
            var result = Contract.DivRemUlong(dividend, divisor);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(dividend, divisor);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendJustLargerThanDivisor()
        {
            ulong dividend = (1UL << 63) + 1; // 2^63 + 1
            ulong divisor = 1UL << 63; // 2^63
            var result = Contract.DivRemUlong(dividend, divisor);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(dividend, divisor);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_LargePrimeNumbers()
        {
            ulong largePrime1 = 18446744073709551557; // Largest prime number smaller than 2^64
            ulong largePrime2 = 18446744073709551533; // Second largest prime number smaller than 2^64
            var result = Contract.DivRemUlong(largePrime1, largePrime2);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(largePrime1, largePrime2);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_PowersOfTwo()
        {
            for (int i = 1; i < 64; i++)
            {
                ulong dividend = 1UL << i;
                ulong divisor = 1UL << (i - 1);
                var result = Contract.DivRemUlong(dividend, divisor);
                Assert.IsNotNull(result);

                var expected = Math.DivRem(dividend, divisor);
                Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
                Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            }
        }

        [TestMethod]
        public void TestDivRemUlong_AlternatingBits()
        {
            ulong alternatingBits = 0xAAAAAAAAAAAAAAAA; // 1010...1010
            var result = Contract.DivRemUlong(alternatingBits, 3);
            Assert.IsNotNull(result);

            var expected = Math.DivRem(alternatingBits, 3);
            Assert.AreEqual(expected.Remainder, checked((ulong)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((ulong)(BigInteger)result[1]));
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestClampByte()
        {
            Assert.AreEqual((byte)5, Contract.ClampByte(5, 0, 10));
            AssertGasConsumed(1047930);
            Assert.ThrowsException<TestException>(() => Contract.ClampByte(5, 10, 0));
            AssertGasConsumed(1062750);
            Assert.AreEqual((byte)5, Contract.ClampByte(0, 5, 10));
            AssertGasConsumed(1047930);
            Assert.AreEqual((byte)5, Contract.ClampByte(10, 0, 5));
            AssertGasConsumed(1047930);
            Assert.AreEqual((byte)0, Contract.ClampByte(0, 0, 10));
            AssertGasConsumed(1047930);
            Assert.AreEqual((byte)10, Contract.ClampByte(10, 0, 10));
            AssertGasConsumed(1047930);
            Assert.AreEqual((byte)10, Contract.ClampByte(255, 0, 10));
            AssertGasConsumed(1047930);
            Assert.AreEqual((byte)10, Contract.ClampByte(20, 0, 10));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampSByte()
        {
            Assert.AreEqual((sbyte)0, Contract.ClampSByte(0, -5, 5));
            AssertGasConsumed(1047930);
            Assert.AreEqual((sbyte)-5, Contract.ClampSByte(-10, -5, 5));
            AssertGasConsumed(1047930);
            Assert.AreEqual((sbyte)5, Contract.ClampSByte(10, -5, 5));
            AssertGasConsumed(1047930);
            Assert.AreEqual((sbyte)-5, Contract.ClampSByte(sbyte.MinValue, -5, 5));
            AssertGasConsumed(1047930);
            Assert.AreEqual((sbyte)5, Contract.ClampSByte(sbyte.MaxValue, -5, 5));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampShort()
        {
            Assert.AreEqual((short)0, Contract.ClampShort(0, -1000, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((short)-1000, Contract.ClampShort(-2000, -1000, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((short)1000, Contract.ClampShort(2000, -1000, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((short)-1000, Contract.ClampShort(short.MinValue, -1000, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((short)1000, Contract.ClampShort(short.MaxValue, -1000, 1000));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampUShort()
        {
            Assert.AreEqual((ushort)500, Contract.ClampUShort(500, 0, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((ushort)0, Contract.ClampUShort(0, 0, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((ushort)1000, Contract.ClampUShort(1000, 0, 1000));
            AssertGasConsumed(1047930);
            Assert.AreEqual((ushort)1000, Contract.ClampUShort(ushort.MaxValue, 0, 1000));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampInt()
        {
            Assert.AreEqual(0, Contract.ClampInt(0, -1000000, 1000000));
            AssertGasConsumed(1047930);
            Assert.AreEqual(-1000000, Contract.ClampInt(-2000000, -1000000, 1000000));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000, Contract.ClampInt(2000000, -1000000, 1000000));
            AssertGasConsumed(1047930);
            Assert.AreEqual(-1000000, Contract.ClampInt(int.MinValue, -1000000, 1000000));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000, Contract.ClampInt(int.MaxValue, -1000000, 1000000));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampUInt()
        {
            Assert.AreEqual(500000U, Contract.ClampUInt(500000U, 0U, 1000000U));
            AssertGasConsumed(1047930);
            Assert.AreEqual(0U, Contract.ClampUInt(0U, 0U, 1000000U));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000U, Contract.ClampUInt(1000000U, 0U, 1000000U));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000U, Contract.ClampUInt(uint.MaxValue, 0U, 1000000U));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampLong()
        {
            Assert.AreEqual(0L, Contract.ClampLong(0L, -1000000000000L, 1000000000000L));
            AssertGasConsumed(1047930);
            Assert.AreEqual(-1000000000000L, Contract.ClampLong(-2000000000000L, -1000000000000L, 1000000000000L));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000000000L, Contract.ClampLong(2000000000000L, -1000000000000L, 1000000000000L));
            AssertGasConsumed(1047930);
            Assert.AreEqual(-1000000000000L, Contract.ClampLong(long.MinValue, -1000000000000L, 1000000000000L));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000000000L, Contract.ClampLong(long.MaxValue, -1000000000000L, 1000000000000L));
            AssertGasConsumed(1047930);
        }

        [TestMethod]
        public void TestClampULong()
        {
            Assert.AreEqual(500000000000UL, Contract.ClampULong(500000000000UL, 0UL, 1000000000000UL));
            AssertGasConsumed(1047930);
            Assert.AreEqual(0UL, Contract.ClampULong(0UL, 0UL, 1000000000000UL));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000000000UL, Contract.ClampULong(1000000000000UL, 0UL, 1000000000000UL));
            AssertGasConsumed(1047930);
            Assert.AreEqual(1000000000000UL, Contract.ClampULong(ulong.MaxValue, 0UL, 1000000000000UL));
            AssertGasConsumed(1048020);
        }
    }
}
