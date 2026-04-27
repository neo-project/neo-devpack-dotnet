// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Extensions.cs file belongs to the neo project and is free
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
    public class UnitTest_Extensions : DebugAndTestBase<Contract_Extensions>
    {
        [TestMethod]
        public void TestSum()
        {
            Assert.AreEqual(5, Contract.TestSum(3, 2));
            AssertGasConsumed(1065060);
        }

        [TestMethod]
        public void TestExtensionMemberMethod()
        {
            Assert.AreEqual(8, Contract.TestExtensionMemberMethod(4));
        }

        [TestMethod]
        public void TestExtensionMemberProperty()
        {
            Assert.AreEqual(12, Contract.TestExtensionMemberProperty(4));
        }

        [TestMethod]
        public void TestExtensionMemberCombination()
        {
            Assert.AreEqual(20, Contract.TestExtensionMemberCombination(4));
        }

        [TestMethod]
        public void TestExtensionMemberPropertySetter()
        {
            Assert.AreEqual(7, Contract.TestExtensionMemberPropertySetter(7));
        }
    }
}
