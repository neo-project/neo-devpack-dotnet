// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Event.cs file belongs to the neo project and is free
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
    public class UnitTest_Event() : DebugAndTestBase<Contract_Event>
    {
        [TestMethod]
        public void Test_Good()
        {
            var abi = Contract_Event.Manifest.Abi;
            var events = abi.Events[0].ToJson().ToString(false);

            string expecteventabi = @"{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}";
            Assert.AreEqual(expecteventabi, events);
        }

        [TestMethod]
        public void TestEvent()
        {
            var flag = false;

            Contract.OnTransfer += (a, b, c) =>
            {
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, a);
                CollectionAssert.AreEqual(new byte[] { 4, 5, 6 }, b);
                Assert.AreEqual(7, c);
                flag = true;
            };

            Contract.Test();
            Assert.IsTrue(flag);
        }
    }
}
