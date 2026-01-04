// Copyright (C) 2015-2026 The Neo Project.
//
// HasCallATests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class HasCallATests
    {
        [TestMethod]
        public void Test_HasCallA()
        {
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Lambda.Nef));
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Linq.Nef));
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Delegate.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_Polymorphism.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_TryCatch.Nef));
        }
    }
}
