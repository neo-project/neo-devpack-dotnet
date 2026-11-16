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

            Assert.IsTrue(Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), Encoding.UTF8.GetBytes("")));

            Assert.IsFalse(Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), Encoding.UTF8.GetBytes("W")));

            Assert.IsFalse(Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), Encoding.UTF8.GetBytes("Hello!")));

            Assert.ThrowsExactly<TestException>(() => Contract.TestStartsWith(Encoding.UTF8.GetBytes("Hello"), null));

            Assert.ThrowsExactly<TestException>(() => Contract.TestStartsWith(null, Encoding.UTF8.GetBytes("")));
        }

        [TestMethod]
        public void TestIndexOf()
        {
            Assert.AreEqual(4, Contract.TestIndexOf());
        }

        [TestMethod]
        public void TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith());

            Assert.IsTrue(Contract.TestEndsWith(Encoding.UTF8.GetBytes("Hello World")));

            Assert.IsTrue(Contract.TestEndsWith(Encoding.UTF8.GetBytes("World")));

            Assert.IsFalse(Contract.TestEndsWith(Encoding.UTF8.GetBytes("orld")));

            Assert.ThrowsExactly<TestException>(() => Contract.TestEndsWith(null));
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Contract.TestContains());
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            Assert.IsTrue(Contract.TestNumberOnly());

            Assert.IsFalse(Contract.TestNumberRejectsNonDigit());
        }

        [TestMethod]
        public void TestAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestAlphabetOnly());

            Assert.IsFalse(Contract.TestAlphabetRejectsNumber());
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

            Assert.IsFalse(Contract.TestLowerAlphabetRejectsUpper());
        }

        [TestMethod]
        public void TestUpperAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestUpperAlphabetOnly());

            Assert.IsFalse(Contract.TestUpperAlphabetRejectsLower());
        }
    }
}
