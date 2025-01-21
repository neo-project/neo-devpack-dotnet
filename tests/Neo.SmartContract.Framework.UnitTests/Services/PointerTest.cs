// Copyright (C) 2015-2024 The Neo Project.
//
// PointerTest.cs file belongs to the neo project and is free
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
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class PointerTest : DebugAndTestBase<Contract_Pointers>
    {
        [TestMethod]
        public void Test_CreatePointer()
        {
            var item = Contract.CreateFuncPointer();
            Assert.IsInstanceOfType(item, typeof(Pointer));

            // Test pointer

            item = Engine.Execute(Contract_Pointers.Nef.Script, ((Pointer)item).Position, (e) => { e.Push(1); });

            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void Test_ExecutePointer()
        {
            Assert.AreEqual(123, Contract.CallFuncPointer());
        }

        [TestMethod]
        public void Test_ExecutePointerWithArgs()
        {
            // Internall

            Assert.AreEqual(new BigInteger(new byte[] { 11, 22, 33 }), Contract.CallFuncPointerWithArg());

            // With pointer

            var item = Contract.CreateFuncPointerWithArg();
            Assert.IsInstanceOfType(item, typeof(Pointer));

            // Test pointer

            item = Engine.Execute(Contract_Pointers.Nef.Script, ((Pointer)item).Position, (e) => { e.Push(123); });
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }
    }
}
