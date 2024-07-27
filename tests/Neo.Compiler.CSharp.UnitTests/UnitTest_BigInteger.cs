using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BigInteger : TestBase<Contract_BigInteger>
    {
        public UnitTest_BigInteger() : base(Contract_BigInteger.Nef, Contract_BigInteger.Manifest) { }

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
            Assert.ThrowsException<TestException>(() => Contract.Testsbyte(128));
            Assert.ThrowsException<TestException>(() => Contract.Testsbyte(-129));
        }

        [TestMethod]
        public void Test_byte()
        {
            Assert.AreEqual(0, Contract.Testbyte(0));
            Assert.AreEqual(255, Contract.Testbyte(255));
            Assert.ThrowsException<TestException>(() => Contract.Testbyte(-1));
            Assert.ThrowsException<TestException>(() => Contract.Testbyte(256));
        }

        [TestMethod]
        public void Test_short()
        {
            Assert.AreEqual(32767, Contract.Testshort(32767));
            Assert.AreEqual(-32768, Contract.Testshort(-32768));
            Assert.ThrowsException<TestException>(() => Contract.Testshort(32768));
            Assert.ThrowsException<TestException>(() => Contract.Testshort(-32769));
        }

        [TestMethod]
        public void Test_ushort()
        {
            Assert.AreEqual(0, Contract.Testushort(0));
            Assert.AreEqual(65535, Contract.Testushort(65535));
            Assert.ThrowsException<TestException>(() => Contract.Testushort(-1));
            Assert.ThrowsException<TestException>(() => Contract.Testushort(65536));
        }

        [TestMethod]
        public void Test_int()
        {
            Assert.AreEqual(-2147483648, Contract.Testint(-2147483648));
            Assert.AreEqual(2147483647, Contract.Testint(2147483647));
            Assert.ThrowsException<TestException>(() => Contract.Testint(-2147483649));
            Assert.ThrowsException<TestException>(() => Contract.Testint(2147483648));
        }

        [TestMethod]
        public void Test_uint()
        {
            Assert.AreEqual(0, Contract.Testuint(0));
            Assert.AreEqual(4294967295, Contract.Testuint(4294967295));
            Assert.ThrowsException<TestException>(() => Contract.Testuint(-1));
            Assert.ThrowsException<TestException>(() => Contract.Testuint(4294967296));
        }

        [TestMethod]
        public void Test_long()
        {
            Assert.AreEqual(-9223372036854775808, Contract.Testlong(-9223372036854775808));
            Assert.AreEqual(9223372036854775807, Contract.Testlong(9223372036854775807));
            Assert.ThrowsException<TestException>(() => Contract.Testlong(BigInteger.Parse("-9223372036854775809")));
            Assert.ThrowsException<TestException>(() => Contract.Testlong(9223372036854775808));
        }

        [TestMethod]
        public void Test_ulong()
        {
            Assert.AreEqual(0, Contract.Testulong(0));
            Assert.AreEqual(18446744073709551615, Contract.Testulong(18446744073709551615));
            Assert.ThrowsException<TestException>(() => Contract.Testulong(BigInteger.Parse("18446744073709551616")));
            Assert.ThrowsException<TestException>(() => Contract.Testulong(-1));
        }

        [TestMethod]
        public void Test_char()
        {
            Assert.AreEqual(0, Contract.Testchar(0));
            Assert.AreEqual(65535, Contract.Testchar(char.MaxValue));
            Assert.ThrowsException<TestException>(() => Contract.Testchar(-1));
            Assert.ThrowsException<TestException>(() => Contract.Testchar(65536));

            // char.MaxValue is not a UTF-8 character, thus can not convert to string in neo
            Assert.ThrowsException<DecoderFallbackException>(() => Contract.Testchartostring(char.MaxValue));
            Assert.AreEqual("A", Contract.Testchartostring('A'));
        }

        [TestMethod]
        public void Test_IsEven()
        {
            // Test 0
            Assert.AreEqual(new BigInteger(0).IsEven, Contract.TestIsEven(0));
            // Test 1
            Assert.AreEqual(new BigInteger(1).IsEven, Contract.TestIsEven(1));
            // Test 2
            Assert.AreEqual(new BigInteger(2).IsEven, Contract.TestIsEven(2));
            // Test -1
            Assert.AreEqual(new BigInteger(-1).IsEven, Contract.TestIsEven(-1));
            // Test -2
            Assert.AreEqual(new BigInteger(-2).IsEven, Contract.TestIsEven(-2));
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
        }

        [TestMethod]
        public void Test_GreatestCommonDivisor()
        {
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(48, 18), Contract.TestGreatestCommonDivisor(48, 18));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(-48, -18), Contract.TestGreatestCommonDivisor(-48, -18));
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(24, 12), Contract.TestGreatestCommonDivisor(24, 12));
        }
    }
}
