// Copyright (C) 2015-2026 The Neo Project.
//
// GasBaselineSyncScriptTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    /// <summary>
    /// Integration tests for scripts/gas/sync-baseline.ps1 and merge-baseline.ps1 using synthetic snapshots in
    /// temporary directories. They need PowerShell 7 (pwsh on PATH or NEO_PWSH) and are inconclusive without it.
    /// </summary>
    [TestClass]
    public class GasBaselineSyncScriptTests
    {
        private const string Asm = "Asm.Tests";
        private const string M1 = "Asm.Tests.ClassA.M1#1";
        private const string M2 = "Asm.Tests.ClassA.M2#1";

        private string _root = string.Empty;
        private string _repo = string.Empty;

        private string BaselineDir => Path.Combine(_root, "tests", Asm, "GasBaselines", Asm);
        private string CandidateRoot => Path.Combine(_root, "candidates");
        private string CandidateDir => Path.Combine(CandidateRoot, Asm);
        private string Stage => Path.Combine(_root, "stage");
        private string BaselineFile => Path.Combine(BaselineDir, "Asm.Tests.ClassA.json");

        [TestInitialize]
        public void Setup()
        {
            _repo = FindRepositoryRoot();
            _root = Path.Combine(Path.GetTempPath(), "gas-sync-" + Path.GetRandomFileName());
            Directory.CreateDirectory(BaselineDir);
            Directory.CreateDirectory(CandidateDir);
            File.WriteAllText(BaselineFile, Json((M1, 100), (M2, 200)));
            WriteManifest();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_root)) Directory.Delete(_root, true);
        }

        private static string FindRepositoryRoot()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
                if (File.Exists(Path.Combine(dir.FullName, "scripts", "gas", "sync-baseline.ps1"))) return dir.FullName;
            Assert.Inconclusive("Repository scripts were not found.");
            return string.Empty;
        }

        private static string Json(params (string Id, long Gas)[] entries) =>
            "{\n" + string.Join(",\n", entries.Select(e => $"  \"{e.Id}\": {e.Gas}")) + "\n}";

        private void Candidate(string file, params (string Id, long Gas)[] entries) =>
            File.WriteAllText(Path.Combine(CandidateDir, file), Json(entries));

        private void WriteManifest(string sha = "abc", string suite = "success", bool filtered = false) =>
            File.WriteAllText(Path.Combine(CandidateRoot, "manifest.json"),
                $"{{\"schema\":1,\"sha\":\"{sha}\",\"filtered\":{(filtered ? "true" : "false")},\"suites\":{{\"{Asm}\":\"{suite}\"}}}}");

        private (int ExitCode, string Output) Pwsh(params string[] arguments)
        {
            var info = new ProcessStartInfo(Environment.GetEnvironmentVariable("NEO_PWSH") ?? "pwsh")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            foreach (var argument in new[] { "-NoProfile", "-NonInteractive", "-File" }.Concat(arguments)) info.ArgumentList.Add(argument);
            info.Environment.Remove("CI");
            info.Environment.Remove("GITHUB_ACTIONS");
            Process? process;
            try { process = Process.Start(info); }
            catch (Win32Exception) { Assert.Inconclusive("PowerShell 7 (pwsh) is not available."); return (0, string.Empty); }
            using (process!)
            {
                var error = process.StandardError.ReadToEndAsync();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return (process.ExitCode, output + error.Result);
            }
        }

        private (int ExitCode, string Output) Plan(params string[] extra) =>
            Pwsh(new[]
            {
                Path.Combine(_repo, "scripts", "gas", "sync-baseline.ps1"), "-Mode", "Plan", "-TestsRoot", Path.Combine(_root, "tests"),
                "-CandidateRoot", CandidateRoot, "-ManifestPath", Path.Combine(CandidateRoot, "manifest.json"), "-ExpectedSha", "abc",
                "-RequiredAssemblies", Asm, "-StagingDirectory", Stage,
            }.Concat(extra).ToArray());

        private (int ExitCode, string Output) Apply(params string[] extra) =>
            Pwsh(new[]
            {
                Path.Combine(_repo, "scripts", "gas", "sync-baseline.ps1"), "-Mode", "Apply", "-TestsRoot", Path.Combine(_root, "tests"),
                "-RequiredAssemblies", Asm, "-StagingDirectory", Stage,
            }.Concat(extra).ToArray());

        private JsonElement Result(string name = "result.json") =>
            JsonDocument.Parse(File.ReadAllText(Path.Combine(Stage, name))).RootElement.Clone();

        private string Baseline => File.ReadAllText(BaselineFile);

        [TestMethod]
        public void UnchangedGasDoesNotUpdate()
        {
            Candidate("a.json", (M1, 100), (M2, 200));
            Assert.AreEqual(0, Plan().ExitCode);
            Assert.AreEqual("NoChange", Result().GetProperty("status").GetString());
            Assert.IsEmpty(Directory.GetFiles(Path.Combine(Stage), "*.json", SearchOption.AllDirectories).Where(f => !f.EndsWith("result.json")));
        }

        [TestMethod]
        public void LowerGasIsStagedAndApplied()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            Assert.AreEqual(0, Plan().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline, "Plan must not touch committed baselines.");
            Assert.AreEqual("Update", Result().GetProperty("status").GetString());
            Assert.IsTrue(Result().GetProperty("autoMergeEligible").GetBoolean());
            Assert.AreEqual(10, Result().GetProperty("totalGasSaved").GetInt64());

            Assert.AreEqual(0, Apply().ExitCode);
            Assert.AreEqual(Json((M1, 90), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void StagingRejectsEvenAnUnrelatedBaselineChange()
        {
            Candidate("a.json", (M1, 95), (M2, 200));
            Assert.AreEqual(0, Plan().ExitCode);
            File.WriteAllText(BaselineFile, Json((M1, 100), (M2, 210)));
            Assert.AreEqual(1, Apply().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 210)), Baseline);
        }

        [TestMethod]
        public void ParameterSeparatorsDoNotConflateTestIdentities()
        {
            const string first = "Asm.Tests.ClassA.Time[12:30]#1";
            const string second = "Asm.Tests.ClassA.Time[12:45]#1";
            File.WriteAllText(BaselineFile, Json((first, 100)));
            Candidate("a.json", (first, 100), (second, 90));
            var plan = Plan();
            Assert.AreEqual(0, plan.ExitCode, plan.Output);
            Assert.AreEqual(1, Result().GetProperty("newTests").GetInt32());
            Assert.IsFalse(Result().GetProperty("autoMergeEligible").GetBoolean());
        }

        [TestMethod]
        public void ManifestRunIdentityMustMatch()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            Assert.AreEqual(1, Plan("-ExpectedRunId", "42", "-ExpectedRunAttempt", "2").ExitCode);
        }

        [TestMethod]
        public void WorkflowPowerShellBlocksParse()
        {
            var result = Pwsh(Path.Combine(_repo, "scripts", "gas", "validate-workflows.ps1"), "-WorkflowDirectory", Path.Combine(_repo, ".github", "workflows"));
            Assert.AreEqual(0, result.ExitCode, result.Output);
            StringAssert.Contains(Workflow("gas-baseline-sync.yml"), "--body-file");
            StringAssert.Contains(Workflow("gas-baseline-sync.yml"), "--force-with-lease=");
        }

        [TestMethod]
        public void RepeatedSynchronizationIsIdempotent()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            Plan();
            Apply();
            var before = Baseline;
            Assert.AreEqual(0, Plan().ExitCode);
            Assert.AreEqual("NoChange", Result().GetProperty("status").GetString());
            Assert.AreEqual(before, Baseline);
        }

        [TestMethod]
        public void HigherGasIsRejectedAndBaselineStaysUnchanged()
        {
            Candidate("a.json", (M1, 110), (M2, 150));
            Assert.AreEqual(1, Plan().ExitCode);
            Assert.AreEqual("Rejected", Result().GetProperty("status").GetString());
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
            Assert.IsEmpty(Directory.GetDirectories(Stage), "Nothing may be staged for a rejected run.");
        }

        [TestMethod]
        public void ExplicitApprovalAcceptsIncreases()
        {
            Candidate("a.json", (M1, 110), (M2, 200));
            Assert.AreEqual(0, Plan("-AllowIncrease").ExitCode);
            Assert.IsFalse(Result().GetProperty("autoMergeEligible").GetBoolean());
            Assert.AreEqual(1, Apply().ExitCode, "Apply without approval must refuse increases.");
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
            Assert.AreEqual(0, Apply("-AllowIncrease").ExitCode);
            Assert.AreEqual(Json((M1, 110), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void FunctionalFailuresNeverUpdate()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            File.WriteAllText(Path.Combine(CandidateDir, "x.failed"), "Asm.Tests.ClassA.M3\n");
            Assert.AreEqual(1, Plan().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void FailedSuiteInManifestIsRejected()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            WriteManifest(suite: "failure");
            Assert.AreEqual(1, Plan().ExitCode);
        }

        [TestMethod]
        public void MissingFilteredOrMismatchedManifestIsRejected()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            WriteManifest(filtered: true);
            Assert.AreEqual(1, Plan().ExitCode);
            WriteManifest(sha: "other");
            Assert.AreEqual(1, Plan().ExitCode);
            File.Delete(Path.Combine(CandidateRoot, "manifest.json"));
            Assert.AreEqual(1, Plan().ExitCode);
        }

        [TestMethod]
        public void UnstableMeasurementsAreRejected()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            File.WriteAllText(Path.Combine(CandidateDir, "x.unstable"), "Asm.Tests.ClassA.M1\n");
            Assert.AreEqual(1, Plan().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void NewTestsAreOnboardedButNeverAutoMerged()
        {
            const string m3 = "Asm.Tests.ClassB.M3#1";
            Candidate("a.json", (M1, 100), (M2, 200), (m3, 300));
            Assert.AreEqual(0, Plan().ExitCode);
            Assert.AreEqual(1, Result().GetProperty("newTests").GetInt32());
            Assert.IsFalse(Result().GetProperty("autoMergeEligible").GetBoolean());
            Assert.AreEqual(0, Apply().ExitCode);
            Assert.AreEqual(Json((m3, 300)), File.ReadAllText(Path.Combine(BaselineDir, "Asm.Tests.ClassB.json")));
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline, "Unrelated files must not change.");
        }

        [TestMethod]
        public void UnexpectedExecutionInExistingTestIsRejected()
        {
            Candidate("a.json", (M1, 100), (M2, 200), ("Asm.Tests.ClassA.M1#2", 5));
            Assert.AreEqual(1, Plan().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void MissingMeasurementIsNeverDeletedSilently()
        {
            Candidate("a.json", (M1, 90));
            Assert.AreEqual(1, Plan().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void FilteredRunPreservesUntouchedEntries()
        {
            Candidate("a.json", (M1, 90));
            Assert.AreEqual(0, Plan("-Partial").ExitCode);
            Assert.AreEqual(0, Apply().ExitCode);
            Assert.AreEqual(Json((M1, 90), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void DuplicateIdsAreRejected()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            Candidate("b.json", (M1, 90));
            Assert.AreEqual(1, Plan().ExitCode);
            StringAssert.Contains(Result().GetProperty("reasons")[0].GetString(), "Duplicate");
        }

        [TestMethod]
        public void InvalidOrNegativeJsonIsRejectedWithoutChangingFiles()
        {
            File.WriteAllText(Path.Combine(CandidateDir, "a.json"), "{ not json");
            Assert.AreEqual(1, Plan().ExitCode);
            File.WriteAllText(Path.Combine(CandidateDir, "a.json"), Json((M1, -5), (M2, 200)));
            Assert.AreEqual(1, Plan().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void InvalidStagedFilesAreRejectedOnApplyAndNothingChanges()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            Plan();
            File.WriteAllText(Path.Combine(Stage, Asm, "Asm.Tests.ClassA.json"), "{ broken");
            Assert.AreEqual(1, Apply().ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void StagedPayloadCannotTouchOtherPathsOrRemoveMeasurements()
        {
            Candidate("a.json", (M1, 90), (M2, 200));
            Plan();
            File.WriteAllText(Path.Combine(Stage, Asm, "evil.txt"), "x");
            Assert.AreEqual(1, Apply().ExitCode, "Only .json baseline files are accepted.");

            File.Delete(Path.Combine(Stage, Asm, "evil.txt"));
            File.WriteAllText(Path.Combine(Stage, Asm, "Asm.Tests.ClassA.json"), Json((M1, 90)));
            Assert.AreEqual(1, Apply().ExitCode, "Staged files must not drop measurements.");

            File.WriteAllText(Path.Combine(Stage, Asm, "Asm.Tests.ClassA.json"), Json((M1, 90), (M2, 200)));
            Directory.CreateDirectory(Path.Combine(Stage, "Other.Assembly"));
            Assert.AreEqual(1, Apply().ExitCode, "Unexpected assemblies are rejected.");
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void StaleStagingAfterBaselineMovedOnIsRejected()
        {
            Candidate("a.json", (M1, 95), (M2, 200));
            Plan();
            File.WriteAllText(BaselineFile, Json((M1, 90), (M2, 200)));
            Assert.AreEqual(1, Apply().ExitCode, "A concurrent synchronization must not overwrite newer values.");
            Assert.AreEqual(Json((M1, 90), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void MergeBaselinePreservesExistingEntriesUnlessReplacing()
        {
            var input = Path.Combine(_root, "input");
            Directory.CreateDirectory(input);
            File.WriteAllText(Path.Combine(input, "one.json"), Json((M1, 80)));
            var script = Path.Combine(_repo, "scripts", "gas", "merge-baseline.ps1");

            Assert.AreEqual(0, Pwsh(script, "-InputDirectory", input, "-Output", BaselineDir).ExitCode);
            Assert.AreEqual(Json((M1, 80), (M2, 200)), Baseline);

            Assert.AreEqual(0, Pwsh(script, "-InputDirectory", input, "-Output", BaselineDir, "-Replace").ExitCode);
            Assert.AreEqual(Json((M1, 80)), Baseline);
        }

        [TestMethod]
        public void MergeBaselineRejectsDuplicatesAndInvalidInputWithoutChanges()
        {
            var input = Path.Combine(_root, "input");
            Directory.CreateDirectory(input);
            var script = Path.Combine(_repo, "scripts", "gas", "merge-baseline.ps1");
            File.WriteAllText(Path.Combine(input, "one.json"), Json((M1, 80)));
            File.WriteAllText(Path.Combine(input, "two.json"), Json((M1, 70)));
            Assert.AreNotEqual(0, Pwsh(script, "-InputDirectory", input, "-Output", BaselineDir).ExitCode);

            File.Delete(Path.Combine(input, "two.json"));
            File.WriteAllText(Path.Combine(input, "one.json"), "{ nope");
            Assert.AreNotEqual(0, Pwsh(script, "-InputDirectory", input, "-Output", BaselineDir).ExitCode);
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        [TestMethod]
        public void MergeBaselineIsRefusedInCi()
        {
            var input = Path.Combine(_root, "input");
            Directory.CreateDirectory(input);
            File.WriteAllText(Path.Combine(input, "one.json"), Json((M1, 80)));
            var info = new ProcessStartInfo(Environment.GetEnvironmentVariable("NEO_PWSH") ?? "pwsh") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (var argument in new[] { "-NoProfile", "-File", Path.Combine(_repo, "scripts", "gas", "merge-baseline.ps1"), "-InputDirectory", input, "-Output", BaselineDir })
                info.ArgumentList.Add(argument);
            info.Environment["CI"] = "true";
            Process? process;
            try { process = Process.Start(info); }
            catch (Win32Exception) { Assert.Inconclusive("PowerShell 7 (pwsh) is not available."); return; }
            using (process!)
            {
                process.StandardOutput.ReadToEnd();
                process.StandardError.ReadToEnd();
                process.WaitForExit();
                Assert.AreNotEqual(0, process.ExitCode);
            }
            Assert.AreEqual(Json((M1, 100), (M2, 200)), Baseline);
        }

        private string Workflow(string name)
        {
            var path = Path.Combine(_repo, ".github", "workflows", name);
            if (!File.Exists(path)) Assert.Inconclusive("Workflow files are not available.");
            return File.ReadAllText(path, Encoding.UTF8);
        }

        [TestMethod]
        public void TransactionRollsBackAllAssembliesAndExcludesConcurrentProcesses()
        {
            var result = Pwsh(Path.Combine(_repo, "scripts", "gas", "test-transaction.ps1"), "-Root", Path.Combine(_root, "transaction"));
            Assert.AreEqual(0, result.ExitCode, result.Output);
        }

        [TestMethod]
        public void SynchronizationWorkflowNeverRunsForkCodeWithWriteCredentials()
        {
            var sync = Workflow("gas-baseline-sync.yml");
            var approve = Workflow("gas-baseline-approve.yml");
            foreach (var text in new[] { sync, approve, Workflow("main.yml") })
                Assert.DoesNotContain("pull_request_target", text);

            StringAssert.Contains(sync, "workflow_run:");
            Assert.DoesNotContain("\n  pull_request", sync);
            StringAssert.Contains(sync, "github.event.workflow_run.event == 'push'");
            StringAssert.Contains(sync, "github.event.workflow_run.head_repository.full_name == github.repository");
            StringAssert.Contains(sync, "permissions: {}");
            StringAssert.Contains(sync, "cancel-in-progress: false");

            // Only the Publish job gets the app token; the planning job that reads artifacts has no write credentials.
            var planJob = sync.Substring(sync.IndexOf("  Plan:", StringComparison.Ordinal), sync.IndexOf("  Publish:", StringComparison.Ordinal) - sync.IndexOf("  Plan:", StringComparison.Ordinal));
            Assert.DoesNotContain("create-github-app-token", planJob);
            Assert.DoesNotContain("contents: write", sync);
            Assert.DoesNotContain("pull-requests: write", sync);
        }

        [TestMethod]
        public void MainWorkflowOnlyCollectsCandidatesAndStaysReadOnly()
        {
            var main = Workflow("main.yml");
            StringAssert.Contains(main, "NEO_GAS_BASELINE_CANDIDATE_DIR");
            Assert.DoesNotContain("NEO_GAS_BASELINE_GENERATE_DIR", main);
            Assert.DoesNotContain("sync-baseline.ps1", main);
            Assert.DoesNotContain("git push", main);
        }

        [TestMethod]
        public void ApprovalRequiresProtectedEnvironmentAndExactCommit()
        {
            var approve = Workflow("gas-baseline-approve.yml");
            StringAssert.Contains(approve, "workflow_dispatch:");
            StringAssert.Contains(approve, "environment: gas-baseline-approval");
            StringAssert.Contains(approve, "head_sha");
            Assert.DoesNotContain("labeled", approve);
            Assert.DoesNotContain("auto", approve.Substring(approve.IndexOf("gh pr create", StringComparison.Ordinal)));
        }
    }
}
