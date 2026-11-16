// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CSharp15.cs file belongs to the neo project and is free
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
    public class UnitTest_CSharp15 : DebugAndTestBase<Contract_CSharp15>
    {
        [TestMethod]
        public void Test_ConvertToMillimeters()
        {
            Assert.AreEqual(3000, Contract.ConvertToMillimeters(3));
        }

        [TestMethod]
        public void Test_DescribeClosedGate()
        {
            Assert.AreEqual("closed", Contract.DescribeClosedGate());
        }

        [TestMethod]
        public void Test_DescribeGate()
        {
            Assert.AreEqual("open", Contract.DescribeGate(true));
            Assert.AreEqual("closed", Contract.DescribeGate(false));
        }
    }
}
