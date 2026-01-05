// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Multiple.cs file belongs to the neo project and is free
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
    public class UnitTest_Multiple
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            var engine = new TestEngine(true);
            var a = engine.Deploy<Contract_MultipleA>(Contract_MultipleA.Nef, Contract_MultipleA.Manifest);
            var b = engine.Deploy<Contract_MultipleB>(Contract_MultipleB.Nef, Contract_MultipleB.Manifest);

            Assert.IsTrue(a.Test());
            Assert.IsFalse(b.Test());
        }
    }
}
