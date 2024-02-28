using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class AttributeTest : TestBase<Contract_Attribute>
    {
        public AttributeTest() : base(Contract_Attribute.Nef, Contract_Attribute.Manifest) { }

        [TestMethod]
        public void attribute_test()
        {
            Engine.SetTransactionSigners(UInt160.Zero);
            Assert.IsTrue(Contract.Test());

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsException<VMUnhandledException>(() => Contract.Test());
        }

        [TestMethod]
        public void reentrant_test()
        {
            // return in the middle

            Contract.ReentrantTest(0);

            // Method end

            Contract.ReentrantTest(1);

            // Reentrant test

            var ex = Assert.ThrowsException<Exception>(() => Contract.ReentrantTest(123));
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
