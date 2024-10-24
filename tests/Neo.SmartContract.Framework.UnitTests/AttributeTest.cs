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
            AssertGasConsumed(2836230);

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<TestException>(() => Contract.Test());
            AssertGasConsumed(2851800);
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle

            Contract.ReentrantTest(0);
            AssertGasConsumed(6802290);

            // Method end

            Contract.ReentrantTest(1);
            AssertGasConsumed(6803340);

            // Reentrant test

            var ex = Assert.ThrowsException<TestException>(() => Contract.ReentrantTest(123));
            AssertGasConsumed(6822210);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }

        [TestMethod]
        public void TestReentrant2()
        {
            // should be ok
            Contract.ReentrantB();
            AssertGasConsumed(6538180);

            // Reentrant test
            var ex = Assert.ThrowsException<TestException>(Contract.ReentrantA);
            AssertGasConsumed(7442140);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
