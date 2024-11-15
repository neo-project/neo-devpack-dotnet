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
            AssertGasConsumed(2836110);

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<TestException>(() => Contract.Test());
            AssertGasConsumed(2851680);
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle

            Contract.ReentrantTest(0);
            AssertGasConsumed(6801150);

            // Method end

            Contract.ReentrantTest(1);
            AssertGasConsumed(6802200);

            // Reentrant test

            var ex = Assert.ThrowsException<TestException>(() => Contract.ReentrantTest(123));
            AssertGasConsumed(6820170);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }

        [TestMethod]
        public void TestReentrant2()
        {
            // should be ok
            Contract.ReentrantB();
            AssertGasConsumed(6537040);

            // Reentrant test
            var ex = Assert.ThrowsException<TestException>(Contract.ReentrantA);
            AssertGasConsumed(7440100);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
