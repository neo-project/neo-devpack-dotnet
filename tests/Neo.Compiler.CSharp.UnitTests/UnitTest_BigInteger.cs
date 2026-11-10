// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_BigInteger.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BigInteger : DebugAndTestBase<Contract_BigInteger>
    {
        [TestMethod]
        public void Test_ParseConstant()
        {
            Assert.AreEqual(BigInteger.Parse("100000000000000000000000000"), Contract.ParseConstant());
        }

        [TestMethod]
        public void Test_Pow()
        {
            Assert.AreEqual(8, Contract.TestPow(2, 3));
        }

        [TestMethod]
        public void Test_Sqrt()
        {
            Assert.AreEqual(2, Contract.TestSqrt(4));
        }

        [TestMethod]
        public void Test_Sbyte()
        {
            Assert.AreEqual(127, Contract.Testsbyte(127));
            Assert.AreEqual(-128, Contract.Testsbyte(-128));
            Assert.ThrowsExactly<TestException>(() => Contract.Testsbyte(128));
            Assert.ThrowsExactly<TestException>(() => Contract.Testsbyte(-129));
        }

        [TestMethod]
        public void Test_byte()
        {
            Assert.AreEqual(0, Contract.Testbyte(0));
            Assert.AreEqual(255, Contract.Testbyte(255));
            Assert.ThrowsExactly<TestException>(() => Contract.Testbyte(-1));
            Assert.ThrowsExactly<TestException>(() => Contract.Testbyte(256));
        }

        [TestMethod]
        public void Test_short()
        {
            Assert.AreEqual(32767, Contract.Testshort(32767));
            Assert.AreEqual(-32768, Contract.Testshort(-32768));
            Assert.ThrowsExactly<TestException>(() => Contract.Testshort(32768));
            Assert.ThrowsExactly<TestException>(() => Contract.Testshort(-32769));
        }

        [TestMethod]
        public void Test_ushort()
        {
            Assert.AreEqual(0, Contract.Testushort(0));
            Assert.AreEqual(65535, Contract.Testushort(65535));
            Assert.ThrowsExactly<TestException>(() => Contract.Testushort(-1));
            Assert.ThrowsExactly<TestException>(() => Contract.Testushort(65536));
        }

        [TestMethod]
        public void Test_int()
        {
            Assert.AreEqual(-2147483648, Contract.Testint(-2147483648));
            Assert.AreEqual(2147483647, Contract.Testint(2147483647));
            Assert.ThrowsExactly<TestException>(() => Contract.Testint(-2147483649));
            Assert.ThrowsExactly<TestException>(() => Contract.Testint(2147483648));
        }

        [TestMethod]
        public void Test_uint()
        {
            Assert.AreEqual(0, Contract.Testuint(0));
            Assert.AreEqual(4294967295, Contract.Testuint(4294967295));
            Assert.ThrowsExactly<TestException>(() => Contract.Testuint(-1));
            Assert.ThrowsExactly<TestException>(() => Contract.Testuint(4294967296));
        }

        [TestMethod]
        public void Test_long()
        {
            Assert.AreEqual(-9223372036854775808, Contract.Testlong(-9223372036854775808));
            Assert.AreEqual(9223372036854775807, Contract.Testlong(9223372036854775807));
            Assert.ThrowsExactly<TestException>(() => Contract.Testlong(BigInteger.Parse("-9223372036854775809")));
            Assert.ThrowsExactly<TestException>(() => Contract.Testlong(9223372036854775808));
        }

        [TestMethod]
        public void Test_ulong()
        {
            Assert.AreEqual(0, Contract.Testulong(0));
            Assert.AreEqual(18446744073709551615, Contract.Testulong(18446744073709551615));
            Assert.ThrowsExactly<TestException>(() => Contract.Testulong(BigInteger.Parse("18446744073709551616")));
            Assert.ThrowsExactly<TestException>(() => Contract.Testulong(-1));
        }

        [TestMethod]
        public void Test_char()
        {
            Assert.AreEqual(0, Contract.Testchar(0));
            Assert.AreEqual(65535, Contract.Testchar(char.MaxValue));
            Assert.ThrowsExactly<TestException>(() => Contract.Testchar(-1));
            Assert.ThrowsExactly<TestException>(() => Contract.Testchar(65536));

            Assert.AreEqual(char.MaxValue.ToString(), Contract.Testchartostring(char.MaxValue));
            Assert.AreEqual("A", Contract.Testchartostring('A'));
        }

        [TestMethod]
        public void Test_IsEven()
        {
            for (int i = -2; i <= 2; ++i)
            {
                Assert.AreEqual(new BigInteger(i).IsEven, Contract.TestIsEven(i));
            }
        }

        [TestMethod]
        public void Test_IsEvenIsOdd_Parity()
        {
            var max = BigInteger.Pow(2, 255) - 1;
            var min = -BigInteger.Pow(2, 255);
            var values = new List<BigInteger>
            {
                0, 1, -1, 2, -2, 3, -3, 4, -4, 255, -255, 256, -256,
                long.MaxValue, long.MinValue, (BigInteger)long.MaxValue + 1, (BigInteger)long.MinValue - 1,
                (BigInteger)ulong.MaxValue, (BigInteger)ulong.MaxValue + 1,
                max, max - 1, min, min + 1
            };
            for (int n = 1; n < 255; n++)
            {
                var p = BigInteger.Pow(2, n);
                values.Add(p); values.Add(-p);
                values.Add(p - 1); values.Add(-(p - 1));
                values.Add(p + 1); values.Add(-(p + 1));
            }

            foreach (var v in values.Distinct())
            {
                bool even = v.IsEven;
                Assert.AreEqual(even, Contract.TestIsEven(v), $"IsEven {v}");
                Assert.AreEqual(!even, Contract.TestIsOdd(v), $"IsOdd {v}");
                Assert.AreEqual(v % 2 == 0, Contract.TestModIsEven(v), $"ModIsEven {v}");
                Assert.AreEqual((v & BigInteger.One) == 0, Contract.TestAndIsEven(v), $"AndIsEven {v}");
                Assert.AreEqual(v % 2 != 0, Contract.TestModIsOdd(v), $"ModIsOdd {v}");
                Assert.AreEqual((v & BigInteger.One) != 0, Contract.TestAndIsOdd(v), $"AndIsOdd {v}");
                Assert.AreNotEqual(Contract.TestIsEven(v), Contract.TestIsOdd(v));
            }
        }

        [TestMethod]
        public void Test_Add()
        {
            Assert.AreEqual(new BigInteger(1111111110), Contract.TestAdd(123456789, 987654321));
        }

        [TestMethod]
        public void Test_Subtract()
        {
            Assert.AreEqual(new BigInteger(-864197532), Contract.TestSubtract(123456789, 987654321));
        }

        [TestMethod]
        public void Test_Multiply()
        {
            Assert.AreEqual(new BigInteger(39483), Contract.TestMultiply(123, 321));
        }

        [TestMethod]
        public void Test_Divide()
        {
            Assert.AreEqual(BigInteger.Divide(123456, 123), Contract.TestDivide(123456, 123));
        }

        [TestMethod]
        public void Test_Negate()
        {
            Assert.AreEqual(new BigInteger(-123456), Contract.TestNegate(123456));
        }

        [TestMethod]
        public void Test_Remainder()
        {
            Assert.AreEqual(BigInteger.Remainder(123456, 123), Contract.TestRemainder(123456, 123));
        }

        [TestMethod]
        public void Test_Compare()
        {
            Assert.AreEqual(BigInteger.Compare(123, 321), Contract.TestCompare(123, 321));
            Assert.AreEqual(BigInteger.Compare(123, 123), Contract.TestCompare(123, 123));
            Assert.AreEqual(BigInteger.Compare(123, -321), Contract.TestCompare(123, -321));
            Assert.AreEqual(BigInteger.Compare(-123, 321), Contract.TestCompare(-123, 321));
            Assert.AreEqual(BigInteger.Compare(-123, -321), Contract.TestCompare(-123, -321));
            Assert.AreEqual(BigInteger.Compare(long.MaxValue, 1), Contract.TestCompare(long.MaxValue, 1));
            Assert.AreEqual(BigInteger.Compare(long.MinValue, 1), Contract.TestCompare(long.MinValue, 1));
            Assert.AreEqual(BigInteger.Compare(long.MaxValue, -1), Contract.TestCompare(long.MaxValue, -1));
            Assert.AreEqual(BigInteger.Compare(long.MinValue, -1), Contract.TestCompare(long.MinValue, -1));
        }

        [TestMethod]
        public void Test_GreatestCommonDivisor()
        {
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(48, 18), Contract.TestGreatestCommonDivisor(48, 18));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(18, 48), Contract.TestGreatestCommonDivisor(18, 48));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(-48, -18), Contract.TestGreatestCommonDivisor(-48, -18));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(24, 12), Contract.TestGreatestCommonDivisor(24, 12));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(24, 0), Contract.TestGreatestCommonDivisor(24, 0));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(-24, 0), Contract.TestGreatestCommonDivisor(-24, 0));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(0, -24), Contract.TestGreatestCommonDivisor(0, -24));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(0, 24), Contract.TestGreatestCommonDivisor(0, 24));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(0, 0), Contract.TestGreatestCommonDivisor(0, 0));

            // Coprime numbers
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(17, 13), Contract.TestGreatestCommonDivisor(17, 13));
            Assert.AreEqual(BigInteger.One, Contract.TestGreatestCommonDivisor(17, 13));

            // Equal arguments
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(42, 42), Contract.TestGreatestCommonDivisor(42, 42));
            Assert.AreEqual((BigInteger)42, Contract.TestGreatestCommonDivisor(42, 42));

            // BigInteger.One
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(BigInteger.One, BigInteger.One), Contract.TestGreatestCommonDivisor(BigInteger.One, BigInteger.One));
            Assert.AreEqual(BigInteger.One, Contract.TestGreatestCommonDivisor(BigInteger.One, BigInteger.One));

            // Second argument already zero: only the initial comparison is executed
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(10, 0), Contract.TestGreatestCommonDivisor(10, 0));
            Assert.AreEqual((BigInteger)10, Contract.TestGreatestCommonDivisor(10, 0));

            // Second argument initially non-zero
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(0, 10), Contract.TestGreatestCommonDivisor(0, 10));
            Assert.AreEqual((BigInteger)10, Contract.TestGreatestCommonDivisor(0, 10));
        }

        // New test methods
        [TestMethod]
        public void Test_CharToString()
        {
            Assert.AreEqual("A", Contract.Testchartostring(65)); // 'A' has ASCII value 65
            Assert.ThrowsExactly<TestException>(() => Contract.Testchartostring(65536)); // Invalid char value
        }

        [TestMethod]
        public void Test_Equals()
        {
            Assert.IsTrue(Contract.TestEquals(123, 123));
            Assert.IsFalse(Contract.TestEquals(123, 321));
        }

        [TestMethod]
        public void Test_IsOne()
        {
            Assert.IsTrue(Contract.TestIsOne(1));
            Assert.IsFalse(Contract.TestIsOne(0));
            Assert.IsFalse(Contract.TestIsOne(-1));
        }

        [TestMethod]
        public void Test_IsZero()
        {
            Assert.IsTrue(Contract.TestIsZero(0));
            Assert.IsFalse(Contract.TestIsZero(1));
            Assert.IsFalse(Contract.TestIsZero(-1));
        }

        [TestMethod]
        public void Test_ModPow()
        {
            // Test with number = 10, exponent = 3, modulus = 30
            Assert.AreEqual(BigInteger.ModPow(10, 3, 30), Contract.TestModPow());
        }

        [TestMethod]
        public void Test_Sign()
        {
            Assert.AreEqual(1, Contract.TestSign(10)); // Positive number
            Assert.AreEqual(0, Contract.TestSign(0)); // Zero
            Assert.AreEqual(-1, Contract.TestSign(-10)); // Negative number
        }
    }
}
