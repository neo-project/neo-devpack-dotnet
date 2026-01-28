// Copyright (C) 2015-2026 The Neo Project.
//
// NativeTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM.Types;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class NativeTest : DebugAndTestBase<Contract_Native>
    {
        [TestMethod]
        public void Test_Policy()
        {
            Assert.AreEqual(1000L, Contract.Policy_GetFeePerByte());
            Assert.IsFalse(Contract.Policy_IsBlocked(Alice.Account));
        }

        [TestMethod]
        public void Test_ContractManagementIsContract()
        {
            Assert.IsFalse(Contract.ContractManagement_IsContract(Alice.Account));

            var contractManagementHash = UInt160.Parse("0xfffdc93764dbaddd97c48f252a53ea4643faa3fd");
            Assert.IsTrue(Contract.ContractManagement_IsContract(contractManagementHash));
        }
    }
}
