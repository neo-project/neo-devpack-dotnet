// Copyright (C) 2015-2025 The Neo Project.
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
using System.Text.RegularExpressions;

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
    public void Unsupported_Syntax_Summary_Is_UpToDate()
    {
        var unsupportedIds = Probes
            .Where(p => p.Status == SyntaxSupportStatus.Unsupported)
            .Select(p => p.Id)
            .Distinct()
            .ToArray();

        var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
        var summaryPath = Path.Combine(repoRoot, "docs", "csharp-syntax", "UnsupportedFeatures.md");
        var content = File.ReadAllText(summaryPath);
        var referencedIds = Regex.Matches(content, @"\(`?([a-z0-9_]+)`?\)")
            .Select(match => match.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);

        var missing = unsupportedIds.Where(id => !referencedIds.Contains(id)).ToArray();
        CollectionAssert.AreEquivalent(Array.Empty<string>(), missing, "UnsupportedFeatures.md is missing entries: " + string.Join(", ", missing));

        var unknown = referencedIds.Where(id => !unsupportedIds.Contains(id)).ToArray();
        CollectionAssert.AreEquivalent(Array.Empty<string>(), unknown, "UnsupportedFeatures.md references unexpected ids: " + string.Join(", ", unknown));
    }

}
