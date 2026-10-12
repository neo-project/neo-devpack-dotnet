// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_GasSnapshotBaseline.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_GasSnapshotBaseline : DebugAndTestBase<Contract_String>
{
    [TestMethod]
    public void RepresentativeStringOperationsStayWithinGasBaseline()
    {
        var current = GasSnapshot.Capture([
            ("string-builder-basic", Measure(() => Contract.TestStringBuilderBasic())),
            ("string-builder-length", Measure(() => Contract.TestStringBuilderLength())),
            ("string-builder-clear", Measure(() => Contract.TestStringBuilderClear()))
        ]);

        var baseline = GasSnapshot.FromJson("""
            {
              "string-builder-basic": 2662530,
              "string-builder-clear": 1978440,
              "string-builder-length": 2665110
            }
            """);

        current.AssertGasWithin(baseline);
    }

    private long Measure<T>(Func<T> operation)
    {
        operation();
        return Engine.FeeConsumed.Value;
    }
}
