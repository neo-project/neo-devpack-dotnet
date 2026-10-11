// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringBuilder.cs file belongs to the neo project and is free
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
    public class UnitTest_StringBuilder : DebugAndTestBase<Contract_StringBuilder>
    {
        [TestMethod]
        public void TestAppendPrimitiveValues()
        {
            Assert.AreEqual("-1|2|-3|4|-5|6|-7|8|True|False", Contract.AppendPrimitiveValues());
            AssertGasConsumed(15788310);
        }

        [TestMethod]
        public void TestAppendEmpty()
        {
            Assert.AreEqual("Neo", Contract.AppendEmpty());
            AssertGasConsumed(1052100);
        }

        [TestMethod]
        public void TestAppendConstEmpty()
        {
            Assert.AreEqual("Neo", Contract.AppendConstEmpty());
            AssertGasConsumed(1052100);
        }

        [TestMethod]
        public void TestAppendEmptyMultiple()
        {
            Assert.AreEqual("Neo", Contract.AppendEmptyMultiple());
            AssertGasConsumed(1056480);
        }

        [TestMethod]
        public void TestAppendEmptyChained()
        {
            Assert.AreEqual("Neo N3", Contract.AppendEmptyChained());
            AssertGasConsumed(1362300);
        }

        [TestMethod]
        public void TestAppendEmptyKeepsInstance()
        {
            Assert.AreEqual("Neo!", Contract.AppendEmptyKeepsInstance());
        }

        [TestMethod]
        public void TestAppendEmptyKeepsContent()
        {
            Assert.AreEqual("ab", Contract.AppendEmptyKeepsContent());
        }

        [TestMethod]
        public void TestAppendEmptyNullReceiverFaults()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.AppendEmptyNullReceiver());
        }

        [TestMethod]
        public void TestAppendRuntimeString()
        {
            Assert.AreEqual("Neo", Contract.AppendRuntimeString(""));
            Assert.AreEqual("Neo N3", Contract.AppendRuntimeString(" N3"));
        }

        [TestMethod]
        public void TestAppendNonEmpty()
        {
            Assert.AreEqual("Neo N3", Contract.AppendNonEmpty());
            AssertGasConsumed(1360230);
        }
    }
}
