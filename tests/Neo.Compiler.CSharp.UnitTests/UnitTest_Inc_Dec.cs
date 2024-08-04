using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inc_Dec : TestBase2<Contract_Inc_Dec>
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
            Assert.AreEqual(986370, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(986280, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(988170, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(988080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            Contract.UnitTest_Param_Inc_Checked(0);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked(0));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(0));
            Assert.AreEqual(1063140, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_Checked(uint.MaxValue);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(uint.MinValue));
            Assert.AreEqual(1063140, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(-1));
            Assert.AreEqual(1063140, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(1));
            Assert.AreEqual(1063860, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked(0));
            Assert.AreEqual(1049010, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MaxValue);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MinValue);
            Assert.AreEqual(1049010, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_UnChecked(-1);
            Assert.AreEqual(1049010, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_UnChecked(1);
            Assert.AreEqual(1049010, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(986790, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(986430, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(988590, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(988230, Engine.FeeConsumed.Value);
        }

        // Parameter Tests for int
        [TestMethod]
        public void Test_Param_Inc_Checked_Int()
        {
            Assert.AreEqual(new BigInteger(checked(0 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(0));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Inc_Checked_Int(int.MaxValue));
            Assert.AreEqual(1063290, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(checked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_Checked_Int(int.MinValue));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(checked(-1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(-1));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(checked(1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(1));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(0));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MaxValue));
            Assert.AreEqual(1049520, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MinValue));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(2));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(-2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(-2));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            Contract.UnitTest_Param_Dec_Checked_Int(0);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_Checked_Int(int.MaxValue);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked_Int(int.MinValue));
            Assert.AreEqual(1063140, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_Checked_Int(1);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Contract.UnitTest_Param_Dec_Checked_Int(-1);
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(0));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MaxValue));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MinValue));
            Assert.AreEqual(1049160, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(2));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Assert.AreEqual(new BigInteger(unchecked(-2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(-2));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            Contract.UnitTest_Not_DeadLoop(); // No error
            Assert.AreEqual(993450, Engine.FeeConsumed.Value);
        }
    }
}
