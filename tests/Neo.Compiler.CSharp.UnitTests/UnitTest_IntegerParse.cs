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
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(sbyte.MinValue), Contract.TestSbyteparse("-128"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("128"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("-129"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(byte.MinValue), Contract.TestByteparse("0"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(byte.MaxValue), Contract.TestByteparse("255"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("-1"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("256"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(ushort.MinValue), Contract.TestUshortparse("0"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(ushort.MaxValue), Contract.TestUshortparse("65535"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("-1"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("65536"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(short.MinValue), Contract.TestShortparse("-32768"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(short.MaxValue), Contract.TestShortparse("32767"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("-32769"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("32768"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            Assert.AreEqual(new BigInteger(ulong.MinValue), Contract.TestUlongparse("0"));
            Assert.AreEqual(2032740, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(ulong.MaxValue), Contract.TestUlongparse("18446744073709551615"));
            Assert.AreEqual(2032740, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("-1"));
            Assert.AreEqual(2048100, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("18446744073709551616"));
            Assert.AreEqual(2048100, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void LongParse_Test()
        {
            Assert.AreEqual(new BigInteger(long.MinValue), Contract.TestLongparse("-9223372036854775808"));
            Assert.AreEqual(2032740, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(long.MaxValue), Contract.TestLongparse("9223372036854775807"));
            Assert.AreEqual(2032740, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("-9223372036854775809"));
            Assert.AreEqual(2048100, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("9223372036854775808"));
            Assert.AreEqual(2048100, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            Assert.AreEqual(new BigInteger(uint.MinValue), Contract.TestUintparse("0"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(uint.MaxValue), Contract.TestUintparse("4294967295"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("-1"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("4294967296"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void IntParse_Test()
        {
            Assert.AreEqual(new BigInteger(int.MinValue), Contract.TestIntparse("-2147483648"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(int.MaxValue), Contract.TestIntparse("2147483647"));
            Assert.AreEqual(2032650, Engine.FeeConsumed.Value);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(" 20 "));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("-2147483649"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("2147483648"));
            Assert.AreEqual(2048010, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(""));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("abc"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("@"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
        }
    }
}
