// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Tuple.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Tuple : DebugAndTestBase<Contract_Tuple>
    {
        [TestMethod]
        public void Test_Assign()
        {
            var tuple = Contract.T1()! as Struct;
            AssertGasConsumed(2719230);
            Assert.AreEqual(5, tuple!.Count);
            Assert.AreEqual(1, tuple[2].GetInteger());
            Assert.AreEqual(4, tuple[3].GetInteger());
            Assert.AreEqual(2, ((Struct)tuple[4])[1].GetInteger());
        }

        [TestMethod]
        public void Test_ReadTupleElements()
        {
            Assert.AreEqual(7034007, (int)Contract.ReadTupleElements(7, 3)!.Value);
        }

        [TestMethod]
        public void Test_WriteTupleElement()
        {
            Assert.AreEqual(99, (int)Contract.WriteTupleElement(7)!.Value);
        }

        [TestMethod]
        public void Test_ReadTupleRest()
        {
            Assert.AreEqual(8005, (int)Contract.ReadTupleRest(7)!.Value);
        }

        [TestMethod]
        public void Test_TupleAssignmentEvaluationOrder()
        {
            Assert.AreEqual(1120012, (int)Contract.TupleAssignmentEvaluationOrder(7)!.Value);
        }

        [TestMethod]
        public void Test_TuplePropertyAndStaticAssignment()
        {
            Assert.AreEqual(1123313, (int)Contract.TuplePropertyAndStaticAssignment(7)!.Value);
        }
    }
}
