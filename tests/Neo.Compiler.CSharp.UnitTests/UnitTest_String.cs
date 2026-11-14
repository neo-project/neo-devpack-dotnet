// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_String.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using PrimitiveStackItem = Neo.VM.Types.PrimitiveType;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_String : DebugAndTestBase<Contract_String>
    {
        [TestMethod]
        public void Test_TestSubstring()
        {
            var log = new List<string>();
            TestEngine.OnRuntimeLogDelegate method = (UInt160 sender, string msg) =>
            {
                log.Add(msg);
            };

            Contract.OnRuntimeLog += method;
            Contract.TestSubstring();
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(2, log.Count);
            Assert.AreEqual("1234567", log[0]);
            Assert.AreEqual("1234", log[1]);
        }

        [TestMethod]
        public void Test_TestMain()
        {
            var log = new List<string>();
            TestEngine.OnRuntimeLogDelegate method = (UInt160 sender, string msg) =>
            {
                log.Add(msg);
            };

            Contract.OnRuntimeLog += method;
            Contract.TestMain();
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("Hello, Mark ! Current timestamp is 1468595301000.", log[0]);
        }

        [TestMethod]
        public void Test_TestEqual()
        {
            var log = new List<string>();
            TestEngine.OnRuntimeLogDelegate method = (UInt160 sender, string msg) =>
            {
                log.Add(msg);
            };

            Contract.OnRuntimeLog += method;
            Contract.TestEqual();
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("True", log[0]);
        }

        [TestMethod]
        public void Test_TestEmpty()
        {
            Assert.AreEqual("", Contract.TestEmpty());
        }

        [TestMethod]
        public void Test_TestIsNullOrEmpty()
        {
            Assert.IsTrue(Contract.TestIsNullOrEmpty(""));

            Assert.IsTrue(Contract.TestIsNullOrEmpty(null));

            Assert.IsFalse(Contract.TestIsNullOrEmpty("hello world"));
        }

        [TestMethod]
        public void Test_TestIsNullOrWhiteSpace()
        {
            Assert.IsTrue(Contract.TestIsNullOrWhiteSpace("   "));

            Assert.IsTrue(Contract.TestIsNullOrWhiteSpace(null));

            Assert.IsFalse(Contract.TestIsNullOrWhiteSpace("hello world"));

            // Every byte of the tab to carriage return range is whitespace
            Assert.IsTrue(Contract.TestIsNullOrWhiteSpace("\t\n\v\f\r"));
            Assert.IsTrue(Contract.TestIsNullOrWhiteSpace("\t\n\v\f\r "));

            // non-ASCII is not supported yet.
            Assert.IsFalse(Contract.TestIsNullOrWhiteSpace("\0"));
            Assert.IsFalse(Contract.TestIsNullOrWhiteSpace("\u0001"));
            Assert.IsFalse(Contract.TestIsNullOrWhiteSpace("\u00A0"));
            Assert.IsFalse(Contract.TestIsNullOrWhiteSpace(" \u00A0 "));
        }

        [TestMethod]
        public void Test_TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith("hello world"));

            Assert.IsTrue(Contract.TestEndWith("world"));

            Assert.IsFalse(Contract.TestEndWith("worl"));

            Assert.IsFalse(Contract.TestEndWith("world-1"));

            Assert.IsFalse(Contract.TestEndWith("hel"));

            Assert.IsFalse(Contract.TestEndWith("hello"));

            Assert.IsTrue(Contract.TestEndWith("hello world", ""));

            Assert.IsFalse(Contract.TestEndWith("", "world"));

            Assert.IsTrue(Contract.TestEndWith("hello world", "ello world"));

            Assert.Throws<TestException>(() => Assert.IsTrue(Contract.TestEndWith(null, "A")));

            Assert.Throws<TestException>(() => Assert.IsTrue(Contract.TestEndWith("A", null)));

            Assert.IsFalse(Contract.TestEndsWithOneByte("hello"));

            Assert.IsTrue(Contract.TestEndsWithOneByte("hello/"));

            Assert.IsFalse(Contract.TestEndsWithOneByte(""));

            Assert.IsTrue(Contract.TestEndsWithOneByte("/"));
        }

        [TestMethod]
        public void Test_TestContains()
        {
            Assert.IsTrue(Contract.TestContains("hello world"));

            Assert.IsFalse(Contract.TestContains("hello"));
        }

        [TestMethod]
        public void Test_TestContainsChar()
        {
            Assert.IsTrue(Contract.TestContainsChar("hello world", 'w'));

            Assert.IsTrue(Contract.TestContainsChar("hello world", ' '));

            Assert.IsFalse(Contract.TestContainsChar("hello", 'z'));

            Assert.IsFalse(Contract.TestContainsChar("", '\0'));

            Assert.IsFalse(Contract.TestContainsChar("abc", '\0'));

            Assert.IsTrue(Contract.TestContainsChar("\0abc", '\0'));

            Assert.IsTrue(Contract.TestContainsChar("a\0b", '\0'));
        }

        [TestMethod]
        public void Test_TestStartsWith()
        {
            Assert.IsTrue(Contract.TestStartsWith("worldwide"));

            Assert.IsFalse(Contract.TestStartsWith("hello world"));

            Assert.IsFalse(Contract.TestStartsWithOneByte("hello"));

            Assert.IsTrue(Contract.TestStartsWithOneByte("/hello"));

            Assert.IsTrue(Contract.TestStartsWithOneByte("/"));

            Assert.IsFalse(Contract.TestStartsWithOneByte(""));

            Assert.IsTrue(Contract.TestStartsWithPrefix("abc", ""));

            Assert.IsFalse(Contract.TestStartsWithPrefix("", "a"));

            Assert.IsTrue(Contract.TestStartsWithPrefix("abc", "abc"));

            Assert.IsTrue(Contract.TestStartsWithPrefix("", ""));

            Assert.ThrowsExactly<TestException>(() => Assert.IsTrue(Contract.TestStartsWithPrefix(null, "")));

            Assert.ThrowsExactly<TestException>(() => Assert.IsTrue(Contract.TestStartsWithPrefix("", null)));
        }

        [TestMethod]
        public void Test_TestStartsWithConstantPrefixGas()
        {
            var p50 = new string('x', 50);
            Assert.IsTrue(Contract.TestStartsWith2("abcd"));
            Assert.IsFalse(Contract.TestStartsWith2("xxcd"));
            Assert.IsFalse(Contract.TestStartsWith2("a"));
            Assert.IsTrue(Contract.TestStartsWith50(p50 + "yz"));
            Assert.IsFalse(Contract.TestStartsWith50("y" + p50));
            Assert.IsFalse(Contract.TestStartsWith50("xx"));
        }

        [TestMethod]
        public void Test_TestStartsWithConstantPrefix()
        {
            Assert.IsTrue(Contract.TestStartsWith("world"));
            Assert.IsFalse(Contract.TestStartsWith("worl"));
            Assert.IsFalse(Contract.TestStartsWith(""));
            Assert.IsTrue(Contract.TestStartsWithUtf8("éxito"));
            Assert.IsFalse(Contract.TestStartsWithUtf8("éy"));
            Assert.IsFalse(Contract.TestStartsWithUtf8("e"));
            Assert.IsTrue(Contract.TestStartsWithEmbeddedNull("a\0bc"));
            Assert.IsFalse(Contract.TestStartsWithEmbeddedNull("a\0c"));
        }

        [TestMethod]
        public void Test_TestCompare()
        {
            var compare = Contract_String.Manifest.Abi.GetMethod("testCompare", 2);
            Assert.IsNotNull(compare);

            Assert.AreEqual(0, Contract.TestCompare("alpha", "alpha"));

            Assert.IsTrue(Contract.TestCompare("alpha", "beta") < 0);

            Assert.IsTrue(Contract.TestCompare("beta", "alpha") > 0);
        }

        [TestMethod]
        public void Test_TestCompare_EdgeCases()
        {
            // Empty vs non-empty
            var emptyVsNonEmpty = Contract.TestCompare("", "a");
            var nonEmptyVsEmpty = Contract.TestCompare("a", "");
            Assert.IsTrue(emptyVsNonEmpty < 0);
            Assert.IsTrue(nonEmptyVsEmpty > 0);

            // Single-character ordering
            var aVsB = Contract.TestCompare("a", "b");
            var bVsA = Contract.TestCompare("b", "a");
            Assert.IsTrue(aVsB < 0);
            Assert.IsTrue(bVsA > 0);

            // Null handling
            Assert.IsTrue(Contract.TestCompare(null, "a") < 0);
            Assert.IsTrue(Contract.TestCompare("a", null) > 0);
            Assert.AreEqual(0, Contract.TestCompare(null, null));
            // A null sorts before the empty string, so these must not collapse to 0.
            Assert.IsTrue(Contract.TestCompare(null, "") < 0);
            Assert.IsTrue(Contract.TestCompare("", null) > 0);
        }

        [TestMethod]
        public void Test_TestIndexOf()
        {
            Assert.AreEqual(6, Contract.TestIndexOf("hello world"));

            Assert.AreEqual(-1, Contract.TestIndexOf("hello"));
        }

        [TestMethod]
        public void Test_TestIndexOfEmpty()
        {
            Assert.AreEqual(0, Contract.TestIndexOfEmpty(""));
            Assert.AreEqual(0, Contract.TestIndexOfEmpty("a"));
            Assert.AreEqual(0, Contract.TestIndexOfEmpty("hello"));
            Assert.AreEqual(0, Contract.TestIndexOfEmpty("éxito"));
            Assert.AreEqual(0, Contract.TestIndexOfEmpty("a\0b"));
            Assert.AreEqual(0, Contract.TestIndexOfEmpty(new string('x', 500)));
            Assert.AreEqual(6, Contract.TestIndexOf("hello world"));
        }

        [TestMethod]
        public void Test_TestIndexOfEmptyNullReceiver()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.TestIndexOfEmpty(null));
        }

        [TestMethod]
        public void Test_TestLastIndexOf()
        {
            var method = Contract_String.Manifest.Abi.GetMethod("testLastIndexOf", 1);
            Assert.IsNotNull(method);

            Assert.AreEqual(6, Contract.TestLastIndexOf("hello world"));

            Assert.AreEqual(-1, Contract.TestLastIndexOf("hello"));

            Assert.AreEqual(12, Contract.TestLastIndexOf("world hello world"));
        }

        [TestMethod]
        public void Test_LastIndexOfString_Value()
        {
            Assert.AreEqual(6, Contract.TestLastIndexOfValue("hello world", "world"));
            Assert.AreEqual(12, Contract.TestLastIndexOfValue("world hello world", "world"));
            Assert.AreEqual(-1, Contract.TestLastIndexOfValue("hello", "world"));
            Assert.AreEqual(-1, Contract.TestLastIndexOfValue("abc", "abcd"));
            Assert.AreEqual(0, Contract.TestLastIndexOfValue("abc", "ab"));
            Assert.AreEqual(3, Contract.TestLastIndexOfValue("abcab", "ab"));

            // Overlapping matches
            Assert.AreEqual(2, Contract.TestLastIndexOfValue("aaaa", "aa"));

            // Empty search value
            Assert.AreEqual(3, Contract.TestLastIndexOfValue("abc", ""));
            Assert.AreEqual(-1, Contract.TestLastIndexOfValue("", "abc"));

            Assert.AreEqual(0, Contract.TestLastIndexOfValue("", ""));
        }

        [TestMethod]
        public void Test_TestInterpolatedStringHandler()
        {
            Assert.AreEqual("SByte: -42, Byte: 42, UShort: 1000, UInt: 1000000, ULong: 1000000000000, BigInteger: 1000000000000000000000, Char: A, String: Hello, ECPoint: gEoSozeEfSovUXCsVZuNcRBW1u4iMsv5gXsvft7fJrnC, ByteString: System.Byte[], Bool: True", Contract.TestInterpolatedStringHandler());
        }

        [TestMethod]
        public void Test_TestTrim()
        {
            Assert.AreEqual("Hello, World!", Contract.TestTrim("  Hello, World!  "));

            Assert.AreEqual("No Trim", Contract.TestTrim("No Trim"));

            Assert.AreEqual("", Contract.TestTrim("   "));

            // Test various whitespace characters
            Assert.AreEqual("Trim Test", Contract.TestTrim("\t\n\r Trim Test \t\n\r"));

            Assert.AreEqual("Multiple Spaces", Contract.TestTrim("   Multiple Spaces   "));

            Assert.AreEqual("Mix of Whitespace", Contract.TestTrim(" \t \n \r Mix of Whitespace \r \n \t "));
        }

        [TestMethod]
        public void Test_TestTrimStart()
        {
            Assert.AreEqual("Hello", Contract.TestTrimStart("   Hello"));
            Assert.AreEqual("Hello", Contract.TestTrimStart("Hello"));
            Assert.AreEqual("", Contract.TestTrimStart("   "));
            Assert.AreEqual("", Contract.TestTrimStart(""));
            Assert.AreEqual("Hello", Contract.TestTrimStart("\t\n\v\f\r Hello"));
        }

        [TestMethod]
        public void Test_TestTrimStartChar()
        {
            Assert.AreEqual("Hello", Contract.TestTrimStartChar("***Hello", '*'));

            Assert.AreEqual("Hello", Contract.TestTrimStartChar("Hello", '*'));
        }

        [TestMethod]
        public void Test_TestTrimEnd()
        {
            Assert.AreEqual("Hello", Contract.TestTrimEnd("Hello   "));

            Assert.AreEqual("Hello", Contract.TestTrimEnd("Hello"));

            Assert.AreEqual("", Contract.TestTrimEnd("   "));
        }

        [TestMethod]
        public void Test_TestTrimEndChar()
        {
            Assert.AreEqual("Hello", Contract.TestTrimEndChar("Hello***", '*'));

            Assert.AreEqual("Hello", Contract.TestTrimEndChar("Hello", '*'));
        }

        [TestMethod]
        public void Test_TestTrimArray()
        {
            Assert.AreEqual("Hello", Contract.TestTrimArray("***Hello***"));
        }

        [TestMethod]
        public void Test_TestTrimStartArray()
        {
            Assert.AreEqual("Hello***", Contract.TestTrimStartArray("***Hello***"));
        }

        [TestMethod]
        public void Test_TestTrimEndArray()
        {
            Assert.AreEqual("***Hello", Contract.TestTrimEndArray("***Hello***"));
        }

        [TestMethod]
        public void TestTrimConst()
        {
            Assert.AreEqual("Hello", Contract.TestTrimConst("***Hello***"));
            Assert.AreEqual(string.Empty, Contract.TestTrimConst(string.Empty));
            Assert.AreEqual(string.Empty, Contract.TestTrimConst("***"));

            Assert.AreEqual("Hello***", Contract.TestTrimConstStart("***Hello***"));
            Assert.AreEqual(string.Empty, Contract.TestTrimConstStart(string.Empty));
            Assert.AreEqual(string.Empty, Contract.TestTrimConstStart("***"));

            Assert.AreEqual("***Hello", Contract.TestTrimConstEnd("***Hello***"));
            Assert.AreEqual(string.Empty, Contract.TestTrimConstEnd(string.Empty));
            Assert.AreEqual(string.Empty, Contract.TestTrimConstEnd("***"));
        }

        [TestMethod]
        public void Test_TestPickItem()
        {
            Assert.AreEqual('e', Contract.TestPickItem("Hello", 1));

            Assert.AreEqual('d', Contract.TestPickItem("World", 4));

            // Test invalid index
            Assert.ThrowsExactly<TestException>(() => Contract.TestPickItem("Test", 5));
        }

        [TestMethod]
        public void Test_TestSubstringToEnd()
        {
            Assert.AreEqual("World", Contract.TestSubstringToEnd("Hello World", 6));

            Assert.AreEqual("", Contract.TestSubstringToEnd("Test", 4));

            // Test invalid start index
            Assert.ThrowsExactly<TestException>(() => Contract.TestSubstringToEnd("Test", 5));
        }

        [TestMethod]
        public void Test_TestConcat()
        {
            Assert.AreEqual("HelloWorld", Contract.TestConcat("Hello", "World"));

            Assert.AreEqual("Test", Contract.TestConcat("Test", null));

            Assert.AreEqual("Test", Contract.TestConcat(null, "Test"));

            Assert.AreEqual("", Contract.TestConcat(null, null));

            // Test with empty strings
            Assert.AreEqual("", Contract.TestConcat("", ""));
        }

        [TestMethod]
        public void Test_TestSplit()
        {
            CollectionAssert.AreEqual(new[] { "hello", "world" }, ConvertToStrings(Contract.TestSplit("hello world")));
            CollectionAssert.AreEqual(new[] { "", "leading", "space" }, ConvertToStrings(Contract.TestSplit(" leading space")));
        }

        [TestMethod]
        public void Test_TestSplitRemoveEmpty()
        {
            CollectionAssert.AreEqual(new[] { "hello", "world" }, ConvertToStrings(Contract.TestSplitRemoveEmpty("hello  world")));
            CollectionAssert.AreEqual(System.Array.Empty<string>(), ConvertToStrings(Contract.TestSplitRemoveEmpty("   ")));
        }

        [TestMethod]
        public void Test_TestSplitCharArray()
        {
            CollectionAssert.AreEqual(new[] { "hello", "world" }, ConvertToStrings(Contract.TestSplitCharArray("hello  world")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeTwoByte()
        {
            CollectionAssert.AreEqual("aéb".Split('é'), ConvertToStrings(Contract.TestSplitUnicodeTwoByte("aéb")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeTwoByteMultiple()
        {
            CollectionAssert.AreEqual("aébéc".Split('é'), ConvertToStrings(Contract.TestSplitUnicodeTwoByte("aébéc")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeSeparatorNotPresent()
        {
            CollectionAssert.AreEqual("abc".Split('é'), ConvertToStrings(Contract.TestSplitUnicodeTwoByte("abc")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeThreeByte()
        {
            CollectionAssert.AreEqual("a中b".Split('中'), ConvertToStrings(Contract.TestSplitUnicodeThreeByte("a中b")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeThreeByteMultiple()
        {
            CollectionAssert.AreEqual("a中b中c".Split('中'), ConvertToStrings(Contract.TestSplitUnicodeThreeByte("a中b中c")));
        }

        [TestMethod]
        public void Test_TestSplitNullChar()
        {
            CollectionAssert.AreEqual("a\0b".Split('\0'), ConvertToStrings(Contract.TestSplitNullChar("a\0b")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeRemoveEmpty()
        {
            CollectionAssert.AreEqual("éaéé".Split('é', System.StringSplitOptions.RemoveEmptyEntries), ConvertToStrings(Contract.TestSplitUnicodeRemoveEmpty("éaéé")));
        }

        [TestMethod]
        public void Test_TestSplitUnicodeCharArray()
        {
            CollectionAssert.AreEqual("中a中中b".Split(new[] { '中' }, System.StringSplitOptions.RemoveEmptyEntries), ConvertToStrings(Contract.TestSplitUnicodeCharArray("中a中中b")));
        }

        [TestMethod]
        public void Test_TestSplitRuntimeCharAscii()
        {
            CollectionAssert.AreEqual("a,b,c".Split(','), ConvertToStrings(Contract.TestSplitRuntimeChar("a,b,c", ',')));
        }

        [TestMethod]
        public void Test_TestSplitRuntimeCharTwoByte()
        {
            CollectionAssert.AreEqual("aébéc".Split('é'), ConvertToStrings(Contract.TestSplitRuntimeChar("aébéc", 'é')));
        }

        [TestMethod]
        public void Test_TestSplitRuntimeCharThreeByte()
        {
            Assert.AreSequenceEqual("a中b中c".Split('中'), ConvertToStrings(Contract.TestSplitRuntimeChar("a中b中c", '中')));
        }

        [TestMethod]
        public void Test_TestSplitRuntimeCharNull()
        {
            CollectionAssert.AreEqual("a\0b\0c".Split('\0'), ConvertToStrings(Contract.TestSplitRuntimeChar("a\0b\0c", '\0')));
        }

        [TestMethod]
        public void Test_TestSplitStringArray()
        {
            CollectionAssert.AreEqual(new[] { "hello", "world" }, ConvertToStrings(Contract.TestSplitStringArray("hello  world")));
        }

        [TestMethod]
        public void Test_TestRemove()
        {
            Assert.AreEqual("Hello", Contract.TestRemove("HelloWorld", 5));

            Assert.AreEqual("", Contract.TestRemove("Neo", 0));

            Assert.AreEqual("HelloWorld", Contract.TestRemove("HelloWorld", 10));
        }

        [TestMethod]
        public void Test_TestRemoveRange()
        {
            Assert.AreEqual("HeWorld", Contract.TestRemoveRange("HelloWorld", 2, 3));

            Assert.AreEqual("Hello", Contract.TestRemoveRange("HelloWorld", 5, 5));

            Assert.AreEqual("HelloWorld", Contract.TestRemoveRange("HelloWorld", 10, 0));
            Assert.AreEqual("HelloWorld", Contract.TestRemoveRange("HelloWorld", 0, 0));

            Assert.ThrowsExactly<TestException>(() => Contract.TestRemoveRange("0", 1, -1));
            Assert.ThrowsExactly<TestException>(() => Contract.TestRemoveRange("HelloWorld", -1, 0));
            Assert.ThrowsExactly<TestException>(() => Contract.TestRemoveRange("HelloWorld", 11, 0));
            Assert.ThrowsExactly<TestException>(() => Contract.TestRemoveRange("HelloWorld", 9, 2));
        }

        [TestMethod]
        public void Test_TestInsert()
        {
            Assert.AreEqual("Hello Neo World", Contract.TestInsert("Hello World", 6, "Neo "));

            Assert.AreEqual(">>>Hello World", Contract.TestInsert("Hello World", 0, ">>>"));

            Assert.AreEqual("Hello World<<<", Contract.TestInsert("Hello World", 11, "<<<"));

            Assert.ThrowsExactly<TestException>(() => Contract.TestInsert("Hello World", 0, null));
        }

        [TestMethod]
        public void Test_TestReplace()
        {
            // Single occurrence, plus matches at the start and the end.
            Assert.AreEqual("Hello Neo", Contract.TestReplace("Hello World", "World", "Neo"));
            Assert.AreEqual("XYbc", Contract.TestReplace("abc", "a", "XY"));
            Assert.AreEqual("abXY", Contract.TestReplace("abc", "c", "XY"));

            // Multiple occurrences.
            Assert.AreEqual("bbb", Contract.TestReplace("aaa", "a", "b"));
            Assert.AreEqual("a-b-c", Contract.TestReplace("aXXbXXc", "XX", "-"));

            // No occurrence returns the original string.
            Assert.AreEqual("abc", Contract.TestReplace("abc", "x", "y"));

            // newValue containing oldValue must not loop or re-match the inserted text.
            Assert.AreEqual("aa", Contract.TestReplace("a", "a", "aa"));
            Assert.AreEqual("xbxxbx", Contract.TestReplace("bb", "b", "xbx"));

            // Replacing with the empty string removes every occurrence.
            Assert.AreEqual("abc", Contract.TestReplace("a-b-c", "-", ""));

            // An empty oldValue throws in C#; the contract faults instead of looping forever.
            Assert.ThrowsExactly<TestException>(() => Contract.TestReplace("abc", "", "x"));

            // Matches are taken left to right and are not allowed to overlap.
            Assert.AreEqual("ba", Contract.TestReplace("aaa", "aa", "b"));
            // Replacing the whole string leaves an empty tail.
            Assert.AreEqual("x", Contract.TestReplace("abc", "abc", "x"));
            // The tail shortcut also has to work for an empty input.
            Assert.AreEqual("", Contract.TestReplace("", "a", "b"));
            // Multi-byte characters are matched and copied byte-wise.
            Assert.AreEqual("aXb", Contract.TestReplace("a\u20acb", "\u20ac", "X"));
        }

        [TestMethod]
        public void Test_TestIndexOfChar()
        {
            Assert.AreEqual(1, Contract.TestIndexOfChar("Hello", 'e'));

            Assert.AreEqual(-1, Contract.TestIndexOfChar("World", 'x'));

            // Test with empty string
            Assert.AreEqual(-1, Contract.TestIndexOfChar("", 'a'));

            Assert.AreEqual(-1, Contract.TestIndexOfChar("", '\0'));
            Assert.AreEqual(-1, Contract.TestIndexOfChar("abc", '\0'));
            Assert.AreEqual(0, Contract.TestIndexOfChar("\0abc", '\0'));
            Assert.AreEqual(1, Contract.TestIndexOfChar("a\0b", '\0'));

            Assert.AreEqual(-1, Contract.TestIndexOfConstChar(""));

            Assert.AreEqual(1, Contract.TestIndexOfConstChar("H*llo"));

            Assert.AreEqual(-1, Contract.TestIndexOfConstChar("Hello"));
        }

        [TestMethod]
        public void Test_TestLastIndexOfChar()
        {
            var method = Contract_String.Manifest.Abi.GetMethod("testLastIndexOfChar", 2);
            Assert.IsNotNull(method);

            Assert.AreEqual("Hello".LastIndexOf('l'), Contract.TestLastIndexOfChar("Hello", 'l'));

            Assert.AreEqual(-1, Contract.TestLastIndexOfChar("World", 'x'));

            Assert.AreEqual("hello world".LastIndexOf('o'), Contract.TestLastIndexOfChar("hello world", 'o'));

            Assert.AreEqual("abc".LastIndexOf('c'), Contract.TestLastIndexOfChar("abc", 'c'));
            Assert.AreEqual("abc".LastIndexOf('a'), Contract.TestLastIndexOfChar("abc", 'a'));

            // Test with empty string
            Assert.AreEqual(-1, Contract.TestLastIndexOfChar("", 'a'));

            Assert.AreEqual(-1, Contract.TestLastIndexOfChar("", '\0'));
            Assert.AreEqual(-1, Contract.TestLastIndexOfChar("abc", '\0'));
            Assert.AreEqual(0, Contract.TestLastIndexOfChar("\0abc", '\0'));
            Assert.AreEqual(1, Contract.TestLastIndexOfChar("a\0b", '\0'));
            Assert.AreEqual(2, Contract.TestLastIndexOfChar("ab\0", '\0'));

            Assert.AreEqual(1, Contract.TestLastIndexOfChar("aé", 'é'));
            Assert.AreEqual(1, Contract.TestLastIndexOfChar("aΩ", 'Ω'));
            Assert.AreEqual(1, Contract.TestLastIndexOfChar("a中", '中'));

            Assert.AreEqual("He**o".LastIndexOf('*'), Contract.TestLastIndexOfConstChar("He**o"));

            Assert.AreEqual("hell* w*rld".LastIndexOf('*'), Contract.TestLastIndexOfConstChar("hell* w*rld"));
        }

        [TestMethod]
        public void Test_TestToLower()
        {
            Assert.AreEqual("hello world", Contract.TestToLower("Hello World"));

            Assert.AreEqual("123", Contract.TestToLower("123"));

            // Test with already lowercase string
            Assert.AreEqual("lowercase", Contract.TestToLower("lowercase"));

            Assert.AreEqual("", Contract.TestToLower(""));
        }

        [TestMethod]
        public void Test_TestToUpper()
        {
            Assert.AreEqual("HELLO WORLD", Contract.TestToUpper("Hello World"));

            Assert.AreEqual("123", Contract.TestToUpper("123"));

            // Test with already uppercase string
            Assert.AreEqual("UPPERCASE", Contract.TestToUpper("UPPERCASE"));
        }

        [TestMethod]
        public void Test_TestTrimChar()
        {
            Assert.AreEqual("Hello World", Contract.TestTrimChar("***Hello World***", '*'));

            Assert.AreEqual("Test", Contract.TestTrimChar("Test", '*'));

            // Test with string containing only trim characters
            Assert.AreEqual("", Contract.TestTrimChar("****", '*'));
        }

        [TestMethod]
        public void Test_TestTrimChar_NonAscii()
        {
            Assert.AreEqual("caf", Contract.TestTrimChar("café", 'é'));
            Assert.AreEqual("caaf", Contract.TestTrimChar("ééécaafééé", 'é'));
            Assert.AreEqual(string.Empty, Contract.TestTrimChar("ééé", 'é'));

            Assert.AreEqual("café", Contract.TestTrimStartChar("éééééééécafé", 'é'));
            Assert.AreEqual("测试", Contract.TestTrimStartChar("中中测试", '中'));
            Assert.AreEqual(string.Empty, Contract.TestTrimStartChar("中中", '中'));

            Assert.AreEqual("caf", Contract.TestTrimEndChar("caféééé", 'é'));
            Assert.AreEqual("测试", Contract.TestTrimEndChar("测试中中", '中'));
            Assert.AreEqual(string.Empty, Contract.TestTrimEndChar("中中", '中'));

            Assert.AreEqual("caf", Contract.TestTrimArrayNonAscii("ééécaféé"));
            Assert.AreEqual(string.Empty, Contract.TestTrimArrayNonAscii("éé"));
            Assert.AreEqual("测试", Contract.TestTrimStartArrayNonAscii("中中测试"));
            Assert.AreEqual(string.Empty, Contract.TestTrimStartArrayNonAscii("中中"));
            Assert.AreEqual("测试", Contract.TestTrimEndArrayNonAscii("测试中中"));
            Assert.AreEqual(string.Empty, Contract.TestTrimEndArrayNonAscii("中中"));
        }

        [TestMethod]
        public void TrimChar_Utf8BoundariesMatchDotNet()
        {
            foreach (char trimChar in new[] { '\0', '\u007f', '\u0080', '\u00ff', '\u0100', '\u07ff', '\u0800', '\uffff' })
            {
                foreach (string value in new[] { string.Empty, "a", "é", "中", new string(trimChar, 3), $"{trimChar}{trimChar}a{trimChar}", $"a{trimChar}b" })
                {
                    Assert.AreEqual(value.Trim(trimChar), Contract.TestTrimChar(value, trimChar));
                    Assert.AreEqual(value.TrimStart(trimChar), Contract.TestTrimStartChar(value, trimChar));
                    Assert.AreEqual(value.TrimEnd(trimChar), Contract.TestTrimEndChar(value, trimChar));
                }
            }
        }

        [TestMethod]
        public void Test_TestLength()
        {
            Assert.AreEqual(11, Contract.TestLength("Hello World"));

            Assert.AreEqual(0, Contract.TestLength(""));

            // Test with very long string
            Assert.AreEqual(1000, Contract.TestLength(new string('a', 1000)));
        }

        [TestMethod]
        public void Test_StringCharCount()
        {
            Assert.AreEqual("aaa", Contract.TestStringCharCount('a', 3));

            Assert.ThrowsExactly<TestException>(() => Contract.TestStringCharCount('a', -1));

            Assert.AreEqual("", Contract.TestStringCharCount('a', 0));
        }

        [TestMethod]
        public void Test_StringBuilderBasic()
        {
            Assert.AreEqual("neo compiler\nruntime\n", Contract.TestStringBuilderBasic());
        }

        [TestMethod]
        public void Test_StringBuilderLength()
        {
            Assert.AreEqual(14, Contract.TestStringBuilderLength());
        }

        [TestMethod]
        public void Test_StringBuilderClear()
        {
            Assert.AreEqual("neo\ncontracts", Contract.TestStringBuilderClear());
        }

        [TestMethod]
        public void Test_StringBuilderAppendLineConstants()
        {
            Assert.AreEqual("start:Hello\n\nHello World\nNeo\nhéllo 你好 \0 end\nrt\n\n", Contract.TestStringBuilderAppendLineConstants("rt"));
        }

        [TestMethod]
        public void Test_AppendLineHelloGas()
        {
            Assert.AreEqual("aHello\n", Contract.TestAppendLineHello());
        }

        [TestMethod]
        public void Test_AppendLineEmptyGas()
        {
            Assert.AreEqual("a\n", Contract.TestAppendLineEmpty());
        }

        [TestMethod]
        public void Test_AppendLineConstStringGas()
        {
            Assert.AreEqual("aNeo\n", Contract.TestAppendLineConstString());
        }

        [TestMethod]
        public void Test_AppendLineDynamicGas()
        {
            Assert.AreEqual("art\n", Contract.TestAppendLineDynamic("rt"));
        }

        [TestMethod]
        public void Test_AppendLineLongGas()
        {
            Assert.AreEqual("a0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789\n", Contract.TestAppendLineLong());
        }

        [TestMethod]
        public void Test_StringBuilderAppendBuilder()
        {
            Assert.AreEqual("neo compiler\npreview", Contract.TestStringBuilderAppendBuilder());
        }
        private static string[] ConvertToStrings(IList<object>? items)
        {
            if (items is null)
                return System.Array.Empty<string>();

            var result = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                result[i] = items[i] switch
                {
                    string s => s,
                    byte[] bytes => System.Text.Encoding.UTF8.GetString(bytes),
                    PrimitiveStackItem primitive => primitive.GetString() ?? throw new AssertFailedException("Unexpected null string stack item."),
                    _ => throw new AssertFailedException($"Unexpected stack item type: {items[i]?.GetType().FullName}")
                };
            }

            return result;
        }
    }
}
