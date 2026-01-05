// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Concat.cs file belongs to the neo project and is free
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
    public class UnitTest_Concat : DebugAndTestBase<Contract_Concat>
    {
        [TestMethod]
        public void TestStringAdd1()
        {
            Assert.AreEqual("ahello", Contract.TestStringAdd1("a"));
            AssertGasConsumed(1354680);
        }

        [TestMethod]
        public void TestStringAdd2()
        {
            Assert.AreEqual("abhello", Contract.TestStringAdd2("a", "b"));
            AssertGasConsumed(1662180);
        }

        [TestMethod]
        public void TestStringAdd3()
        {
            Assert.AreEqual("abchello", Contract.TestStringAdd3("a", "b", "c"));
            AssertGasConsumed(1969680);
        }

        [TestMethod]
        public void TestStringAdd4()
        {
            Assert.AreEqual("abcdhello", Contract.TestStringAdd4("a", "b", "c", "d"));
            AssertGasConsumed(2277180);
        }
    }
}
