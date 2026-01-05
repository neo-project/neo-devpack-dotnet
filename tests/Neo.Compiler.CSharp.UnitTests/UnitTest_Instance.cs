// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Instance.cs file belongs to the neo project and is free
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
    public class UnitTest_Instance : DebugAndTestBase<Contract_Instance>
    {
        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(3, Contract.Sum(2));
            AssertGasConsumed(1376940);
            Assert.AreEqual(4, Contract.Sum(3));
            AssertGasConsumed(1376940);
            Assert.AreEqual(8, Contract.Sum2(3));
            AssertGasConsumed(1414890);
        }
    }
}
