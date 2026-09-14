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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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

    private static void AssertRepositoryPath(string repositoryRoot, string reference, string capabilityId, string field)
    {
        var pathPart = reference.Split('#', 2)[0];
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
