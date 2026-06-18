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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class RegexTest : DebugAndTestBase<Contract_Regex>
    {
        [TestMethod]
        public void TestStartWith()
        {
            Assert.IsTrue(Contract.TestStartWith());
            AssertGasConsumed(1987140);
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
            AssertGasConsumed(1989840);
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Contract.TestContains());
            AssertGasConsumed(1987890);
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            Assert.IsTrue(Contract.TestNumberOnly());
            AssertGasConsumed(1044270);
            Assert.IsFalse(Contract.TestNumberRejectsNonDigit());
            AssertGasConsumed(1047270);
        }

        [TestMethod]
        public void TestAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestAlphabetOnly());
            AssertGasConsumed(1238610);
            Assert.IsFalse(Contract.TestAlphabetRejectsNumber());
            AssertGasConsumed(1032720);
        }

        [TestMethod]
        public void TestLowerAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestLowerAlphabetOnly());
            AssertGasConsumed(1111470);
            Assert.IsFalse(Contract.TestLowerAlphabetRejectsUpper());
            AssertGasConsumed(1017630);
        }

        [TestMethod]
        public void TestUpperAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestUpperAlphabetOnly());
            AssertGasConsumed(1111470);
            Assert.IsFalse(Contract.TestUpperAlphabetRejectsLower());
            AssertGasConsumed(1017870);
        }
    }
}
