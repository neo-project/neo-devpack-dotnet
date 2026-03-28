// Copyright (C) 2015-2026 The Neo Project.
//
// GasAssertionRangeTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class GasAssertionRangeTest : DebugAndTestBase<Contract_String>
{
    [TestMethod]
    public void AssertGasConsumed_AllowsAbsoluteTolerance()
    {
        Engine.FeeConsumed.Value = 1_000;

        AssertGasConsumed(950, 50);
        AssertGasConsumed(1_050, 50);
    }

    [TestMethod]
    public void AssertGasConsumedInRange_AllowsInclusiveRange()
    {
        Engine.FeeConsumed.Value = 1_000;

        AssertGasConsumedInRange(1_000, 1_000);
        AssertGasConsumedInRange(900, 1_100);
    }

    [TestMethod]
    public void AssertGasConsumed_FailsOutsideTolerance()
    {
        Engine.FeeConsumed.Value = 1_000;

        var exception = Assert.ThrowsExactly<AssertFailedException>(() => AssertGasConsumed(900, 50));

        StringAssert.Contains(exception.Message, "between 850 and 950");
        StringAssert.Contains(exception.Message, "1000");
    }

    [TestMethod]
    public void AssertGasConsumed_FailsForNegativeTolerance()
    {
        var exception = Assert.ThrowsExactly<AssertFailedException>(() => AssertGasConsumed(1_000, -1));

        StringAssert.Contains(exception.Message, "non-negative");
    }

    [TestMethod]
    public void AssertGasConsumedInRange_FailsForInvalidWindow()
    {
        var exception = Assert.ThrowsExactly<AssertFailedException>(() => AssertGasConsumedInRange(1_100, 900));

        StringAssert.Contains(exception.Message, "minimum");
        StringAssert.Contains(exception.Message, "maximum");
    }

    [TestMethod]
    public void AssertGasConsumedInRange_FailsWhenActualGasIsBelowMinimum()
    {
        Engine.FeeConsumed.Value = 1_000;

        var exception = Assert.ThrowsExactly<AssertFailedException>(() => AssertGasConsumedInRange(1_001, 1_100));

        StringAssert.Contains(exception.Message, "between 1001 and 1100");
        StringAssert.Contains(exception.Message, "1000");
    }
}
