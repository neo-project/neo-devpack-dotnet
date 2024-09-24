using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;

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
            AssertGasConsumed(1047810);

            Assert.IsTrue(Contract.TestIsNullOrEmpty(null));
            AssertGasConsumed(1047300);

            Assert.IsFalse(Contract.TestIsNullOrEmpty("hello world"));
            AssertGasConsumed(1047810);
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
    }
}
