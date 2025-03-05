// Copyright (C) 2015-2025 The Neo Project.
//
// OracleTest.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class OracleTest : DebugAndTestBase<Contract_IOracle>
    {
        [TestMethod]
        public void Test_OracleResponse()
        {
            Assert.ThrowsException<TestException>(() => Contract.OnOracleResponse("http://127.0.0.1", "test", 0x14, "{}"));

            Engine.OnGetCallingScriptHash = (current, expected) => Engine.Native.Oracle.Hash;
            Contract.OnOracleResponse("http://127.0.0.1", "test", 0x14, "{}");
            AssertLogs("Oracle call!");
        }
    }
}
