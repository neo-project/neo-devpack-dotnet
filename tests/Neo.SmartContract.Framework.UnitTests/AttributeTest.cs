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
            AssertGasConsumed(3284760);

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<TestException>(() => Contract.Test());
            AssertGasConsumed(3300330);
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle

            Contract.ReentrantTest(0);
            AssertGasConsumed(7286040);

            // Method end

            Contract.ReentrantTest(1);
            AssertGasConsumed(7287090);

            // Reentrant test

            var ex = Assert.ThrowsException<TestException>(() => Contract.ReentrantTest(123));
            AssertGasConsumed(7306020);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }

        [TestMethod]
        public void TestReentrant2()
        {
            // should be ok
            Contract.ReentrantB();
            AssertGasConsumed(7023850);

            // Reentrant test
            var ex = Assert.ThrowsException<TestException>(Contract.ReentrantA);
            AssertGasConsumed(8193010);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
