// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Delegate.cs file belongs to the neo project and is free
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
    public class UnitTest_Delegate : DebugAndTestBase<Contract_Delegate>
    {
        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(5, Contract.SumFunc(2, 3));
            AssertGasConsumed(1065180);
        }

        [TestMethod]
        public void TestDelegate()
        {
            Contract.TestDelegate();
            AssertGasConsumed(3400740);
        }
    }
}
