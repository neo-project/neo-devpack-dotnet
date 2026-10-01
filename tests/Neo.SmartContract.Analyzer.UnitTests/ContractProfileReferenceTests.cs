// Copyright (C) 2015-2026 The Neo Project.
//
// ContractProfileReferenceTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class ContractProfileReferenceTests
{
    [TestMethod]
    public void ProfileFileReferences_ShouldResolveInsideRepository()
    {
        var repositoryRoot = FindRepositoryRoot();
        var profilePath = Path.Combine(
            repositoryRoot,
            "tests",
            "Neo.SmartContract.Analyzer.UnitTests",
            "Fixtures",
            "ContractProfile.valid.json");
        var profile = JsonNode.Parse(File.ReadAllText(profilePath))!.AsObject();

        foreach (var capability in profile["capabilities"]!.AsObject())
        {
            var value = capability.Value!.AsObject();
            foreach (var implementation in value["implementation"]?.AsArray() ?? [])
            {
                AssertRepositoryPath(repositoryRoot, implementation!.GetValue<string>(), capability.Key, "implementation");
            }

            foreach (var evidence in value["evidence"]!.AsArray())
            {
                var reference = evidence!.AsObject()["reference"]!.GetValue<string>();
                AssertRepositoryPath(repositoryRoot, reference, capability.Key, "evidence");
            }

            if (value["semanticDifference"] is JsonObject semanticDifference)
            {
                AssertRepositoryPath(
                    repositoryRoot,
                    semanticDifference["documentation"]!.GetValue<string>(),
                    capability.Key,
                    "semanticDifference.documentation");
            }
        }
    }

    [TestMethod]
    public void ProfileDiagnosticIds_ShouldBeKnownAndWellFormed()
    {
        var repositoryRoot = FindRepositoryRoot();
        var profilePath = Path.Combine(
            repositoryRoot,
            "tests",
            "Neo.SmartContract.Analyzer.UnitTests",
            "Fixtures",
            "ContractProfile.valid.json");
        var profile = JsonNode.Parse(File.ReadAllText(profilePath))!.AsObject();
        var profileDiagnosticIds = new List<string>();

        foreach (var capability in profile["capabilities"]!.AsObject())
        {
            if (capability.Value?["diagnostic"] is JsonObject diagnostic)
            {
                profileDiagnosticIds.Add(diagnostic["id"]!.GetValue<string>());
            }
        }

        var knownAnalyzerIds = NeoAnalyzerSuite.Create()
            .SelectMany(analyzer => analyzer.SupportedDiagnostics)
            .Select(descriptor => descriptor.Id);
        var knownCompilerIds = LoadCompilerDiagnosticIds(repositoryRoot);
        var knownIds = knownAnalyzerIds
            .Concat(knownCompilerIds)
            .ToHashSet(StringComparer.Ordinal);
        var unknownIds = profileDiagnosticIds
            .Where(id => !knownIds.Contains(id))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();
        var malformedIds = profileDiagnosticIds
            .Where(id => !Regex.IsMatch(id, "^NC[0-9]{4}$", RegexOptions.CultureInvariant))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.AreEqual(
            0,
            unknownIds.Length,
            "Profile diagnostics are missing from the compiler or analyzer catalog: " + string.Join(", ", unknownIds));
        Assert.AreEqual(
            0,
            malformedIds.Length,
            "Profile diagnostics do not use the NC#### format: " + string.Join(", ", malformedIds));
    }

    private static void AssertRepositoryPath(string repositoryRoot, string reference, string capabilityId, string field)
    {
        var parts = reference.Split('#', 2);
        var pathPart = parts[0];
        if (!pathPart.StartsWith("src/", StringComparison.Ordinal) &&
            !pathPart.StartsWith("tests/", StringComparison.Ordinal) &&
            !pathPart.StartsWith("docs/", StringComparison.Ordinal))
        {
            return;
        }

        var fullPath = Path.Combine(repositoryRoot, pathPart.Replace('/', Path.DirectorySeparatorChar));
        Assert.IsTrue(
            File.Exists(fullPath) || Directory.Exists(fullPath),
            $"Profile capability '{capabilityId}' has a missing {field} reference: '{reference}'.");

        if (parts.Length == 2 && File.Exists(fullPath) &&
            string.Equals(Path.GetExtension(fullPath), ".md", StringComparison.OrdinalIgnoreCase))
        {
            var anchor = parts[1];
            var marker = $"<a id=\"{anchor}\"></a>";
            StringAssert.Contains(
                File.ReadAllText(fullPath),
                marker,
                $"Profile capability '{capabilityId}' has a missing Markdown anchor for {field}: '{reference}'.");
        }
    }

    private static IEnumerable<string> LoadCompilerDiagnosticIds(string repositoryRoot)
    {
        var sourcePath = Path.Combine(
            repositoryRoot,
            "src",
            "Neo.Compiler.CSharp",
            "Diagnostic",
            "DiagnosticId.cs");
        var source = File.ReadAllText(sourcePath);
        return Regex.Matches(
                source,
                @"\bconst\s+string\s+\w+\s*=\s*""(NC[0-9]{4})""",
                RegexOptions.CultureInvariant)
            .Cast<Match>()
            .Select(match => match.Groups[1].Value);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "profiles", "neo-csharp-profile.schema.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate the repository root containing the Neo C# profile schema.");
    }
}
