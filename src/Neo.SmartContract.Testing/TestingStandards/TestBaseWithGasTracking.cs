// Copyright (C) 2015-2026 The Neo Project.
//
// TestBaseWithGasTracking.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using System;
using System.Linq;

namespace Neo.SmartContract.Testing.TestingStandards;

/// <summary>
/// Test base that opts in to automatic GAS regression tracking against version-controlled baselines.
/// </summary>
public class TestBaseWithGasTracking<T> : TestBase<T> where T : SmartContract, IContractInfo
{
    private GasExecutionScope? _gasScope;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public TestBaseWithGasTracking() : base() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest file</param>
    /// <param name="debugInfoFile">Debug info file</param>
    public TestBaseWithGasTracking(string nefFile, string manifestFile, string? debugInfoFile = null) :
        base(nefFile, manifestFile, debugInfoFile)
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest</param>
    /// <param name="debugInfo">Debug info</param>
    public TestBaseWithGasTracking(NefFile nefFile, ContractManifest manifestFile, NeoDebugInfo? debugInfo = null) :
        base(nefFile, manifestFile, debugInfo)
    { }

    /// <summary>
    /// Whether a GAS tracking scope is currently active for this test instance
    /// </summary>
    protected bool IsGasTrackingActive => _gasScope is not null;

    [TestInitialize]
    public void OnGasTrackingInitialize()
    {
        if (TestContext is null) return;

        var name = TestContext.TestName ?? string.Empty;
        var display = TestContext.TestDisplayName;
        var caseId = !string.IsNullOrEmpty(display) && display != name && display.StartsWith(name, StringComparison.Ordinal)
            ? display.Substring(name.Length).Trim().TrimStart('(').TrimEnd(')')
            : null;

        _gasScope = GasTrackingSession.Begin(
            GetType().Assembly,
            GasExecutionTracker.CreateTestId(GetType().FullName ?? GetType().Name, name, caseId));
    }

    /// <summary>
    /// Runs the base cleanup exactly once (inherited [TestCleanup] discovery) and then completes GAS tracking.
    /// </summary>
    [TestCleanup]
    public override void OnCleanup()
    {
        try
        {
            base.OnCleanup();
        }
        finally
        {
            CompleteGasTracking();
        }
    }

    private void CompleteGasTracking()
    {
        var scope = _gasScope;
        if (scope is null) return;
        _gasScope = null;

        var passed = TestContext?.CurrentTestOutcome == UnitTestOutcome.Passed;
        var report = GasTrackingSession.Complete(GetType().Assembly, scope, passed);

        // Functional failures keep their own diagnostics; GAS is only asserted for otherwise passing tests.
        if (passed && report is not null && report.HasFailures)
        {
            var failures = string.Join(Environment.NewLine, report.Changes.Where(c => c.IsFailure)
                .Select(c => $"{c.Name}: {c.Status} (baseline={c.Baseline?.ToString() ?? "none"}, current={c.Current?.ToString() ?? "none"})"));
            Assert.Fail("GAS regression detected:" + Environment.NewLine + failures);
        }
    }
}
