using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BigInteger : DebugAndTestBase<Contract_BigInteger>
    {
        [TestMethod]
        public void Test_ParseConstant()
        {
            Assert.AreEqual(BigInteger.Parse("100000000000000000000000000"), Contract.ParseConstant());
            AssertGasConsumed(984150);
        }

        [TestMethod]
        public void Test_Pow()
        {
            Assert.AreEqual(8, Contract.TestPow(2, 3));
            AssertGasConsumed(1049040);
        }

        [TestMethod]
        public void Test_Sqrt()
        {
            Assert.AreEqual(2, Contract.TestSqrt(4));
            AssertGasConsumed(1048950);
        }

        [TestMethod]
        public void Test_Sbyte()
        {
            Assert.AreEqual(127, Contract.Testsbyte(127));
            AssertGasConsumed(1047810);
            Assert.AreEqual(-128, Contract.Testsbyte(-128));
            AssertGasConsumed(1047810);
            Assert.ThrowsException<TestException>(() => Contract.Testsbyte(128));
            AssertGasConsumed(1078590);
            Assert.ThrowsException<TestException>(() => Contract.Testsbyte(-129));
            AssertGasConsumed(1078590);
        }

        [TestMethod]
        public void Test_byte()
        {
            Assert.AreEqual(0, Contract.Testbyte(0));
            AssertGasConsumed(1047810);
            Assert.AreEqual(255, Contract.Testbyte(255));
            AssertGasConsumed(1047810);
            Assert.ThrowsException<TestException>(() => Contract.Testbyte(-1));
            AssertGasConsumed(1078590);
            Assert.ThrowsException<TestException>(() => Contract.Testbyte(256));
            AssertGasConsumed(1078590);
        }

        [TestMethod]
        public void Test_short()
        {
            Assert.AreEqual(32767, Contract.Testshort(32767));
            AssertGasConsumed(1047810);
            Assert.AreEqual(-32768, Contract.Testshort(-32768));
            AssertGasConsumed(1047810);
            Assert.ThrowsException<TestException>(() => Contract.Testshort(32768));
            AssertGasConsumed(1078590);
            Assert.ThrowsException<TestException>(() => Contract.Testshort(-32769));
            AssertGasConsumed(1078590);
        }

        [TestMethod]
        public void Test_ushort()
        {
            Assert.AreEqual(0, Contract.Testushort(0));
            AssertGasConsumed(1047810);
            Assert.AreEqual(65535, Contract.Testushort(65535));
            AssertGasConsumed(1047810);
            Assert.ThrowsException<TestException>(() => Contract.Testushort(-1));
            AssertGasConsumed(1078590);
            Assert.ThrowsException<TestException>(() => Contract.Testushort(65536));
            AssertGasConsumed(1078590);
        }

        [TestMethod]
        public void Test_int()
        {
            Assert.AreEqual(-2147483648, Contract.Testint(-2147483648));
            AssertGasConsumed(1047810);
            Assert.AreEqual(2147483647, Contract.Testint(2147483647));
            AssertGasConsumed(1047810);
            Assert.ThrowsException<TestException>(() => Contract.Testint(-2147483649));
            AssertGasConsumed(1078590);
            Assert.ThrowsException<TestException>(() => Contract.Testint(2147483648));
            AssertGasConsumed(1078590);
        }

        [TestMethod]
        public void Test_uint()
        {
            Assert.AreEqual(0, Contract.Testuint(0));
            AssertGasConsumed(1047810);
            Assert.AreEqual(4294967295, Contract.Testuint(4294967295));
            AssertGasConsumed(1047810);
            Assert.ThrowsException<TestException>(() => Contract.Testuint(-1));
            AssertGasConsumed(1078590);
            Assert.ThrowsException<TestException>(() => Contract.Testuint(4294967296));
            AssertGasConsumed(1078590);
        }

        [TestMethod]
        public void Test_long()
        {
            Assert.AreEqual(-9223372036854775808, Contract.Testlong(-9223372036854775808));
            AssertGasConsumed(1047900);
            Assert.AreEqual(9223372036854775807, Contract.Testlong(9223372036854775807));
            AssertGasConsumed(1047900);
            Assert.ThrowsException<TestException>(() => Contract.Testlong(BigInteger.Parse("-9223372036854775809")));
            AssertGasConsumed(1078770);
            Assert.ThrowsException<TestException>(() => Contract.Testlong(9223372036854775808));
            AssertGasConsumed(1078770);
        }

        [TestMethod]
        public void Test_ulong()
        {
            Assert.AreEqual(0, Contract.Testulong(0));
            AssertGasConsumed(1047900);
            Assert.AreEqual(18446744073709551615, Contract.Testulong(18446744073709551615));
            AssertGasConsumed(1047990);
            Assert.ThrowsException<TestException>(() => Contract.Testulong(BigInteger.Parse("18446744073709551616")));
            AssertGasConsumed(1078770);
            Assert.ThrowsException<TestException>(() => Contract.Testulong(-1));
            AssertGasConsumed(1078680);
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
            for (int i = -2; i <= 2; ++i)
            {
                Assert.AreEqual(new BigInteger(i).IsEven, Contract.TestIsEven(i));
                AssertGasConsumed(1047420);
            }
        }

        [TestMethod]
        public void Test_Add()
        {
            Assert.AreEqual(new BigInteger(1111111110), Contract.TestAdd(123456789, 987654321));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void Test_Subtract()
        {
            Assert.AreEqual(new BigInteger(-864197532), Contract.TestSubtract(123456789, 987654321));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void Test_Multiply()
        {
            Assert.AreEqual(new BigInteger(39483), Contract.TestMultiply(123, 321));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void Test_Divide()
        {
            Assert.AreEqual(BigInteger.Divide(123456, 123), Contract.TestDivide(123456, 123));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void Test_Negate()
        {
            Assert.AreEqual(new BigInteger(-123456), Contract.TestNegate(123456));
            AssertGasConsumed(1047150);
        }

        [TestMethod]
        public void Test_Remainder()
        {
            Assert.AreEqual(BigInteger.Remainder(123456, 123), Contract.TestRemainder(123456, 123));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void Test_Compare()
        {
            Assert.AreEqual(BigInteger.Compare(123, 321), Contract.TestCompare(123, 321));
            AssertGasConsumed(1047480);
            Assert.AreEqual(BigInteger.Compare(123, 123), Contract.TestCompare(123, 123));
            AssertGasConsumed(1047480);
            Assert.AreEqual(BigInteger.Compare(123, -321), Contract.TestCompare(123, -321));
            AssertGasConsumed(1047480);
        }

        [TestMethod]
        public void Test_GreatestCommonDivisor()
        {
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(48, 18), Contract.TestGreatestCommonDivisor(48, 18));
            AssertGasConsumed(1049280);
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(-48, -18), Contract.TestGreatestCommonDivisor(-48, -18));
            AssertGasConsumed(1049280);
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(24, 12), Contract.TestGreatestCommonDivisor(24, 12));
            AssertGasConsumed(1047960);
        }

        // New test methods
        [TestMethod]
        public void Test_CharToString()
        {
            Assert.AreEqual("A", Contract.Testchartostring(65)); // 'A' has ASCII value 65
            Assert.ThrowsException<TestException>(() => Contract.Testchartostring(65536)); // Invalid char value
        }

        [TestMethod]
        public void Test_Equals()
        {
            Assert.IsTrue(Contract.TestEquals(123, 123));
            Assert.IsFalse(Contract.TestEquals(123, 321));
            AssertGasConsumed(1047360);
        }

        [TestMethod]
        public void Test_IsOne()
        {
            Assert.IsTrue(Contract.TestIsOne(1));
            Assert.IsFalse(Contract.TestIsOne(0));
            Assert.IsFalse(Contract.TestIsOne(-1));
            AssertGasConsumed(1047300);
        }

        [TestMethod]
        public void Test_IsZero()
        {
            Assert.IsTrue(Contract.TestIsZero(0));
            Assert.IsFalse(Contract.TestIsZero(1));
            Assert.IsFalse(Contract.TestIsZero(-1));
            AssertGasConsumed(1047270);
        }

        [TestMethod]
        public void Test_ModPow()
        {
            // Test with number = 10, exponent = 3, modulus = 30
            Assert.AreEqual(BigInteger.ModPow(10, 3, 30), Contract.TestModPow());
            AssertGasConsumed(1047840);
        }

        [TestMethod]
        public void Test_Sign()
        {
            Assert.AreEqual(1, Contract.TestSign(10)); // Positive number
            Assert.AreEqual(0, Contract.TestSign(0)); // Zero
            Assert.AreEqual(-1, Contract.TestSign(-10)); // Negative number
            AssertGasConsumed(1047150);
        }
    }
}
