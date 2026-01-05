// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Inline.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline : DebugAndTestBase<Contract_Inline>
    {
        [TestMethod]
        public void Test_Inline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("inline"));
            AssertGasConsumed(1048650);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_with_one_parameters"));
            AssertGasConsumed(1049970);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("inline_with_multi_parameters"));
            AssertGasConsumed(1051830);
        }

        [TestMethod]
        public void Test_NoInline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("not_inline"));
            AssertGasConsumed(1067970);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("not_inline_with_one_parameters"));
            AssertGasConsumed(1071270);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("not_inline_with_multi_parameters"));
            AssertGasConsumed(1073220);
        }

        [TestMethod]
        public void Test_NestedInline()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_nested"));
            AssertGasConsumed(1071930);
        }

        [TestMethod]
        public void Test_ArrowMethod()
        {
            Assert.AreEqual(new BigInteger(3), Contract.ArrowMethod());
        }

        [TestMethod]
        public void Test_ArrowMethodNoReturn()
        {
            Contract.ArrowMethodNoRerurn();
        }
    }
}
