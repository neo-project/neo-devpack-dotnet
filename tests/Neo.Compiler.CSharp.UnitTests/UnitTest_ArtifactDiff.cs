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
using Neo.VM;
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
        public void CompareReportsNefManifestEventAndStandardChanges()
        {
            var oldNef = CreateNef("old-compiler", [(byte)OpCode.RET]);
            var newNef = CreateNef("new-compiler", [(byte)OpCode.PUSH1, (byte)OpCode.RET]);
            ContractManifest oldManifest = CreateManifest(
                [],
                name: "OldContract",
                standards: ["NEP-17"],
                events: [Event("Transfer", Parameter("from", ContractParameterType.Hash160))]);
            ContractManifest newManifest = CreateManifest(
                [],
                name: "NewContract",
                standards: ["NEP-11"],
                events: [Event("Mint", Parameter("to", ContractParameterType.Hash160))]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldNef, oldManifest, newNef, newManifest);

            Assert.IsTrue(report.HasBreakingChanges);
            Assert.IsTrue(report.HasWarnings);
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Nef && c.Description.Contains("Checksum changed")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Nef && c.Description.Contains("Script changed: 1 bytes => 2 bytes")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Nef && c.Description.Contains("Compiler changed")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Manifest && c.Description.Contains("Name changed")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Abi && c.Description.Contains("Removed event: Transfer")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Abi && c.Description.Contains("Added event: Mint")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Standard && c.Severity == ArtifactDiffSeverity.Warning && c.Description.Contains("Removed supported standard: NEP-17")));
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Standard && c.Description.Contains("Added supported standard: NEP-11")));
        }

        [TestMethod]
        public void CompareReportsSameSizeScriptContentChanges()
        {
            var oldNef = CreateNef("compiler", [(byte)OpCode.PUSH1, (byte)OpCode.RET]);
            var newNef = CreateNef("compiler", [(byte)OpCode.PUSH2, (byte)OpCode.RET]);
            ContractManifest manifest = CreateManifest([]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldNef, manifest, newNef, manifest);

            Assert.IsFalse(report.HasBreakingChanges);
            Assert.IsFalse(report.HasWarnings);
            Assert.IsTrue(report.Changes.Any(c => c.Category == ArtifactDiffCategory.Nef && c.Description == "Script changed (same size, different content)"));
        }

        [TestMethod]
        public void CompareReportsSafeFlagChanges()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("read", ContractParameterType.Integer, false),
                Method("write", ContractParameterType.Void, true)
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("read", ContractParameterType.Integer, true),
                Method("write", ContractParameterType.Void, false)
            ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsTrue(report.HasWarnings);
            Assert.IsTrue(report.Changes.Any(c => c.Severity == ArtifactDiffSeverity.Info && c.Description.Contains("Changed safe flag: read: no => yes")));
            Assert.IsTrue(report.Changes.Any(c => c.Severity == ArtifactDiffSeverity.Warning && c.Description.Contains("Changed safe flag: write: yes => no")));
        }

        [TestMethod]
        public void CompareReportsOverloadedReturnAndSafeChanges()
        {
            ContractManifest oldManifest = CreateManifest(
            [
                Method("get", ContractParameterType.Integer, true),
                Method("get", ContractParameterType.Integer, true, Parameter("account", ContractParameterType.Hash160))
            ]);
            ContractManifest newManifest = CreateManifest(
            [
                Method("get", ContractParameterType.String, true),
                Method("get", ContractParameterType.Integer, false, Parameter("account", ContractParameterType.Hash160))
            ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsTrue(report.HasBreakingChanges);
            Assert.IsTrue(report.HasWarnings);
            Assert.IsTrue(report.Changes.Any(c => c.Severity == ArtifactDiffSeverity.Breaking && c.Description.Contains("Changed method signature: get() -> Integer => get() -> String")));
            Assert.IsTrue(report.Changes.Any(c => c.Severity == ArtifactDiffSeverity.Warning && c.Description.Contains("Changed safe flag: get(Hash160): yes => no")));
        }

        [TestMethod]
        public void CompareReportsOverloadedEvents()
        {
            ContractManifest oldManifest = CreateManifest(
                [],
                events:
                [
                    Event("Notify"),
                    Event("Notify", Parameter("value", ContractParameterType.Integer))
                ]);
            ContractManifest newManifest = CreateManifest(
                [],
                events:
                [
                    Event("Notify"),
                    Event("Notify", Parameter("value", ContractParameterType.String))
                ]);

            ArtifactDiffReport report = ArtifactDiffReporter.Compare(oldManifest, newManifest);

            Assert.IsTrue(report.HasBreakingChanges);
            Assert.IsTrue(report.Changes.Any(c => c.Description.Contains("Removed event: Notify(Integer)")));
            Assert.IsTrue(report.Changes.Any(c => c.Description.Contains("Added event: Notify(String)")));
        }

        [TestMethod]
        public void PrintReportsNoChanges()
        {
            ContractManifest manifest = CreateManifest([]);
            ArtifactDiffReport report = ArtifactDiffReporter.Compare(manifest, manifest);
            var writer = new StringWriter();

            ArtifactDiffReporter.Print(report, writer);
            string output = writer.ToString();

            Assert.IsFalse(report.HasBreakingChanges);
            Assert.IsFalse(report.HasWarnings);
            StringAssert.Contains(output, "No artifact changes detected.");
        }

        [TestMethod]
        public void CompareValidatesArguments()
        {
            ContractManifest manifest = CreateManifest([]);

            Assert.ThrowsExactly<ArgumentNullException>(() => ArtifactDiffReporter.Compare(null!, manifest));
            Assert.ThrowsExactly<ArgumentNullException>(() => ArtifactDiffReporter.Compare(manifest, null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => ArtifactDiffReporter.Compare(null, manifest, null, null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => ArtifactDiffReporter.Print(null!, new StringWriter()));
            Assert.ThrowsExactly<ArgumentNullException>(() => ArtifactDiffReporter.Print(ArtifactDiffReporter.Compare(manifest, manifest), null!));
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

        [TestMethod]
        public void DiffCommandReportsMissingArtifact()
        {
            string missingPath = Path.Combine(Path.GetTempPath(), "missing-" + Guid.NewGuid().ToString("N") + ".nef");
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            TextWriter originalOut = Console.Out;
            TextWriter originalErr = Console.Error;
            int exitCode;
            try
            {
                Console.SetOut(stdout);
                Console.SetError(stderr);
                exitCode = Program.Main(["diff", missingPath, missingPath, missingPath, missingPath]);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
            }

            Assert.AreEqual(1, exitCode);
            Assert.AreEqual(string.Empty, stdout.ToString());
            StringAssert.Contains(stderr.ToString(), "Error comparing artifacts:");
            StringAssert.Contains(stderr.ToString(), "NEF file not found:");
        }

        private static ContractManifest CreateManifest(
            ContractMethodDescriptor[] methods,
            ContractPermission[]? permissions = null,
            string name = "TestContract",
            string[]? standards = null,
            ContractEventDescriptor[]? events = null)
        {
            return new ContractManifest
            {
                Name = name,
                Groups = [],
                SupportedStandards = standards ?? [],
                Abi = new ContractAbi
                {
                    Methods = methods,
                    Events = events ?? []
                },
                Permissions = permissions ?? [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }

        private static ContractEventDescriptor Event(string name, params ContractParameterDefinition[] parameters)
        {
            return new ContractEventDescriptor
            {
                Name = name,
                Parameters = parameters
            };
        }

        private static NefFile CreateNef(string compiler, byte[] script)
        {
            var nef = new NefFile
            {
                Compiler = compiler,
                Source = "test.cs",
                Tokens = [],
                Script = script
            };
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            return nef;
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
