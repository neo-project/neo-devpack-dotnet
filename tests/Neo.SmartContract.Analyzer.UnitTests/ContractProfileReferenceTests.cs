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

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

            if (value["diagnostic"] is JsonObject diagnostic)
                AssertRepositoryPath(repositoryRoot, diagnostic["helpLink"]!.GetValue<string>(), capability.Key, "diagnostic.helpLink");

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

    [TestMethod]
    [DataRow("profiles/missing.json")]
    [DataRow("MissingEvidenceLabel")]
    [DataRow("tests/Neo.SmartContract.Analyzer.UnitTests")]
    [DataRow("docs/diagnostics/NC4002.md#missing-anchor")]
    [DataRow("src/../README.md")]
    [DataRow("tests/Neo.SmartContract.Analyzer.UnitTests/ContractProfileReferenceTests.cs#MissingEvidenceMethod")]
    public void InvalidFileReferencesMustFail(string reference)
    {
        Assert.ThrowsException<AssertFailedException>(() => AssertRepositoryPath(FindRepositoryRoot(), reference, "test", "evidence"));
    }

    [TestMethod]
    public void AbsoluteFileReferencesMustFail()
    {
        var root = FindRepositoryRoot();
        Assert.ThrowsException<AssertFailedException>(() => AssertRepositoryPath(root, Path.Combine(root, "README.md"), "test", "evidence"));
    }

    [TestMethod]
    public void ExistingCSharpEvidenceMethodIsAccepted()
    {
        AssertRepositoryPath(FindRepositoryRoot(), "tests/Neo.SmartContract.Analyzer.UnitTests/ContractProfileReferenceTests.cs#ProfileDiagnosticIds_ShouldBeKnownAndWellFormed", "test", "evidence");
    }

    private static void AssertRepositoryPath(string repositoryRoot, string reference, string capabilityId, string field)
    {
        var parts = reference.Split('#', 2);
        var pathPart = parts[0];
        Assert.IsFalse(Path.IsPathRooted(pathPart), $"Profile references must be repository-relative: '{reference}'.");
        Assert.IsFalse(pathPart.Contains('\\'), $"Profile references must use forward slashes: '{reference}'.");
        Assert.IsFalse(pathPart.Split('/').Any(part => part is ".." or "." or ""), $"Profile references must use canonical paths: '{reference}'.");
        var fullPath = Path.GetFullPath(Path.Combine(repositoryRoot, pathPart.Replace('/', Path.DirectorySeparatorChar)));
        var relativePath = Path.GetRelativePath(repositoryRoot, fullPath);
        Assert.IsFalse(relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) || Path.IsPathRooted(relativePath),
            $"Profile references must stay inside the repository: '{reference}'.");
        Assert.IsTrue(
            File.Exists(fullPath) || (field == "implementation" && Directory.Exists(fullPath)),
            $"Profile capability '{capabilityId}' has a missing {field} reference: '{reference}'.");

        if (parts.Length == 2 && string.Equals(Path.GetExtension(fullPath), ".cs", StringComparison.OrdinalIgnoreCase))
        {
            var methods = CSharpSyntaxTree.ParseText(File.ReadAllText(fullPath)).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>();
            Assert.IsTrue(methods.Any(method => method.Identifier.ValueText == parts[1]),
                $"Profile capability '{capabilityId}' has a missing C# method for {field}: '{reference}'.");
        }
        else if (parts.Length == 2 && !string.Equals(Path.GetExtension(fullPath), ".md", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Fail($"Profile fragments must reference a C# method or Markdown anchor: '{reference}'.");
        }

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
