// Copyright (C) 2015-2026 The Neo Project.
//
// JsonTest.cs file belongs to the neo project and is free
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
using Neo.VM.Types;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class JsonTest : DebugAndTestBase<Contract_Json>
    {
        [TestMethod]
        public void Test_SerializeDeserialize()
        {
            // Empty Serialize

            Assert.AreEqual("null", Contract.Serialize());

            // Empty Serialize

            var exception = Assert.ThrowsException<TestException>(() => Contract.Deserialize(null));
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);

            // Serialize

            Assert.AreEqual("[null,true,\"asd\"]", Contract.Serialize(new object?[] { null, true, "asd" }));

            // Deserialize

            var item = Contract.Deserialize("[null,true,\"asd\"]");
            Assert.IsInstanceOfType(item, typeof(Array));

            var entry = ((Array)item)[0];
            Assert.IsInstanceOfType(entry, typeof(Null));
            entry = ((Array)item)[1];
            Assert.IsInstanceOfType(entry, typeof(Boolean));
            Assert.AreEqual(true, entry.GetBoolean());
            entry = ((Array)item)[2];
            Assert.IsInstanceOfType(entry, typeof(ByteString));
            Assert.AreEqual("asd", entry.GetString());
        }
    }
}
