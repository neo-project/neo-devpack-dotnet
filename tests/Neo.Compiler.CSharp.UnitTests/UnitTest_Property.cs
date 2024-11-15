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
            Assert.IsFalse(res);
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
            Assert.AreEqual(0, Contract.TestPropertyInc());
            Assert.AreEqual(6, Contract.TestStaticPropertyInc());
            Assert.IsFalse(Contract.ToggleProtectedProperty());
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
    }
}
