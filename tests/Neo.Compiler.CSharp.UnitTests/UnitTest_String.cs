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
            AssertGasConsumed(3075900);
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
            AssertGasConsumed(7625310);
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
            AssertGasConsumed(1970970);
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("True", log[0]);
        }

        [TestMethod]
        public void Test_TestEmpty()
        {
            Assert.AreEqual("", Contract.TestEmpty());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_TestIsNullOrEmpty()
        {
            Assert.IsTrue(Contract.TestIsNullOrEmpty(""));
            AssertGasConsumed(1047780);

            Assert.IsTrue(Contract.TestIsNullOrEmpty(null));
            AssertGasConsumed(1047300);

            Assert.IsFalse(Contract.TestIsNullOrEmpty("hello world"));
            AssertGasConsumed(1047780);
        }

        [TestMethod]
        public void Test_TestIsNullOrWhiteSpace()
        {
            Assert.IsTrue(Contract.TestIsNullOrWhiteSpace("   "));
            AssertGasConsumed(1061910);

            Assert.IsTrue(Contract.TestIsNullOrWhiteSpace(null));
            AssertGasConsumed(1047300);

            Assert.IsFalse(Contract.TestIsNullOrWhiteSpace("hello world"));
            AssertGasConsumed(1052250);
        }

        [TestMethod]
        public void Test_TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith("hello world"));
            AssertGasConsumed(1357650);

            Assert.IsFalse(Contract.TestEndWith("hel"));
            AssertGasConsumed(1049190);

            Assert.IsFalse(Contract.TestEndWith("hello"));
            AssertGasConsumed(1049190);
        }

        [TestMethod]
        public void Test_TestContains()
        {
            Assert.IsTrue(Contract.TestContains("hello world"));
            AssertGasConsumed(2032740);

            Assert.IsFalse(Contract.TestContains("hello"));
            AssertGasConsumed(2032740);
        }

        [TestMethod]
        public void Test_TestStartsWith()
        {
            Assert.IsTrue(Contract.TestStartsWith("worldwide"));
            AssertGasConsumed(2032710);

            Assert.IsFalse(Contract.TestStartsWith("hello world"));
            AssertGasConsumed(2032710);
        }

        [TestMethod]
        public void Test_TestCompare()
        {
            var compare = Contract_String.Manifest.Abi.GetMethod("testCompare", 2);
            Assert.IsNotNull(compare);

            Assert.AreEqual(0, Contract.TestCompare("alpha", "alpha"));
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("alpha", "beta") < 0);
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("beta", "alpha") > 0);
            AssertGasConsumed(2031570);
        }

        /// <summary>
        /// Tests lexicographic ordering that differs from numeric ordering.
        /// These tests would fail with the old subtraction-based implementation
        /// because "10" as a number is greater than "2", but lexicographically
        /// "10" comes before "2" (since '1' < '2').
        /// </summary>
        [TestMethod]
        public void Test_TestCompare_LexicographicVsNumeric()
        {
            // "10" vs "2": Lexicographically "10" < "2" ('1' = 0x31 < '2' = 0x32)
            // But as little-endian numbers: "10" = 0x3031 = 12337, "2" = 0x32 = 50
            // Old subtraction would give: 50 - 12337 = negative (WRONG - says "2" < "10")
            // Correct result: negative ("10" < "2")
            Assert.IsTrue(Contract.TestCompare("10", "2") < 0,
                "Lexicographically '10' should come before '2' because '1' < '2'");
            AssertGasConsumed(2031570);

            // Reverse: "2" > "10"
            Assert.IsTrue(Contract.TestCompare("2", "10") > 0,
                "Lexicographically '2' should come after '10'");
            AssertGasConsumed(2031570);

            // More numeric vs lexicographic differences
            Assert.IsTrue(Contract.TestCompare("100", "99") < 0,
                "Lexicographically '100' < '99' because '1' < '9'");
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("20", "3") < 0,
                "Lexicographically '20' < '3' because '2' < '3'");
            AssertGasConsumed(2031570);
        }

        /// <summary>
        /// Tests comparison of strings with same prefix but different lengths.
        /// Shorter string is lexicographically smaller.
        /// </summary>
        [TestMethod]
        public void Test_TestCompare_PrefixAndLength()
        {
            // Same prefix, different lengths
            Assert.AreEqual(0, Contract.TestCompare("abc", "abc"));
            AssertGasConsumed(2031570);

            // Shorter string is smaller
            Assert.IsTrue(Contract.TestCompare("abc", "abcd") < 0,
                "'abc' should be less than 'abcd' (shorter string with same prefix)");
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("abcd", "abc") > 0,
                "'abcd' should be greater than 'abc'");
            AssertGasConsumed(2031570);

            // Empty string is smallest
            Assert.IsTrue(Contract.TestCompare("", "a") < 0,
                "Empty string should be less than any non-empty string");
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("a", "") > 0,
                "Non-empty string should be greater than empty string");
            AssertGasConsumed(2031570);

            Assert.AreEqual(0, Contract.TestCompare("", ""));
            AssertGasConsumed(2031570);
        }

        /// <summary>
        /// Tests case-sensitive comparison.
        /// ASCII: 'A' = 0x41 (65), 'a' = 0x61 (97)
        /// </summary>
        [TestMethod]
        public void Test_TestCompare_CaseSensitivity()
        {
            // Uppercase letters come before lowercase in ASCII
            Assert.IsTrue(Contract.TestCompare("A", "a") < 0,
                "'A' (0x41) should be less than 'a' (0x61)");
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("a", "A") > 0,
                "'a' should be greater than 'A'");
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("Apple", "apple") < 0,
                "'Apple' should be less than 'apple' (case-sensitive)");
            AssertGasConsumed(2031570);

            Assert.IsTrue(Contract.TestCompare("Z", "a") < 0,
                "'Z' (0x5A) should be less than 'a' (0x61)");
            AssertGasConsumed(2031570);
        }

        /// <summary>
        /// Tests comparison at the byte level (non-ASCII characters).
        /// </summary>
        [TestMethod]
        public void Test_TestCompare_ByteLevelOrdering()
        {
            // Control characters vs printable ASCII
            // '\n' = 0x0A (10), 'A' = 0x41 (65)
            Assert.IsTrue(Contract.TestCompare("\n", "A") < 0,
                "Control characters should be less than printable ASCII");
            AssertGasConsumed(2031570);

            // Space (0x20) vs '0' (0x30)
            Assert.IsTrue(Contract.TestCompare(" ", "0") < 0,
                "Space should be less than '0'");
            AssertGasConsumed(2031570);

            // Digits (0x30-0x39) vs uppercase (0x41-0x5A)
            Assert.IsTrue(Contract.TestCompare("9", "A") < 0,
                "Digits should be less than uppercase letters");
            AssertGasConsumed(2031570);

            // '9' (0x39) vs ':' (0x3A)
            Assert.IsTrue(Contract.TestCompare("9", ":") < 0,
                "'9' should be less than ':'");
            AssertGasConsumed(2031570);
        }

        [TestMethod]
        public void Test_TestIndexOf()
        {
            Assert.AreEqual(6, Contract.TestIndexOf("hello world"));
            AssertGasConsumed(2032470);

            Assert.AreEqual(-1, Contract.TestIndexOf("hello"));
            AssertGasConsumed(2032470);
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
        public void Test_TestInterpolatedStringHandler()
        {
            Assert.AreEqual("SByte: -42, Byte: 42, UShort: 1000, UInt: 1000000, ULong: 1000000000000, BigInteger: 1000000000000000000000, Char: A, String: Hello, ECPoint: gEoSozeEfSovUXCsVZuNcRBW1u4iMsv5gXsvft7fJrnC, ByteString: System.Byte[], Bool: True", Contract.TestInterpolatedStringHandler());
            Assert.AreEqual(11313780, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestTrim()
        {
            Assert.AreEqual("Hello, World!", Contract.TestTrim("  Hello, World!  "));
            AssertGasConsumed(1136010);

            Assert.AreEqual("No Trim", Contract.TestTrim("No Trim"));
            AssertGasConsumed(1118130);

            Assert.AreEqual("", Contract.TestTrim("   "));
            AssertGasConsumed(1124040);

            // Test various whitespace characters
            Assert.AreEqual("Trim Test", Contract.TestTrim("\t\n\r Trim Test \t\n\r"));
            AssertGasConsumed(1153890);
            Assert.AreEqual("Multiple Spaces", Contract.TestTrim("   Multiple Spaces   "));
            AssertGasConsumed(1144950);

            Assert.AreEqual("Mix of Whitespace", Contract.TestTrim(" \t \n \r Mix of Whitespace \r \n \t "));
            AssertGasConsumed(1180710);
        }

        [TestMethod]
        public void Test_TestTrimStart()
        {
            Assert.AreEqual("Hello", Contract.TestTrimStart("   Hello"));
            AssertGasConsumed(1127010);

            Assert.AreEqual("Hello", Contract.TestTrimStart("Hello"));
            AssertGasConsumed(1113600);

            Assert.AreEqual("", Contract.TestTrimStart("   "));
            AssertGasConsumed(1123260);
        }

        [TestMethod]
        public void Test_TestTrimStartChar()
        {
            Assert.AreEqual("Hello", Contract.TestTrimStartChar("***Hello", '*'));
            AssertGasConsumed(1121760);

            Assert.AreEqual("Hello", Contract.TestTrimStartChar("Hello", '*'));
            AssertGasConsumed(1112400);
        }

        [TestMethod]
        public void Test_TestTrimEnd()
        {
            Assert.AreEqual("Hello", Contract.TestTrimEnd("Hello   "));
            AssertGasConsumed(1127940);

            Assert.AreEqual("Hello", Contract.TestTrimEnd("Hello"));
            AssertGasConsumed(1114620);

            Assert.AreEqual("", Contract.TestTrimEnd("   "));
            AssertGasConsumed(1062720);
        }

        [TestMethod]
        public void Test_TestTrimEndChar()
        {
            Assert.AreEqual("Hello", Contract.TestTrimEndChar("Hello***", '*'));
            AssertGasConsumed(1122690);

            Assert.AreEqual("Hello", Contract.TestTrimEndChar("Hello", '*'));
            AssertGasConsumed(1113420);
        }

        [TestMethod]
        public void Test_TestTrimArray()
        {
            Assert.AreEqual("Hello", Contract.TestTrimArray("***Hello***"));
            AssertGasConsumed(1195470);
        }

        [TestMethod]
        public void Test_TestTrimStartArray()
        {
            Assert.AreEqual("Hello***", Contract.TestTrimStartArray("***Hello***"));
            AssertGasConsumed(1183050);
        }

        [TestMethod]
        public void Test_TestTrimEndArray()
        {
            Assert.AreEqual("***Hello", Contract.TestTrimEndArray("***Hello***"));
            AssertGasConsumed(1183980);
        }

        [TestMethod]
        public void Test_TestPickItem()
        {
            Assert.AreEqual('e', Contract.TestPickItem("Hello", 1));
            AssertGasConsumed(1049250);

            Assert.AreEqual('d', Contract.TestPickItem("World", 4));
            AssertGasConsumed(1049250);

            // Test invalid index
            Assert.ThrowsException<TestException>(() => Contract.TestPickItem("Test", 5));
        }

        [TestMethod]
        public void Test_TestSubstringToEnd()
        {
            Assert.AreEqual("World", Contract.TestSubstringToEnd("Hello World", 6));
            AssertGasConsumed(1109250);

            Assert.AreEqual("", Contract.TestSubstringToEnd("Test", 4));
            AssertGasConsumed(1109250);

            // Test invalid start index
            Assert.ThrowsException<TestException>(() => Contract.TestSubstringToEnd("Test", 5));
        }

        [TestMethod]
        public void Test_TestConcat()
        {
            Assert.AreEqual("HelloWorld", Contract.TestConcat("Hello", "World"));
            AssertGasConsumed(1109400);

            Assert.AreEqual("Test", Contract.TestConcat("Test", null));
            AssertGasConsumed(1109490);

            Assert.AreEqual("Test", Contract.TestConcat(null, "Test"));
            AssertGasConsumed(1109490);

            Assert.AreEqual("", Contract.TestConcat(null, null));
            AssertGasConsumed(1109580);

            // Test with empty strings
            Assert.AreEqual("", Contract.TestConcat("", ""));
            AssertGasConsumed(1109400);
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
            AssertGasConsumed(2591820);
        }

        [TestMethod]
        public void Test_TestSplitStringArray()
        {
            CollectionAssert.AreEqual(new[] { "hello", "world" }, ConvertToStrings(Contract.TestSplitStringArray("hello  world")));
            AssertGasConsumed(2592240);
        }

        [TestMethod]
        public void Test_TestRemove()
        {
            Assert.AreEqual("Hello", Contract.TestRemove("HelloWorld", 5));
            AssertGasConsumed(1354920);

            Assert.AreEqual("", Contract.TestRemove("Neo", 0));
            AssertGasConsumed(1354920);

            Assert.AreEqual("HelloWorld", Contract.TestRemove("HelloWorld", 10));
            AssertGasConsumed(1354920);
        }

        [TestMethod]
        public void Test_TestRemoveRange()
        {
            Assert.AreEqual("HeWorld", Contract.TestRemoveRange("HelloWorld", 2, 3));
            AssertGasConsumed(1479450);

            Assert.AreEqual("Hello", Contract.TestRemoveRange("HelloWorld", 5, 5));
            AssertGasConsumed(1479450);
        }

        [TestMethod]
        public void Test_TestInsert()
        {
            Assert.AreEqual("Hello Neo World", Contract.TestInsert("Hello World", 6, "Neo "));
            AssertGasConsumed(1786890);

            Assert.AreEqual(">>>Hello World", Contract.TestInsert("Hello World", 0, ">>>"));
            AssertGasConsumed(1786890);

            Assert.AreEqual("Hello World<<<", Contract.TestInsert("Hello World", 11, "<<<"));
            AssertGasConsumed(1786890);
        }

        [TestMethod]
        public void Test_TestIndexOfChar()
        {
            Assert.AreEqual(1, Contract.TestIndexOfChar("Hello", 'e'));
            AssertGasConsumed(2032320);

            Assert.AreEqual(-1, Contract.TestIndexOfChar("World", 'x'));
            AssertGasConsumed(2032320);

            // Test with empty string
            Assert.AreEqual(-1, Contract.TestIndexOfChar("", 'a'));
            AssertGasConsumed(2032320);
        }

        [TestMethod]
        public void Test_TestToLower()
        {
            Assert.AreEqual("hello world", Contract.TestToLower("Hello World"));
            AssertGasConsumed(2008350);

            Assert.AreEqual("123", Contract.TestToLower("123"));
            AssertGasConsumed(1488390);

            // Test with already lowercase string
            Assert.AreEqual("lowercase", Contract.TestToLower("lowercase"));
            AssertGasConsumed(1877550);
        }

        [TestMethod]
        public void Test_TestToUpper()
        {
            Assert.AreEqual("HELLO WORLD", Contract.TestToUpper("Hello World"));
            AssertGasConsumed(2011590);

            Assert.AreEqual("123", Contract.TestToUpper("123"));
            AssertGasConsumed(1488390);

            // Test with already uppercase string
            Assert.AreEqual("UPPERCASE", Contract.TestToUpper("UPPERCASE"));
            AssertGasConsumed(1877550);
        }

        [TestMethod]
        public void Test_TestTrimChar()
        {
            Assert.AreEqual("Hello World", Contract.TestTrimChar("***Hello World***", '*'));
            AssertGasConsumed(1134300);

            Assert.AreEqual("Test", Contract.TestTrimChar("Test", '*'));
            AssertGasConsumed(1115580);

            // Test with string containing only trim characters
            Assert.AreEqual("", Contract.TestTrimChar("****", '*'));
            AssertGasConsumed(1123260);
        }

        [TestMethod]
        public void Test_TestLength()
        {
            Assert.AreEqual(11, Contract.TestLength("Hello World"));
            AssertGasConsumed(1047360);

            Assert.AreEqual(0, Contract.TestLength(""));
            AssertGasConsumed(1047360);

            // Test with very long string
            Assert.AreEqual(1000, Contract.TestLength(new string('a', 1000)));
            AssertGasConsumed(1062480);
        }

        [TestMethod]
        public void Test_StringBuilderBasic()
        {
            Assert.AreEqual("neo compiler\nruntime\n", Contract.TestStringBuilderBasic());
            AssertGasConsumed(2908200);
        }

        [TestMethod]
        public void Test_StringBuilderLength()
        {
            Assert.AreEqual(14, Contract.TestStringBuilderLength());
            AssertGasConsumed(3156150);
        }

        [TestMethod]
        public void Test_StringBuilderClear()
        {
            Assert.AreEqual("neo\ncontracts", Contract.TestStringBuilderClear());
            AssertGasConsumed(1978680);
        }

        [TestMethod]
        public void Test_StringBuilderAppendBuilder()
        {
            Assert.AreEqual("neo compiler\npreview", Contract.TestStringBuilderAppendBuilder());
            AssertGasConsumed(2418510);
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
