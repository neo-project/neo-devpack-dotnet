// Copyright (C) 2015-2026 The Neo Project.
//
// TestCleanupBaseTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Reflection;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards;

[TestClass]
public class TestCleanupBaseTests
{
    private static readonly MethodInfo ResolveCoverageMergePathMethod =
        typeof(TestCleanupBase).GetMethod("ResolveCoverageMergePath", BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("ResolveCoverageMergePath method not found.");

    [TestMethod]
    public void ResolveCoverageMergePath_PrefersCoverageMergeJoin()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", "/tmp/direct-coverage.json"),
            ("COVERLET_MERGE_WITH", "/p:MergeWith=/tmp/fallback-coverage.json"));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.AreEqual("/tmp/direct-coverage.json", path);
    }

    [TestMethod]
    public void ResolveCoverageMergePath_FallsBackToCoverletMergeWithArgument()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", null),
            ("COVERLET_MERGE_WITH", "/p:MergeWith=/tmp/merged-coverage.json"));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.AreEqual("/tmp/merged-coverage.json", path);
    }

    [TestMethod]
    public void ResolveCoverageMergePath_TreatsWhitespaceCoverageMergeJoinAsUnset()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", "   "),
            ("COVERLET_MERGE_WITH", "/p:MergeWith=/tmp/fallback-coverage.json"));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.AreEqual("/tmp/fallback-coverage.json", path);
    }

    [TestMethod]
    public void ResolveCoverageMergePath_StripsQuotedMergePathAndTrailingArguments()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", null),
            ("COVERLET_MERGE_WITH", "/p:MergeWith=\"/tmp/coverage report.json\" /p:CollectCoverage=true"));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.AreEqual("/tmp/coverage report.json", path);
    }

    [TestMethod]
    public void ResolveCoverageMergePath_ReturnsNullWhenMergeWithPrefixIsMissing()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", null),
            ("COVERLET_MERGE_WITH", "/p:CollectCoverage=true"));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.IsNull(path);
    }

    [TestMethod]
    public void ResolveCoverageMergePath_ReturnsNullWhenMergeWithValueIsEmpty()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", null),
            ("COVERLET_MERGE_WITH", "/p:MergeWith=\"\""));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.IsNull(path);
    }

    [TestMethod]
    public void ResolveCoverageMergePath_ReturnsNullWhenUnset()
    {
        using var _ = new EnvironmentVariablesScope(
            ("COVERAGE_MERGE_JOIN", null),
            ("COVERLET_MERGE_WITH", null));

        var path = (string?)ResolveCoverageMergePathMethod.Invoke(null, null);

        Assert.IsNull(path);
    }

    private sealed class EnvironmentVariablesScope : IDisposable
    {
        private readonly (string Name, string? Value)[] _original;

        public EnvironmentVariablesScope(params (string Name, string? Value)[] variables)
        {
            _original = [.. Array.ConvertAll(variables, variable => (variable.Name, Environment.GetEnvironmentVariable(variable.Name)))];

            foreach (var (name, value) in variables)
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        public void Dispose()
        {
            foreach (var (name, value) in _original)
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }
    }
}
