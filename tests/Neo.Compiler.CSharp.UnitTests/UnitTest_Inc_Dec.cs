using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inc_Dec : TestBase<Contract_Inc_Dec>
    {
        public UnitTest_Inc_Dec() : base(Contract_Inc_Dec.Nef, Contract_Inc_Dec.Manifest) { }

        [TestMethod]
        public void Test_Property_Inc_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Property_Inc_Checked());
        }

        [TestMethod]
        public void Test_Property_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked());
        }

        [TestMethod]
        public void Test_Property_Dec_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Property_Dec_Checked());
        }

        [TestMethod]
        public void Test_Property_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked());
        }

        [TestMethod]
        public void Test_Local_Inc_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Local_Inc_Checked());
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked());
        }

        [TestMethod]
        public void Test_Local_Dec_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Local_Dec_Checked());
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked());
        }

        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Param_Inc_Checked(0));
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked(0));
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Param_Dec_Checked(0));
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked(0));
        }

        // Test Methods for int type
        [TestMethod]
        public void Test_IntProperty_Inc_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Property_Inc_Checked_Int());
        }

        [TestMethod]
        public void Test_IntProperty_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked_Int());
        }

        [TestMethod]
        public void Test_IntProperty_Dec_Checked()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Property_Dec_Checked_Int());
        }

        [TestMethod]
        public void Test_IntProperty_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked_Int());
        }

        // Local Variable Tests for int
        [TestMethod]
        public void Test_Local_Inc_Checked_Int()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Local_Inc_Checked_Int());
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked_Int());
        }

        [TestMethod]
        public void Test_Local_Dec_Checked_Int()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Local_Dec_Checked_Int());
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked_Int());
        }

        // Parameter Tests for int
        [TestMethod]
        public void Test_Param_Inc_Checked_Int()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Param_Inc_Checked_Int(0));
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(0));
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.UnitTest_Param_Dec_Checked_Int(0));
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(0));
        }

        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            Contract.UnitTest_Not_DeadLoop(); // No error
        }
    }
}
