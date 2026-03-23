using System;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;

namespace Neo.SmartContract.Framework.UnitTests;

internal static class DynamicCoverageMergeHelper
{
    public static void Merge(Neo.SmartContract.Testing.SmartContract contract, NeoDebugInfo debugInfo)
    {
        if (ResolveCoverageMergePath() is not string path)
            return;

        var coverage = contract.GetCoverage();
        if (coverage is null)
            return;

        new CoverletJsonFormat((coverage, debugInfo)).Write(path, true);
    }

    private static string? ResolveCoverageMergePath()
    {
        var direct = Environment.GetEnvironmentVariable("COVERAGE_MERGE_JOIN");
        if (!string.IsNullOrWhiteSpace(direct))
            return Environment.ExpandEnvironmentVariables(direct);

        var mergeWithArg = Environment.GetEnvironmentVariable("COVERLET_MERGE_WITH");
        if (string.IsNullOrWhiteSpace(mergeWithArg))
            return null;

        const string prefix = "/p:MergeWith=";
        var index = mergeWithArg.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return null;

        var value = mergeWithArg[(index + prefix.Length)..].Trim();
        if (value.Length == 0)
            return null;

        var spaceIndex = value.IndexOf(' ');
        if (spaceIndex >= 0)
            value = value[..spaceIndex];

        value = value.Trim().Trim('"', '\'');
        if (value.Length == 0)
            return null;

        return Environment.ExpandEnvironmentVariables(value);
    }
}
