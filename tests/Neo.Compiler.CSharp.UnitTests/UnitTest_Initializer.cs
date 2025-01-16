// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Initializer.cs file belongs to the neo project and is free
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
    public class UnitTest_Initializer : DebugAndTestBase<Contract_Initializer>
    {
        [TestMethod]
        public void Initializer_Test()
        {
            Assert.AreEqual(3, Contract.Sum());
            AssertGasConsumed(1052100);
            Assert.AreEqual(12, Contract.Sum1(5, 7));
            AssertGasConsumed(1113210);
            Assert.AreEqual(12, Contract.Sum2(5, 7));
            AssertGasConsumed(1605330);
        }
    }
}
