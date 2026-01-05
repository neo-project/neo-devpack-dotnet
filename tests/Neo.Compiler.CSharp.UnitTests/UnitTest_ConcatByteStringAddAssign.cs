// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ConcatByteStringAddAssign.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ConcatByteStringAddAssign : DebugAndTestBase<Contract_ConcatByteStringAddAssign>
    {
        [TestMethod]
        public void Test_ByteStringAdd()
        {
            Assert.AreEqual("abc", Encoding.ASCII.GetString(Contract.ByteStringAddAssign(Encoding.ASCII.GetBytes("a"), Encoding.ASCII.GetBytes("b"), "c")!));
            AssertGasConsumed(1970160);
        }
    }
}
