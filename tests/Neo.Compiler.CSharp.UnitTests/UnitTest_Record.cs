// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Record.cs file belongs to the neo project and is free
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
    public class UnitTest_Record : DebugAndTestBase<Contract_Record>
    {
        [TestMethod]
        public void Test_CreateRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_CreateRecord(name, age)!;
            AssertGasConsumed(1618170);
            var arr = result as Struct;
            Assert.AreEqual(2, arr!.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_CreateRecord2()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_CreateRecord2(name, age)!;
            AssertGasConsumed(1618320);
            var arr = result as Struct;
            Assert.AreEqual(2, arr!.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_UpdateRecord(name, age)! as Struct;
            AssertGasConsumed(2004900);
            Assert.AreEqual(2, result!.Count);
            Assert.AreEqual(name, result[0].GetString());
            Assert.AreEqual(age, result[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord2()
        {
            var name = "klsas";
            var age = 2;
            var result = Contract.Test_UpdateRecord2(name, age)!;
            AssertGasConsumed(2575650);
            var arr = result as Struct;
            Assert.AreEqual(2, arr!.Count);
            Assert.AreEqual("0" + name, arr[0].GetString());
            Assert.AreEqual(age + 1, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_DeconstructRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_DeconstructRecord(name, age)!;
            AssertGasConsumed(1679970);
            Assert.AreEqual(name, result);
        }
    }
}
