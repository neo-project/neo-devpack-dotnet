// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_StaticConstruct.cs file belongs to the neo project and is free
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
    public class UnitTest_StaticConstruct : DebugAndTestBase<Contract_StaticConstruct>
    {
        [TestMethod]
        public void Test_StaticConsturct()
        {
            var var1 = Contract.TestStatic();
            AssertGasConsumed(987120);
            // static byte[] callscript = ExecutionEngine.EntryScriptHash;
            // ...
            // return callscript

            Assert.IsNotNull(var1);
            Assert.AreEqual(4, var1);
        }
    }
}
