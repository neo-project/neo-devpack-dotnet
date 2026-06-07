// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ArtifactDiff.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ArtifactDiff
    {
        [TestMethod]
        public void CompareReportsAddedAndRemovedMethods()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("balanceOf", ContractParameterType.Integer, true, Parameter("account", ContractParameterType.Hash160)),
                Method("transfer", ContractParameterType.Boolean, false, Parameter("to", ContractParameterType.Hash160), Parameter("amount", ContractParameterType.Integer))
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("balanceOf", ContractParameterType.Integer, true, Parameter("account", ContractParameterType.Hash160)),
                Method("mint", ContractParameterType.Void, false, Parameter("to", ContractParameterType.Hash160), Parameter("amount", ContractParameterType.Integer))
            ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsTrue(report.HasBreakingChanges);
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Abi && c.Severity == ArtifactDiffSeverity.Breaking && c.Description.Contains("Removed method: transfer")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Abi && c.Severity == ArtifactDiffSeverity.Info && c.Description.Contains("Added method: mint")));
        }

        [TestMethod]
        public void CompareMarksMethodSignatureChangesAsBreaking()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("transfer", ContractParameterType.Boolean, false, Parameter("to", ContractParameterType.Hash160), Parameter("amount", ContractParameterType.Integer))
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("transfer", ContractParameterType.Void, false, Parameter("to", ContractParameterType.Hash160), Parameter("amount", ContractParameterType.Integer), Parameter("data", ContractParameterType.Any))
            ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsTrue(report.HasBreakingChanges);
            Assert.IsTrue(report.Changes.Any(c => c.Description.Contains("Changed method signature: transfer(Hash160, Integer) -> Boolean => transfer(Hash160, Integer, Any) -> Void")));
        }

        [TestMethod]
        public void CompareMarksPermissionExpansionAsWarning()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("transfer", ContractParameterType.Boolean, false)
            ],
            [
                Permission("0x0102030405060708090a0b0c0d0e0f1011121314", "balanceOf")
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("transfer", ContractParameterType.Boolean, false)
            ],
            [
                Permission("0x0102030405060708090a0b0c0d0e0f1011121314", "balanceOf"),
                Permission("*", "*")
            ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsFalse(report.HasBreakingChanges);
            Assert.IsTrue(report.HasWarnings);
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Permission && c.Severity == ArtifactDiffSeverity.Warning && c.Description.Contains("Added permission: *::*")));
        }

        [TestMethod]
        public void CompareHandlesOverloadedAbiMethods()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("get", ContractParameterType.Integer, true),
                Method("get", ContractParameterType.Integer, true, Parameter("account", ContractParameterType.Hash160))
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("get", ContractParameterType.Integer, true)
            ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsTrue(report.HasBreakingChanges);
            Assert.IsTrue(report.Changes.Any(c => c.Description.Contains("Removed method: get(Hash160) -> Integer")));
        }

        [TestMethod]
        public void PrintIncludesSummaryAndChanges()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("get", ContractParameterType.Integer, true)
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("get", ContractParameterType.Integer, true),
                Method("set", ContractParameterType.Void, false, Parameter("value", ContractParameterType.Integer))
            ]);
            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);
            var writer = new StringWriter();

            ArtifactDiffReporter.Print(report, writer);
            string output = writer.ToString();

            Assert.IsTrue(output.Contains("Artifact Diff Report"));
            Assert.IsTrue(output.Contains("Breaking changes:"));
            Assert.IsTrue(output.Contains("no"));
            Assert.IsTrue(output.Contains("[info] ABI: Added method: set"), output);
        }

        [TestMethod]
        public void DiffCommandPrintsArtifactDiffReport()
        {
            string directory = Path.Combine(Path.GetTempPath(), "NeoArtifactDiff_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string oldNefPath = Path.Combine(directory, "old.nef");
            string oldManifestPath = Path.Combine(directory, "old.manifest.json");
            string newNefPath = Path.Combine(directory, "new.nef");
            string newManifestPath = Path.Combine(directory, "new.manifest.json");

            try
            {
                File.WriteAllBytes(oldNefPath, Contract_ABISafe.Nef.ToArray());
                File.WriteAllText(oldManifestPath, Contract_ABISafe.Manifest.ToJson().ToString(false));
                File.WriteAllBytes(newNefPath, Contract_NEP17.Nef.ToArray());
                File.WriteAllText(newManifestPath, Contract_NEP17.Manifest.ToJson().ToString(false));

                var stdout = new StringWriter();
                var stderr = new StringWriter();
                TextWriter originalOut = Console.Out;
                TextWriter originalErr = Console.Error;
                int exitCode;
                try
                {
                    Console.SetOut(stdout);
                    Console.SetError(stderr);
                    exitCode = Program.Main(["diff", oldNefPath, oldManifestPath, newNefPath, newManifestPath]);
                }
                finally
                {
                    Console.SetOut(originalOut);
                    Console.SetError(originalErr);
                }

                Assert.AreEqual(0, exitCode, stderr.ToString());
                Assert.AreEqual(string.Empty, stderr.ToString());
                StringAssert.Contains(stdout.ToString(), "Artifact Diff Report");
            }
            finally
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
            }
        }

        [TestMethod]
        public void DiffCommandFailOnBreakingReturnsTwo()
        {
            string directory = Path.Combine(Path.GetTempPath(), "NeoArtifactDiff_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string oldNefPath = Path.Combine(directory, "old.nef");
            string oldManifestPath = Path.Combine(directory, "old.manifest.json");
            string newNefPath = Path.Combine(directory, "new.nef");
            string newManifestPath = Path.Combine(directory, "new.manifest.json");

            try
            {
                File.WriteAllBytes(oldNefPath, Contract_ABISafe.Nef.ToArray());
                File.WriteAllText(oldManifestPath, Contract_ABISafe.Manifest.ToJson().ToString(false));
                File.WriteAllBytes(newNefPath, Contract_NEP17.Nef.ToArray());
                File.WriteAllText(newManifestPath, Contract_NEP17.Manifest.ToJson().ToString(false));

                var stdout = new StringWriter();
                var stderr = new StringWriter();
                TextWriter originalOut = Console.Out;
                TextWriter originalErr = Console.Error;
                int exitCode;
                try
                {
                    Console.SetOut(stdout);
                    Console.SetError(stderr);
                    exitCode = Program.Main(["diff", oldNefPath, oldManifestPath, newNefPath, newManifestPath, "--fail-on-breaking"]);
                }
                finally
                {
                    Console.SetOut(originalOut);
                    Console.SetError(originalErr);
                }

                Assert.AreEqual(2, exitCode, stderr.ToString());
                Assert.AreEqual(string.Empty, stderr.ToString());
                StringAssert.Contains(stdout.ToString(), "Breaking changes:");
            }
            finally
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
            }
        }

        private static ContractManifest CreateManifest(ContractMethodDescriptor[] methods, ContractPermission[]? permissions = null)
        {
            return new ContractManifest
            {
                Name = "TestContract",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods = methods,
                    Events = []
                },
                Permissions = permissions ?? [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }

        private static ContractMethodDescriptor Method(string name, ContractParameterType returnType, bool safe, params ContractParameterDefinition[] parameters)
        {
            return new ContractMethodDescriptor
            {
                Name = name,
                Parameters = parameters,
                ReturnType = returnType,
                Safe = safe
            };
        }

        private static ContractParameterDefinition Parameter(string name, ContractParameterType type)
        {
            return new ContractParameterDefinition
            {
                Name = name,
                Type = type
            };
        }

        private static ContractPermission Permission(string contract, params string[] methods)
        {
            return new ContractPermission
            {
                Contract = ContractPermissionDescriptor.FromJson(contract),
                Methods = WildcardContainer<string>.Create(methods)
            };
        }
    }
}
