// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_Out.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Out : DebugAndTestBase<Contract_Out>
    {
        [TestMethod]
        public void Test_BasicOut()
        {
            Assert.AreEqual(42, Contract.TestOutVar());
            Assert.AreEqual(42, Contract.TestExistingVar());
        }

        [TestMethod]
        public void Test_OutInLoop()
        {
            Assert.AreEqual(210, Contract.TestOutInLoop()); // 42 * 5
        }

        [TestMethod]
        public void Test_OutConditional()
        {
            Assert.AreEqual("42", Contract.TestOutConditional(true));
            Assert.AreEqual("Hello", Contract.TestOutConditional(false));
        }

        [TestMethod]
        public void Test_OutSwitch()
        {
            Assert.AreEqual(42, Contract.TestOutSwitch(1));
            Assert.AreEqual(10, Contract.TestOutSwitch(2));
            Assert.AreEqual(-1, Contract.TestOutSwitch(3));
        }

        [TestMethod]
        public void Test_OutDiscard()
        {
            // This test is mainly to ensure that the method compiles and runs without error
            // Since we're discarding values, we can't really assert on them
            Contract.TestOutDiscard();
        }

        [TestMethod]
        public void Test_NestedOut()
        {
            var result = Contract.TestNestedOut()!;
            Assert.AreEqual((BigInteger)84, result[0], "TestNestedOut should return 84 as the first element");
            Assert.AreEqual((BigInteger)42, result[1], "TestNestedOut should return 42 as the second element");
        }
    }
}
