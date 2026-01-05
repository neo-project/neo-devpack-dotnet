// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Throw.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Throw : DebugAndTestBase<Contract_Throw>
    {
        [TestMethod]
        public void Test_Throw()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestMain([]));
            AssertGasConsumed(1063530);
            Assert.IsTrue(exception.Message.Contains("Please supply at least one argument."));
        }

        [TestMethod]
        public void Test_NotThrow()
        {
            Contract.TestMain(["test"]);
            AssertGasConsumed(1111290);
        }
    }
}
