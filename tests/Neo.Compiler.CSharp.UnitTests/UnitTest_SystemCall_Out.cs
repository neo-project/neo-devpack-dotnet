// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SystemCall_Out.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

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
            AssertGasConsumed(2172570);

            res = Contract.TestByteTryParse("256");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2172600);

            res = Contract.TestByteTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2167110);

            AssertTryParseFailure(Contract.TestByteTryParse("abc"));
            AssertGasConsumed(1181010);

            // Edge cases
            res = Contract.TestByteTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166930);

            res = Contract.TestByteTryParse("255");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)255, res[1]);
            AssertGasConsumed(2172570);

            AssertTryParseFailure(Contract.TestByteTryParse(" 123 "));
            AssertGasConsumed(1181010);
        }

        [TestMethod]
        public void TestSByteTryParse()
        {
            var res = Contract.TestSByteTryParse("100");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)100, res[1]);
            AssertGasConsumed(2172420);

            res = Contract.TestSByteTryParse("-128");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-128), res[1]);
            AssertGasConsumed(2172570);

            res = Contract.TestSByteTryParse("128");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2172450);

            AssertTryParseFailure(Contract.TestSByteTryParse("abc"));
            AssertGasConsumed(1181010);

            // Edge cases
            res = Contract.TestSByteTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166780);

            res = Contract.TestSByteTryParse("127");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)127, res[1]);
            AssertGasConsumed(2172420);

            res = Contract.TestSByteTryParse("-129");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2172600);
        }

        [TestMethod]
        public void TestShortTryParse()
        {
            var res = Contract.TestShortTryParse("32000");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)32000, res[1]);
            AssertGasConsumed(2178060);

            res = Contract.TestShortTryParse("-32768");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-32768), res[1]);
            AssertGasConsumed(2178210);

            res = Contract.TestShortTryParse("32768");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2178090);

            AssertTryParseFailure(Contract.TestShortTryParse("abc"));
            AssertGasConsumed(1181010);

            // Edge cases
            res = Contract.TestShortTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166780);

            res = Contract.TestShortTryParse("32767");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)32767, res[1]);
            AssertGasConsumed(2178060);

            res = Contract.TestShortTryParse("-32769");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2178240);
        }

        [TestMethod]
        public void TestUShortTryParse()
        {
            var res = Contract.TestUShortTryParse("65000");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)65000, res[1]);
            AssertGasConsumed(2178210);

            res = Contract.TestUShortTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166930);

            res = Contract.TestUShortTryParse("65536");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2178240);

            res = Contract.TestUShortTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2167110);

            // Edge cases
            res = Contract.TestUShortTryParse("65535");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)65535, res[1]);
            AssertGasConsumed(2178210);

            AssertTryParseFailure(Contract.TestUShortTryParse("1.5"));
            AssertGasConsumed(1183830);

            AssertTryParseFailure(Contract.TestUShortTryParse("0x1234"));
            AssertGasConsumed(1183830);
        }

        [TestMethod]
        public void TestIntTryParse()
        {
            var res = Contract.TestIntTryParse("2147483647");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)2147483647, res[1]);
            AssertGasConsumed(2192160);

            res = Contract.TestIntTryParse("-2147483648");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-2147483648), res[1]);
            AssertGasConsumed(2192310);

            res = Contract.TestIntTryParse("2147483648");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2192190);

            AssertTryParseFailure(Contract.TestIntTryParse("abc"));
            AssertGasConsumed(1181010);

            // Edge cases
            res = Contract.TestIntTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166780);

            res = Contract.TestIntTryParse("-0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166930);

            AssertTryParseFailure(Contract.TestIntTryParse("2147483647.5"));
            AssertGasConsumed(1209210);
        }

        [TestMethod]
        public void TestUIntTryParse()
        {
            var res = Contract.TestUIntTryParse("4294967295");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)4294967295, res[1]);
            AssertGasConsumed(2192310);

            res = Contract.TestUIntTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166930);

            res = Contract.TestUIntTryParse("4294967296");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2192340);

            res = Contract.TestUIntTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2167110);

            // Edge cases
            res = Contract.TestUIntTryParse("4294967294");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)4294967294, res[1]);
            AssertGasConsumed(2192310);

            res = Contract.TestUIntTryParse("+1");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)1, res[1]);
            AssertGasConsumed(2167110);

            res = Contract.TestUIntTryParse("00123");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)123, res[1]);
            AssertGasConsumed(2112600);
        }

        [TestMethod]
        public void TestLongTryParse()
        {
            var res = Contract.TestLongTryParse("9223372036854775807");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)9223372036854775807, res[1]);
            AssertGasConsumed(2217540);

            res = Contract.TestLongTryParse("-9223372036854775808");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)(-9223372036854775808), res[1]);
            AssertGasConsumed(2217690);

            res = Contract.TestLongTryParse("9223372036854775808");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2217570);

            AssertTryParseFailure(Contract.TestLongTryParse("abc"));
            AssertGasConsumed(1181010);

            // Edge cases
            res = Contract.TestLongTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166780);

            res = Contract.TestLongTryParse("-0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2166930);

            res = Contract.TestLongTryParse("9223372036854775806");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)9223372036854775806, res[1]);
            AssertGasConsumed(2217540);
        }

        [TestMethod]
        public void TestULongTryParse()
        {
            var res = Contract.TestULongTryParse("18446744073709551615");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)18446744073709551615, res[1]);
            AssertGasConsumed(2220600);

            res = Contract.TestULongTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2167020);

            res = Contract.TestULongTryParse("18446744073709551616");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2220630);

            res = Contract.TestULongTryParse("-1");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
            AssertGasConsumed(2167200);

            // Edge cases
            res = Contract.TestULongTryParse("18446744073709551614");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)18446744073709551614, res[1]);
            AssertGasConsumed(2220600);

            res = Contract.TestULongTryParse("+1");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)1, res[1]);
            AssertGasConsumed(2167200);

            res = Contract.TestULongTryParse("000000000000000001");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.AreEqual((BigInteger)1, res[1]);
            AssertGasConsumed(2214960);
        }

        [TestMethod]
        public void TestBoolTryParse_DotNetStyleLiterals()
        {
            var res = Contract.TestBoolTryParse("true");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1177050);

            res = Contract.TestBoolTryParse("false");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1178310);

            res = Contract.TestBoolTryParse("True");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1181010);

            res = Contract.TestBoolTryParse("False");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1192830);
        }

        [TestMethod]
        public void TestBoolTryParse_ExtendedNeoVmLiterals()
        {
            var res = Contract.TestBoolTryParse("1");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1184970);

            res = Contract.TestBoolTryParse("0");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1196790);

            // Edge cases
            res = Contract.TestBoolTryParse("TRUE");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1179690);

            res = Contract.TestBoolTryParse("FALSE");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1191510);

            res = Contract.TestBoolTryParse("yes");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1186290);

            res = Contract.TestBoolTryParse("no");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1198110);

            res = Contract.TestBoolTryParse("t");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1182330);

            res = Contract.TestBoolTryParse("f");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1194150);

            res = Contract.TestBoolTryParse("Y");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsTrue((bool)res[1]);
            AssertGasConsumed(1190250);

            res = Contract.TestBoolTryParse("N");
            Assert.IsNotNull(res);
            Assert.IsTrue((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1202070);
        }

        [TestMethod]
        [DataRow("yes", true)]
        [DataRow("no", false)]
        [DataRow("1", true)]
        [DataRow("0", false)]
        public void TestBoolTryParse_DiffersFromDotNetForExtendedLiterals(string value, bool expected)
        {
            Assert.IsFalse(bool.TryParse(value, out _));
            var result = Contract.TestBoolTryParse(value);
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result[0]);
            Assert.AreEqual(expected, (bool)result[1]);
        }

        [TestMethod]
        public void TestBoolTryParse_InvalidInputsRemainRejected()
        {
            var res = Contract.TestBoolTryParse(" true ");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1202130);

            res = Contract.TestBoolTryParse("invalid");
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.IsFalse((bool)res[1]);
            AssertGasConsumed(1202130);
        }

        private static void AssertTryParseFailure(IList<object>? res)
        {
            Assert.IsNotNull(res);
            Assert.IsFalse((bool)res[0]);
            Assert.AreEqual((BigInteger)0, res[1]);
        }
    }
}
