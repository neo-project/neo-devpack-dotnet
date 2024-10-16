using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class AttributeTest : DebugAndTestBase<Contract_Attribute>
    {
        [TestMethod]
        public void TestAttribute()
        {
            Engine.SetTransactionSigners(UInt160.Zero);
            Assert.IsTrue(Contract.Test());
            AssertGasConsumed(2853570);

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<TestException>(() => Contract.Test());
            AssertGasConsumed(2869140);
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle

            Contract.ReentrantTest(0);
            AssertGasConsumed(6819630);

            // Method end

            Contract.ReentrantTest(1);
            AssertGasConsumed(6820680);

            // Reentrant test

            var ex = Assert.ThrowsException<TestException>(() => Contract.ReentrantTest(123));
            AssertGasConsumed(6839550);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }

        [TestMethod]
        public void TestReentrant2()
        {
            // should be ok
            Contract.ReentrantB();
            AssertGasConsumed(6555520);

            // Reentrant test
            var ex = Assert.ThrowsException<TestException>(Contract.ReentrantA);
            AssertGasConsumed(7476820);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
