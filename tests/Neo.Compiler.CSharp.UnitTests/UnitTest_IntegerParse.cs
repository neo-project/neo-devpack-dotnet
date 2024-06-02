using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IntegerParse : TestBase<Contract_IntegerParse>
    {
        public UnitTest_IntegerParse() : base(Contract_IntegerParse.Nef, Contract_IntegerParse.Manifest) { }

        [TestMethod]
        public void SByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(sbyte.MaxValue), Contract.TestSbyteparse("127"));
            Assert.AreEqual(new BigInteger(sbyte.MinValue), Contract.TestSbyteparse("-128"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("128"));
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("-129"));
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("@"));
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(byte.MinValue), Contract.TestByteparse("0"));
            Assert.AreEqual(new BigInteger(byte.MaxValue), Contract.TestByteparse("255"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("-1"));
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("256"));
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("@"));
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(ushort.MinValue), Contract.TestUshortparse("0"));
            Assert.AreEqual(new BigInteger(ushort.MaxValue), Contract.TestUshortparse("65535"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("-1"));
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("65536"));
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("@"));
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(short.MinValue), Contract.TestShortparse("-32768"));
            Assert.AreEqual(new BigInteger(short.MaxValue), Contract.TestShortparse("32767"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("-32769"));
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("32768"));
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("@"));
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            Assert.AreEqual(new BigInteger(ulong.MinValue), Contract.TestUlongparse("0"));
            Assert.AreEqual(new BigInteger(ulong.MaxValue), Contract.TestUlongparse("18446744073709551615"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("-1"));
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("18446744073709551616"));
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("@"));
        }

        [TestMethod]
        public void LongParse_Test()
        {
            Assert.AreEqual(new BigInteger(long.MinValue), Contract.TestLongparse("-9223372036854775808"));
            Assert.AreEqual(new BigInteger(long.MaxValue), Contract.TestLongparse("9223372036854775807"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("-9223372036854775809"));
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("9223372036854775808"));
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("@"));
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            Assert.AreEqual(new BigInteger(uint.MinValue), Contract.TestUintparse("0"));
            Assert.AreEqual(new BigInteger(uint.MaxValue), Contract.TestUintparse("4294967295"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("-1"));
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("4294967296"));
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("@"));
        }

        [TestMethod]
        public void IntParse_Test()
        {
            Assert.AreEqual(new BigInteger(int.MinValue), Contract.TestIntparse("-2147483648"));
            Assert.AreEqual(new BigInteger(int.MaxValue), Contract.TestIntparse("2147483647"));

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(" 20 "));
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("-2147483649"));
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("2147483648"));
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(""));
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("abc"));
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("@"));
        }
    }
}
