// Copyright (C) 2015-2026 The Neo Project.
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
            AssertGasConsumed(1048080);
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
            for (char c = '\0'; c < 128; c++)
            {
                Assert.AreEqual(char.IsPunctuation(c), Contract.TestCharIsPunctuation(c), $"IsPunctuation failed for '{c}'");
                AssertGasConsumed(1047450);
            }
            Assert.IsTrue(Contract.TestCharIsPunctuation('.'));
            AssertGasConsumed(1047450);
            Assert.IsTrue(Contract.TestCharIsPunctuation(','));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsPunctuation('a'));
            AssertGasConsumed(1048590);
            Assert.IsFalse(Contract.TestCharIsPunctuation('\u00A9'));

            for (char c = '\0'; c < 128; c++)
            {
                Assert.AreEqual(char.IsSymbol(c), Contract.TestCharIsSymbol(c), $"IsSymbol failed for '{c}'");
                AssertGasConsumed(1047450);
            }
            Assert.IsTrue(Contract.TestCharIsSymbol('$'));
            AssertGasConsumed(1047450);
            Assert.IsTrue(Contract.TestCharIsSymbol('+'));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsSymbol('a'));
            AssertGasConsumed(1049010);
            Assert.IsFalse(Contract.TestCharIsSymbol('\u00A9'));

            for (char c = '\0'; c < 128; c++)
            {
                Assert.AreEqual(char.IsControl(c), Contract.TestCharIsControl(c), $"IsControl failed for '{c}'");
                AssertGasConsumed(1047990);
            }
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
            Assert.AreEqual(' ', Contract.TestCharToUpper(' '));
            AssertGasConsumed(1047450);
            Assert.AreEqual('D', Contract.TestCharToUpper('d'));
            AssertGasConsumed(1047450);
            Assert.AreEqual('a', Contract.TestCharToLower('A'));
            AssertGasConsumed(1047990);
            Assert.AreEqual('a', Contract.TestCharToLower('a'));
            AssertGasConsumed(1047450);
            Assert.AreEqual(' ', Contract.TestCharToLower(' '));
            AssertGasConsumed(1047450);
            Assert.AreEqual('d', Contract.TestCharToLower('D'));
            AssertGasConsumed(1047450);
        }

        [TestMethod]
        public void TestCharToUpperInvariant()
        {
            Assert.AreEqual('A', Contract.TestCharToUpperInvariant('a'));
            AssertGasConsumed(1047990);
            Assert.AreEqual('A', Contract.TestCharToUpperInvariant('A'));
            AssertGasConsumed(1047450);
            Assert.AreEqual(' ', Contract.TestCharToUpperInvariant(' '));
            AssertGasConsumed(1047450);
            Assert.AreEqual('1', Contract.TestCharToUpperInvariant('1'));
            AssertGasConsumed(1047450);
        }

        [TestMethod]
        public void TestCharToLowerInvariant()
        {
            Assert.AreEqual('a', Contract.TestCharToLowerInvariant('A'));
            AssertGasConsumed(1047990);
            Assert.AreEqual('a', Contract.TestCharToLowerInvariant('a'));
            AssertGasConsumed(1047450);
            Assert.AreEqual(' ', Contract.TestCharToLowerInvariant(' '));
            AssertGasConsumed(1047450);
            Assert.AreEqual('1', Contract.TestCharToLowerInvariant('1'));
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

            Assert.IsTrue(Contract.TestCharIsLetterOrDigitResult('b'));
            Assert.IsTrue(Contract.TestCharIsLetterOrDigitResult('B'));
            Assert.IsTrue(Contract.TestCharIsLetterOrDigitResult('1'));
            Assert.IsTrue(Contract.TestCharIsLetterOrDigitResult('\0'));
        }

        [TestMethod]
        public void TestCharIsAsciiLetter()
        {
            // Boundaries of the uppercase and lowercase ranges
            Assert.IsTrue(Contract.TestCharIsAsciiLetter('A'));
            Assert.IsTrue(Contract.TestCharIsAsciiLetter('Z'));
            Assert.IsTrue(Contract.TestCharIsAsciiLetter('a'));
            Assert.IsTrue(Contract.TestCharIsAsciiLetter('z'));

            // Values just outside the uppercase range
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('@'));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('['));

            // Values just outside the lowercase range
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('`'));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('{'));

            // Digits and whitespace
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('0'));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('9'));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter(' '));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('\t'));

            // Non-ASCII characters, including ones that fold into the ASCII letter range
            // when naively ORed with 0x20 (e.g. '\u0080' | 0x20 == '\u00A0', still out of range,
            // and '\u00C0' | 0x20 == '\u00E0', which must still be rejected as non-ASCII).
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('\u0080'));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter('\u00C0'));

            // Values outside the char range (0..65535), passed directly as BigInteger
            Assert.IsFalse(Contract.TestCharIsAsciiLetter(BigInteger.MinusOne));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter(new BigInteger(char.MaxValue) + 1));
            Assert.IsFalse(Contract.TestCharIsAsciiLetter((BigInteger.One << 255) - BigInteger.One)); // Int256.MaxValue
        }

        [TestMethod]
        public void TestCharIsBetween()
        {
            // Value inside the range
            Assert.IsTrue(Contract.TestCharIsBetween('m', 'a', 'z'));
            AssertGasConsumed(1048590);
            // Value equal to the lower bound (inclusive)
            Assert.IsTrue(Contract.TestCharIsBetween('a', 'a', 'z'));
            AssertGasConsumed(1048590);
            // Value equal to the upper bound (inclusive)
            Assert.IsTrue(Contract.TestCharIsBetween('z', 'a', 'z'));
            AssertGasConsumed(1048590);
            // Value below the range
            Assert.IsFalse(Contract.TestCharIsBetween('a' - 1, 'a', 'z'));
            AssertGasConsumed(1048590);
            // Value above the range
            Assert.IsFalse(Contract.TestCharIsBetween('z' + 1, 'a', 'z'));
            AssertGasConsumed(1048590);

            Assert.IsFalse(Contract.TestCharIsBetween('A', 'a', 'z'));
            AssertGasConsumed(1048590);
            Assert.IsFalse(Contract.TestCharIsBetween('0', 'a', 'z'));
            AssertGasConsumed(1048590);

            // minInclusive == maxInclusive
            Assert.IsTrue(Contract.TestCharIsBetween('a', 'a', 'a'));
            AssertGasConsumed(1048590);
            Assert.IsFalse(Contract.TestCharIsBetween('b', 'a', 'a'));
            AssertGasConsumed(1048590);

            // minInclusive > maxInclusive: no value can ever satisfy the range
            Assert.IsFalse(Contract.TestCharIsBetween('a', 'z', 'a'));
            AssertGasConsumed(1048590);

            // Full range from char.MinValue to char.MaxValue
            Assert.IsTrue(Contract.TestCharIsBetween(char.MinValue, char.MinValue, char.MaxValue));
            AssertGasConsumed(1048590);
            Assert.IsTrue(Contract.TestCharIsBetween(char.MaxValue, char.MinValue, char.MaxValue));
            AssertGasConsumed(1048590);
            Assert.IsTrue(Contract.TestCharIsBetween((char)32767, char.MinValue, char.MaxValue));
            AssertGasConsumed(1048590);

            // value == char.MaxValue and maxInclusive == char.MaxValue: preserves inclusivity of the upper bound
            Assert.IsTrue(Contract.TestCharIsBetween(char.MaxValue, 'a', char.MaxValue));
            AssertGasConsumed(1048590);
            Assert.IsFalse(Contract.TestCharIsBetween(char.MaxValue, char.MaxValue, (char)('a' - 1)));
            AssertGasConsumed(1048590);

            BigInteger int256Max = (BigInteger.One << 255) - BigInteger.One;
            Assert.IsTrue(Contract.TestCharIsBetween(int256Max, BigInteger.Zero, int256Max));
            AssertGasConsumed(1048770);
            Assert.IsTrue(Contract.TestCharIsBetween(BigInteger.Zero, BigInteger.Zero, int256Max));
            AssertGasConsumed(1048680);
            Assert.IsTrue(Contract.TestCharIsBetween(int256Max, int256Max, int256Max));
            AssertGasConsumed(1048860);
        }

        [TestMethod]
        public void TestCharParseAndTryParse()
        {
            Assert.AreEqual('A', Contract.TestCharParse("A"));
            Assert.AreEqual('0', Contract.TestCharParse("0"));

            var ex = Assert.ThrowsException<TestException>(() => Contract.TestCharParse("TooLong"));
            Assert.Contains("NotOneChar", ex.Message);

            ex = Assert.ThrowsException<TestException>(() => Contract.TestCharParse(string.Empty));
            Assert.Contains("NotOneChar", ex.Message);

            ex = Assert.ThrowsException<TestException>(() => Contract.TestCharParse(null));
            Assert.Contains("Null", ex.Message);

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
