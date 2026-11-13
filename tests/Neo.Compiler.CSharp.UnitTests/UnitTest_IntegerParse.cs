// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_IntegerParse.cs file belongs to the neo project and is free
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
            Assert.AreEqual(new BigInteger(sbyte.MinValue), Contract.TestSbyteparse("-128"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse("128"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse("-129"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestSbyteparse("@"));
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(byte.MinValue), Contract.TestByteparse("0"));
            Assert.AreEqual(new BigInteger(byte.MaxValue), Contract.TestByteparse("255"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse("-1"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse("256"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestByteparse("@"));
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(ushort.MinValue), Contract.TestUshortparse("0"));
            Assert.AreEqual(new BigInteger(ushort.MaxValue), Contract.TestUshortparse("65535"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse("-1"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse("65536"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUshortparse("@"));
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(short.MinValue), Contract.TestShortparse("-32768"));
            Assert.AreEqual(new BigInteger(short.MaxValue), Contract.TestShortparse("32767"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse("-32769"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse("32768"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestShortparse("@"));
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            Assert.AreEqual(new BigInteger(ulong.MinValue), Contract.TestUlongparse("0"));
            Assert.AreEqual(new BigInteger(ulong.MaxValue), Contract.TestUlongparse("18446744073709551615"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse("-1"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse("18446744073709551616"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUlongparse("@"));
        }

        [TestMethod]
        public void LongParse_Test()
        {
            Assert.AreEqual(new BigInteger(long.MinValue), Contract.TestLongparse("-9223372036854775808"));
            Assert.AreEqual(new BigInteger(long.MaxValue), Contract.TestLongparse("9223372036854775807"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse("-9223372036854775809"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse("9223372036854775808"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestLongparse("@"));
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            Assert.AreEqual(new BigInteger(uint.MinValue), Contract.TestUintparse("0"));
            Assert.AreEqual(new BigInteger(uint.MaxValue), Contract.TestUintparse("4294967295"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse("-1"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse("4294967296"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestUintparse("@"));
        }

        [TestMethod]
        public void IntParse_Test()
        {
            Assert.AreEqual(new BigInteger(int.MinValue), Contract.TestIntparse("-2147483648"));
            Assert.AreEqual(new BigInteger(int.MaxValue), Contract.TestIntparse("2147483647"));

            //test backspace trip
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse("20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse(" 20 "));
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse("-2147483649"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse("2147483648"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse(""));
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse("abc"));
            Assert.ThrowsExactly<TestException>(() => Contract.TestIntparse("@"));
        }
    }
}
