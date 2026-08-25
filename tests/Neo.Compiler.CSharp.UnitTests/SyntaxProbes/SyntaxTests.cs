// Copyright (C) 2015-2026 The Neo Project.
//
// SyntaxTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class SyntaxTests
{
    private static readonly IReadOnlyList<SyntaxProbeCase> Probes = SyntaxProbeLoader.Load();

    public static IEnumerable<object[]> GetSyntaxProbes()
    {
        foreach (var probe in Probes)
        {
            yield return new object[] { probe };
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(GetSyntaxProbes), DynamicDataSourceType.Method)]
    public void Syntax_Feature_Probe(SyntaxProbeCase probe)
    {
        Helper.AssertProbe(probe);
    }

    [TestMethod]
    public void Syntax_Probe_HasUniqueIds()
    {
        var duplicates = Probes
            .GroupBy(p => p.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        CollectionAssert.AreEquivalent(Array.Empty<string>(), duplicates, "Duplicate probe identifiers found.");
    }

    [TestMethod]
    public void EveryUnsupportedProbe_HasExpectedDiagnosticIds()
    {
        var unsupportedProbeIds = Probes
            .Where(static probe => probe.Status == SyntaxSupportStatus.Unsupported)
            .Select(static probe => probe.Id)
            .ToArray();

        CollectionAssert.AreEquivalent(
            unsupportedProbeIds,
            SyntaxProbeExpectedDiagnostics.ProbeIds.ToArray(),
            "Expected diagnostic mappings must match unsupported syntax probes exactly.");
    }

    [TestMethod]
    public void Unsupported_Syntax_Summary_Is_UpToDate()
    {
        var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
        var summaryPath = Path.Combine(repoRoot, "docs", "csharp-syntax", "UnsupportedFeatures.md");
        var actual = NormalizeLineEndings(File.ReadAllText(summaryPath));

        Assert.AreEqual(
            NormalizeLineEndings(RenderUnsupportedFeaturesSummary()),
            actual,
            "UnsupportedFeatures.md must be regenerated from the versioned syntax probes.");
    }

    private static string RenderUnsupportedFeaturesSummary()
    {
        StringBuilder builder = new();
        builder.AppendLine("# Unsupported C# Features in Neo Compiler");
        builder.AppendLine();
        builder.AppendLine("The versioned syntax checklists flag every feature the Neo compiler currently rejects. This page is generated from those checklists so the status remains accurate.");
        builder.AppendLine();
        builder.AppendLine("## Summary by Version");
        builder.AppendLine();

        foreach (var version in Probes
                     .Where(static probe => probe.Status == SyntaxSupportStatus.Unsupported)
                     .GroupBy(static probe => probe.Version))
        {
            builder.Append("- **C# ")
                .Append(version.Key.AsSpan("csharp-".Length))
                .AppendLine(" Syntax Checklist**  ");
            foreach (var probe in version)
            {
                builder.Append("  - ")
                    .Append(probe.Title)
                    .Append(" (`")
                    .Append(probe.Id)
                    .AppendLine("`)");
            }

            builder.AppendLine();
        }

        builder.AppendLine("## Next Actions");
        builder.AppendLine();
        builder.AppendLine("1. Confirm with the compiler team which gaps are expected versus candidates for future support.");
        builder.AppendLine("2. File GitHub issues or backlog items for each unsupported feature that should be implemented.");
        builder.AppendLine("3. Update the version checklists and rerun this script whenever support status changes.");
        return builder.ToString();
    }

    private static string NormalizeLineEndings(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal);
}
