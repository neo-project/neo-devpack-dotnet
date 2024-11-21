using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.IsNotNull(result);

            var expected = byte.DivRem(10, 4);
            Assert.AreEqual(expected.Remainder, (byte)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (byte)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemShort_test()
        {
            var result = Contract.DivRemShort((short)10, (short)3);
            Assert.IsNotNull(result);

            var expected = short.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, checked((short)(BigInteger)result[0]));
            Assert.AreEqual(expected.Quotient, checked((short)(BigInteger)result[1]));
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemInt_test()
        {
            int l = 10, r = 3;
            foreach (int lSign in new int[] { -1, 1 })
                foreach (int rSign in new int[] { -1, 1 })
                {
                    var result = Contract.DivRemInt(l * lSign, r * rSign)!;
                    var expected = int.DivRem(l * lSign, r * rSign);
                    Assert.AreEqual(expected.Remainder, (int)(BigInteger)result[0]);
                    Assert.AreEqual(expected.Quotient, (int)(BigInteger)result[1]);
                    AssertGasConsumed(1109280);
                }

            var overflowResult = Contract.DivRemByte(int.MinValue, -1)!;
            Assert.AreEqual(0, (int)(BigInteger)overflowResult[0]);
            Assert.AreEqual(-(BigInteger)int.MinValue, (BigInteger)overflowResult[1]);  // no exception
        }

        [TestMethod]
        public void divRemLong_test()
        {
            var result = Contract.DivRemLong(10L, 3L);
            Assert.IsNotNull(result);

            var expected = long.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (long)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (long)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemSByte_test()
        {
            var result = Contract.DivRemSbyte((sbyte)10, (sbyte)3);
            Assert.IsNotNull(result);

            var expected = sbyte.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (sbyte)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (sbyte)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemUShort_test()
        {
            var result = Contract.DivRemUshort((ushort)10, (ushort)3);
            Assert.IsNotNull(result);

            var expected = ushort.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (ushort)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ushort)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemUInt_test()
        {
            var result = Contract.DivRemUint((uint)10, (uint)3);
            Assert.IsNotNull(result);

            var expected = uint.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (uint)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (uint)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void divRemULong_test()
        {
            var result = Contract.DivRemUlong((ulong)10, (ulong)3);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
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

            var expected = ulong.DivRem(10, 3);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, (ulong)2);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(ulong.MaxValue, 2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_DivideByOne()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, (ulong)1);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(ulong.MaxValue, 1);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendSmallerThanDivisor()
        {
            var result = Contract.DivRemUlong((ulong)3, (ulong)10);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(3, 10);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void TestDivRemUlong_DivideByMaxValue()
        {
            var result = Contract.DivRemUlong((ulong)10, ulong.MaxValue);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(10, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
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

            var expected = ulong.DivRem(ulong.MaxValue, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueMinusOneDividedByMaxValue()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue - 1, ulong.MaxValue);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(ulong.MaxValue - 1, ulong.MaxValue);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_MaxValueDividedByTwo()
        {
            var result = Contract.DivRemUlong(ulong.MaxValue, 2UL);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(ulong.MaxValue, 2UL);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_LargeNumbersDivisibleWithoutRemainder()
        {
            ulong large1 = ulong.MaxValue / 2;
            ulong large2 = 2;
            var result = Contract.DivRemUlong(large1, large2);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(large1, large2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109280);
        }

        [TestMethod]
        public void TestDivRemUlong_ConsecutiveLargeNumbers()
        {
            ulong large1 = ulong.MaxValue - 1;
            ulong large2 = ulong.MaxValue;
            var result = Contract.DivRemUlong(large1, large2);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(large1, large2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendJustSmallerThanDivisor()
        {
            ulong dividend = (1UL << 63) - 1; // 2^63 - 1
            ulong divisor = 1UL << 63; // 2^63
            var result = Contract.DivRemUlong(dividend, divisor);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(dividend, divisor);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109370);
        }

        [TestMethod]
        public void TestDivRemUlong_DividendJustLargerThanDivisor()
        {
            ulong dividend = (1UL << 63) + 1; // 2^63 + 1
            ulong divisor = 1UL << 63; // 2^63
            var result = Contract.DivRemUlong(dividend, divisor);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(dividend, divisor);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
            AssertGasConsumed(1109460);
        }

        [TestMethod]
        public void TestDivRemUlong_LargePrimeNumbers()
        {
            ulong largePrime1 = 18446744073709551557; // Largest prime number smaller than 2^64
            ulong largePrime2 = 18446744073709551533; // Second largest prime number smaller than 2^64
            var result = Contract.DivRemUlong(largePrime1, largePrime2);
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(largePrime1, largePrime2);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
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
            Assert.IsNotNull(result);

            var expected = ulong.DivRem(alternatingBits, 3);
            Assert.AreEqual(expected.Remainder, (ulong)(BigInteger)result[0]);
            Assert.AreEqual(expected.Quotient, (ulong)(BigInteger)result[1]);
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

        [TestMethod]
        public void TestClampBigInteger()
        {
            Assert.AreEqual(BigInteger.Zero, Contract.ClampBigInteger(BigInteger.Zero, BigInteger.MinusOne, BigInteger.One));
            AssertGasConsumed(1047930);
            Assert.AreEqual(BigInteger.MinusOne, Contract.ClampBigInteger(BigInteger.MinusOne, BigInteger.MinusOne, BigInteger.One));
            AssertGasConsumed(1047930);
            Assert.AreEqual(BigInteger.One, Contract.ClampBigInteger(BigInteger.One, BigInteger.MinusOne, BigInteger.One));
            AssertGasConsumed(1047930);
            Assert.AreEqual(BigInteger.MinusOne, Contract.ClampBigInteger(BigInteger.MinusOne, BigInteger.MinusOne, BigInteger.MinusOne));
            AssertGasConsumed(1047930);
            Assert.AreEqual(BigInteger.One, Contract.ClampBigInteger(BigInteger.One, BigInteger.MinusOne, BigInteger.One));
            Assert.ThrowsException<TestException>(() => Contract.ClampBigInteger(BigInteger.MinusOne, BigInteger.One, BigInteger.MinusOne));
            AssertGasConsumed(1062750);
            Assert.ThrowsException<TestException>(() => Contract.ClampBigInteger(BigInteger.One, BigInteger.One, BigInteger.MinusOne));
            AssertGasConsumed(1062750);
        }

        [TestMethod]
        public void TestMethodIntCopySign()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(int.CopySign(5, 1), Contract.CopySignInt(5, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(int.CopySign(5, -1), Contract.CopySignInt(5, -1));
            AssertGasConsumed(1047600);
            Assert.AreEqual(int.CopySign(-5, 1), Contract.CopySignInt(-5, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(int.CopySign(-5, -1), Contract.CopySignInt(-5, -1));
            AssertGasConsumed(1047600);


            // Test with max values
            Assert.AreEqual(int.CopySign(int.MaxValue, 1), Contract.CopySignInt(int.MaxValue, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(int.CopySign(int.MaxValue, -1), Contract.CopySignInt(int.MaxValue, -1));
            AssertGasConsumed(1047600);
            Assert.ThrowsException<TestException>(() => Contract.CopySignInt(int.MinValue, 1));
            AssertGasConsumed(1062900);
            Assert.AreEqual(int.CopySign(int.MinValue, -1), Contract.CopySignInt(int.MinValue, -1));
            AssertGasConsumed(1047600);

            // Test with min values
            Assert.AreEqual(int.MaxValue, Contract.CopySignInt(int.MaxValue, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(-int.MaxValue, Contract.CopySignInt(int.MaxValue, -1));
            AssertGasConsumed(1047600);
            Assert.ThrowsException<TestException>(() => Contract.CopySignInt(int.MinValue, 1));
            AssertGasConsumed(1062900);
            Assert.AreEqual(int.MinValue, Contract.CopySignInt(int.MinValue, -1));
            AssertGasConsumed(1047600);

            // Test with zero
            Assert.AreEqual(0, Contract.CopySignInt(0, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(0, Contract.CopySignInt(0, -1));
            AssertGasConsumed(1047600);
        }

        [TestMethod]
        public void TestMethodSByteCopySign()
        {
            // Test with normal and edge cases
            Assert.AreEqual(sbyte.CopySign(5, 1), Contract.CopySignSbyte(5, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(sbyte.CopySign(5, 0), Contract.CopySignSbyte(5U, 0));
            AssertGasConsumed(1047540);

            // Test with max values
            Assert.AreEqual(sbyte.CopySign(sbyte.MaxValue, 1), Contract.CopySignSbyte(sbyte.MaxValue, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(sbyte.CopySign(sbyte.MaxValue, 0), Contract.CopySignSbyte(sbyte.MaxValue, 0));
            AssertGasConsumed(1047540);
            Assert.ThrowsException<TestException>(() => Contract.CopySignSbyte(sbyte.MinValue, 0));
            AssertGasConsumed(1062900);
            Assert.AreEqual(sbyte.CopySign(sbyte.MaxValue, 0), Contract.CopySignSbyte(sbyte.MaxValue, 0));
            AssertGasConsumed(1047540);

            // Test with zero
            Assert.AreEqual(0U, Contract.CopySignSbyte(0U, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(0U, Contract.CopySignSbyte(0U, 0));
            AssertGasConsumed(1047540);

            // Test with negative values
            Assert.AreEqual(sbyte.CopySign(5, -1), Contract.CopySignSbyte(5U, -1));
            AssertGasConsumed(1047600);
        }

        [TestMethod]
        public void TestMethodShortCopySign()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(short.CopySign(5, 1), Contract.CopySignShort(5, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(short.CopySign(5, -1), Contract.CopySignShort(5, -1));
            AssertGasConsumed(1047600);
            Assert.AreEqual(short.CopySign(-5, 1), Contract.CopySignShort(-5, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(short.CopySign(-5, -1), Contract.CopySignShort(-5, -1));
            AssertGasConsumed(1047600);

            // Test with max values
            Assert.AreEqual(short.CopySign(short.MaxValue, 1), Contract.CopySignShort(short.MaxValue, 1));
            AssertGasConsumed(1047540);
            Assert.AreEqual(short.CopySign(short.MaxValue, -1), Contract.CopySignShort(short.MaxValue, -1));
            AssertGasConsumed(1047600);
            Assert.ThrowsException<TestException>(() => Contract.CopySignShort(short.MinValue, 1));
            AssertGasConsumed(1062900);
            Assert.AreEqual(short.CopySign(short.MinValue, -1), Contract.CopySignShort(short.MinValue, -1));
            AssertGasConsumed(1047600);
        }

        [TestMethod]
        public void TestMethodIntCreateChecked()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(int.CreateChecked(5), Contract.CreateCheckedInt(5));
            AssertGasConsumed(1047450);
            Assert.AreEqual(int.CreateChecked(0), Contract.CreateCheckedInt(0));
            AssertGasConsumed(1047450);
            Assert.AreEqual(int.CreateChecked(-5), Contract.CreateCheckedInt(-5));
            AssertGasConsumed(1047450);

            // Test with max values
            Assert.AreEqual(int.CreateChecked(int.MaxValue), Contract.CreateCheckedInt(int.MaxValue));
            AssertGasConsumed(1047450);
            Assert.AreEqual(int.CreateChecked(int.MinValue), Contract.CreateCheckedInt(int.MinValue));
            AssertGasConsumed(1047450);

            // Test with min values
            Assert.AreEqual(int.CreateChecked(int.MaxValue), Contract.CreateCheckedInt(int.MaxValue));
            AssertGasConsumed(1047450);
            Assert.AreEqual(int.CreateChecked(int.MinValue), Contract.CreateCheckedInt(int.MinValue));
            AssertGasConsumed(1047450);
        }

        [TestMethod]
        public void TestMethodShortCreateChecked()
        {
            Assert.AreEqual(short.CreateChecked(5), Contract.CreateCheckedShort(5));
            AssertGasConsumed(1047450);
            Assert.AreEqual(short.CreateChecked(0), Contract.CreateCheckedShort(0));
            AssertGasConsumed(1047450);
            Assert.AreEqual(short.CreateChecked(-5), Contract.CreateCheckedShort(-5));
            AssertGasConsumed(1047450);
            Assert.AreEqual(short.CreateChecked(short.MaxValue), Contract.CreateCheckedShort(short.MaxValue));
            AssertGasConsumed(1047450);
            Assert.AreEqual(short.CreateChecked(short.MinValue), Contract.CreateCheckedShort(short.MinValue));
            AssertGasConsumed(1047450);
        }

        [TestMethod]
        public void TestMethodIntCreateSaturating()
        {
            // Test with notmal and edge cases
            Assert.AreEqual(int.CreateSaturating(5), Contract.CreateSaturatingInt(5));
            AssertGasConsumed(1048170);
            Assert.AreEqual(int.CreateSaturating(0), Contract.CreateSaturatingInt(0));
            AssertGasConsumed(1048170);
            Assert.AreEqual(int.CreateSaturating(-5), Contract.CreateSaturatingInt(-5));
            AssertGasConsumed(1048170);

            // Test with max values
            Assert.AreEqual(int.MaxValue, Contract.CreateSaturatingInt(int.MaxValue));
            AssertGasConsumed(1048170);
        }


        [TestMethod]
        public void TestMethodIntIsEvenInteger()
        {
            for (int i = 0; i < 6; ++i)
            {
                Assert.AreEqual(int.IsEvenInteger(i), Contract.IsEvenIntegerInt(i));
                AssertGasConsumed(1047420);
            }
        }

        [TestMethod]
        public void TestIsOddIntegerInt()
        {
            Assert.AreEqual(false, Contract.IsOddIntegerInt(0));
            AssertGasConsumed(1047420);
            Assert.AreEqual(true, Contract.IsOddIntegerInt(1));
            AssertGasConsumed(1047420);
            Assert.AreEqual(false, Contract.IsOddIntegerInt(2));
            AssertGasConsumed(1047420);
            Assert.AreEqual(true, Contract.IsOddIntegerInt(3));
            AssertGasConsumed(1047420);
            Assert.AreEqual(false, Contract.IsOddIntegerInt(4));
            AssertGasConsumed(1047420);
            Assert.AreEqual(true, Contract.IsOddIntegerInt(5));
            AssertGasConsumed(1047420);
        }

        [TestMethod]
        public void TestIsNegativeInt()
        {
            Assert.AreEqual(int.IsNegative(0), Contract.IsNegativeInt(0));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsNegative(1), Contract.IsNegativeInt(1));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsNegative(-1), Contract.IsNegativeInt(-1));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsNegative(int.MaxValue), Contract.IsNegativeInt(int.MaxValue));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsNegative(int.MinValue), Contract.IsNegativeInt(int.MinValue));
            AssertGasConsumed(1047300);
        }

        [TestMethod]
        public void TestIsPositiveInt()
        {
            Assert.AreEqual(int.IsPositive(0), Contract.IsPositiveInt(0));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsPositive(1), Contract.IsPositiveInt(1));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsPositive(-1), Contract.IsPositiveInt(-1));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsPositive(int.MaxValue), Contract.IsPositiveInt(int.MaxValue));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsPositive(int.MinValue), Contract.IsPositiveInt(int.MinValue));
            AssertGasConsumed(1047300);
        }

        [TestMethod]
        public void TestIsPow2Int()
        {
            Assert.AreEqual(int.IsPow2(0), Contract.IsPow2Int(0));
            AssertGasConsumed(1047330);
            Assert.AreEqual(int.IsPow2(1), Contract.IsPow2Int(1));
            AssertGasConsumed(1047720);
            Assert.AreEqual(int.IsPow2(2), Contract.IsPow2Int(2));
            AssertGasConsumed(1047720);
            Assert.AreEqual(int.IsPow2(3), Contract.IsPow2Int(3));
            AssertGasConsumed(1047720);
            Assert.AreEqual(int.IsPow2(4), Contract.IsPow2Int(4));
            AssertGasConsumed(1047720);
        }

        [TestMethod]
        public void TestMethodsInt()
        {
            Assert.AreEqual(int.CopySign(5, -1), Contract.CopySignInt(5, -1));
            AssertGasConsumed(1047600);
            Assert.AreEqual(int.CreateChecked(5), Contract.CreateCheckedInt(5));
            AssertGasConsumed(1047450);
            Assert.AreEqual(int.CreateSaturating(5), Contract.CreateSaturatingInt(5));
            AssertGasConsumed(1048170);
            Assert.AreEqual(int.IsEvenInteger(5), Contract.IsEvenIntegerInt(5));
            AssertGasConsumed(1047420);
            Assert.AreEqual(int.IsOddInteger(5), Contract.IsOddIntegerInt(5));
            AssertGasConsumed(1047420);
            Assert.AreEqual(int.IsNegative(5), Contract.IsNegativeInt(5));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsPositive(5), Contract.IsPositiveInt(5));
            AssertGasConsumed(1047300);
            Assert.AreEqual(int.IsPow2(5), Contract.IsPow2Int(5));
            AssertGasConsumed(1047720);
            Assert.AreEqual(int.LeadingZeroCount(5), Contract.LeadingZeroCountInt(5));
            AssertGasConsumed(1049970);
            Assert.AreEqual(int.Log2(5), Contract.Log2Int(5));
            AssertGasConsumed(1049250);
        }

        [TestMethod]
        public void TestMethodLongCopySign()
        {
            Assert.AreEqual(long.CopySign(5L, 1L), Contract.CopySignLong(5L, 1L));
            Assert.AreEqual(long.CopySign(5L, -1L), Contract.CopySignLong(5L, -1L));
            Assert.AreEqual(long.CopySign(-5L, 1L), Contract.CopySignLong(-5L, 1L));
            Assert.AreEqual(long.CopySign(-5L, -1L), Contract.CopySignLong(-5L, -1L));
            Assert.AreEqual(long.CopySign(long.MaxValue, 1L), Contract.CopySignLong(long.MaxValue, 1L));
            Assert.AreEqual(long.CopySign(long.MaxValue, -1L), Contract.CopySignLong(long.MaxValue, -1L));
            Assert.ThrowsException<TestException>(() => Contract.CopySignLong(long.MinValue, 1L));
            Assert.AreEqual(long.CopySign(long.MinValue, -1L), Contract.CopySignLong(long.MinValue, -1L));
        }

        [TestMethod]
        public void TestMethodByteCreateChecked()
        {
            Assert.AreEqual(byte.CreateChecked(5), Contract.CreateCheckedByte(5));
            Assert.AreEqual(byte.CreateChecked(0), Contract.CreateCheckedByte(0));
            Assert.AreEqual(byte.CreateChecked(byte.MaxValue), Contract.CreateCheckedByte(byte.MaxValue));
            Assert.ThrowsException<TestException>(() => Contract.CreateCheckedByte(-1));
            Assert.ThrowsException<TestException>(() => Contract.CreateCheckedByte(256));
        }

        [TestMethod]
        public void TestMethodLongCreateChecked()
        {
            Assert.AreEqual(long.CreateChecked(5L), Contract.CreateCheckedLong(5L));
            Assert.AreEqual(long.CreateChecked(0L), Contract.CreateCheckedLong(0L));
            Assert.AreEqual(long.CreateChecked(-5L), Contract.CreateCheckedLong(-5L));
            Assert.AreEqual(long.CreateChecked(long.MaxValue), Contract.CreateCheckedLong(long.MaxValue));
            Assert.AreEqual(long.CreateChecked(long.MinValue), Contract.CreateCheckedLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodUlongCreateChecked()
        {
            Assert.AreEqual(ulong.CreateChecked(5UL), Contract.CreateCheckedUlong(5UL));
            Assert.AreEqual(ulong.CreateChecked(0UL), Contract.CreateCheckedUlong(0UL));
            Assert.AreEqual(ulong.CreateChecked(ulong.MaxValue), Contract.CreateCheckedUlong(ulong.MaxValue));
            Assert.ThrowsException<TestException>(() => Contract.CreateCheckedUlong(-1L));
        }

        [TestMethod]
        public void TestMethodCharCreateChecked()
        {
            Assert.AreEqual(int.CreateChecked('A'), Contract.CreateCheckedChar('A'));
            Assert.AreEqual(int.CreateChecked('\0'), Contract.CreateCheckedChar('\0'));
            Assert.AreEqual(int.CreateChecked(char.MaxValue), Contract.CreateCheckedChar(char.MaxValue));
        }

        [TestMethod]
        public void TestMethodSbyteCreateChecked()
        {
            Assert.AreEqual(sbyte.CreateChecked(5), Contract.CreateCheckedSbyte(5));
            Assert.AreEqual(sbyte.CreateChecked(0), Contract.CreateCheckedSbyte(0));
            Assert.AreEqual(sbyte.CreateChecked(-5), Contract.CreateCheckedSbyte(-5));
            Assert.AreEqual(sbyte.CreateChecked(sbyte.MaxValue), Contract.CreateCheckedSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.CreateChecked(sbyte.MinValue), Contract.CreateCheckedSbyte(sbyte.MinValue));
            Assert.ThrowsException<TestException>(() => Contract.CreateCheckedSbyte(128));
            Assert.ThrowsException<TestException>(() => Contract.CreateCheckedSbyte(-129));
        }

        [TestMethod]
        public void TestMethodByteSaturating()
        {
            Assert.AreEqual(byte.CreateSaturating(5), Contract.CreateSaturatingByte(5));
            Assert.AreEqual(byte.CreateSaturating(0), Contract.CreateSaturatingByte(0));
            Assert.AreEqual(byte.CreateSaturating(byte.MaxValue), Contract.CreateSaturatingByte(byte.MaxValue));
            Assert.AreEqual(byte.CreateSaturating(-1), Contract.CreateSaturatingByte(-1));
            Assert.AreEqual(byte.CreateSaturating(256), Contract.CreateSaturatingByte(256));
        }

        [TestMethod]
        public void TestMethodLongCreateSaturating()
        {
            Assert.AreEqual(long.CreateSaturating(5L), Contract.CreateSaturatingLong(5L));
            Assert.AreEqual(long.CreateSaturating(0L), Contract.CreateSaturatingLong(0L));
            Assert.AreEqual(long.CreateSaturating(-5L), Contract.CreateSaturatingLong(-5L));
            Assert.AreEqual(long.CreateSaturating(long.MaxValue), Contract.CreateSaturatingLong(long.MaxValue));
            Assert.AreEqual(long.CreateSaturating(long.MinValue), Contract.CreateSaturatingLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodUlongCreateSaturating()
        {
            Assert.AreEqual(ulong.CreateSaturating(5UL), Contract.CreateSaturatingUlong(5UL));
            Assert.AreEqual(ulong.CreateSaturating(0UL), Contract.CreateSaturatingUlong(0UL));
            Assert.AreEqual(ulong.CreateSaturating(ulong.MaxValue), Contract.CreateSaturatingUlong(ulong.MaxValue));
            Assert.AreEqual(0UL, Contract.CreateSaturatingUlong(-1L));
        }

        [TestMethod]
        public void TestMethodCharCreateSaturating()
        {
            Assert.AreEqual(int.CreateSaturating('A'), Contract.CreateSaturatingChar('A'));
            Assert.AreEqual(int.CreateSaturating('\0'), Contract.CreateSaturatingChar('\0'));
            Assert.AreEqual(int.CreateSaturating(char.MaxValue), Contract.CreateSaturatingChar(char.MaxValue));
        }

        [TestMethod]
        public void TestMethodSbyteCreateSaturating()
        {
            Assert.AreEqual(sbyte.CreateSaturating(5), Contract.CreateSaturatingSbyte(5));
            Assert.AreEqual(sbyte.CreateSaturating(0), Contract.CreateSaturatingSbyte(0));
            Assert.AreEqual(sbyte.CreateSaturating(-5), Contract.CreateSaturatingSbyte(-5));
            Assert.AreEqual(sbyte.CreateSaturating(sbyte.MaxValue), Contract.CreateSaturatingSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.CreateSaturating(sbyte.MinValue), Contract.CreateSaturatingSbyte(sbyte.MinValue));
            Assert.AreEqual(sbyte.CreateSaturating(128), Contract.CreateSaturatingSbyte(128));
            Assert.AreEqual(sbyte.CreateSaturating(-129), Contract.CreateSaturatingSbyte(-129));
        }

        [TestMethod]
        public void TestMethodUIntIsEvenInteger()
        {
            Assert.AreEqual(uint.IsEvenInteger(0U), Contract.IsEventUInt(0U));
            Assert.AreEqual(uint.IsEvenInteger(1U), Contract.IsEventUInt(1U));
            Assert.AreEqual(uint.IsEvenInteger(2U), Contract.IsEventUInt(2U));
            Assert.AreEqual(uint.IsEvenInteger(uint.MaxValue), Contract.IsEventUInt(uint.MaxValue));
        }

        [TestMethod]
        public void TestMethodLongIsEvenInteger()
        {
            Assert.AreEqual(long.IsEvenInteger(0L), Contract.IsEvenLong(0L));
            Assert.AreEqual(long.IsEvenInteger(1L), Contract.IsEvenLong(1L));
            Assert.AreEqual(long.IsEvenInteger(2L), Contract.IsEvenLong(2L));
            Assert.AreEqual(long.IsEvenInteger(long.MaxValue), Contract.IsEvenLong(long.MaxValue));
            Assert.AreEqual(long.IsEvenInteger(long.MinValue), Contract.IsEvenLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodUlongIsEvenInteger()
        {
            Assert.AreEqual(ulong.IsEvenInteger(0UL), Contract.IsEvenUlong(0UL));
            Assert.AreEqual(ulong.IsEvenInteger(1UL), Contract.IsEvenUlong(1UL));
            Assert.AreEqual(ulong.IsEvenInteger(2UL), Contract.IsEvenUlong(2UL));
            Assert.AreEqual(ulong.IsEvenInteger(ulong.MaxValue), Contract.IsEvenUlong(ulong.MaxValue));
        }

        [TestMethod]
        public void TestMethodShortIsEvenInteger()
        {
            Assert.AreEqual(short.IsEvenInteger((short)0), Contract.IsEvenShort((short)0));
            Assert.AreEqual(short.IsEvenInteger((short)1), Contract.IsEvenShort((short)1));
            Assert.AreEqual(short.IsEvenInteger((short)2), Contract.IsEvenShort((short)2));
            Assert.AreEqual(short.IsEvenInteger(short.MaxValue), Contract.IsEvenShort(short.MaxValue));
            Assert.AreEqual(short.IsEvenInteger(short.MinValue), Contract.IsEvenShort(short.MinValue));
        }

        [TestMethod]
        public void TestMethodUshortIsEvenInteger()
        {
            Assert.AreEqual(ushort.IsEvenInteger((ushort)0), Contract.IsEvenUshort((ushort)0));
            Assert.AreEqual(ushort.IsEvenInteger((ushort)1), Contract.IsEvenUshort((ushort)1));
            Assert.AreEqual(ushort.IsEvenInteger((ushort)2), Contract.IsEvenUshort((ushort)2));
            Assert.AreEqual(ushort.IsEvenInteger(ushort.MaxValue), Contract.IsEvenUshort(ushort.MaxValue));
        }

        [TestMethod]
        public void TestMethodByteIsEvenInteger()
        {
            Assert.AreEqual(byte.IsEvenInteger((byte)0), Contract.IsEvenByte((byte)0));
            Assert.AreEqual(byte.IsEvenInteger((byte)1), Contract.IsEvenByte((byte)1));
            Assert.AreEqual(byte.IsEvenInteger((byte)2), Contract.IsEvenByte((byte)2));
            Assert.AreEqual(byte.IsEvenInteger(byte.MaxValue), Contract.IsEvenByte(byte.MaxValue));
        }

        [TestMethod]
        public void TestMethodSbyteIsEvenInteger()
        {
            Assert.AreEqual(sbyte.IsEvenInteger((sbyte)0), Contract.IsEvenSbyte((sbyte)0));
            Assert.AreEqual(sbyte.IsEvenInteger((sbyte)1), Contract.IsEvenSbyte((sbyte)1));
            Assert.AreEqual(sbyte.IsEvenInteger((sbyte)2), Contract.IsEvenSbyte((sbyte)2));
            Assert.AreEqual(sbyte.IsEvenInteger(sbyte.MaxValue), Contract.IsEvenSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.IsEvenInteger(sbyte.MinValue), Contract.IsEvenSbyte(sbyte.MinValue));
        }

        // Add similar methods for IsOddInteger for all types

        [TestMethod]
        public void TestMethodLongIsNegative()
        {
            Assert.AreEqual(long.IsNegative(0L), Contract.IsNegativeLong(0L));
            Assert.AreEqual(long.IsNegative(1L), Contract.IsNegativeLong(1L));
            Assert.AreEqual(long.IsNegative(-1L), Contract.IsNegativeLong(-1L));
            Assert.AreEqual(long.IsNegative(long.MaxValue), Contract.IsNegativeLong(long.MaxValue));
            Assert.AreEqual(long.IsNegative(long.MinValue), Contract.IsNegativeLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodShortIsNegative()
        {
            Assert.AreEqual(short.IsNegative((short)0), Contract.IsNegativeShort((short)0));
            Assert.AreEqual(short.IsNegative((short)1), Contract.IsNegativeShort((short)1));
            Assert.AreEqual(short.IsNegative((short)-1), Contract.IsNegativeShort((short)-1));
            Assert.AreEqual(short.IsNegative(short.MaxValue), Contract.IsNegativeShort(short.MaxValue));
            Assert.AreEqual(short.IsNegative(short.MinValue), Contract.IsNegativeShort(short.MinValue));
        }

        [TestMethod]
        public void TestMethodSbyteIsNegative()
        {
            Assert.AreEqual(sbyte.IsNegative((sbyte)0), Contract.IsNegativeSbyte((sbyte)0));
            Assert.AreEqual(sbyte.IsNegative((sbyte)1), Contract.IsNegativeSbyte((sbyte)1));
            Assert.AreEqual(sbyte.IsNegative((sbyte)-1), Contract.IsNegativeSbyte((sbyte)-1));
            Assert.AreEqual(sbyte.IsNegative(sbyte.MaxValue), Contract.IsNegativeSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.IsNegative(sbyte.MinValue), Contract.IsNegativeSbyte(sbyte.MinValue));
        }

        [TestMethod]
        public void TestMethodUIntIsPow2()
        {
            Assert.AreEqual(uint.IsPow2(0U), Contract.IsPow2UInt(0U));
            Assert.AreEqual(uint.IsPow2(1U), Contract.IsPow2UInt(1U));
            Assert.AreEqual(uint.IsPow2(2U), Contract.IsPow2UInt(2U));
            Assert.AreEqual(uint.IsPow2(4U), Contract.IsPow2UInt(4U));
            Assert.AreEqual(uint.IsPow2(uint.MaxValue), Contract.IsPow2UInt(uint.MaxValue));
        }

        [TestMethod]
        public void TestMethodLongIsPow2()
        {
            Assert.AreEqual(long.IsPow2(0L), Contract.IsPow2Long(0L));
            Assert.AreEqual(long.IsPow2(1L), Contract.IsPow2Long(1L));
            Assert.AreEqual(long.IsPow2(2L), Contract.IsPow2Long(2L));
            Assert.AreEqual(long.IsPow2(4L), Contract.IsPow2Long(4L));
            Assert.AreEqual(long.IsPow2(long.MaxValue), Contract.IsPow2Long(long.MaxValue));
            Assert.AreEqual(long.IsPow2(long.MinValue), Contract.IsPow2Long(long.MinValue));
        }

        [TestMethod]
        public void TestMethodUlongIsPow2()
        {
            Assert.AreEqual(ulong.IsPow2(0UL), Contract.IsPow2Ulong(0UL));
            Assert.AreEqual(ulong.IsPow2(1UL), Contract.IsPow2Ulong(1UL));
            Assert.AreEqual(ulong.IsPow2(2UL), Contract.IsPow2Ulong(2UL));
            Assert.AreEqual(ulong.IsPow2(4UL), Contract.IsPow2Ulong(4UL));
            Assert.AreEqual(ulong.IsPow2(ulong.MaxValue), Contract.IsPow2Ulong(ulong.MaxValue));
        }

        [TestMethod]
        public void TestMethodShortIsPow2()
        {
            Assert.AreEqual(short.IsPow2((short)0), Contract.IsPow2Short((short)0));
            Assert.AreEqual(short.IsPow2((short)1), Contract.IsPow2Short((short)1));
            Assert.AreEqual(short.IsPow2((short)2), Contract.IsPow2Short((short)2));
            Assert.AreEqual(short.IsPow2((short)4), Contract.IsPow2Short((short)4));
            Assert.AreEqual(short.IsPow2(short.MaxValue), Contract.IsPow2Short(short.MaxValue));
            Assert.AreEqual(short.IsPow2(short.MinValue), Contract.IsPow2Short(short.MinValue));
        }

        [TestMethod]
        public void TestMethodUshortIsPow2()
        {
            Assert.AreEqual(ushort.IsPow2((ushort)0), Contract.IsPow2Ushort((ushort)0));
            Assert.AreEqual(ushort.IsPow2((ushort)1), Contract.IsPow2Ushort((ushort)1));
            Assert.AreEqual(ushort.IsPow2((ushort)2), Contract.IsPow2Ushort((ushort)2));
            Assert.AreEqual(ushort.IsPow2((ushort)4), Contract.IsPow2Ushort((ushort)4));
            Assert.AreEqual(ushort.IsPow2(ushort.MaxValue), Contract.IsPow2Ushort(ushort.MaxValue));
        }

        [TestMethod]
        public void TestMethodByteIsPow2()
        {
            Assert.AreEqual(byte.IsPow2((byte)0), Contract.IsPow2Byte((byte)0));
            Assert.AreEqual(byte.IsPow2((byte)1), Contract.IsPow2Byte((byte)1));
            Assert.AreEqual(byte.IsPow2((byte)2), Contract.IsPow2Byte((byte)2));
            Assert.AreEqual(byte.IsPow2((byte)4), Contract.IsPow2Byte((byte)4));
            Assert.AreEqual(byte.IsPow2(byte.MaxValue), Contract.IsPow2Byte(byte.MaxValue));
        }

        [TestMethod]
        public void TestMethodSbyteIsPow2()
        {
            Assert.AreEqual(sbyte.IsPow2((sbyte)0), Contract.IsPow2Sbyte((sbyte)0));
            Assert.AreEqual(sbyte.IsPow2((sbyte)1), Contract.IsPow2Sbyte((sbyte)1));
            Assert.AreEqual(sbyte.IsPow2((sbyte)2), Contract.IsPow2Sbyte((sbyte)2));
            Assert.AreEqual(sbyte.IsPow2((sbyte)4), Contract.IsPow2Sbyte((sbyte)4));
            Assert.AreEqual(sbyte.IsPow2(sbyte.MaxValue), Contract.IsPow2Sbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.IsPow2(sbyte.MinValue), Contract.IsPow2Sbyte(sbyte.MinValue));
        }

        [TestMethod]
        public void TestMethodUIntLeadingZeroCount()
        {
            Assert.AreEqual(uint.LeadingZeroCount(0U), Contract.LeadingZeroCountUInt(0U));
            Assert.AreEqual(uint.LeadingZeroCount(1U), Contract.LeadingZeroCountUInt(1U));
            Assert.AreEqual(uint.LeadingZeroCount(uint.MaxValue), Contract.LeadingZeroCountUInt(uint.MaxValue));
        }

        [TestMethod]
        public void TestMethodLongLeadingZeroCount()
        {
            Assert.AreEqual(long.LeadingZeroCount(0L), Contract.LeadingZeroCountLong(0L));
            Assert.AreEqual(long.LeadingZeroCount(1L), Contract.LeadingZeroCountLong(1L));
            Assert.AreEqual(long.LeadingZeroCount(-1L), Contract.LeadingZeroCountLong(-1L));
            Assert.AreEqual(long.LeadingZeroCount(long.MaxValue), Contract.LeadingZeroCountLong(long.MaxValue));
            Assert.AreEqual(long.LeadingZeroCount(long.MinValue), Contract.LeadingZeroCountLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodShortLeadingZeroCount()
        {
            Assert.AreEqual(short.LeadingZeroCount((short)0), Contract.LeadingZeroCountShort((short)0));
            Assert.AreEqual(short.LeadingZeroCount((short)1), Contract.LeadingZeroCountShort((short)1));
            Assert.AreEqual(short.LeadingZeroCount((short)-1), Contract.LeadingZeroCountShort((short)-1));
            Assert.AreEqual(short.LeadingZeroCount(short.MaxValue), Contract.LeadingZeroCountShort(short.MaxValue));
            Assert.AreEqual(short.LeadingZeroCount(short.MinValue), Contract.LeadingZeroCountShort(short.MinValue));
        }

        [TestMethod]
        public void TestMethodUshortLeadingZeroCount()
        {
            Assert.AreEqual(ushort.LeadingZeroCount((ushort)0), Contract.LeadingZeroCountUshort((ushort)0));
            Assert.AreEqual(ushort.LeadingZeroCount((ushort)1), Contract.LeadingZeroCountUshort((ushort)1));
            Assert.AreEqual(ushort.LeadingZeroCount(ushort.MaxValue), Contract.LeadingZeroCountUshort(ushort.MaxValue));
        }

        [TestMethod]
        public void TestMethodByteLeadingZeroCount()
        {
            Assert.AreEqual(byte.LeadingZeroCount((byte)0), Contract.LeadingZeroCountByte((byte)0));
            Assert.AreEqual(byte.LeadingZeroCount((byte)1), Contract.LeadingZeroCountByte((byte)1));
            Assert.AreEqual(byte.LeadingZeroCount(byte.MaxValue), Contract.LeadingZeroCountByte(byte.MaxValue));
        }

        [TestMethod]
        public void TestMethodSbyteLeadingZeroCount()
        {

            Assert.AreEqual(sbyte.LeadingZeroCount((sbyte)0), Contract.LeadingZeroCountSbyte((sbyte)0));
            Assert.AreEqual(sbyte.LeadingZeroCount((sbyte)1), Contract.LeadingZeroCountSbyte((sbyte)1));
            Assert.AreEqual(sbyte.LeadingZeroCount((sbyte)-1), Contract.LeadingZeroCountSbyte((sbyte)-1));
            Assert.AreEqual(sbyte.LeadingZeroCount(sbyte.MaxValue), Contract.LeadingZeroCountSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.LeadingZeroCount(sbyte.MinValue), Contract.LeadingZeroCountSbyte(sbyte.MinValue));
        }

        [TestMethod]
        public void TestMethodUIntLog2()
        {
            Assert.AreEqual(uint.Log2(1U), Contract.Log2UInt(1U));
            Assert.AreEqual(uint.Log2(2U), Contract.Log2UInt(2U));
            Assert.AreEqual(uint.Log2(uint.MaxValue), Contract.Log2UInt(uint.MaxValue));
            Assert.AreEqual(uint.Log2(0), Contract.Log2UInt(0U));
        }

        [TestMethod]
        public void TestMethodLongLog2()
        {
            Assert.AreEqual(long.Log2(1L), Contract.Log2Long(1L));
            Assert.AreEqual(long.Log2(2L), Contract.Log2Long(2L));
            Assert.AreEqual(long.Log2(long.MaxValue), Contract.Log2Long(long.MaxValue));
            Assert.AreEqual(long.Log2(0), Contract.Log2Long(0L));
            Assert.ThrowsException<TestException>(() => Contract.Log2Long(-1L));
        }

        [TestMethod]
        public void TestMethodShortLog2()
        {
            Assert.AreEqual(short.Log2((short)1), Contract.Log2Short((short)1));
            Assert.AreEqual(short.Log2((short)2), Contract.Log2Short((short)2));
            Assert.AreEqual(short.Log2(short.MaxValue), Contract.Log2Short(short.MaxValue));
            Assert.AreEqual(short.Log2(0), Contract.Log2Short((short)0));
            Assert.ThrowsException<TestException>(() => Contract.Log2Short((short)-1));
        }

        [TestMethod]
        public void TestMethodUshortLog2()
        {
            Assert.AreEqual(ushort.Log2((ushort)1), Contract.Log2Ushort((ushort)1));
            Assert.AreEqual(ushort.Log2((ushort)2), Contract.Log2Ushort((ushort)2));
            Assert.AreEqual(ushort.Log2(ushort.MaxValue), Contract.Log2Ushort(ushort.MaxValue));
            Assert.AreEqual(ushort.Log2(0), Contract.Log2Ushort((ushort)0));
        }

        [TestMethod]
        public void TestMethodByteLog2()
        {
            Assert.AreEqual(byte.Log2((byte)1), Contract.Log2Byte((byte)1));
            Assert.AreEqual(byte.Log2((byte)2), Contract.Log2Byte((byte)2));
            Assert.AreEqual(byte.Log2(byte.MaxValue), Contract.Log2Byte(byte.MaxValue));
            Assert.AreEqual(byte.Log2((byte)0), Contract.Log2Byte((byte)0));
        }

        [TestMethod]
        public void TestMethodSbyteLog2()
        {
            Assert.AreEqual(sbyte.Log2((sbyte)1), Contract.Log2Sbyte((sbyte)1));
            Assert.AreEqual(sbyte.Log2((sbyte)2), Contract.Log2Sbyte((sbyte)2));
            Assert.AreEqual(sbyte.Log2(sbyte.MaxValue), Contract.Log2Sbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.Log2((sbyte)0), Contract.Log2Sbyte((sbyte)0));
            Assert.ThrowsException<TestException>(() => Contract.Log2Sbyte((sbyte)-1));
        }

        [TestMethod]
        public void TestMethodLongIsPositive()
        {
            Assert.AreEqual(long.IsPositive(0L), Contract.IsPositiveLong(0L));
            Assert.AreEqual(long.IsPositive(1L), Contract.IsPositiveLong(1L));
            Assert.AreEqual(long.IsPositive(-1L), Contract.IsPositiveLong(-1L));
            Assert.AreEqual(long.IsPositive(long.MaxValue), Contract.IsPositiveLong(long.MaxValue));
            Assert.AreEqual(long.IsPositive(long.MinValue), Contract.IsPositiveLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodShortIsPositive()
        {
            Assert.AreEqual(short.IsPositive((short)0), Contract.IsPositiveShort((short)0));
            Assert.AreEqual(short.IsPositive((short)1), Contract.IsPositiveShort((short)1));
            Assert.AreEqual(short.IsPositive((short)-1), Contract.IsPositiveShort((short)-1));
            Assert.AreEqual(short.IsPositive(short.MaxValue), Contract.IsPositiveShort(short.MaxValue));
            Assert.AreEqual(short.IsPositive(short.MinValue), Contract.IsPositiveShort(short.MinValue));
        }

        [TestMethod]
        public void TestMethodSbyteIsPositive()
        {
            Assert.AreEqual(sbyte.IsPositive((sbyte)0), Contract.IsPositiveSbyte((sbyte)0));
            Assert.AreEqual(sbyte.IsPositive((sbyte)1), Contract.IsPositiveSbyte((sbyte)1));
            Assert.AreEqual(sbyte.IsPositive((sbyte)-1), Contract.IsPositiveSbyte((sbyte)-1));
            Assert.AreEqual(sbyte.IsPositive(sbyte.MaxValue), Contract.IsPositiveSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.IsPositive(sbyte.MinValue), Contract.IsPositiveSbyte(sbyte.MinValue));
        }

        // IsOddInteger tests for all types

        [TestMethod]
        public void TestMethodUIntIsOddInteger()
        {
            Assert.AreEqual(uint.IsOddInteger(0U), Contract.IsOddUInt(0U));
            Assert.AreEqual(uint.IsOddInteger(1U), Contract.IsOddUInt(1U));
            Assert.AreEqual(uint.IsOddInteger(2U), Contract.IsOddUInt(2U));
            Assert.AreEqual(uint.IsOddInteger(uint.MaxValue), Contract.IsOddUInt(uint.MaxValue));
        }

        [TestMethod]
        public void TestMethodLongIsOddInteger()
        {
            Assert.AreEqual(long.IsOddInteger(0L), Contract.IsOddLong(0L));
            Assert.AreEqual(long.IsOddInteger(1L), Contract.IsOddLong(1L));
            Assert.AreEqual(long.IsOddInteger(2L), Contract.IsOddLong(2L));
            Assert.AreEqual(long.IsOddInteger(long.MaxValue), Contract.IsOddLong(long.MaxValue));
            Assert.AreEqual(long.IsOddInteger(long.MinValue), Contract.IsOddLong(long.MinValue));
        }

        [TestMethod]
        public void TestMethodUlongIsOddInteger()
        {
            Assert.AreEqual(ulong.IsOddInteger(0UL), Contract.IsOddUlong(0UL));
            Assert.AreEqual(ulong.IsOddInteger(1UL), Contract.IsOddUlong(1UL));
            Assert.AreEqual(ulong.IsOddInteger(2UL), Contract.IsOddUlong(2UL));
            Assert.AreEqual(ulong.IsOddInteger(ulong.MaxValue), Contract.IsOddUlong(ulong.MaxValue));
        }

        [TestMethod]
        public void TestMethodShortIsOddInteger()
        {
            Assert.AreEqual(short.IsOddInteger((short)0), Contract.IsOddShort((short)0));
            Assert.AreEqual(short.IsOddInteger((short)1), Contract.IsOddShort((short)1));
            Assert.AreEqual(short.IsOddInteger((short)2), Contract.IsOddShort((short)2));
            Assert.AreEqual(short.IsOddInteger(short.MaxValue), Contract.IsOddShort(short.MaxValue));
            Assert.AreEqual(short.IsOddInteger(short.MinValue), Contract.IsOddShort(short.MinValue));
        }

        [TestMethod]
        public void TestMethodUshortIsOddInteger()
        {
            Assert.AreEqual(ushort.IsOddInteger((ushort)0), Contract.IsOddUshort((ushort)0));
            Assert.AreEqual(ushort.IsOddInteger((ushort)1), Contract.IsOddUshort((ushort)1));
            Assert.AreEqual(ushort.IsOddInteger((ushort)2), Contract.IsOddUshort((ushort)2));
            Assert.AreEqual(ushort.IsOddInteger(ushort.MaxValue), Contract.IsOddUshort(ushort.MaxValue));
        }

        [TestMethod]
        public void TestMethodByteIsOddInteger()
        {
            Assert.AreEqual(byte.IsOddInteger((byte)0), Contract.IsOddByte((byte)0));
            Assert.AreEqual(byte.IsOddInteger((byte)1), Contract.IsOddByte((byte)1));
            Assert.AreEqual(byte.IsOddInteger((byte)2), Contract.IsOddByte((byte)2));
            Assert.AreEqual(byte.IsOddInteger(byte.MaxValue), Contract.IsOddByte(byte.MaxValue));
        }

        [TestMethod]
        public void TestMethodSbyteIsOddInteger()
        {
            Assert.AreEqual(sbyte.IsOddInteger((sbyte)0), Contract.IsOddSbyte((sbyte)0));
            Assert.AreEqual(sbyte.IsOddInteger((sbyte)1), Contract.IsOddSbyte((sbyte)1));
            Assert.AreEqual(sbyte.IsOddInteger((sbyte)2), Contract.IsOddSbyte((sbyte)2));
            Assert.AreEqual(sbyte.IsOddInteger(sbyte.MaxValue), Contract.IsOddSbyte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.IsOddInteger(sbyte.MinValue), Contract.IsOddSbyte(sbyte.MinValue));
        }

        [TestMethod]
        public void TestRotateLeftByte()
        {
            Assert.AreEqual(byte.RotateLeft(1, 1), Contract.RotateLeftByte(1, 1));
            Assert.AreEqual(byte.RotateLeft(0x12, 4), Contract.RotateLeftByte(0x12, 4));
            Assert.AreEqual(byte.RotateLeft(byte.MaxValue, 1), Contract.RotateLeftByte(byte.MaxValue, 1));
            Assert.AreEqual(byte.RotateLeft(0x80, 1), Contract.RotateLeftByte(0x80, 1));
            Assert.AreEqual(byte.RotateLeft(0x01, 8), Contract.RotateLeftByte(0x01, 8));
            Assert.AreEqual(byte.RotateLeft(0x01, 9), Contract.RotateLeftByte(0x01, 9));
        }

        [TestMethod]
        public void TestRotateLeftSByte()
        {
            Assert.AreEqual(sbyte.RotateLeft((sbyte)1, 1), Contract.RotateLeftSByte((sbyte)1, 1));
            Assert.AreEqual(sbyte.RotateLeft((sbyte)0x12, 4), Contract.RotateLeftSByte((sbyte)0x12, 4));
            Assert.AreEqual(sbyte.RotateLeft(sbyte.MaxValue, 1), Contract.RotateLeftSByte(sbyte.MaxValue, 1));
            Assert.AreEqual(sbyte.RotateLeft(sbyte.MinValue, 1), Contract.RotateLeftSByte(sbyte.MinValue, 1));
            Assert.AreEqual(sbyte.RotateLeft((sbyte)0x01, 8), Contract.RotateLeftSByte((sbyte)0x01, 8));
            Assert.AreEqual(sbyte.RotateLeft((sbyte)0x01, 9), Contract.RotateLeftSByte((sbyte)0x01, 9));
        }

        [TestMethod]
        public void TestRotateLeftShort()
        {
            Assert.AreEqual(short.RotateLeft((short)1, 1), Contract.RotateLeftShort((short)1, 1));
            Assert.AreEqual(short.RotateLeft((short)0x1234, 4), Contract.RotateLeftShort((short)0x1234, 4));
            Assert.AreEqual(short.RotateLeft(short.MaxValue, 1), Contract.RotateLeftShort(short.MaxValue, 1));
            Assert.AreEqual(short.RotateLeft(short.MinValue, 1), Contract.RotateLeftShort(short.MinValue, 1));
            Assert.AreEqual(short.RotateLeft((short)0x0001, 16), Contract.RotateLeftShort((short)0x0001, 16));
            Assert.AreEqual(short.RotateLeft((short)0x0001, 17), Contract.RotateLeftShort((short)0x0001, 17));
        }

        [TestMethod]
        public void TestRotateLeftUShort()
        {
            Assert.AreEqual(ushort.RotateLeft((ushort)1, 1), Contract.RotateLeftUShort((ushort)1, 1));
            Assert.AreEqual(ushort.RotateLeft((ushort)0x1234, 4), Contract.RotateLeftUShort((ushort)0x1234, 4));
            Assert.AreEqual(ushort.RotateLeft(ushort.MaxValue, 1), Contract.RotateLeftUShort(ushort.MaxValue, 1));
            Assert.AreEqual(ushort.RotateLeft((ushort)0x8000, 1), Contract.RotateLeftUShort((ushort)0x8000, 1));
            Assert.AreEqual(ushort.RotateLeft((ushort)0x0001, 16), Contract.RotateLeftUShort((ushort)0x0001, 16));
            Assert.AreEqual(ushort.RotateLeft((ushort)0x0001, 17), Contract.RotateLeftUShort((ushort)0x0001, 17));
        }

        [TestMethod]
        public void TestRotateLeftInt()
        {
            Assert.AreEqual(int.RotateLeft(1, 1), Contract.RotateLeftInt(1, 1));
            Assert.AreEqual(int.RotateLeft(0x12345678, 4), Contract.RotateLeftInt(0x12345678, 4));
            Assert.AreEqual(int.RotateLeft(-1, 1), Contract.RotateLeftInt(-1, 1));
            Assert.AreEqual(int.RotateLeft(int.MaxValue, 1), Contract.RotateLeftInt(int.MaxValue, 1));
            Assert.AreEqual(int.RotateLeft(int.MinValue, 1), Contract.RotateLeftInt(int.MinValue, 1));
            Assert.AreEqual(int.RotateLeft(0x00000001, 32), Contract.RotateLeftInt(0x00000001, 32));
            Assert.AreEqual(int.RotateLeft(0x00000001, 33), Contract.RotateLeftInt(0x00000001, 33));
        }

        [TestMethod]
        public void TestRotateLeftUInt()
        {

            Assert.AreEqual(uint.RotateLeft(1U, 1), Contract.RotateLeftUInt(1U, 1));
            Assert.AreEqual(uint.RotateLeft(0x12345678U, 4), Contract.RotateLeftUInt(0x12345678U, 4));
            Assert.AreEqual(uint.RotateLeft(uint.MaxValue, 1), Contract.RotateLeftUInt(uint.MaxValue, 1));
            Assert.AreEqual(uint.RotateLeft(0x80000000U, 1), Contract.RotateLeftUInt(0x80000000U, 1));
            Assert.AreEqual(uint.RotateLeft(0x00000001U, 32), Contract.RotateLeftUInt(0x00000001U, 32));
            Assert.AreEqual(uint.RotateLeft(0x00000001U, 33), Contract.RotateLeftUInt(0x00000001U, 33));
        }

        [TestMethod]
        public void TestRotateLeftLong()
        {
            Assert.AreEqual(long.RotateLeft(1L, 1), Contract.RotateLeftLong(1L, 1));
            Assert.AreEqual(long.RotateLeft(0x123456789ABCDEFL, 4), Contract.RotateLeftLong(0x123456789ABCDEFL, 4));
            Assert.AreEqual(long.RotateLeft(-1L, 1), Contract.RotateLeftLong(-1L, 1));
            Assert.AreEqual(long.RotateLeft(long.MaxValue, 1), Contract.RotateLeftLong(long.MaxValue, 1));
            Assert.AreEqual(long.RotateLeft(long.MinValue, 1), Contract.RotateLeftLong(long.MinValue, 1));
            Assert.AreEqual(long.RotateLeft(0x0000000000000001L, 64), Contract.RotateLeftLong(0x0000000000000001L, 64));
            Assert.AreEqual(long.RotateLeft(0x0000000000000001L, 65), Contract.RotateLeftLong(0x0000000000000001L, 65));
        }

        [TestMethod]
        public void TestRotateLeftULong()
        {
            Assert.AreEqual(ulong.RotateLeft(1UL, 1), Contract.RotateLeftULong(1UL, 1));
            Assert.AreEqual(ulong.RotateLeft(0x123456789ABCDEFFUL, 4), Contract.RotateLeftULong(0x123456789ABCDEFFUL, 4));
            Assert.AreEqual(ulong.RotateLeft(ulong.MaxValue, 1), Contract.RotateLeftULong(ulong.MaxValue, 1));
            Assert.AreEqual(ulong.RotateLeft(0x8000000000000000UL, 1), Contract.RotateLeftULong(0x8000000000000000UL, 1));
            Assert.AreEqual(ulong.RotateLeft(0x0000000000000001UL, 64), Contract.RotateLeftULong(0x0000000000000001UL, 64));
            Assert.AreEqual(ulong.RotateLeft(0x0000000000000001UL, 65), Contract.RotateLeftULong(0x0000000000000001UL, 65));
        }

        [TestMethod]
        public void TestRotateRightByte()
        {
            Assert.AreEqual(byte.RotateRight(1, 1), Contract.RotateRightByte(1, 1));
            Assert.AreEqual(byte.RotateRight(0x12, 4), Contract.RotateRightByte(0x12, 4));
            Assert.AreEqual(byte.RotateRight(byte.MaxValue, 1), Contract.RotateRightByte(byte.MaxValue, 1));
            Assert.AreEqual(byte.RotateRight(0x01, 1), Contract.RotateRightByte(0x01, 1));
            Assert.AreEqual(byte.RotateRight(0x01, 8), Contract.RotateRightByte(0x01, 8));
            Assert.AreEqual(byte.RotateRight(0x01, 9), Contract.RotateRightByte(0x01, 9));
        }

        [TestMethod]
        public void TestRotateRightSByte()
        {
            Assert.AreEqual(sbyte.RotateRight((sbyte)1, 1), Contract.RotateRightSByte((sbyte)1, 1));
            Assert.AreEqual(sbyte.RotateRight((sbyte)0x12, 4), Contract.RotateRightSByte((sbyte)0x12, 4));
            Assert.AreEqual(sbyte.RotateRight(sbyte.MaxValue, 1), Contract.RotateRightSByte(sbyte.MaxValue, 1));
            Assert.AreEqual(sbyte.RotateRight(sbyte.MinValue, 1), Contract.RotateRightSByte(sbyte.MinValue, 1));
            Assert.AreEqual(sbyte.RotateRight((sbyte)0x01, 8), Contract.RotateRightSByte((sbyte)0x01, 8));
            Assert.AreEqual(sbyte.RotateRight((sbyte)0x01, 9), Contract.RotateRightSByte((sbyte)0x01, 9));
        }

        [TestMethod]
        public void TestRotateRightShort()
        {

            Assert.AreEqual(short.RotateRight((short)1, 1), Contract.RotateRightShort((short)1, 1));
            Assert.AreEqual(short.RotateRight((short)0x1234, 4), Contract.RotateRightShort((short)0x1234, 4));
            Assert.AreEqual(short.RotateRight(short.MaxValue, 1), Contract.RotateRightShort(short.MaxValue, 1));
            Assert.AreEqual(short.RotateRight(short.MinValue, 1), Contract.RotateRightShort(short.MinValue, 1));
            Assert.AreEqual(short.RotateRight((short)0x0001, 16), Contract.RotateRightShort((short)0x0001, 16));
            Assert.AreEqual(short.RotateRight((short)0x0001, 17), Contract.RotateRightShort((short)0x0001, 17));
        }

        [TestMethod]
        public void TestRotateRightUShort()
        {
            Assert.AreEqual(ushort.RotateRight((ushort)1, 1), Contract.RotateRightUShort((ushort)1, 1));
            Assert.AreEqual(ushort.RotateRight((ushort)0x1234, 4), Contract.RotateRightUShort((ushort)0x1234, 4));
            Assert.AreEqual(ushort.RotateRight(ushort.MaxValue, 1), Contract.RotateRightUShort(ushort.MaxValue, 1));
            Assert.AreEqual(ushort.RotateRight((ushort)0x0001, 1), Contract.RotateRightUShort((ushort)0x0001, 1));
            Assert.AreEqual(ushort.RotateRight((ushort)0x0001, 16), Contract.RotateRightUShort((ushort)0x0001, 16));
            Assert.AreEqual(ushort.RotateRight((ushort)0x0001, 17), Contract.RotateRightUShort((ushort)0x0001, 17));
        }

        [TestMethod]
        public void TestRotateRightInt()
        {

            Assert.AreEqual(int.RotateRight(1, 1), Contract.RotateRightInt(1, 1));
            Assert.AreEqual(int.RotateRight(0x12345678, 4), Contract.RotateRightInt(0x12345678, 4));
            Assert.AreEqual(int.RotateRight(-1, 1), Contract.RotateRightInt(-1, 1));
            Assert.AreEqual(int.RotateRight(int.MaxValue, 1), Contract.RotateRightInt(int.MaxValue, 1));
            Assert.AreEqual(int.RotateRight(int.MinValue, 1), Contract.RotateRightInt(int.MinValue, 1));
            Assert.AreEqual(int.RotateRight(0x00000001, 32), Contract.RotateRightInt(0x00000001, 32));
            Assert.AreEqual(int.RotateRight(0x00000001, 33), Contract.RotateRightInt(0x00000001, 33));
        }

        [TestMethod]
        public void TestRotateRightUInt()
        {
            Assert.AreEqual(uint.RotateRight(1U, 1), Contract.RotateRightUInt(1U, 1));
            Assert.AreEqual(uint.RotateRight(0x12345678U, 4), Contract.RotateRightUInt(0x12345678U, 4));
            Assert.AreEqual(uint.RotateRight(uint.MaxValue, 1), Contract.RotateRightUInt(uint.MaxValue, 1));
            Assert.AreEqual(uint.RotateRight(0x00000001U, 1), Contract.RotateRightUInt(0x00000001U, 1));
            Assert.AreEqual(uint.RotateRight(0x00000001U, 32), Contract.RotateRightUInt(0x00000001U, 32));
            Assert.AreEqual(uint.RotateRight(0x00000001U, 33), Contract.RotateRightUInt(0x00000001U, 33));
        }

        [TestMethod]
        public void TestRotateRightLong()
        {
            Assert.AreEqual(long.RotateRight(1L, 1), Contract.RotateRightLong(1L, 1));
            Assert.AreEqual(long.RotateRight(0x123456789ABCDEFL, 4), Contract.RotateRightLong(0x123456789ABCDEFL, 4));
            Assert.AreEqual(long.RotateRight(-1L, 1), Contract.RotateRightLong(-1L, 1));
            Assert.AreEqual(long.RotateRight(long.MaxValue, 1), Contract.RotateRightLong(long.MaxValue, 1));
            Assert.AreEqual(long.RotateRight(long.MinValue, 1), Contract.RotateRightLong(long.MinValue, 1));
            Assert.AreEqual(long.RotateRight(0x0000000000000001L, 64), Contract.RotateRightLong(0x0000000000000001L, 64));
            Assert.AreEqual(long.RotateRight(0x0000000000000001L, 65), Contract.RotateRightLong(0x0000000000000001L, 65));
        }
        [TestMethod]
        public void TestRotateRightULong()
        {
            Assert.AreEqual(ulong.RotateRight(1UL, 1), Contract.RotateRightULong(1UL, 1));
            Assert.AreEqual(ulong.RotateRight(0x123456789ABCDEFFUL, 4), Contract.RotateRightULong(0x123456789ABCDEFFUL, 4));
            Assert.AreEqual(ulong.RotateRight(ulong.MaxValue, 1), Contract.RotateRightULong(ulong.MaxValue, 1));
            Assert.AreEqual(ulong.RotateRight(0x0000000000000001UL, 1), Contract.RotateRightULong(0x0000000000000001UL, 1));
            Assert.AreEqual(ulong.RotateRight(0x0000000000000001UL, 64), Contract.RotateRightULong(0x0000000000000001UL, 64));
            Assert.AreEqual(ulong.RotateRight(0x0000000000000001UL, 65), Contract.RotateRightULong(0x0000000000000001UL, 65));
        }

        [TestMethod]
        public void TestPopCountByte()
        {
            Assert.AreEqual(byte.PopCount((byte)0), Contract.PopCountByte((byte)0));
            Assert.AreEqual(byte.PopCount((byte)1), Contract.PopCountByte((byte)1));
            Assert.AreEqual(byte.PopCount((byte)255), Contract.PopCountByte((byte)255));
            Assert.AreEqual(byte.PopCount((byte)170), Contract.PopCountByte((byte)170));
            Assert.AreEqual(byte.PopCount((byte)85), Contract.PopCountByte((byte)85));
        }

        [TestMethod]
        public void TestPopCountSByte()
        {
            Assert.AreEqual(sbyte.PopCount((sbyte)0), Contract.PopCountSByte((sbyte)0));
            Assert.AreEqual(sbyte.PopCount((sbyte)1), Contract.PopCountSByte((sbyte)1));
            Assert.AreEqual(sbyte.PopCount((sbyte)-1), Contract.PopCountSByte((sbyte)-1));
            Assert.AreEqual(sbyte.PopCount(sbyte.MaxValue), Contract.PopCountSByte(sbyte.MaxValue));
            Assert.AreEqual(sbyte.PopCount(sbyte.MinValue), Contract.PopCountSByte(sbyte.MinValue));
        }

        [TestMethod]
        public void TestPopCountShort()
        {

            Assert.AreEqual(short.PopCount((short)0), Contract.PopCountShort((short)0));
            Assert.AreEqual(short.PopCount((short)1), Contract.PopCountShort((short)1));
            Assert.AreEqual(short.PopCount((short)-1), Contract.PopCountShort((short)-1));
            Assert.AreEqual(short.PopCount(short.MaxValue), Contract.PopCountShort(short.MaxValue));
            Assert.AreEqual(short.PopCount(short.MinValue), Contract.PopCountShort(short.MinValue));
        }

        [TestMethod]
        public void TestPopCountUShort()
        {
            Assert.AreEqual(ushort.PopCount((ushort)0), Contract.PopCountUShort((ushort)0));
            Assert.AreEqual(ushort.PopCount((ushort)1), Contract.PopCountUShort((ushort)1));
            Assert.AreEqual(ushort.PopCount(ushort.MaxValue), Contract.PopCountUShort(ushort.MaxValue));
            Assert.AreEqual(ushort.PopCount((ushort)43690), Contract.PopCountUShort((ushort)43690));
            Assert.AreEqual(ushort.PopCount((ushort)21845), Contract.PopCountUShort((ushort)21845));
        }

        [TestMethod]
        public void TestPopCountInt()
        {
            Assert.AreEqual(int.PopCount(0), Contract.PopCountInt(0));
            Assert.AreEqual(int.PopCount(1), Contract.PopCountInt(1));
            Assert.AreEqual(int.PopCount(-1), Contract.PopCountInt(-1));
            Assert.AreEqual(int.PopCount(int.MaxValue), Contract.PopCountInt(int.MaxValue));
            Assert.AreEqual(int.PopCount(int.MinValue), Contract.PopCountInt(int.MinValue));
        }

        [TestMethod]
        public void TestPopCountUInt()
        {
            Assert.AreEqual(uint.PopCount(0U), Contract.PopCountUInt(0U));
            Assert.AreEqual(uint.PopCount(1U), Contract.PopCountUInt(1U));
            Assert.AreEqual(uint.PopCount(uint.MaxValue), Contract.PopCountUInt(uint.MaxValue));
            Assert.AreEqual(uint.PopCount(2863311530U), Contract.PopCountUInt(2863311530U));
            Assert.AreEqual(uint.PopCount(1431655765U), Contract.PopCountUInt(1431655765U));
        }

        [TestMethod]
        public void TestPopCountLong()
        {
            Assert.AreEqual(long.PopCount(0L), Contract.PopCountLong(0L));
            Assert.AreEqual(long.PopCount(1L), Contract.PopCountLong(1L));
            Assert.AreEqual(long.PopCount(-1L), Contract.PopCountLong(-1L));
            Assert.AreEqual(long.PopCount(long.MaxValue), Contract.PopCountLong(long.MaxValue));
            Assert.AreEqual(long.PopCount(long.MinValue), Contract.PopCountLong(long.MinValue));
        }

        [TestMethod]
        public void TestPopCountULong()
        {
            Assert.AreEqual(ulong.PopCount(0UL), Contract.PopCountULong(0UL));
            Assert.AreEqual(ulong.PopCount(1UL), Contract.PopCountULong(1UL));
            Assert.AreEqual(ulong.PopCount(ulong.MaxValue), Contract.PopCountULong(ulong.MaxValue));
            Assert.AreEqual(ulong.PopCount(12297829382473034410UL), Contract.PopCountULong(12297829382473034410UL));
            Assert.AreEqual(ulong.PopCount(6148914691236517205UL), Contract.PopCountULong(6148914691236517205UL));
        }

        [TestMethod]
        public void TestPopCountBigInteger()
        {
            Assert.AreEqual(BigInteger.PopCount(BigInteger.Zero), Contract.PopCountBigInteger(BigInteger.Zero));
            Assert.AreEqual(BigInteger.PopCount(BigInteger.One), Contract.PopCountBigInteger(BigInteger.One));
            Assert.AreEqual(BigInteger.PopCount(BigInteger.MinusOne), Contract.PopCountBigInteger(BigInteger.MinusOne));
            Assert.ThrowsException<TestException>(() => Contract.PopCountBigInteger(BigInteger.Parse("123456789123456789")));
            Assert.ThrowsException<TestException>(() => Contract.PopCountBigInteger(BigInteger.Parse("-987654321987654321")));
        }

        [TestMethod]
        public void TestMethodBigIntgerIsPow2()
        {
            for (int i = -2; i <= 4; ++i)
                Assert.AreEqual(new BigInteger(i).IsPowerOfTwo, Contract.IsPow2BigInteger(i));
        }
    }
}
