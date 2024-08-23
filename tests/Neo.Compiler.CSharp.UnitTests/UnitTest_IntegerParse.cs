using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IntegerParse : DebugAndTestBase<Contract_IntegerParse>
    {
        [TestMethod]
        public void SByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(sbyte.MaxValue), Contract.TestSbyteparse("127"));
            AssertGasConsumed(2032650);
            Assert.AreEqual(new BigInteger(sbyte.MinValue), Contract.TestSbyteparse("-128"));
            AssertGasConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("128"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("-129"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(byte.MinValue), Contract.TestByteparse("0"));
            AssertGasConsumed(2032650);
            Assert.AreEqual(new BigInteger(byte.MaxValue), Contract.TestByteparse("255"));
            AssertGasConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("-1"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("256"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(ushort.MinValue), Contract.TestUshortparse("0"));
            AssertGasConsumed(2032650);
            Assert.AreEqual(new BigInteger(ushort.MaxValue), Contract.TestUshortparse("65535"));
            AssertGasConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("-1"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("65536"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(short.MinValue), Contract.TestShortparse("-32768"));
            AssertGasConsumed(2032650);
            Assert.AreEqual(new BigInteger(short.MaxValue), Contract.TestShortparse("32767"));
            AssertGasConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("-32769"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("32768"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            Assert.AreEqual(new BigInteger(ulong.MinValue), Contract.TestUlongparse("0"));
            AssertGasConsumed(2032740);
            Assert.AreEqual(new BigInteger(ulong.MaxValue), Contract.TestUlongparse("18446744073709551615"));
            AssertGasConsumed(2032740);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("-1"));
            AssertGasConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("18446744073709551616"));
            AssertGasConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void LongParse_Test()
        {
            Assert.AreEqual(new BigInteger(long.MinValue), Contract.TestLongparse("-9223372036854775808"));
            AssertGasConsumed(2032740);
            Assert.AreEqual(new BigInteger(long.MaxValue), Contract.TestLongparse("9223372036854775807"));
            AssertGasConsumed(2032740);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("-9223372036854775809"));
            AssertGasConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("9223372036854775808"));
            AssertGasConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            Assert.AreEqual(new BigInteger(uint.MinValue), Contract.TestUintparse("0"));
            AssertGasConsumed(2032650);
            Assert.AreEqual(new BigInteger(uint.MaxValue), Contract.TestUintparse("4294967295"));
            AssertGasConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("-1"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("4294967296"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("@"));
            AssertGasConsumed(2032230);
        }

        [TestMethod]
        public void IntParse_Test()
        {
            Assert.AreEqual(new BigInteger(int.MinValue), Contract.TestIntparse("-2147483648"));
            AssertGasConsumed(2032650);
            Assert.AreEqual(new BigInteger(int.MaxValue), Contract.TestIntparse("2147483647"));
            AssertGasConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(" 20 "));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("-2147483649"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("2147483648"));
            AssertGasConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(""));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("abc"));
            AssertGasConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("@"));
            AssertGasConsumed(2032230);
        }
    }
}
