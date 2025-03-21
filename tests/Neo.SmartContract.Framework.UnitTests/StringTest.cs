// Copyright (C) 2015-2025 The Neo Project.
//
// StringTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StringTest : DebugAndTestBase<Contract_String>
    {
        [TestMethod]
        public void TestStringAdd()
        {
            // ab => 3
            Assert.AreEqual(3, Contract.TestStringAdd("a", "b"));
            AssertGasConsumed(1357590);

            // hello => 4
            Assert.AreEqual(4, Contract.TestStringAdd("he", "llo"));
            AssertGasConsumed(1356420);

            // world => 5
            Assert.AreEqual(5, Contract.TestStringAdd("wo", "rld"));
            AssertGasConsumed(1357680);
        }

        [TestMethod]
        public void TestStringAddInt()
        {
            Assert.AreEqual("Neo3", Contract.TestStringAddInt("Neo", 3));
            AssertGasConsumed(2460480);
        }
    }
}
