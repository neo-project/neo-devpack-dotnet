// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StructSemantics.cs file belongs to the neo project and is free
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
    public class UnitTest_StructSemantics : DebugAndTestBase<Contract_StructSemantics>
    {
        [TestMethod]
        public void Test_CopyOnLocalDeclaration()
        {
            Assert.AreEqual(7099, (int)Contract.CopyOnLocalDeclaration(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyOnAssignment()
        {
            Assert.AreEqual(7099, (int)Contract.CopyOnAssignment(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyIsDeep()
        {
            Assert.AreEqual(7099, (int)Contract.CopyIsDeep(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyThroughClassField()
        {
            Assert.AreEqual(7099, (int)Contract.CopyThroughClassField(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyIntoArrayElement()
        {
            Assert.AreEqual(7099, (int)Contract.CopyIntoArrayElement(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyThroughObjectInitializer()
        {
            Assert.AreEqual(7099, (int)Contract.CopyThroughObjectInitializer(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyOnBoxing()
        {
            Assert.AreEqual(7099, (int)Contract.CopyOnBoxing(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyIntoObjectArray()
        {
            Assert.AreEqual(7099, (int)Contract.CopyIntoObjectArray(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyInForeach()
        {
            Assert.AreEqual(7, (int)Contract.CopyInForeach(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyOnWithExpression()
        {
            Assert.AreEqual(7004, (int)Contract.CopyOnWithExpression(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyInDeconstructionForeach()
        {
            Assert.AreEqual(10, (int)Contract.CopyInDeconstructionForeach(7)!.Value);
        }

        [TestMethod]
        public void Test_CopyOnTupleAssignment()
        {
            Assert.AreEqual(7099, (int)Contract.CopyOnTupleAssignment(7, 3)!.Value);
        }

        [TestMethod]
        public void Test_DefaultTupleFields()
        {
            Assert.AreEqual(0, (int)Contract.DefaultTupleFields()!.Value);
        }

        [TestMethod]
        public void Test_ArgumentIsCopied()
        {
            Assert.AreEqual(99007, (int)Contract.ArgumentIsCopied(7)!.Value);
        }

        [TestMethod]
        public void Test_DefaultStructFields()
        {
            Assert.AreEqual(0, (int)Contract.DefaultStructFields()!.Value);
        }

        [TestMethod]
        public void Test_DefaultNestedStructFields()
        {
            Assert.AreEqual(0, (int)Contract.DefaultNestedStructFields()!.Value);
        }

        [TestMethod]
        public void Test_WriteNestedStructFields()
        {
            Assert.AreEqual(8003, (int)Contract.WriteNestedStructFields(7, 3)!.Value);
        }
    }
}
