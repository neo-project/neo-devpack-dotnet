// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_ABI_Safe.cs file belongs to the neo project and is free
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
    public class UnitTest_ABI_Safe : DebugAndTestBase<Contract_ABISafe>
    {
        [TestMethod]
        public void UnitTest_TestSafe()
        {
            Assert.IsFalse(Contract_ABISafe.Manifest.Abi.Methods[0].Safe);
            Assert.IsTrue(Contract_ABISafe.Manifest.Abi.Methods[1].Safe);
            Assert.IsFalse(Contract_ABISafe.Manifest.Abi.Methods[2].Safe);
        }

        [TestMethod]
        public void Method1Test()
        {
            Assert.AreEqual(1, Contract.UnitTest_001());
        }

        [TestMethod]
        public void Method3Test()
        {
            Assert.AreEqual(3, Contract.UnitTest_003());
        }
    }
}
