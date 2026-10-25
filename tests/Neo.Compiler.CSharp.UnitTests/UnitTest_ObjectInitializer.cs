// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ObjectInitializer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
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
    public class UnitTest_ObjectInitializer : DebugAndTestBase<Contract_ObjectInitializer>
    {
        [TestMethod]
        public void Test_IndexInitializer()
        {
            Assert.AreEqual(new BigInteger(78), Contract.IndexInitializer(7));
        }

        [TestMethod]
        public void Test_PropertyInitializer()
        {
            Assert.AreEqual(new BigInteger(7), Contract.PropertyInitializer(7));
        }

        [TestMethod]
        public void Test_FieldInitializer()
        {
            Assert.AreEqual(new BigInteger(7), Contract.FieldInitializer(7));
        }
    }
}
