// Copyright (C) 2015-2025 The Neo Project.
//
// DebugAndTestBase.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests;

public class DebugAndTestBase<T> : TestBase<T>
    where T : SmartContract.Testing.SmartContract, IContractInfo
{

    // allowing specific derived class to enable/disable Gas test
    protected virtual bool TestGasConsume { set; get; } = true;

    static DebugAndTestBase()
    {
        TestCleanup.TestInitialize(typeof(T));
    }

    protected void AssertGasConsumed(long gasConsumed)
    {
        if (TestGasConsume)
            Assert.AreEqual(gasConsumed, Engine.FeeConsumed.Value);
    }
}
