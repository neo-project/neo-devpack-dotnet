// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Char.cs file belongs to the neo project and is free
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
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Char : DebugAndTestBase<Contract_Char>
    {
        protected override bool TestGasConsume => false;

        public static IEnumerable<object[]> CharTestData =>
            new List<object[]>
            {
                new object[] { '0', true, false, false, false, false },
                new object[] { '9', true, false, false, false, false },
                new object[] { 'a', false, true, false, true, false },
                new object[] { 'Z', false, true, false, false, true },
                new object[] { ' ', false, false, true, false, false },
                new object[] { '\t', false, false, true, false, false },
                new object[] { '$', false, false, false, false, false },
                new object[] { '\n', false, false, true, false, false },
                new object[] { '\x1f', false, false, false, false, false },
            };

        [TestMethod]
        [DynamicData(nameof(CharTestData))]
        public void TestCharProperties(char c, bool isDigit, bool isLetter, bool isWhiteSpace, bool isLower, bool isUpper)
        {
            Assert.AreEqual(isDigit, Contract.TestCharIsDigit(c), $"IsDigit failed for '{c}'");
            AssertGasConsumed(1047330);
            Assert.AreEqual(isLetter, Contract.TestCharIsLetter(c), $"IsLetter failed for '{c}'");
            AssertGasConsumed(1047990);
            Assert.AreEqual(isWhiteSpace, Contract.TestCharIsWhiteSpace(c), $"IsWhiteSpace failed for '{c}'");
            Assert.AreEqual(isLower, Contract.TestCharIsLower(c), $"IsLower failed for '{c}'");
            AssertGasConsumed(1047330);
            Assert.AreEqual(isUpper, Contract.TestCharIsUpper(c), $"IsUpper failed for '{c}'");
            AssertGasConsumed(1047330);
        }

        [TestMethod]
        public void TestCharGetNumericValue()
        {
            Assert.AreEqual(0, Contract.TestCharGetNumericValue('0'));
            AssertGasConsumed(1047720);
            Assert.AreEqual(9, Contract.TestCharGetNumericValue('9'));
            AssertGasConsumed(1047720);
            Assert.AreEqual(-1, Contract.TestCharGetNumericValue('a'));
            AssertGasConsumed(1047540);
            Assert.AreEqual(-1, Contract.TestCharGetNumericValue('$'));
            AssertGasConsumed(1047540);
        }

        [TestMethod]
        public void TestCharSpecialCategories()
        {
            Assert.IsTrue(Contract.TestCharIsPunctuation('.'));
            AssertGasConsumed(1047450);
            Assert.IsTrue(Contract.TestCharIsPunctuation(','));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsPunctuation('a'));
            AssertGasConsumed(1048590);

            Assert.IsTrue(Contract.TestCharIsSymbol('$'));
            AssertGasConsumed(1047450);
            Assert.IsTrue(Contract.TestCharIsSymbol('+'));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsSymbol('a'));
            AssertGasConsumed(1049010);

            Assert.IsTrue(Contract.TestCharIsControl('\n'));
            AssertGasConsumed(1047990);
            Assert.IsTrue(Contract.TestCharIsControl('\0'));
            AssertGasConsumed(1047990);
            Assert.IsFalse(Contract.TestCharIsControl('a'));
            AssertGasConsumed(1047990);
        }

        [TestMethod]
        public void TestCharSurrogates()
        {
            Assert.IsTrue(Contract.TestCharIsSurrogate('\uD800'));
            AssertGasConsumed(1047990);
            Assert.IsTrue(Contract.TestCharIsSurrogate('\uDFFF'));
            AssertGasConsumed(1047990);
            Assert.IsFalse(Contract.TestCharIsSurrogate('a'));
            AssertGasConsumed(1047990);

            Assert.IsTrue(Contract.TestCharIsHighSurrogate('\uD800'));
            AssertGasConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsHighSurrogate('\uDC00'));
            AssertGasConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsHighSurrogate('a'));
            AssertGasConsumed(1047330);

            Assert.IsTrue(Contract.TestCharIsLowSurrogate('\uDC00'));
            AssertGasConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsLowSurrogate('\uD800'));
            AssertGasConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsLowSurrogate('a'));
            AssertGasConsumed(1047330);
        }

        [TestMethod]
        public void TestCharConversions()
        {
            Assert.AreEqual('A', Contract.TestCharToUpper('a'));
            AssertGasConsumed(1047990);
            Assert.AreEqual('A', Contract.TestCharToUpper('A'));
            AssertGasConsumed(1047450);
            Assert.AreEqual('a', Contract.TestCharToLower('A'));
            AssertGasConsumed(1047990);
            Assert.AreEqual('a', Contract.TestCharToLower('a'));
            AssertGasConsumed(1047450);
        }

        [TestMethod]
        public void TestCharIsLetterOrDigit()
        {
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('a'));
            AssertGasConsumed(1048170);
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('A'));
            AssertGasConsumed(1047870);
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('0'));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsLetterOrDigit('$'));
            AssertGasConsumed(1048170);
        }

        [TestMethod]
        public void TestCharIsBetween()
        {
            Assert.IsTrue(Contract.TestCharIsBetween('a', 'a', 'z'));
            AssertGasConsumed(1047870);
            Assert.IsTrue(Contract.TestCharIsBetween('z', 'a', 'z'));
            AssertGasConsumed(1047870);

            Assert.IsFalse(Contract.TestCharIsBetween('A', 'a', 'z'));
            AssertGasConsumed(1047990);
            Assert.IsFalse(Contract.TestCharIsBetween('0', 'a', 'z'));
            AssertGasConsumed(1047990);
        }

        [TestMethod]
        public void TestCharParseAndTryParse()
        {
            Assert.AreEqual('A', Contract.TestCharParse("A"));
            Assert.AreEqual('0', Contract.TestCharParse("0"));
            Assert.ThrowsException<TestException>(() => Contract.TestCharParse("TooLong"));
            Assert.ThrowsException<TestException>(() => Contract.TestCharParse(string.Empty));

            var result = Contract.TestCharTryParse("Z");
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result![0]);
            Assert.AreEqual((BigInteger)'Z', result[1]);

            result = Contract.TestCharTryParse("long");
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)result![0]);
            Assert.AreEqual((BigInteger)0, result[1]);

            result = Contract.TestCharTryParse(string.Empty);
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)result![0]);
            Assert.AreEqual((BigInteger)0, result[1]);
        }
    }
}
