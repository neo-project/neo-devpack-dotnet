using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_SystemCall_Out : DebugAndTestBase<Contract_Syscall_Out>
    {
        [TestMethod]
        public void TestByteTryParse()
        {
            var res = Contract.TestByteTryParse("123");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)123, res[1]);

            res = Contract.TestByteTryParse("256");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestByteTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestByteTryParse("abc"));

            // Edge cases
            res = Contract.TestByteTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestByteTryParse("255");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)255, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestByteTryParse(" 123 "));
        }

        [TestMethod]
        public void TestSByteTryParse()
        {
            var res = Contract.TestSByteTryParse("100");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)100, res[1]);

            res = Contract.TestSByteTryParse("-128");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-128), res[1]);

            res = Contract.TestSByteTryParse("128");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestSByteTryParse("abc"));

            // Edge cases
            res = Contract.TestSByteTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestSByteTryParse("127");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)127, res[1]);

            res = Contract.TestSByteTryParse("-129");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
        }

        [TestMethod]
        public void TestShortTryParse()
        {
            var res = Contract.TestShortTryParse("32000");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)32000, res[1]);

            res = Contract.TestShortTryParse("-32768");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-32768), res[1]);

            res = Contract.TestShortTryParse("32768");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestShortTryParse("abc"));

            // Edge cases
            res = Contract.TestShortTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestShortTryParse("32767");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)32767, res[1]);

            res = Contract.TestShortTryParse("-32769");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
        }

        [TestMethod]
        public void TestUShortTryParse()
        {
            var res = Contract.TestUShortTryParse("65000");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)65000, res[1]);

            res = Contract.TestUShortTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestUShortTryParse("65536");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestUShortTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            // Edge cases
            res = Contract.TestUShortTryParse("65535");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)65535, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestUShortTryParse("1.5"));

            Assert.ThrowsException<TestException>(() => Contract.TestUShortTryParse("0x1234"));
        }

        [TestMethod]
        public void TestIntTryParse()
        {
            var res = Contract.TestIntTryParse("2147483647");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)2147483647, res[1]);

            res = Contract.TestIntTryParse("-2147483648");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-2147483648), res[1]);

            res = Contract.TestIntTryParse("2147483648");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestIntTryParse("abc"));

            // Edge cases
            res = Contract.TestIntTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestIntTryParse("-0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestIntTryParse("2147483647.5"));
        }

        [TestMethod]
        public void TestUIntTryParse()
        {
            var res = Contract.TestUIntTryParse("4294967295");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)4294967295, res[1]);

            res = Contract.TestUIntTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestUIntTryParse("4294967296");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestUIntTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            // Edge cases
            res = Contract.TestUIntTryParse("4294967294");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)4294967294, res[1]);

            res = Contract.TestUIntTryParse("+1");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)1, res[1]);

            res = Contract.TestUIntTryParse("00123");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)123, res[1]);
        }

        [TestMethod]
        public void TestLongTryParse()
        {
            var res = Contract.TestLongTryParse("9223372036854775807");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)9223372036854775807, res[1]);

            res = Contract.TestLongTryParse("-9223372036854775808");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-9223372036854775808), res[1]);

            res = Contract.TestLongTryParse("9223372036854775808");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            Assert.ThrowsException<TestException>(() => Contract.TestLongTryParse("abc"));

            // Edge cases
            res = Contract.TestLongTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestLongTryParse("-0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestLongTryParse("9223372036854775806");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)9223372036854775806, res[1]);
        }

        [TestMethod]
        public void TestULongTryParse()
        {
            var res = Contract.TestULongTryParse("18446744073709551615");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)18446744073709551615, res[1]);

            res = Contract.TestULongTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestULongTryParse("18446744073709551616");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            res = Contract.TestULongTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);

            // Edge cases
            res = Contract.TestULongTryParse("18446744073709551614");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)18446744073709551614, res[1]);

            res = Contract.TestULongTryParse("+1");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)1, res[1]);

            res = Contract.TestULongTryParse("000000000000000001");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)1, res[1]);
        }

        [TestMethod]
        public void TestBoolTryParse()
        {
            var res = Contract.TestBoolTryParse("true");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("false");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse("True");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("False");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse("1");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            // Edge cases
            res = Contract.TestBoolTryParse("TRUE");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("FALSE");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse(" true ");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse("yes");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("no");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse("t");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("f");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse("Y");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);

            res = Contract.TestBoolTryParse("N");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);

            res = Contract.TestBoolTryParse("invalid");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
        }
    }
}
