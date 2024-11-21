using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inc_Dec : DebugAndTestBase<Contract_Inc_Dec>
    {
        [TestMethod]
        public void Test_Property_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Inc_Checked());
            AssertGasConsumed(1000440);
        }

        [TestMethod]
        public void Test_Property_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked());
            AssertGasConsumed(986130);
        }

        [TestMethod]
        public void Test_Property_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Dec_Checked());
            AssertGasConsumed(1000290);
        }

        [TestMethod]
        public void Test_Property_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked());
            AssertGasConsumed(986040);
        }

        [TestMethod]
        public void Test_Local_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Inc_Checked());
            AssertGasConsumed(1002360);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked());
            AssertGasConsumed(988050);
        }

        [TestMethod]
        public void Test_Local_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Dec_Checked());
            AssertGasConsumed(1002210);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked());
            AssertGasConsumed(987960);
        }

        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            Contract.UnitTest_Param_Inc_Checked(0);
            AssertGasConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked(0));
            AssertGasConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(0));
            AssertGasConsumed(1063140);

            Contract.UnitTest_Param_Dec_Checked(uint.MaxValue);
            AssertGasConsumed(1048710);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(uint.MinValue));
            AssertGasConsumed(1063140);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(-1));
            AssertGasConsumed(1063140);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(1));
            AssertGasConsumed(1063740);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked(0));
            AssertGasConsumed(1048890);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MaxValue);
            AssertGasConsumed(1048710);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MinValue);
            AssertGasConsumed(1048890);

            Contract.UnitTest_Param_Dec_UnChecked(-1);
            AssertGasConsumed(1048890);

            Contract.UnitTest_Param_Dec_UnChecked(1);
            AssertGasConsumed(1048890);
        }

        // Test Methods for int type
        [TestMethod]
        public void Test_IntProperty_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Inc_Checked_Int());
            AssertGasConsumed(1000440);
        }

        [TestMethod]
        public void Test_IntProperty_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked_Int());
            AssertGasConsumed(986550);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Dec_Checked_Int());
            AssertGasConsumed(1000290);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked_Int());
            AssertGasConsumed(986190);
        }

        // Local Variable Tests for int
        [TestMethod]
        public void Test_Local_Inc_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Inc_Checked_Int());
            AssertGasConsumed(1002360);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked_Int());
            AssertGasConsumed(988470);
        }

        [TestMethod]
        public void Test_Local_Dec_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Dec_Checked_Int());
            AssertGasConsumed(1002210);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked_Int());
            AssertGasConsumed(988110);
        }

        // Parameter Tests for int
        [TestMethod]
        public void Test_Param_Inc_Checked_Int()
        {
            Assert.AreEqual(new BigInteger(checked(0 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(0));
            AssertGasConsumed(1048710);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Inc_Checked_Int(int.MaxValue));
            AssertGasConsumed(1063290);

            Assert.AreEqual(new BigInteger(checked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_Checked_Int(int.MinValue));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(checked(-1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(-1));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(checked(1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(1));
            AssertGasConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(0));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MaxValue));
            AssertGasConsumed(1049400);

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MinValue));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(2));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(-2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(-2));
            AssertGasConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            Contract.UnitTest_Param_Dec_Checked_Int(0);
            AssertGasConsumed(1048710);

            Contract.UnitTest_Param_Dec_Checked_Int(int.MaxValue);
            AssertGasConsumed(1048710);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked_Int(int.MinValue));
            AssertGasConsumed(1063140);

            Contract.UnitTest_Param_Dec_Checked_Int(1);
            AssertGasConsumed(1048710);

            Contract.UnitTest_Param_Dec_Checked_Int(-1);
            AssertGasConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(0));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MaxValue));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MinValue));
            AssertGasConsumed(1049040);

            Assert.AreEqual(new BigInteger(unchecked(2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(2));
            AssertGasConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(-2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(-2));
            AssertGasConsumed(1048710);
        }

        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            Contract.UnitTest_Not_DeadLoop(); // No error
            AssertGasConsumed(993450);
        }
    }
}
