// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_PartialCrossFile.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    /// <summary>
    /// Tests for partial class compilation with cross-file method invocations.
    /// This verifies the fix for issue #1363 where syntax nodes from different
    /// syntax trees caused NC5001 errors during compilation.
    /// </summary>
    [TestClass]
    public class UnitTest_PartialCrossFile : DebugAndTestBase<Contract_PartialCrossFile>
    {
        [TestMethod]
        public void Test_GetBaseValue()
        {
            // Test method from first partial file returning constant
            Assert.AreEqual(100, Contract.GetBaseValue());
        }

        [TestMethod]
        public void Test_GetMultiplier()
        {
            // Test method from second partial file returning constant
            Assert.AreEqual(5, Contract.GetMultiplier());
        }

        [TestMethod]
        public void Test_CrossFileCall()
        {
            // Test method in first file calling method from second file
            // GetMultiplier() * BaseValue = 5 * 100 = 500
            Assert.AreEqual(500, Contract.TestCrossFileCall());
        }

        [TestMethod]
        public void Test_CrossFileCallReverse()
        {
            // Test method in second file calling method from first file
            // GetBaseValue() + Multiplier = 100 + 5 = 105
            Assert.AreEqual(105, Contract.TestCrossFileCallReverse());
        }

        [TestMethod]
        public void Test_ExpressionBodyCrossFile()
        {
            // Test expression-bodied member using constants from both files
            // BaseValue + Multiplier = 100 + 5 = 105
            Assert.AreEqual(105, Contract.ExpressionBodyTest());
        }

        [TestMethod]
        public void Test_ComplexCrossFileExpression()
        {
            // Test complex expression using constants and methods from both files
            // (BaseValue * Multiplier) + GetBaseValue() = (100 * 5) + 100 = 600
            Assert.AreEqual(600, Contract.ComplexCrossFileExpression());
        }
    }
}
