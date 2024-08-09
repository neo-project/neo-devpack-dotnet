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
            Assert.AreEqual(3075900, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(7625310, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1970970, Engine.FeeConsumed.Value);
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("True", log[0]);
        }

        [TestMethod]
        public void Test_TestEmpty()
        {
            Assert.AreEqual("", Contract.TestEmpty());
            Assert.AreEqual(984270, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestIsNullOrEmpty()
        {
            Assert.IsTrue(Contract.TestIsNullOrEmpty(""));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestIsNullOrEmpty(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);

            Assert.IsFalse(Contract.TestIsNullOrEmpty("hello world"));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith("hello world"));
            Assert.AreEqual(1357710, Engine.FeeConsumed.Value);

            Assert.IsFalse(Contract.TestEndWith("hel"));
            Assert.AreEqual(1049310, Engine.FeeConsumed.Value);

            Assert.IsFalse(Contract.TestEndWith("hello"));
            Assert.AreEqual(1049310, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestContains()
        {
            Assert.IsTrue(Contract.TestContains("hello world"));
            Assert.AreEqual(2032800, Engine.FeeConsumed.Value);

            Assert.IsFalse(Contract.TestContains("hello"));
            Assert.AreEqual(2032800, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestIndexOf()
        {
            Assert.AreEqual(6, Contract.TestIndexOf("hello world"));
            Assert.AreEqual(2032530, Engine.FeeConsumed.Value);

            Assert.AreEqual(-1, Contract.TestIndexOf("hello"));
            Assert.AreEqual(2032530, Engine.FeeConsumed.Value);
        }
    }
}
