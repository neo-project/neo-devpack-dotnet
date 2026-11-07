// Copyright (C) 2015-2026 The Neo Project.
//
// RegexTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class RegexTest : DebugAndTestBase<Contract_Regex>
    {
        [TestMethod]
        public void TestStartWith()
        {
            Assert.IsTrue(Contract.TestStartWith());
            AssertGasConsumed(1310910);

            Assert.IsTrue(Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), Encoding.UTF8.GetBytes("")));
            AssertGasConsumed(1373940);

            Assert.IsFalse(Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), Encoding.UTF8.GetBytes("W")));
            AssertGasConsumed(1373940);

            Assert.IsFalse(Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), Encoding.UTF8.GetBytes("Hello!")));
            AssertGasConsumed(1065630);

            Assert.ThrowsExactly<TestException>(() => Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), null));
            AssertGasConsumed(1064790);

            Assert.ThrowsExactly<TestException>(() => Contract.TestStartsWith(null, Encoding.UTF8.GetBytes("")));
            AssertGasConsumed(1065090);
        }

        [TestMethod]
        public void TestIndexOf()
        {
            Assert.AreEqual(4, Contract.TestIndexOf());
            AssertGasConsumed(1986900);
        }

        [TestMethod]
        public void TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith());
            AssertGasConsumed(1310910);

            Assert.IsTrue(Contract.TestEndsWith(Encoding.UTF8.GetBytes("Hello World")));
            AssertGasConsumed(1373880);

            Assert.IsTrue(Contract.TestEndsWith(Encoding.UTF8.GetBytes("World")));
            AssertGasConsumed(1373880);

            Assert.IsFalse(Contract.TestEndsWith(Encoding.UTF8.GetBytes("orld")));
            AssertGasConsumed(1065570);

            Assert.ThrowsExactly<TestException>(() => Contract.TestEndsWith(null));
            AssertGasConsumed(1065030);
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Contract.TestContains());
            AssertGasConsumed(1987170);
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            Assert.IsTrue(Contract.TestNumberOnly());
            AssertGasConsumed(1033830);

            Assert.IsFalse(Contract.TestNumberRejectsNonDigit());
            AssertGasConsumed(1036350);
        }

        [TestMethod]
        public void TestAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestAlphabetOnly());
            AssertGasConsumed(1181430);

            Assert.IsFalse(Contract.TestAlphabetRejectsNumber());
            AssertGasConsumed(1025520);
        }

        [TestMethod]
        public void TestAlphabetAllByteValues()
        {
            for (int b = 0; b < 256; b++)
            {
                bool expected = (b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z');
                Assert.AreEqual(expected, Contract.TestAlphabetInput(new byte[] { (byte)b }), $"byte {b}");
            }
        }

        [TestMethod]
        public void TestAlphabetInputs()
        {
            Assert.IsTrue(Contract.TestAlphabetInput(new byte[0]));
            Assert.IsTrue(Contract.TestAlphabetInput(System.Text.Encoding.ASCII.GetBytes("AZaz")));
            Assert.IsTrue(Contract.TestAlphabetInput(System.Text.Encoding.ASCII.GetBytes("HeLLo")));
            Assert.IsFalse(Contract.TestAlphabetInput(System.Text.Encoding.ASCII.GetBytes("Hello World")));
            Assert.IsFalse(Contract.TestAlphabetInput(System.Text.Encoding.ASCII.GetBytes("abc1")));
            Assert.IsFalse(Contract.TestAlphabetInput(System.Text.Encoding.ASCII.GetBytes("@[`{")));
            Assert.IsFalse(Contract.TestAlphabetInput(new byte[] { 0x41, 0x00 }));
            Assert.IsFalse(Contract.TestAlphabetInput(new byte[] { 0x61, 0xC3, 0xA9 }));
            Assert.IsFalse(Contract.TestAlphabetInput(new byte[] { 0xC1 }));
            Assert.IsFalse(Contract.TestAlphabetInput(new byte[] { 0xE1 }));
        }

        [TestMethod]
        public void TestLowerAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestLowerAlphabetOnly());
            AssertGasConsumed(1084710);

            Assert.IsFalse(Contract.TestLowerAlphabetRejectsUpper());
            AssertGasConsumed(1014090);
        }

        [TestMethod]
        public void TestUpperAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestUpperAlphabetOnly());
            AssertGasConsumed(1084710);

            Assert.IsFalse(Contract.TestUpperAlphabetRejectsLower());
            AssertGasConsumed(1014090);
        }
    }
}
