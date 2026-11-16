// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LabeledBreakContinue.cs file belongs to the neo project and is free
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
    public class UnitTest_LabeledBreakContinue : DebugAndTestBase<Contract_LabeledBreakContinue>
    {
        [TestMethod]
        public void Test_BreakOuterFromNestedForEach()
        {
            Assert.AreEqual(1112, Contract.BreakOuterFromNestedForEach());
        }

        [TestMethod]
        public void Test_ContinueOuterFromNestedForEach()
        {
            Assert.AreEqual(1115, Contract.ContinueOuterFromNestedForEach());
        }

        [TestMethod]
        public void Test_BreakLabeledWhile()
        {
            Assert.AreEqual(109, Contract.BreakLabeledWhile());
        }

        [TestMethod]
        public void Test_ContinueLabeledFor()
        {
            Assert.AreEqual(108, Contract.ContinueLabeledFor());
        }

        [TestMethod]
        public void Test_BreakLabeledSwitch()
        {
            Assert.AreEqual(10011, Contract.BreakLabeledSwitch(1, 1));
            Assert.AreEqual(10121, Contract.BreakLabeledSwitch(1, 2));
            Assert.AreEqual(11000, Contract.BreakLabeledSwitch(2, 1));
        }

        [TestMethod]
        public void Test_BreakOuterThroughTryFinally()
        {
            Assert.AreEqual(1201, Contract.BreakOuterThroughTryFinally());
        }

        [TestMethod]
        public void Test_ContinueOuterThroughTryFinally()
        {
            Assert.AreEqual(1304, Contract.ContinueOuterThroughTryFinally());
        }

        [TestMethod]
        public void Test_BreakOutermostFromThreeLevels()
        {
            Assert.AreEqual(1003, Contract.BreakOutermostFromThreeLevels());
        }

        [TestMethod]
        public void Test_ContinueMiddleFromInnermost()
        {
            Assert.AreEqual(1216, Contract.ContinueMiddleFromInnermost());
        }

        [TestMethod]
        public void Test_BreakLabeledDoWhile()
        {
            Assert.AreEqual(118, Contract.BreakLabeledDoWhile());
        }
    }
}
