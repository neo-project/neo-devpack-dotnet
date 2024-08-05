using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CompoundAssignments : TestBase<Contract_ComplexAssign>
    {
        [TestMethod]
        public void Test_AddAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Add_Assign_Checked);
            Assert.AreEqual(1002120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_AddAssign_UnChecked()
        {
            var values = Contract.UnitTest_Add_Assign_UnChecked()!;
            Assert.AreEqual(1480950, Engine.FeeConsumed.Value);
            // Asserting the expected values after overflow
            Assert.AreEqual(BigInteger.Zero, values[0]); // uint.MaxValue + 1 overflows to 0
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 1)), values[1]); // int.MaxValue + 1 overflows to int.MinValue
        }

        [TestMethod]
        public void Test_SubAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Sub_Assign_Checked);
            Assert.AreEqual(1001970, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_SubAssign_UnChecked()
        {
            var values = Contract.UnitTest_Sub_Assign_UnChecked()!;
            Assert.AreEqual(1480500, Engine.FeeConsumed.Value);
            // Asserting the expected values after underflow
            Assert.AreEqual(new BigInteger(uint.MaxValue), values[0]); // uint.MinValue - 1 underflows to uint.MaxValue
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 1)), values[1]); // int.MinValue - 1 underflows to int.MaxValue
        }

        [TestMethod]
        public void Test_MulAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Mul_Assign_Checked);
            Assert.AreEqual(1002120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_MulAssign_UnChecked()
        {
            var values = Contract.UnitTest_Mul_Assign_UnChecked()!;
            Assert.AreEqual(1480950, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue * 2)), values[0]); // Multiplying by 2 should not change the value
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue * 2)), values[1]); // Same here
        }

        [TestMethod]
        public void Test_LeftShiftAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Left_Shift_Assign_Checked);
            Assert.AreEqual(1002120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_LeftShiftAssign_UnChecked()
        {
            var values = Contract.UnitTest_Left_Shift_Assign_UnChecked()!;
            Assert.AreEqual(1480950, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue << 1)), values[0]);
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue << 1)), values[1]);
        }

        [TestMethod]
        public void Test_RightShiftAssign_Checked()
        {
            var values = Contract.UnitTest_Right_Shift_Assign_Checked()!;
            Assert.AreEqual(1479990, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(checked(uint.MinValue >> 1)), values[0]);
            Assert.AreEqual(new BigInteger(checked(int.MinValue >> 1)), values[1]);
        }

        [TestMethod]
        public void Test_RightShiftAssign_UnChecked()
        {
            var values = Contract.UnitTest_Right_Shift_Assign_UnChecked()!;
            Assert.AreEqual(1479990, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue >> 1)), values[0]);
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue >> 1)), values[1]);
        }
    }
}
