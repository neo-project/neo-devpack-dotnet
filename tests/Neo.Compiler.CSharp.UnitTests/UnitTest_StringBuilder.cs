// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringBuilder.cs file belongs to the neo project and is free
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
    public class UnitTest_StringBuilder : DebugAndTestBase<Contract_StringBuilder>
    {
        protected override bool TestGasConsume => false;

        [TestMethod]
        public void TestAppendPrimitiveValues()
        {
            Assert.AreEqual("-1|2|-3|4|-5|6|-7|8|True|False", Contract.AppendPrimitiveValues());
        }
    }
}
