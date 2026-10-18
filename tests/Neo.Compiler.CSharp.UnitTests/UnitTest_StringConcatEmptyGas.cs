// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringConcatEmptyGas.cs file belongs to the neo project and is free
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
    public class UnitTest_StringConcatEmptyGas : DebugAndTestBase<Contract_StringConcatEmpty>
    {
        [TestMethod]
        public void Test_ConcatNeoRight()
        {
            Assert.AreEqual("Neo", Contract.ConcatNeoRight());
            AssertGasConsumed(984450);
        }

        [TestMethod]
        public void Test_ConcatNeoLeft()
        {
            Assert.AreEqual("Neo", Contract.ConcatNeoLeft());
            AssertGasConsumed(984450);
        }

        [TestMethod]
        public void Test_ConcatBoth()
        {
            Assert.AreEqual("", Contract.ConcatBoth());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_ConcatNullRight()
        {
            Assert.AreEqual("", Contract.ConcatNullRight());
            AssertGasConsumed(984540);
        }

        [TestMethod]
        public void Test_ConcatNullLeft()
        {
            Assert.AreEqual("", Contract.ConcatNullLeft());
            AssertGasConsumed(984540);
        }

        [TestMethod]
        public void Test_ConcatRight()
        {
            Assert.AreEqual("Neo", Contract.ConcatRight("Neo"));
            AssertGasConsumed(1047420);
            Assert.AreEqual("", Contract.ConcatRight(null));
        }

        [TestMethod]
        public void Test_ConcatLeft()
        {
            Assert.AreEqual("Neo", Contract.ConcatLeft("Neo"));
            AssertGasConsumed(1047420);
            Assert.AreEqual("", Contract.ConcatLeft(null));
        }

        [TestMethod]
        public void Test_ConcatNormal()
        {
            Assert.AreEqual("NeoN3", Contract.ConcatNormal());
            AssertGasConsumed(1292130);
        }
    }
}
