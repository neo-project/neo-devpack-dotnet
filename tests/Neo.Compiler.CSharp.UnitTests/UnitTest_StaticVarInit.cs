// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StaticVarInit.cs file belongs to the neo project and is free
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
    public class UnitTest_StaticVarInit : DebugAndTestBase<Contract_StaticVarInit>
    {
        [TestMethod]
        public void Test_StaticVarInit()
        {
            var var1 = Contract.StaticInit();
            AssertGasConsumed(1000470);
            Assert.AreEqual(var1, Contract.Hash);

            var var2 = Contract.DirectGet();
            AssertGasConsumed(985530);
            Assert.AreEqual(var1, var2);
        }
    }
}
