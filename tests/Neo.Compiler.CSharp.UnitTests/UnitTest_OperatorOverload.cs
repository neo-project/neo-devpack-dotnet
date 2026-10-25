// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_OperatorOverload.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
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
    public class UnitTest_OperatorOverload : DebugAndTestBase<Contract_OperatorOverload>
    {
        [TestMethod]
        public void Test_EqualityUsesOperatorBody()
        {
            Assert.AreEqual(1, Contract.EqualityUsesOperatorBody());
        }

        [TestMethod]
        public void Test_InequalityUsesOperatorBody()
        {
            Assert.AreEqual(1, Contract.InequalityUsesOperatorBody());
        }

        [TestMethod]
        public void Test_RelationalUsesOperatorBody()
        {
            Assert.AreEqual(1, Contract.RelationalUsesOperatorBody());
        }

        [TestMethod]
        public void Test_ArithmeticUsesOperatorBody()
        {
            Assert.AreEqual(866, Contract.ArithmeticUsesOperatorBody());
        }

        [TestMethod]
        public void Test_ImplicitConversionCallsOperator()
        {
            Assert.AreEqual(708, Contract.ImplicitConversionCallsOperator());
        }

        [TestMethod]
        public void Test_ExplicitConversionCallsOperator()
        {
            Assert.AreEqual(7003, Contract.ExplicitConversionCallsOperator());
        }

        [TestMethod]
        public void Test_ChainedOperators()
        {
            Assert.AreEqual(8010, Contract.ChainedOperators());
        }
    }
}
