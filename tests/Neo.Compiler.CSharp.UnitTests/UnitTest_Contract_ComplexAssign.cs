// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Contract_ComplexAssign.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CompoundAssignments : DebugAndTestBase<Contract_ComplexAssign>
    {
        [TestMethod]
        public void Test_AddAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Add_Assign_Checked);
            AssertGasConsumed(1001970);
        }

        [TestMethod]
        public void Test_AddAssign_UnChecked()
        {
            var values = Contract.UnitTest_Add_Assign_UnChecked()!;
            AssertGasConsumed(1049760);
            // Asserting the expected values after overflow
            Assert.AreEqual(BigInteger.Zero, values[0]); // uint.MaxValue + 1 overflows to 0
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 1)), values[1]); // int.MaxValue + 1 overflows to int.MinValue
        }

        [TestMethod]
        public void Test_SubAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Sub_Assign_Checked);
            AssertGasConsumed(1001820);
        }

        [TestMethod]
        public void Test_SubAssign_UnChecked()
        {
            var values = Contract.UnitTest_Sub_Assign_UnChecked()!;
            AssertGasConsumed(1049310);
            // Asserting the expected values after underflow
            Assert.AreEqual(new BigInteger(uint.MaxValue), values[0]); // uint.MinValue - 1 underflows to uint.MaxValue
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 1)), values[1]); // int.MinValue - 1 underflows to int.MaxValue
        }

        [TestMethod]
        public void Test_MulAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Mul_Assign_Checked);
            AssertGasConsumed(1002120);
        }

        [TestMethod]
        public void Test_MulAssign_UnChecked()
        {
            var values = Contract.UnitTest_Mul_Assign_UnChecked()!;
            AssertGasConsumed(1050060);
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue * 2)), values[0]); // Multiplying by 2 should not change the value
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue * 2)), values[1]); // Same here
        }

        [TestMethod]
        public void Test_LeftShiftAssign_Checked()
        {
            Assert.ThrowsException<TestException>(Contract.UnitTest_Left_Shift_Assign_Checked);
            AssertGasConsumed(1002120);
        }

        [TestMethod]
        public void Test_LeftShiftAssign_UnChecked()
        {
            var values = Contract.UnitTest_Left_Shift_Assign_UnChecked()!;
            AssertGasConsumed(1050060);
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue << 1)), values[0]);
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue << 1)), values[1]);
        }

        [TestMethod]
        public void Test_RightShiftAssign_Checked()
        {
            var values = Contract.UnitTest_Right_Shift_Assign_Checked()!;
            AssertGasConsumed(1049100);
            Assert.AreEqual(new BigInteger(checked(uint.MinValue >> 1)), values[0]);
            Assert.AreEqual(new BigInteger(checked(int.MinValue >> 1)), values[1]);
        }

        [TestMethod]
        public void Test_RightShiftAssign_UnChecked()
        {
            var values = Contract.UnitTest_Right_Shift_Assign_UnChecked()!;
            AssertGasConsumed(1049100);
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue >> 1)), values[0]);
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue >> 1)), values[1]);
        }

        [TestMethod]
        public void Test_Member_Element_Complex_Assign()
        {
            Contract.UnitTest_Member_Element_Complex_Assign();
            AssertGasConsumed(1800780);
        }
    }
}
