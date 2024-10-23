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
            AssertGasConsumed(3021600);

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<TestException>(() => Contract.Test());
            AssertGasConsumed(3037170);
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle

            Contract.ReentrantTest(0);
            AssertGasConsumed(6987660);

            // Method end

            Contract.ReentrantTest(1);
            AssertGasConsumed(6988710);

            // Reentrant test

            var ex = Assert.ThrowsException<TestException>(() => Contract.ReentrantTest(123));
            AssertGasConsumed(7007580);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }

        [TestMethod]
        public void TestReentrant2()
        {
            // should be ok
            Contract.ReentrantB();
            AssertGasConsumed(6723550);

            // Reentrant test
            var ex = Assert.ThrowsException<TestException>(Contract.ReentrantA);
            AssertGasConsumed(7627570);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
