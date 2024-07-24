using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
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
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Inc_Checked());
            Assert.AreEqual(1000560, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Property_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked());
            Assert.AreEqual(986430, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Property_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Dec_Checked());
            Assert.AreEqual(1000410, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Property_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked());
            Assert.AreEqual(986340, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Inc_Checked());
            Assert.AreEqual(1002360, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked());
            Assert.AreEqual(988230, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Dec_Checked());
            Assert.AreEqual(1002210, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked());
            Assert.AreEqual(988140, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Inc_Checked(0));
            Assert.AreEqual(1063500, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked(0));
            Assert.AreEqual(1049370, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(0));
            Assert.AreEqual(1063350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked(0));
            Assert.AreEqual(1049280, Engine.FeeConsumed.Value);
        }

        // Test Methods for int type
        [TestMethod]
        public void Test_IntProperty_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Inc_Checked_Int());
            Assert.AreEqual(1000560, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_IntProperty_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked_Int());
            Assert.AreEqual(986850, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Dec_Checked_Int());
            Assert.AreEqual(1000410, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked_Int());
            Assert.AreEqual(986490, Engine.FeeConsumed.Value);
        }

        // Local Variable Tests for int
        [TestMethod]
        public void Test_Local_Inc_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Inc_Checked_Int());
            Assert.AreEqual(1002360, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked_Int());
            Assert.AreEqual(988650, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Dec_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Dec_Checked_Int());
            Assert.AreEqual(1002210, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked_Int());
            Assert.AreEqual(988290, Engine.FeeConsumed.Value);
        }

        // Parameter Tests for int
        [TestMethod]
        public void Test_Param_Inc_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Inc_Checked_Int(0));
            Assert.AreEqual(1063500, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(0));
            Assert.AreEqual(1049790, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked_Int(0));
            Assert.AreEqual(1063350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(0));
            Assert.AreEqual(1049430, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            Contract.UnitTest_Not_DeadLoop(); // No error
            Assert.AreEqual(993450, Engine.FeeConsumed.Value);
        }
    }
}
