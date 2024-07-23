using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class AttributeTest : TestBase<Contract_Attribute>
    {
        public AttributeTest() : base(Contract_Attribute.Nef, Contract_Attribute.Manifest) { }

        [TestMethod]
        public void TestAttribute()
        {
            Engine.SetTransactionSigners(UInt160.Zero);
            Assert.IsTrue(Contract.Test());

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<TestException>(() => Contract.Test());
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle

            Contract.ReentrantTest(0);
            Assert.AreEqual(1008300070, Engine.FeeConsumed.Value);

            // Method end

            Contract.ReentrantTest(1);
            Assert.AreEqual(1015525390, Engine.FeeConsumed.Value);

            // Reentrant test

            var ex = Assert.ThrowsException<TestException>(() => Contract.ReentrantTest(123));
            Assert.AreEqual(1022769640, Engine.FeeConsumed.Value);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
