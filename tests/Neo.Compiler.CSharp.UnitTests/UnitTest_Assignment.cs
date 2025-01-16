// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_Assignment.cs file belongs to the neo project and is free
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
using System;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assignment : DebugAndTestBase<Contract_Assignment>
    {
        [TestMethod]
        public void Test_Assignment()
        {
            Contract.TestAssignment();
            AssertGasConsumed(989490);
        }

        [TestMethod]
        public void Test_CoalesceAssignment()
        {
            Contract.TestCoalesceAssignment();
            AssertGasConsumed(988950);
        }
    }
}
