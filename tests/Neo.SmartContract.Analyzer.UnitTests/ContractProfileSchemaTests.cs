// Copyright (C) 2015-2026 The Neo Project.
//
// ContractProfileSchemaTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class ContractProfileSchemaTests
    {
        private static readonly string RepositoryRoot = FindRepositoryRoot();
        private static readonly JsonSchema ProfileSchema = LoadSchema();
        private static readonly string ValidProfile = File.ReadAllText(Path.Combine(
            RepositoryRoot,
            "tests",
            "Neo.SmartContract.Analyzer.UnitTests",
            "Fixtures",
            "ContractProfile.valid.json"));

        public static IEnumerable<object[]> InvalidProfileMutations()
        {
            yield return ["unknown support state"];
            yield return ["invalid capability id"];
            yield return ["undeclared capability property"];
            yield return ["unsupported capability without diagnostic"];
            yield return ["unsupported capability with warning severity"];
            yield return ["supported capability without runtime evidence"];
            yield return ["partial capability without excluded contexts"];
            yield return ["semantic difference without differential evidence"];
        }

        public static IEnumerable<object[]> ValidSemanticVersions()
        {
            yield return ["0.0.0"];
            yield return ["1.2.3"];
            yield return ["1.0.0-0.3.7"];
            yield return ["1.0.0-alpha.1"];
            yield return ["1.0.0-01a"];
            yield return ["1.0.0-x.7.z.92"];
            yield return ["1.0.0+001"];
            yield return ["1.0.0-alpha.1+build.001"];
        }

        public static IEnumerable<object[]> InvalidSemanticVersions()
        {
            yield return ["01.0.0"];
            yield return ["1.01.0"];
            yield return ["1.0.01"];
            yield return ["1.0.0-01"];
            yield return ["1.0.0-alpha.01"];
            yield return ["1.0.0-"];
            yield return ["1.0.0-.alpha"];
            yield return ["1.0.0-alpha..1"];
            yield return ["1.0.0-alpha."];
            yield return ["1.0.0+"];
            yield return ["1.0.0+.build"];
            yield return ["1.0.0+build..1"];
            yield return ["1.0.0+build."];
        }

        [TestMethod]
        public void CompleteProfileFixture_ShouldSatisfySchema()
        {
            var result = Evaluate(JsonNode.Parse(ValidProfile)!);

            Assert.IsTrue(result.IsValid, FormatResult(result));
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidProfileMutations), DynamicDataSourceType.Method)]
        public void IncompleteOrUnknownProfileData_ShouldFailSchema(string mutation)
        {
            var profile = JsonNode.Parse(ValidProfile)!.AsObject();
            ApplyMutation(profile, mutation);

            var result = Evaluate(profile);

            Assert.IsFalse(result.IsValid, $"Mutation '{mutation}' unexpectedly satisfied the schema.");
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidSemanticVersions), DynamicDataSourceType.Method)]
        public void ValidSemanticVersion_ShouldSatisfySchema(string semanticVersion)
        {
            var profile = JsonNode.Parse(ValidProfile)!.AsObject();
            profile["profileVersion"] = semanticVersion;

            var result = Evaluate(profile);

            Assert.IsTrue(result.IsValid, $"Semantic version '{semanticVersion}' was rejected.\n{FormatResult(result)}");
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidSemanticVersions), DynamicDataSourceType.Method)]
        public void InvalidSemanticVersion_ShouldFailSchema(string semanticVersion)
        {
            var profile = JsonNode.Parse(ValidProfile)!.AsObject();
            profile["profileVersion"] = semanticVersion;

            var result = Evaluate(profile);

            Assert.IsFalse(result.IsValid, $"Semantic version '{semanticVersion}' unexpectedly satisfied the schema.");
        }

        private static void ApplyMutation(JsonObject profile, string mutation)
        {
            var capabilities = profile["capabilities"]!.AsObject();
            switch (mutation)
            {
                case "unknown support state":
                    capabilities["type.system-single"]!["state"] = "unknown";
                    break;
                case "invalid capability id":
                    capabilities["Invalid capability id"] = capabilities["type.system-single"]!.DeepClone();
                    break;
                case "undeclared capability property":
                    capabilities["type.system-single"]!["unexpected"] = true;
                    break;
                case "unsupported capability without diagnostic":
                    capabilities["type.system-single"]!.AsObject().Remove("diagnostic");
                    break;
                case "unsupported capability with warning severity":
                    capabilities["type.system-single"]!["diagnostic"]!["severity"] = "Warning";
                    break;
                case "supported capability without runtime evidence":
                    RemoveEvidence(capabilities["syntax.expression-bodied-member"]!, "runtime");
                    break;
                case "partial capability without excluded contexts":
                    capabilities["syntax.range-byte-string"]!["contexts"]!.AsObject().Remove("excluded");
                    break;
                case "semantic difference without differential evidence":
                    RemoveEvidence(capabilities["semantic.bool-try-parse"]!, "differential");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mutation), mutation, "Unknown profile mutation.");
            }
        }

        private static void RemoveEvidence(JsonNode capability, string kind)
        {
            var evidence = capability["evidence"]!.AsArray();
            for (var index = evidence.Count - 1; index >= 0; index--)
            {
                if (evidence[index]?["kind"]?.GetValue<string>() == kind)
                {
                    evidence.RemoveAt(index);
                }
            }
        }

        private static EvaluationResults Evaluate(JsonNode profile)
        {
            using var document = JsonDocument.Parse(profile.ToJsonString());
            return ProfileSchema.Evaluate(
                document.RootElement,
                new EvaluationOptions { OutputFormat = OutputFormat.Hierarchical });
        }

        private static JsonSchema LoadSchema()
        {
            var schemaPath = Path.Combine(RepositoryRoot, "profiles", "neo-csharp-profile.schema.json");
            using var document = JsonDocument.Parse(File.ReadAllText(schemaPath));
            return JsonSchema.Build(document.RootElement.Clone());
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null)
            {
                var schemaPath = Path.Combine(directory.FullName, "profiles", "neo-csharp-profile.schema.json");
                if (File.Exists(schemaPath)) return directory.FullName;

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Unable to locate the Neo C# contract profile schema.");
        }

        private static string FormatResult(EvaluationResults result) =>
            JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }
}
