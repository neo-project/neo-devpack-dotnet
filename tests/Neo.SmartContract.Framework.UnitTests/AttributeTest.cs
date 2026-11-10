// Copyright (C) 2015-2026 The Neo Project.
//
// AttributeTest.cs file belongs to the neo project and is free
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
using System;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class AttributeTest : DebugAndTestBase<Contract_Attribute>
    {
        [TestMethod]
        public void TestAttribute()
        {
            Engine.SetTransactionSigners(UInt160.Zero);
            Assert.IsTrue(Contract.Test());

            Engine.SetTransactionSigners(Array.Empty<UInt160>());
            Assert.ThrowsExactly<TestException>(() => Contract.Test());
        }

        [TestMethod]
        public void TestReentrant()
        {
            // return in the middle
            Contract.ReentrantTest(0);

            // Method end
            Contract.ReentrantTest(1);

            // Reentrant test
            var ex = Assert.ThrowsExactly<TestException>(() => Contract.ReentrantTest(123));
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }

        [TestMethod]
        public void TestReentrant2()
        {
            // should be ok
            Contract.ReentrantB();

            // Reentrant test
            var ex = Assert.ThrowsExactly<TestException>(Contract.ReentrantA);
            Assert.IsTrue(ex.Message.Contains("Already entered"));
        }
    }
}
