// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Storage.cs file belongs to the neo project and is free
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
    public class UnitTest_Storage : DebugAndTestBase<Contract_Storage>
    {
        [TestMethod]
        public void Test_Storage()
        {
            var a = Contract;
            Engine.SetTransactionSigners(Bob);
            var b = Engine.Deploy<Contract_Storage>(Contract_Storage.Nef, Contract_Storage.Manifest);

            a.MainA(b.Hash, true);

            Assert.AreEqual(0x01, a.Storage.Get(0xA0).ToArray()[0]);
            Assert.AreEqual(0x02, a.Storage.Get(0xA1).ToArray()[0]);

            Assert.IsTrue(b.Storage.Get(0xB0).IsEmpty);
            Assert.IsTrue(b.Storage.Get(0xB1).IsEmpty);
        }
    }
}
