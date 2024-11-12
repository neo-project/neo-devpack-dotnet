using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property : DebugAndTestBase<Contract_Property>
    {
        [TestMethod]
        public void TestABIOffsetWithoutOptimizer()
        {
            var property = Contract_Property.Manifest.Abi.Methods[0];
            Assert.AreEqual("symbol", property.Name);
        }

        [TestMethod]
        public void TestStaticPropertyDefaultInc()
        {
            var res = Contract.TestStaticPropertyDefaultInc();
            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void TestStaticPropertyValueInc()
        {
            var res = Contract.TestStaticPropertyValueInc();
            Assert.AreEqual(11, res);
        }

        [TestMethod]
        public void TestPropertyDefaultInc()
        {
            var res = Contract.TestPropertyDefaultInc();
            Assert.AreEqual(2, res);
        }

        [TestMethod]
        public void TestPropertyValueInc()
        {
            var res = Contract.TestPropertyValueInc();
            Assert.AreEqual(13, res);
        }

        [TestMethod]
        public void TestStaticFieldDefaultInc()
        {
            var res = Contract.IncTestStaticFieldDefault();
            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void TestStaticFieldValueInc()
        {
            var res = Contract.IncTestStaticFieldValue();
            Assert.AreEqual(2, res);  // Initial value 1 + increment
        }

        [TestMethod]
        public void TestFieldDefaultInc()
        {
            var res = Contract.IncTestFieldDefault();
            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void TestFieldValueInc()
        {
            var res = Contract.IncTestFieldValue();
            Assert.AreEqual(2, res);  // Initial value 2 + increment
        }
    }
}
