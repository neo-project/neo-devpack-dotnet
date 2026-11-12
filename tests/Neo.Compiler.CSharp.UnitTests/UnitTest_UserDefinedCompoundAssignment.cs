// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_UserDefinedCompoundAssignment.cs file belongs to the neo project and is free
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
    public class UnitTest_UserDefinedCompoundAssignment : DebugAndTestBase<Contract_UserDefinedCompoundAssignment>
    {
        [TestMethod]
        public void Test_StructLocalTarget()
        {
            Assert.AreEqual(12, Contract.StructLocalTarget());
        }

        [TestMethod]
        public void Test_StructStaticFieldTarget()
        {
            Assert.AreEqual(7, Contract.StructStaticFieldTarget());
        }

        [TestMethod]
        public void Test_StructArrayElementTarget()
        {
            Assert.AreEqual(1501, Contract.StructArrayElementTarget());
        }

        [TestMethod]
        public void Test_StructMemberAccessTarget()
        {
            Assert.AreEqual(22, Contract.StructMemberAccessTarget());
        }

        [TestMethod]
        public void Test_ClassLocalTarget()
        {
            Assert.AreEqual(35, Contract.ClassLocalTarget());
        }

        [TestMethod]
        public void Test_ClassParameterTarget()
        {
            Assert.AreEqual(52, Contract.ClassParameterTarget());
        }

        [TestMethod]
        public void Test_ReceiverEvaluatedBeforeArgument()
        {
            Assert.AreEqual(0, Contract.ReceiverEvaluatedBeforeArgument());
        }
    }
}
