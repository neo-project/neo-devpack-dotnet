using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;
using Neo.SmartContract.Testing.Exceptions;

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
        public void Test_TestIndexOf()
        {
            Assert.AreEqual(6, Contract.TestIndexOf("hello world"));
            AssertGasConsumed(2032470);

            Assert.AreEqual(-1, Contract.TestIndexOf("hello"));
            AssertGasConsumed(2032470);
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
    }
}
