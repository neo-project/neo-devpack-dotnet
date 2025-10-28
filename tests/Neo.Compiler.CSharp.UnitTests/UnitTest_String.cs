// Copyright (C) 2015-2025 The Neo Project.
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
            AssertGasConsumed(1047900);

            Assert.IsTrue(Contract.TestCompare("alpha", "beta") < 0);
            AssertGasConsumed(1047900);

            Assert.IsTrue(Contract.TestCompare("beta", "alpha") > 0);
            AssertGasConsumed(1047900);
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
            Assert.AreEqual("SByte: -42, Byte: 42, UShort: 1000, UInt: 1000000, ULong: 1000000000000, BigInteger: 1000000000000000000000, Char: A, String: Hello, ECPoint: NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq, ByteString: System.Byte[], Bool: True", Contract.TestInterpolatedStringHandler());
            Assert.AreEqual(11313480, Engine.FeeConsumed.Value);
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
