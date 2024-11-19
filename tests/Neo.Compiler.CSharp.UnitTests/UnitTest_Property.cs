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
        public void TestStaticPropertyInc()
        {
            var res = Contract.TestStaticPropertyInc();
            Assert.AreEqual(6, res);
        }

        [TestMethod]
        public void TestPropertyInc()
        {
            var res = Contract.TestPropertyInc();
            Assert.AreEqual(6, res);
        }

        [TestMethod]
        public void TestStaticPropertyDec()
        {
            var res = Contract.TestStaticPropertyDec();
            Assert.AreEqual(-6, res);
        }

        [TestMethod]
        public void TestPropertyDec()
        {
            var res = Contract.TestPropertyDec();
            Assert.AreEqual(-6, res);
        }

        [TestMethod]
        public void TestStaticPropertyMul()
        {
            var res = Contract.TestStaticPropertyMul();
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void TestPropertyMul()
        {
            var res = Contract.TestPropertyMul();
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void TestConcatPublicProperties()
        {
            var res = Contract.ConcatPublicProperties();
            Assert.AreEqual("TestInitial", res);
        }

        [TestMethod]
        public void TestToggleProtectedProperty()
        {
            var res = Contract.ToggleProtectedProperty();
            Assert.IsTrue(res);
            res = Contract.ToggleProtectedProperty();
            Assert.IsTrue(res);  // this is static field initialized with false
        }

        [TestMethod]
        public void TestGetComputedValue()
        {
            var res = Contract.GetComputedValue();
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void TestReset()
        {
            Contract.TestPropertyInc();
            Contract.TestStaticPropertyInc();
            Contract.SetPublicProperty("Changed");
            Contract.SetPublicStaticProperty("Changed");
            Contract.ToggleProtectedProperty();

            Contract.Reset();

            Assert.AreEqual("Test", Contract.PublicProperty());
            Assert.AreEqual("Initial", Contract.PublicStaticProperty());
            Assert.AreEqual(6, Contract.TestPropertyInc());
            Assert.AreEqual(6, Contract.TestStaticPropertyInc());
            Assert.IsTrue(Contract.ToggleProtectedProperty());
        }

        [TestMethod]
        public void TestUninitializedPropertyInc()
        {
            var res = Contract.UninitializedPropertyInc();
            Assert.AreEqual(6, res);
        }

        [TestMethod]
        public void TestUninitializedPropertyDec()
        {
            var res = Contract.UninitializedPropertyDec();
            Assert.AreEqual(-6, res);
        }

        [TestMethod]
        public void TestUninitializedPropertyMul()
        {
            var res = Contract.UninitializedPropertyMul();
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void TestUninitializedStaticPropertyInc()
        {
            var res = Contract.UninitializedStaticPropertyInc();
            Assert.AreEqual(6, res);
        }

        [TestMethod]
        public void TestUninitializedStaticPropertyDec()
        {
            var res = Contract.UninitializedStaticPropertyDec();
            Assert.AreEqual(-6, res);
        }

        [TestMethod]
        public void TestUninitializedStaticPropertyMul()
        {
            var res = Contract.UninitializedStaticPropertyMul();
            Assert.AreEqual(0, res);
        }

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
