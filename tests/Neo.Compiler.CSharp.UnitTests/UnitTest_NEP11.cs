// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NEP11.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NEP11 : DebugAndTestBase<Contract_NEP11>
    {
        [TestMethod]
        public void UnitTest_Symbol()
        {
            Assert.AreEqual("TEST", Contract.Symbol);
        }

        [TestMethod]
        public void UnitTest_Decimals()
        {
            Assert.AreEqual(0, Contract.Decimals);
        }

        [TestMethod]
        public void UnitTest_Properties_ValidateTokenIdLength()
        {
            var ex = Assert.ThrowsException<TestException>(() => Contract.Properties(new byte[65]));
            StringAssert.Contains(ex.InnerException?.Message ?? ex.Message, "64 or less bytes long");
        }

        [TestMethod]
        public void UnitTest_Properties_MissingToken_Throws()
        {
            var ex = Assert.ThrowsException<TestException>(() => Contract.Properties(new byte[] { 0x01 }));
            StringAssert.Contains(ex.InnerException?.Message ?? ex.Message, "does not exist");
        }

        [TestMethod]
        public void UnitTest_Transfer_MissingToken_Throws()
        {
            var to = TestEngine.GetNewSigner().Account;
            var ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(to, new byte[] { 0x02 }, null));
            StringAssert.Contains(ex.InnerException?.Message ?? ex.Message, "does not exist");
        }

    }
}
