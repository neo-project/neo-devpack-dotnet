// Copyright (C) 2015-2026 The Neo Project.
//
// TestNeo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Native;
using System.Numerics;
using TestingNeoAccountState = Neo.SmartContract.Testing.Native.Models.NeoAccountState;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestNeo
    {
        [TestMethod]
        public void TestNeoWrapperUsesTypedAccountStateModel()
        {
            var method = typeof(NEO).GetMethod(nameof(NEO.GetAccountState));

            Assert.IsNotNull(method);
            Assert.AreEqual(typeof(TestingNeoAccountState), method.ReturnType);
            Assert.IsNotNull(typeof(TestingNeoAccountState).GetProperty(nameof(TestingNeoAccountState.BalanceHeight)));
            Assert.IsNotNull(typeof(TestingNeoAccountState).GetProperty(nameof(TestingNeoAccountState.LastGasPerVote)));
        }

        [TestMethod]
        public void TestGetAccountStateReturnsLatestShape()
        {
            var engine = new TestEngine(true);

            var validatorState = engine.Native.NEO.GetAccountState(engine.ValidatorsAddress);
            Assert.IsNotNull(validatorState);
            Assert.AreEqual(engine.Native.NEO.TotalSupply, validatorState.Balance);
            Assert.IsTrue(validatorState.LastGasPerVote >= BigInteger.Zero);

            Assert.IsNull(engine.Native.NEO.GetAccountState(UInt160.Zero));
        }
    }
}
