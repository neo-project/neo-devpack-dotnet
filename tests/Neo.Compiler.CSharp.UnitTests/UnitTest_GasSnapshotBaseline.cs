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
using System.Globalization;
using System.IO;
using System.Linq;

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

        var path = Path.Combine(AppContext.BaseDirectory, "GasBaselines", "representative-string-operations.json");
        if (!File.Exists(path))
            Assert.Fail($"GAS baseline file not found: {path}");
        var baseline = GasSnapshot.Load(path);

        var report = GasRegressionReport.Evaluate(baseline, current);
        report.WriteReportIfConfiguredAndAssert([
            new("Baseline", "representative-string-operations.json"),
            new("Branch", Environment.GetEnvironmentVariable("GITHUB_HEAD_REF") is { Length: > 0 } branch
                ? branch : Environment.GetEnvironmentVariable("GITHUB_REF_NAME") ?? "local"),
            new("Contract", nameof(Contract_String)),
            new("Neo Core", typeof(ProtocolSettings).Assembly.GetName().Version!.ToString()),
            new("Optimization", TestCleanup.CachedContracts[typeof(Contract_String)].Context.Options.Optimize.ToString()),
            new("Hardforks", string.Join(", ", Engine.ProtocolSettings.Hardforks
                .OrderBy(hardfork => hardfork.Key.ToString(), StringComparer.Ordinal)
                .Select(hardfork => $"{hardfork.Key}@{hardfork.Value.ToString(CultureInfo.InvariantCulture)}"))),
            new("Revision", Environment.GetEnvironmentVariable("GITHUB_SHA") ?? "local")
        ]);
    }

    private long Measure<T>(Func<T> operation)
    {
        operation();
        return Engine.FeeConsumed.Value;
    }
}
