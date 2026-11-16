// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Inc_Dec.cs file belongs to the neo project and is free
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
    public class UnitTest_Inc_Dec : DebugAndTestBase<Contract_Inc_Dec>
    {
        [TestMethod]
        public void Test_Property_Inc_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Property_Inc_Checked());
        }

        [TestMethod]
        public void Test_Property_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked());
        }

        [TestMethod]
        public void Test_Property_Dec_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Property_Dec_Checked());
        }

        [TestMethod]
        public void Test_Property_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked());
        }

        [TestMethod]
        public void Test_Local_Inc_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Local_Inc_Checked());
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked());
        }

        [TestMethod]
        public void Test_Local_Dec_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Local_Dec_Checked());
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked());
        }

        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            Contract.UnitTest_Param_Inc_Checked(0);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked(0));
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Param_Dec_Checked(0));

            Contract.UnitTest_Param_Dec_Checked(uint.MaxValue);

            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Param_Dec_Checked(uint.MinValue));

            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Param_Dec_Checked(-1));

            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Param_Dec_Checked(1));
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked(0));

            Contract.UnitTest_Param_Dec_UnChecked(uint.MaxValue);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MinValue);

            Contract.UnitTest_Param_Dec_UnChecked(-1);

            Contract.UnitTest_Param_Dec_UnChecked(1);
        }

        // Test Methods for int type
        [TestMethod]
        public void Test_IntProperty_Inc_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Property_Inc_Checked_Int());
        }

        [TestMethod]
        public void Test_IntProperty_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked_Int());
        }

        [TestMethod]
        public void Test_IntProperty_Dec_Checked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Property_Dec_Checked_Int());
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
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Local_Inc_Checked_Int());
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked_Int());
        }

        [TestMethod]
        public void Test_Local_Dec_Checked_Int()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Local_Dec_Checked_Int());
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
            Assert.AreEqual(new BigInteger(checked(0 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(0));

            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Param_Inc_Checked_Int(int.MaxValue));

            Assert.AreEqual(new BigInteger(checked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_Checked_Int(int.MinValue));

            Assert.AreEqual(new BigInteger(checked(-1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(-1));

            Assert.AreEqual(new BigInteger(checked(1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(1));
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(0));

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MaxValue));

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MinValue));

            Assert.AreEqual(new BigInteger(unchecked(2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(2));

            Assert.AreEqual(new BigInteger(unchecked(-2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(-2));
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            Contract.UnitTest_Param_Dec_Checked_Int(0);

            Contract.UnitTest_Param_Dec_Checked_Int(int.MaxValue);

            Assert.ThrowsExactly<TestException>(() => Contract.UnitTest_Param_Dec_Checked_Int(int.MinValue));

            Contract.UnitTest_Param_Dec_Checked_Int(1);

            Contract.UnitTest_Param_Dec_Checked_Int(-1);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(0));

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MaxValue));

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MinValue));

            Assert.AreEqual(new BigInteger(unchecked(2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(2));

            Assert.AreEqual(new BigInteger(unchecked(-2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(-2));
        }

        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            Contract.UnitTest_Not_DeadLoop(); // No error
        }
    }
}
