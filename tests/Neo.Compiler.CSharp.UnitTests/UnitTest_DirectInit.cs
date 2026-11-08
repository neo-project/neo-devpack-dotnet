// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DirectInit.cs file belongs to the neo project and is free
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
    [TestClass]
    public class UnitTest_DirectInit : DebugAndTestBase<Contract_DirectInit>
    {
        [TestMethod]
        public void Test_GetUInt160()
        {
            Assert.AreEqual("0x71a87191aef3fcf5e4441d791ded67ebab1aee7e", Contract.TestGetUInt160()?.ToString());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_GetECPoint()
        {
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.TestGetECPoint()?.ToString());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_GetUInt256()
        {
            Assert.AreEqual("0x25898c9489b9c7f07adab10f995b3e492a23dbd79ae24f1a91c24e107986cfed", Contract.TestGetUInt256()?.ToString());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_GetString()
        {
            Assert.AreEqual("hello world", Contract.TestGetString());
            AssertGasConsumed(984270);
        }
    }
}
