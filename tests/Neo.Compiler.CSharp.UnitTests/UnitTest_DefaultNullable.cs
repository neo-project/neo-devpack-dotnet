// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DefaultNullable.cs file belongs to the neo project and is free
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
    public class UnitTest_DefaultNullable : DebugAndTestBase<Contract_DefaultNullable>
    {
        [TestMethod]
        public void TestDefaultLiteral()
        {
            Assert.IsTrue(Contract.TestDefaultLiteral());
            Assert.IsFalse(Contract.TestHasValue());
            Assert.AreEqual(42, Contract.TestNullCoalescing());
            Assert.IsNull(Contract.TestReturnDefault());
            Assert.IsTrue(Contract.TestComparison(null));
            Assert.IsFalse(Contract.TestComparison(42));
        }

        [TestMethod]
        public void TestDefaultExpression()
        {
            Assert.IsTrue(Contract.TestDefaultExpression());
            Assert.IsFalse(Contract.TestHasValueExpression());
            Assert.AreEqual(42, Contract.TestNullCoalescingExpression());
            Assert.IsNull(Contract.TestReturnDefaultExpression());
            Assert.IsTrue(Contract.TestComparisonExpression(null));
            Assert.IsFalse(Contract.TestComparisonExpression(42));
        }

        [TestMethod]
        public void TestNotEqualNull()
        {
            Assert.IsFalse(Contract.TestNotEqualNull());
            Assert.IsFalse(Contract.TestNotEqualNullExpression());
        }
    }
}
