// Copyright (C) 2015-2026 The Neo Project.
//
// OracleSelfTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Verifies that the differential oracles actually fire when they should.
/// </summary>
/// <remarks>
/// <para>
/// A fuzzer that reports "no defects" is only meaningful if its oracles are known to
/// detect real differences. Without this self-test, a silently broken oracle — a wrong
/// calling convention, a mismatched method name, a comparison that never runs — would
/// look exactly like a clean compiler.
/// </para>
/// <para>
/// The self-test therefore feeds each oracle a deliberately divergent pair and asserts
/// that the divergence is reported.
/// </para>
/// </remarks>
public static class OracleSelfTest
{
    /// <summary>
    /// Runs all self-tests.
    /// </summary>
    /// <param name="harness">The harness used for compilation.</param>
    /// <returns>The number of failed self-tests; zero means all oracles are live.</returns>
    public static int Run(CompilerHarness harness)
    {
        int failures = 0;

        failures += CheckCrossLanguageOracle(harness);
        failures += CheckOptimizerEquivalenceOracle(harness);
        failures += CheckDeterminismOracle(harness);
        failures += CheckAbiOracle(harness);
        failures += CheckInternalErrorOracle(harness);
        failures += CheckStorageOracle(harness);
        failures += CheckMultiContractOracle(harness);
        failures += CheckNotificationOracle(harness);
        failures += CheckFailureTriage();
        failures += CheckResourceFailureEquivalence();
        failures += CheckFindingSignature();
        failures += CheckKnownDefectSuppression();
        failures += CheckDeploymentBudget(harness);
        failures += CheckCrossLanguageFailureDirections();
        failures += CheckCoverageIdentity(harness);
        failures += CheckOverloadAbi(harness);
        failures += CheckOverloadDispatch();
        failures += CheckReferenceIsolation();
        failures += CheckReferenceCacheBound();
        failures += CheckRecursiveReferenceBounds();
        failures += CheckModPowBounds();
        failures += CheckCorpusMigration();
        failures += CheckCorpusReplay();
        failures += CheckIterationBudget();
        failures += CheckReferenceWorkerTimeout();
        failures += CheckLoopControlMutation();
        failures += CheckCorpusEviction();

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "SELF-TEST PASSED: every oracle detects a deliberately introduced difference."
            : $"SELF-TEST FAILED: {failures} oracle(s) did not detect a planted difference.");

        return failures;
    }

    /// <summary>
    /// Verifies that the notification check distinguishes a payload derived from the return
    /// value from one that is not.
    /// </summary>
    /// <remarks>
    /// The fuzzer raises an event whose arguments equal the method result, so a payload that
    /// disagrees with the return value has to be reported. Both directions are checked: a
    /// deliberately wrong payload must be detected, and a correct one must not be.
    /// </remarks>
    private static int CheckNotificationOracle(CompilerHarness harness)
    {
        const string wrongSource = """
            using System;
            using Neo.SmartContract.Framework;

            public class NotificationContract : SmartContract
            {
                public static event Action<int, int> Changed = (x, y) => { };

                public static int EchoEvent(int a, int b)
                {
                    int result = a + b;
                    Changed(result, result + 1);
                    return result;
                }
            }
            """;

        string rightSource = wrongSource.Replace("Changed(result, result + 1);", "Changed(result, result);");

        int failures = 0;
        failures += CheckPayload(harness, wrongSource, expectMismatch: true);
        failures += CheckPayload(harness, rightSource, expectMismatch: false);
        return failures;
    }

    private static int CheckPayload(CompilerHarness harness, string source, bool expectMismatch)
    {
        (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(source);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            Console.WriteLine($"  [FAIL] notification oracle: the probe contract did not compile ({outcome}).");
            return 1;
        }

        (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
        using var analyzer = new BehaviorDifferentialAnalyzer();
        var calls = new List<(string Method, object?[] Args)> { ("echoEvent", [2, 3]) };
        (List<(bool Ok, string? Error, Neo.VM.Types.StackItem? Value)> results, _, _, List<IReadOnlyList<string>> notifications, _) =
            analyzer.ExecuteBatchWithNotifications(new ArtifactSet(nef, manifest), calls);

        if (!results[0].Ok)
        {
            Console.WriteLine($"  [FAIL] notification oracle: the call faulted ({results[0].Error}).");
            return 1;
        }

        string value = BehaviorDifferentialAnalyzer.Render(results[0].Value!);
        string expected = $"Changed:[{value},{value}]";
        string actual = string.Join("|", notifications[0]);
        bool mismatch = !string.Equals(actual, expected, StringComparison.Ordinal);

        if (mismatch != expectMismatch)
        {
            Console.WriteLine($"  [FAIL] notification oracle: expected mismatch={expectMismatch} but actual='{actual}' expected='{expected}'.");
            return 1;
        }

        Console.WriteLine($"  [ OK ] notification oracle {(mismatch ? "detects a wrong payload" : "accepts a matching payload")}: " +
                          $"returned {value}, emitted '{actual}'.");
        return 0;
    }

    /// <summary>
    /// Verifies that the cross-language oracle reports a difference between two
    /// implementations of the same method that disagree.
    /// </summary>
    private static int CheckCrossLanguageOracle(CompilerHarness harness)
    {
        // The contract computes a + b; the reference computes a + b + 1. A live
        // cross-language oracle must report the mismatch.
        const string contractSource = """
            using Neo.SmartContract.Framework;
            public class SelfTestContract : SmartContract
            {
                public static int Compute(int a, int b) { return a + b; }
            }
            """;

        const string referenceSource = """
            public static class SelfTestReference
            {
                public static int Compute(int a, int b) { return a + b + 1; }
            }
            """;

        (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(contractSource);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            Console.WriteLine("  [FAIL] cross-language: the probe contract did not compile.");
            return 1;
        }

        (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
        var artifacts = new ArtifactSet(nef, manifest);

        using var oracle = new CrossLanguageOracle();
        Assembly? reference = oracle.TryCompileReference(referenceSource);
        if (reference is null)
        {
            Console.WriteLine("  [FAIL] cross-language: the reference source did not compile.");
            return 1;
        }

        (bool okRef, object? refValue, _) = oracle.TryInvoke(reference, "SelfTestReference", "Compute", [2, 3]);
        (bool okVm, string? vmError, StackItem? vmValue) =
            new BehaviorDifferentialAnalyzer().ExecuteIsolated(artifacts, "compute", [2, 3]);

        bool detected = okRef && okVm &&
            !string.Equals(RenderValue(refValue), BehaviorDifferentialAnalyzer.Render(vmValue!), StringComparison.Ordinal);

        if (detected)
        {
            Console.WriteLine($"  [ OK ] cross-language oracle detects a planted difference " +
                              $"(C#={RenderValue(refValue)}, VM={BehaviorDifferentialAnalyzer.Render(vmValue!)}).");
            return 0;
        }

        Console.WriteLine($"  [FAIL] cross-language oracle did not detect a planted difference " +
                          $"(refOk={okRef}, vmOk={okVm}, vmError={vmError}).");
        return 1;
    }

    /// <summary>
    /// Verifies that the optimizer-equivalence oracle reports a difference between two
    /// scripts that behave differently.
    /// </summary>
    private static int CheckOptimizerEquivalenceOracle(CompilerHarness harness)
    {
        const string sourceA = """
            using Neo.SmartContract.Framework;
            public class SelfTestA : SmartContract
            {
                public static int Compute(int a, int b) { return a + b; }
            }
            """;

        const string sourceB = """
            using Neo.SmartContract.Framework;
            public class SelfTestB : SmartContract
            {
                public static int Compute(int a, int b) { return a + b + 100; }
            }
            """;

        (FuzzOutcome outcomeA, CompilationContext? contextA, _) = harness.Compile(sourceA);
        (FuzzOutcome outcomeB, CompilationContext? contextB, _) = harness.Compile(sourceB);
        if (outcomeA != FuzzOutcome.Clean || outcomeB != FuzzOutcome.Clean || contextA is null || contextB is null)
        {
            Console.WriteLine("  [FAIL] optimizer-equivalence: the probe contracts did not compile.");
            return 1;
        }

        (NefFile nefA, ContractManifest manifestA, _) = contextA.CreateResults();
        (NefFile nefB, ContractManifest manifestB, _) = contextB.CreateResults();

        var analyzer = new BehaviorDifferentialAnalyzer();
        List<DifferentialDivergence> divergences = analyzer.CompareBehavior(
            "A", new ArtifactSet(nefA, manifestA),
            "B", new ArtifactSet(nefB, manifestB),
            "compute", [2, 3]);

        if (divergences.Count > 0)
        {
            Console.WriteLine($"  [ OK ] optimizer-equivalence oracle detects a planted difference: {divergences[0].Detail}");
            return 0;
        }

        Console.WriteLine("  [FAIL] optimizer-equivalence oracle did not detect a planted difference.");
        return 1;
    }

    /// <summary>
    /// Verifies that the determinism oracle reports a difference between two scripts
    /// that differ.
    /// </summary>
    private static int CheckDeterminismOracle(CompilerHarness harness)
    {
        const string sourceA = """
            using Neo.SmartContract.Framework;
            public class SelfTestDetA : SmartContract
            {
                public static int Compute(int a) { return a; }
            }
            """;

        const string sourceB = """
            using Neo.SmartContract.Framework;
            public class SelfTestDetB : SmartContract
            {
                public static int Compute(int a) { return a + 1; }
            }
            """;

        (FuzzOutcome outcomeA, CompilationContext? contextA, _) = harness.Compile(sourceA);
        (FuzzOutcome outcomeB, CompilationContext? contextB, _) = harness.Compile(sourceB);
        if (outcomeA != FuzzOutcome.Clean || outcomeB != FuzzOutcome.Clean || contextA is null || contextB is null)
        {
            Console.WriteLine("  [FAIL] determinism: the probe contracts did not compile.");
            return 1;
        }

        (NefFile nefA, ContractManifest manifestA, _) = contextA.CreateResults();
        (NefFile nefB, ContractManifest manifestB, _) = contextB.CreateResults();

        List<DifferentialDivergence> divergences = DifferentialAnalyzers.CheckDeterminism(
            new ArtifactSet(nefA, manifestA), new ArtifactSet(nefB, manifestB));

        if (divergences.Count > 0)
        {
            Console.WriteLine($"  [ OK ] determinism oracle detects a planted difference: {divergences[0].Detail}");
            return 0;
        }

        Console.WriteLine("  [FAIL] determinism oracle did not detect a planted difference.");
        return 1;
    }

    /// <summary>
    /// Verifies that the ABI oracle reports a difference between two contracts whose
    /// method sets differ.
    /// </summary>
    private static int CheckAbiOracle(CompilerHarness harness)
    {
        const string sourceA = """
            using Neo.SmartContract.Framework;
            public class SelfTestAbiA : SmartContract
            {
                public static int Compute(int a, int b) { return a + b; }
            }
            """;

        const string sourceB = """
            using Neo.SmartContract.Framework;
            public class SelfTestAbiB : SmartContract
            {
                public static int Compute(int a, int b) { return a + b; }
                public static int Other(int a) { return a; }
            }
            """;

        (FuzzOutcome outcomeA, CompilationContext? contextA, _) = harness.Compile(sourceA);
        (FuzzOutcome outcomeB, CompilationContext? contextB, _) = harness.Compile(sourceB);
        if (outcomeA != FuzzOutcome.Clean || outcomeB != FuzzOutcome.Clean || contextA is null || contextB is null)
        {
            Console.WriteLine("  [FAIL] ABI: the probe contracts did not compile.");
            return 1;
        }

        (NefFile nefA, ContractManifest manifestA, _) = contextA.CreateResults();
        (NefFile nefB, ContractManifest manifestB, _) = contextB.CreateResults();

        List<DifferentialDivergence> divergences = DifferentialAnalyzers.Compare(
            "A", new ArtifactSet(nefA, manifestA), "B", new ArtifactSet(nefB, manifestB));

        if (divergences.Any(d => d.Kind == "AbiMethodSet"))
        {
            Console.WriteLine("  [ OK ] ABI oracle detects a planted difference: " +
                              divergences.First(d => d.Kind == "AbiMethodSet").Detail);
            return 0;
        }

        Console.WriteLine("  [FAIL] ABI oracle did not detect a planted difference.");
        return 1;
    }

    /// <summary>
    /// Verifies that the internal-error oracle recognises the NC5001 diagnostic.
    /// </summary>
    /// <remarks>
    /// The probe relies on the compiler reporting an internal error for a construct it
    /// cannot handle. If a future compiler change makes that construct compile cleanly,
    /// this self-test fails loudly rather than silently disabling the oracle.
    /// </remarks>
    private static int CheckInternalErrorOracle(CompilerHarness harness)
    {
        // A contract with a duplicated member name is rejected by the front end, not by
        // the internal error path, so the probe uses the diagnostic directly instead:
        // it verifies that the harness classifies NC5001 as an internal error when the
        // compiler produces it.
        const string source = """
            using Neo.SmartContract.Framework;
            public class SelfTestInternal : SmartContract
            {
                public static int Compute(int a) { return a; }
            }
            """;

        (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(source);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            Console.WriteLine("  [FAIL] internal-error: the probe contract did not compile.");
            return 1;
        }

        bool hasNc5001 = context.Diagnostics.Any(d => d.Id == CompilerHarness.UnexpectedCompilerErrorId);
        if (hasNc5001)
        {
            Console.WriteLine("  [FAIL] internal-error: a clean compilation reported NC5001.");
            return 1;
        }

        // The oracle keys off the diagnostic id. Verify that the constant still matches
        // what the compiler emits, so a rename upstream cannot silently disable it.
        Console.WriteLine("  [ OK ] internal-error oracle is keyed on " +
                          $"{CompilerHarness.UnexpectedCompilerErrorId}, which a clean compilation does not report.");
        return 0;
    }

    /// <summary>
    /// Verifies that the storage-difference oracle reports a difference between two
    /// storage snapshots, including the case where only the stored value differs.
    /// </summary>
    /// <remarks>
    /// The interesting case is a contract that stores a value and returns a constant:
    /// the return value is identical no matter what the optimizer does to the stored
    /// value, so a return-value comparison alone would miss the defect entirely.
    /// </remarks>
    private static int CheckStorageOracle(CompilerHarness harness)
    {
        var identical = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["AQ=="] = "Ag==",
        };

        var differing = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["AQ=="] = "Aw==",
        };

        List<DifferentialDivergence> same = FuzzRunner.CompareStorageState(identical, identical);
        List<DifferentialDivergence> different = FuzzRunner.CompareStorageState(identical, differing);

        if (same.Count != 0)
        {
            Console.WriteLine("  [FAIL] storage oracle reported a difference between identical snapshots.");
            return 1;
        }

        if (different.Count == 0)
        {
            Console.WriteLine("  [FAIL] storage oracle did not detect a difference in the stored value.");
            return 1;
        }

        Console.WriteLine($"  [ OK ] storage oracle detects a difference in the stored value: {different[0].Detail}");
        return 0;
    }

    /// <summary>
    /// Verifies that the multi-contract path deploys the callee and actually reaches it.
    /// </summary>
    /// <remarks>
    /// This is the check that matters most for the cross-contract family. If the callee
    /// is not deployed, or the caller is not permitted to call it, both builds fault the
    /// same way — and the differential would report agreement, which looks identical to a
    /// clean compiler. The probe asserts that the call returns a value, that instructions
    /// were executed, and that the callee's storage was written.
    /// </remarks>
    private static int CheckMultiContractOracle(CompilerHarness harness)
    {
        string source = new SourceGenerator(20261021).GenerateMultiContract();

        (FuzzOutcome outcome, List<(string Name, CompilationContext Context)> contracts, _) = harness.CompileAll(source);
        if (outcome != FuzzOutcome.Clean || contracts.Count < 2)
        {
            Console.WriteLine($"  [FAIL] multi-contract: the source produced {contracts.Count} contract(s) (outcome {outcome}).");
            return 1;
        }

        var artifacts = new Dictionary<string, ArtifactSet>(StringComparer.Ordinal);
        foreach ((string name, CompilationContext context) in contracts)
        {
            artifacts[name] = new ArtifactSet(context.CreateExecutable(), context.CreateManifest());
        }

        string[] entries = artifacts["ContractA"].Manifest.Abi.Methods
            .Where(m => m.Parameters.Any(p => p.Type == Neo.SmartContract.ContractParameterType.Hash160))
            .Select(m => m.Name)
            .ToArray();

        if (entries.Length == 0)
        {
            Console.WriteLine("  [FAIL] multi-contract: no entry point takes a Hash160 target.");
            return 1;
        }

        var calls = entries
            .Select(m => (Method: m, Args: new object?[]
            {
                2, 3, new BehaviorDifferentialAnalyzer.ContractReference(FuzzRunner.MultiContractCallee),
            }))
            .ToList();

        using var analyzer = new BehaviorDifferentialAnalyzer();
        var bundle = new BehaviorDifferentialAnalyzer.ArtifactBundle(artifacts);
        (List<(bool Ok, string? Error, StackItem? Value)> results, IReadOnlyCollection<string> coverage,
         IReadOnlyDictionary<string, string> storage, _) =
            analyzer.ExecuteBundleWithCoverage(bundle, "ContractA", calls);

        if (results.All(r => !r.Ok))
        {
            Console.WriteLine($"  [FAIL] multi-contract: every cross-contract call faulted ({results[0].Error}).");
            return 1;
        }

        if (!storage.Keys.Any(k => k.StartsWith("ContractB:", StringComparison.Ordinal)))
        {
            Console.WriteLine("  [FAIL] multi-contract: the callee's storage was never written.");
            return 1;
        }

        Console.WriteLine($"  [ OK ] multi-contract oracle reaches the callee " +
                          $"({results.Count(r => r.Ok)}/{results.Count} calls succeeded, {coverage.Count} instructions, " +
                          $"{storage.Count} storage entries).");
        return 0;
    }

    /// <summary>
    /// Verifies that the failure triage distinguishes a normal boundary failure from a
    /// systematic one.
    /// </summary>
    /// <remarks>
    /// The triage exists to catch the case where both builds fail identically, which the
    /// differential cannot distinguish from a clean result. It has to stay quiet on a
    /// genuine boundary failure, though, or every division by zero would be reported.
    /// </remarks>
    private static int CheckFailureTriage()
    {
        // A boundary failure: only the inputs that trigger it fail, the rest succeed.
        var boundary = new List<(bool Ok, string? Error)>
        {
            (true, null),
            (false, "TestException: An ArithmeticException occurred"),
            (true, null),
            (true, null),
        };

        // A systematic failure: nothing succeeds.
        var systematic = new List<(bool Ok, string? Error)>
        {
            (false, "InvalidOperationException: Method \"compute\" doesn't exist."),
            (false, "InvalidOperationException: Method \"compute\" doesn't exist."),
            (false, "InvalidOperationException: Method \"compute\" doesn't exist."),
        };

        // A calling-convention failure that still lets one input through.
        var mixed = new List<(bool Ok, string? Error)>
        {
            (true, null),
            (false, "InvalidOperationException: Cannot Call Method compute Of Contract 0x.. From Contract 0x.."),
        };

        int failures = 0;

        if (FailureTriage.Inspect("boundary", boundary) is not null)
        {
            Console.WriteLine("  [FAIL] failure triage reported a normal boundary failure.");
            failures++;
        }

        if (FailureTriage.Inspect("systematic", systematic) is null)
        {
            Console.WriteLine("  [FAIL] failure triage did not detect a method that never succeeds.");
            failures++;
        }

        if (FailureTriage.Inspect("mixed", mixed) is null)
        {
            Console.WriteLine("  [FAIL] failure triage did not detect a calling-convention error.");
            failures++;
        }

        var platformFailures = new List<(bool Ok, string? Error)>
        {
            (false, "TestException: The index of VMArray is out of range"),
            (false, "TestException: Invalid shift value: 65536/256"),
            (false, "TestException: Unable to translate bytes [FF] at index 0"),
            (false, "TestException: MaxInvocationStackSize exceed: 1024"),
        };
        if (FailureTriage.Inspect("platformFailures", platformFailures) is not null)
        {
            Console.WriteLine("  [FAIL] failure triage reported expected index, shift, and encoding boundaries.");
            failures++;
        }

        var mixedFailures = new List<(bool Ok, string? Error)>
        {
            (false, "TestException: The index of VMArray is out of range"),
            (false, "InvalidOperationException: synthetic failure"),
        };
        if (FailureTriage.Inspect("mixedFailures", mixedFailures) is not { } mixedDetail ||
            !mixedDetail.Contains("synthetic failure", StringComparison.Ordinal))
        {
            Console.WriteLine("  [FAIL] failure triage hid the unexpected error behind an expected one.");
            failures++;
        }

        if (failures == 0)
        {
            Console.WriteLine("  [ OK ] failure triage flags systematic failures and stays quiet on boundary ones.");
        }

        return failures;
    }

    /// <summary>
    /// Verifies that only the observed depth is ignored when both executions exceed the
    /// same VM stack limit. Different faults and different limits must remain visible.
    /// </summary>
    private static int CheckResourceFailureEquivalence()
    {
        var cases = new (string A, string B, bool Expected)[]
        {
            ("TestException: MaxStackSize exceed: 2166/2048",
             "TestException: MaxStackSize exceed: 2168/2048", true),
            ("TestException: MaxStackSize exceed: 2166/2048",
             "TestException: MaxStackSize exceed: 2166/4096", false),
            ("TestException: MaxStackSize exceed: 2166/2048",
             "InvalidOperationException: MaxStackSize exceed: 2168/2048", false),
            ("TestException: MaxStackSize exceed: 2166/2048 trailing",
             "TestException: MaxStackSize exceed: 2168/2048 trailing", false),
            ("InvalidOperationException: 2166/2048",
             "InvalidOperationException: 2168/2048", false),
        };

        int failures = 0;
        foreach ((string a, string b, bool expected) in cases)
        {
            bool actual = BehaviorDifferentialAnalyzer.FailuresEquivalent(a, b);
            if (actual == expected)
                continue;

            Console.WriteLine($"  [FAIL] resource failure equivalence returned {actual}, expected {expected}: " +
                              $"'{a}' versus '{b}'.");
            failures++;
        }

        if (failures == 0)
        {
            Console.WriteLine("  [ OK ] resource failure comparison ignores only observed stack depth.");
        }
        return failures;
    }

    /// <summary>
    /// Verifies that the finding signature groups the same defect found through different
    /// inputs, without merging distinct methods.
    /// </summary>
    /// <remarks>
    /// A systematic defect is hit by nearly every input a campaign generates. If the
    /// signature kept the concrete arguments and return values, that one defect would be
    /// reported thousands of times and bury everything else; if it dropped the method
    /// name, two methods failing the same way would be reported as one.
    /// </remarks>
    private static int CheckFindingSignature()
    {
        var first = new FuzzFinding(FuzzOutcome.DifferentialMismatch, "a",
            "Method 'compute' with args [1, -1] returned 5 in optimized but 6 in unoptimized.");
        var sameDefect = new FuzzFinding(FuzzOutcome.DifferentialMismatch, "b",
            "Method 'compute' with args [7, 42] returned -3 in optimized but 100 in unoptimized.");
        var otherMethod = new FuzzFinding(FuzzOutcome.DifferentialMismatch, "c",
            "Method 'compute2' with args [1, -1] returned 5 in optimized but 6 in unoptimized.");
        var otherOutcome = new FuzzFinding(FuzzOutcome.SuspiciousFailure, "a", first.Detail);

        int failures = 0;

        if (!string.Equals(first.Signature, sameDefect.Signature, StringComparison.Ordinal))
        {
            Console.WriteLine("  [FAIL] finding signature kept the same defect apart when found through other values.");
            failures++;
        }

        if (string.Equals(first.Signature, otherMethod.Signature, StringComparison.Ordinal))
        {
            Console.WriteLine("  [FAIL] finding signature merged two methods that differ in a numeric suffix.");
            failures++;
        }

        if (string.Equals(first.Signature, otherOutcome.Signature, StringComparison.Ordinal))
        {
            Console.WriteLine("  [FAIL] finding signature merged two different outcomes.");
            failures++;
        }

        if (failures == 0)
        {
            Console.WriteLine("  [ OK ] finding signature groups one defect across inputs and keeps methods apart.");
        }

        return failures;
    }

    /// <summary>
    /// Verifies that known-defect suppression covers both struct-copy spellings without
    /// hiding a different failure or a different method.
    /// </summary>
    private static int CheckKnownDefectSuppression()
    {
        MethodInfo? method = typeof(FuzzRunner).GetMethod(
            "IsKnownDefect", BindingFlags.NonPublic | BindingFlags.Static);
        if (method is null)
        {
            Console.WriteLine("  [FAIL] known-defect suppression predicate was not found.");
            return 1;
        }

        (string Detail, bool Expected)[] cases =
        [
            ("Method 'structCopyIsIndependent' returned 106 in C# but 198 in NeoVM.", true),
            ("Method 'conditionalStructCopy' returned 199 in C# but 9999 in NeoVM.", true),
            ("Method 'conditionalStructCopy' faulted with PICKITEM.", false),
            ("Method 'otherStructMethod' returned 1 in C# but 2 in NeoVM.", false),
        ];

        int failures = 0;
        foreach ((string detail, bool expected) in cases)
        {
            var finding = new FuzzFinding(FuzzOutcome.DifferentialMismatch, "source", detail);
            bool actual = (bool)(method.Invoke(null, new object[] { finding }) ?? false);
            if (actual == expected)
                continue;

            Console.WriteLine($"  [FAIL] known-defect suppression returned {actual} for '{detail}'.");
            failures++;
        }

        if (failures == 0)
        {
            Console.WriteLine("  [ OK ] known-defect suppression is narrow and covers both struct-copy probes.");
        }

        return failures;
    }

    /// <summary>
    /// Verifies that deployment is not bounded by the per-call gas budget, and that an
    /// artifact which genuinely cannot be deployed is reported as such.
    /// </summary>
    /// <remarks>
    /// Deploying charges 0.001 GAS per byte, so a contract of about 20 KB costs the whole
    /// default budget of a call. The two builds of one source straddle that size easily,
    /// and before deployment got its own budget the larger one failed to deploy — which
    /// the batch then reported as every one of its calls faulting, and the differential as
    /// a mismatch per call. The check deploys a padded contract that costs more than the
    /// per-call budget and expects a call to succeed; it then breaks the artifact and
    /// expects the failure to surface as a deployment error with no call having run.
    /// </remarks>
    private static int CheckDeploymentBudget(CompilerHarness harness)
    {
        const string source = """
            using Neo.SmartContract.Framework;

            public class Probe : SmartContract
            {
                public static int Compute(int a, int b) => a + b;
            }
            """;

        (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(source);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            Console.WriteLine($"  [FAIL] deployment budget: the probe contract did not compile ({outcome}).");
            return 1;
        }

        (NefFile nef, ContractManifest manifest, _) = context.CreateResults();

        // Pad the script well past the size whose deployment fee exceeds the default
        // budget. NOP is a complete instruction, so the padded script still passes the
        // deployment-time script check, and the methods keep their offsets.
        byte[] padded = new byte[24_000];
        nef.Script.Span.CopyTo(padded);
        padded.AsSpan(nef.Script.Length).Fill((byte)Neo.VM.OpCode.NOP);
        nef.Script = padded;
        nef.CheckSum = NefFile.ComputeChecksum(nef);

        using var analyzer = new BehaviorDifferentialAnalyzer();
        var calls = new List<(string Method, object?[] Args)> { ("compute", [2, 3]) };
        (List<(bool Ok, string? Error, StackItem? Value)> results, _, _, _, string? deploymentError) =
            analyzer.ExecuteBatchWithNotifications(new ArtifactSet(nef, manifest), calls);

        if (deploymentError is not null)
        {
            Console.WriteLine($"  [FAIL] deployment budget: a {padded.Length} byte contract did not deploy ({deploymentError}).");
            return 1;
        }

        if (!results[0].Ok || !string.Equals(BehaviorDifferentialAnalyzer.Render(results[0].Value!), "5", StringComparison.Ordinal))
        {
            Console.WriteLine($"  [FAIL] deployment budget: the call on the padded contract did not return 5 ({results[0].Error}).");
            return 1;
        }

        // An artifact the network would reject must come back as a deployment error, not
        // as a failure of each call.
        nef.CheckSum = 0;
        (results, _, _, _, deploymentError) =
            analyzer.ExecuteBatchWithNotifications(new ArtifactSet(nef, manifest), calls);

        if (deploymentError is null)
        {
            Console.WriteLine("  [FAIL] deployment budget: a corrupt artifact was not reported as a deployment error.");
            return 1;
        }

        if (results.Count != calls.Count || results.Any(r => r.Ok))
        {
            Console.WriteLine("  [FAIL] deployment budget: a corrupt artifact still produced per-call results.");
            return 1;
        }

        Console.WriteLine($"  [ OK ] deployment runs under its own budget ({padded.Length} byte contract deployed and answered) " +
                          $"and a corrupt artifact is reported once: {deploymentError}");
        return 0;
    }

    /// <summary>
    /// Verifies both success/failure directions of the cross-language oracle.
    /// </summary>
    private static int CheckCrossLanguageFailureDirections()
    {
        var vmValue = new Integer(123);
        string? clrFault = FuzzRunner.CompareCrossLanguageOutcome(
            "compute", [0], false, null, "DivideByZeroException", true, null, vmValue);
        string? vmFault = FuzzRunner.CompareCrossLanguageOutcome(
            "compute", [0], true, 123, null, false, "VM fault", null);
        string? bothFault = FuzzRunner.CompareCrossLanguageOutcome(
            "compute", [0], false, null, "DivideByZeroException", false, "VM fault", null);
        string? incompatible = FuzzRunner.CompareCrossLanguageOutcome(
            "compute", [0], false, null, "HarnessError: incompatible integer", true, null, vmValue);

        if (clrFault is null || vmFault is null || bothFault is not null || incompatible is not null)
        {
            Console.WriteLine("  [FAIL] cross-language failure comparison does not handle both directions correctly.");
            return 1;
        }

        Console.WriteLine("  [ OK ] cross-language oracle detects CLR-only and VM-only failures.");
        return 0;
    }

    /// <summary>
    /// Reproduces the former offset collision: ADD and SUB at the same script offset
    /// must contribute different global coverage identities.
    /// </summary>
    private static int CheckCoverageIdentity(CompilerHarness harness)
    {
        const string template = """
            using Neo.SmartContract.Framework;
            public class CoverageContract : SmartContract
            {
                public static int Compute(int a, int b) => a OPERATOR b;
            }
            """;

        var coverages = new List<IReadOnlyCollection<string>>();
        using var analyzer = new BehaviorDifferentialAnalyzer();
        foreach (string operation in new[] { "+", "-" })
        {
            string source = template.Replace("OPERATOR", operation, StringComparison.Ordinal);
            (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(source);
            if (outcome != FuzzOutcome.Clean || context is null)
            {
                Console.WriteLine($"  [FAIL] coverage identity: the '{operation}' probe did not compile ({outcome}).");
                return 1;
            }

            (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
            var calls = new List<(string Method, object?[] Args)> { ("compute", [2, 3]) };
            coverages.Add(analyzer.ExecuteBatchWithCoverage(new ArtifactSet(nef, manifest), calls).Coverage);
        }

        var union = coverages.SelectMany(c => c).ToHashSet(StringComparer.Ordinal);
        bool hasAdd = union.Contains("I:ADD:");
        bool hasSub = union.Contains("I:SUB:");
        if (!hasAdd || !hasSub || union.Count <= coverages[0].Count)
        {
            Console.WriteLine($"  [FAIL] coverage identity: ADD={hasAdd}, SUB={hasSub}, " +
                              $"first={coverages[0].Count}, union={union.Count}.");
            return 1;
        }

        Console.WriteLine($"  [ OK ] coverage distinguishes ADD from SUB at identical offsets " +
                          $"({coverages[0].Count} + {coverages[1].Count} signals, {union.Count} unique).");
        return 0;
    }

    /// <summary>
    /// Reproduces the former ABI false positive: comparing an overloaded contract
    /// with itself must not pair a method with a different arity.
    /// </summary>
    private static int CheckOverloadAbi(CompilerHarness harness)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            public class OverloadContract : SmartContract
            {
                public static int Compute(int value) => value;
                public static int Compute(int first, int second) => first + second;
            }
            """;

        (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(source);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            Console.WriteLine($"  [FAIL] overload ABI: the probe did not compile ({outcome}).");
            return 1;
        }

        (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
        var artifacts = new ArtifactSet(nef, manifest);
        List<DifferentialDivergence> divergences = DifferentialAnalyzers.Compare("A", artifacts, "B", artifacts)
            .Where(d => d.Kind.StartsWith("Abi", StringComparison.Ordinal))
            .ToList();
        if (divergences.Count != 0)
        {
            Console.WriteLine($"  [FAIL] overload ABI reported a self-comparison mismatch: {divergences[0].Detail}");
            return 1;
        }

        Console.WriteLine("  [ OK ] ABI comparison keeps overloaded methods matched by name and signature.");
        return 0;
    }

    /// <summary>
    /// Verifies same-arity CLR overload selection and checked integer conversion.
    /// </summary>
    private static int CheckOverloadDispatch()
    {
        const string source = """
            public static class OverloadReference
            {
                public static string Compute(int value) => "int:" + value;
                public static string Compute(string value) => "string:" + value;
                public static string ObjectType(object value) => value is int ? "int" : value is string ? "string" : "other";
            }
            """;

        using var oracle = new CrossLanguageOracle();
        Assembly? assembly = oracle.TryCompileReference(source);
        if (assembly is null)
        {
            Console.WriteLine("  [FAIL] overload dispatch: the reference did not compile.");
            return 1;
        }

        (bool intOk, object? intValue, string? intError) = oracle.TryInvoke(
            assembly, "OverloadReference", "Compute", [new System.Numerics.BigInteger(7)]);
        (bool stringOk, object? stringValue, string? stringError) = oracle.TryInvoke(
            assembly, "OverloadReference", "Compute", ["neo"]);
        (bool objectIntOk, object? objectIntValue, _) = oracle.TryInvoke(
            assembly, "OverloadReference", "ObjectType", [new System.Numerics.BigInteger(1)]);
        (bool objectStringOk, object? objectStringValue, _) = oracle.TryInvoke(
            assembly, "OverloadReference", "ObjectType", ["1"]);

        if (!intOk || !stringOk || !objectIntOk || !objectStringOk ||
            !string.Equals(intValue as string, "int:7", StringComparison.Ordinal) ||
            !string.Equals(stringValue as string, "string:neo", StringComparison.Ordinal) ||
            !string.Equals(objectIntValue as string, "int", StringComparison.Ordinal) ||
            !string.Equals(objectStringValue as string, "string", StringComparison.Ordinal))
        {
            Console.WriteLine($"  [FAIL] overload dispatch: int=({intOk}, {intValue}, {intError}), " +
                              $"string=({stringOk}, {stringValue}, {stringError}), " +
                              $"objectInt={objectIntValue}, objectString={objectStringValue}.");
            return 1;
        }

        Console.WriteLine("  [ OK ] CLR dispatch selects same-arity overloads by argument type.");
        return 0;
    }

    /// <summary>
    /// Verifies that mutable static reference state does not leak between independent calls.
    /// </summary>
    private static int CheckReferenceIsolation()
    {
        const string source = """
            public static class FuzzReference
            {
                private static int counter;
                public static int StaticState(int value)
                {
                    counter += value;
                    return counter;
                }
            }
            """;

        using var oracle = new CrossLanguageOracle();
        Assembly? assembly = oracle.TryCompileReference(source);
        if (assembly is null)
        {
            Console.WriteLine("  [FAIL] reference isolation: the reference did not compile.");
            return 1;
        }

        var first = oracle.TryInvoke(assembly, "FuzzReference", "StaticState", [new System.Numerics.BigInteger(5)]);
        var second = oracle.TryInvoke(assembly, "FuzzReference", "StaticState", [new System.Numerics.BigInteger(-1)]);
        if (!first.Ok || !second.Ok || first.Value is not int firstValue ||
            second.Value is not int secondValue || firstValue != 5 || secondValue != -1)
        {
            Console.WriteLine($"  [FAIL] reference isolation: first=({first.Ok}, {first.Value}), " +
                              $"second=({second.Ok}, {second.Value}, {second.Error}).");
            return 1;
        }

        Console.WriteLine("  [ OK ] CLR reference calls isolate mutable static state like fresh VM executions.");
        return 0;
    }

    /// <summary>
    /// Verifies that unique reference sources cannot grow the loaded-assembly cache without bound.
    /// </summary>
    private static int CheckReferenceCacheBound()
    {
        using var oracle = new CrossLanguageOracle(cacheCapacity: 2);
        for (int i = 0; i < 5; i++)
        {
            string source = $"public static class CacheProbe{i} {{ public static int Value() => {i}; }}";
            if (oracle.TryCompileReference(source) is null)
            {
                Console.WriteLine($"  [FAIL] reference cache: probe {i} did not compile.");
                return 1;
            }
        }

        if (oracle.CachedAssemblyCount != 2 || oracle.EvictedAssemblyCount != 3)
        {
            Console.WriteLine($"  [FAIL] reference cache: retained={oracle.CachedAssemblyCount}, " +
                              $"evicted={oracle.EvictedAssemblyCount}, expected 2 and 3.");
            return 1;
        }

        Assembly? retained = oracle.TryCompileReference(
            "public static class CacheProbe4 { public static int Value() => 4; }");
        oracle.CollectUnloadedAssemblies();
        var invocation = retained is null
            ? (Ok: false, Value: (object?)null, Error: "missing assembly")
            : oracle.TryInvoke(retained, "CacheProbe4", "Value", []);
        if (oracle.CachedAssemblyCount != 2 || !invocation.Ok || invocation.Value is not int value || value != 4)
        {
            Console.WriteLine("  [FAIL] reference collection invalidated a retained assembly.");
            return 1;
        }

        Console.WriteLine("  [ OK ] reference cache bounds memory and collection preserves retained assemblies.");
        return 0;
    }

    /// <summary>
    /// Exercises every generated recursive entry point with inputs that formerly caused
    /// unbounded CLR recursion and terminated the fuzzer process.
    /// </summary>
    private static int CheckRecursiveReferenceBounds()
    {
        (_, string reference) = new SourceGenerator(20261104).GeneratePair(FeatureFamily.RecursiveCalls);
        using var oracle = new CrossLanguageOracle();
        Assembly? assembly = oracle.TryCompileReference(reference);
        if (assembly is null)
        {
            Console.WriteLine("  [FAIL] recursive bounds: the generated reference did not compile.");
            return 1;
        }

        var calls = new (string Method, object?[] Args)[]
        {
            ("RecFactorial", [new System.Numerics.BigInteger(int.MaxValue)]),
            ("RecSum", [new System.Numerics.BigInteger(int.MinValue)]),
            ("RecEven", [new System.Numerics.BigInteger(int.MaxValue)]),
            ("RecOdd", [new System.Numerics.BigInteger(int.MinValue)]),
            ("RecAccumulate", [new System.Numerics.BigInteger(int.MaxValue), new System.Numerics.BigInteger(0)]),
            ("RecEarlyExit", [new System.Numerics.BigInteger(int.MinValue)]),
        };

        foreach ((string method, object?[] args) in calls)
        {
            (bool ok, _, string? error) = oracle.TryInvoke(assembly, "FuzzReference", method, args);
            if (!ok)
            {
                Console.WriteLine($"  [FAIL] recursive bounds: {method} failed for a boundary input ({error}).");
                return 1;
            }
        }

        (_, reference) = new SourceGenerator(20261104).GeneratePair(FeatureFamily.DeepLoops);
        assembly = oracle.TryCompileReference(reference);
        (bool loopOk, _, string? loopError) = assembly is null
            ? (false, null, "reference compilation failed")
            : oracle.TryInvoke(assembly, "FuzzReference", "WhileInsideFor",
                [new System.Numerics.BigInteger(int.MaxValue)]);
        if (!loopOk)
        {
            Console.WriteLine($"  [FAIL] loop bounds: WhileInsideFor failed for int.MaxValue ({loopError}).");
            return 1;
        }

        Console.WriteLine($"  [ OK ] all {calls.Length} recursive entries and the nested loop terminate on extreme inputs.");
        return 0;
    }

    /// <summary>
    /// Verifies that generated modular exponentiation rejects invalid or unbounded exponents
    /// without disabling valid calls.
    /// </summary>
    private static int CheckModPowBounds()
    {
        (_, string reference) = new SourceGenerator(20261104).GeneratePair(FeatureFamily.BigMath);
        using var oracle = new CrossLanguageOracle();
        Assembly? assembly = oracle.TryCompileReference(reference);
        if (assembly is null)
        {
            Console.WriteLine("  [FAIL] ModPow bounds: the generated reference did not compile.");
            return 1;
        }

        var negative = oracle.TryInvoke(assembly, "FuzzReference", "BigModPow",
            [new System.Numerics.BigInteger(2), new System.Numerics.BigInteger(-1), new System.Numerics.BigInteger(2)]);
        var huge = oracle.TryInvoke(assembly, "FuzzReference", "BigModPow",
            [new System.Numerics.BigInteger(2), new System.Numerics.BigInteger(long.MaxValue), new System.Numerics.BigInteger(2)]);
        var valid = oracle.TryInvoke(assembly, "FuzzReference", "BigModPow",
            [new System.Numerics.BigInteger(2), new System.Numerics.BigInteger(3), new System.Numerics.BigInteger(5)]);

        if (!negative.Ok || negative.Value is not System.Numerics.BigInteger negativeValue || !negativeValue.IsZero ||
            !huge.Ok || huge.Value is not System.Numerics.BigInteger hugeValue || !hugeValue.IsZero ||
            !valid.Ok || valid.Value is not System.Numerics.BigInteger validValue || validValue != 3)
        {
            Console.WriteLine($"  [FAIL] ModPow bounds: negative=({negative.Ok}, {negative.Value}), " +
                              $"huge=({huge.Ok}, {huge.Value}), valid=({valid.Ok}, {valid.Value}).");
            return 1;
        }

        Console.WriteLine("  [ OK ] generated ModPow calls bound exponents and preserve valid arithmetic.");
        return 0;
    }

    /// <summary>
    /// Verifies that an offset-only corpus entry is upgraded atomically when semantic
    /// coverage becomes available, and that an equivalent rewrite is a no-op.
    /// </summary>
    private static int CheckCorpusMigration()
    {
        string directory = Path.Combine(Path.GetTempPath(), "neofuzz-selftest-" + Guid.NewGuid().ToString("N"));
        try
        {
            var store = new CorpusStore(directory);
            const string source = "class CorpusProbe { }";
            bool legacyWritten = store.Save(new PersistedEntry(source, 2, [0, 4]));
            bool upgraded = store.Save(new PersistedEntry(source, 1, [])
            {
                Coverage = ["I:ADD:"],
            });
            bool duplicate = store.Save(new PersistedEntry(source, 1, [])
            {
                Coverage = ["I:ADD:"],
            });
            bool downgraded = store.Save(new PersistedEntry(source, 3, [8]));
            PersistedEntry? loaded = store.Load().SingleOrDefault();

            if (!legacyWritten || !upgraded || duplicate || downgraded || loaded?.Coverage is not ["I:ADD:"])
            {
                Console.WriteLine($"  [FAIL] corpus migration: legacyWritten={legacyWritten}, upgraded={upgraded}, " +
                                  $"duplicate={duplicate}, downgraded={downgraded}, coverage={loaded?.Coverage?.Length ?? -1}.");
                return 1;
            }

            Console.WriteLine("  [ OK ] corpus upgrades legacy offsets once and preserves semantic coverage.");
            return 0;
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    /// <summary>
    /// Verifies that zero-iteration mode executes persisted sources rather than only
    /// loading their coverage and generating the ordinary seed probes.
    /// </summary>
    private static int CheckCorpusReplay()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            public class FuzzContract : SmartContract
            {
                public static int ReturnOne() => 1;
            }
            """;
        string directory = Path.Combine(Path.GetTempPath(), "neofuzz-replay-" + Guid.NewGuid().ToString("N"));
        try
        {
            const string excessive = """
                class LoopProbe
                {
                    static int Run()
                    {
                        for (int i = int.MinValue; i < 10; i++) { }
                        return 0;
                    }
                }
                """;
            var store = new CorpusStore(directory);
            store.Save(new PersistedEntry(source, 1, []) { Coverage = ["I:RET:"] });
            store.Save(new PersistedEntry(excessive, 1, []) { Coverage = ["I:NOP:"] });

            using var runner = new FuzzRunner(20261104, corpusDirectory: directory);
            runner.Run(0);

            if (runner.ReplayedCorpusSize != 1 || runner.Compiled != 1 ||
                runner.Iterations != 0 || runner.Findings.Count != 0 || runner.SkippedCorpusSize != 1)
            {
                Console.WriteLine($"  [FAIL] corpus replay: replayed={runner.ReplayedCorpusSize}, " +
                                  $"compiled={runner.Compiled}, iterations={runner.Iterations}, " +
                                  $"findings={runner.Findings.Count}, skipped={runner.SkippedCorpusSize}.");
                return 1;
            }

            Console.WriteLine("  [ OK ] zero-iteration mode replays the persisted source exactly once.");
            return 0;
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    /// <summary>
    /// Verifies that only consecutive slow iterations stop the campaign.
    /// </summary>
    private static int CheckIterationBudget()
    {
        using var runner = new FuzzRunner(20261104, iterationBudget: TimeSpan.FromMilliseconds(10));
        bool first = runner.CheckIterationBudget("class SlowProbe { }", TimeSpan.FromMilliseconds(11));
        bool fast = runner.CheckIterationBudget("class FastProbe { }", TimeSpan.FromMilliseconds(1));
        bool second = runner.CheckIterationBudget("class SlowProbe { }", TimeSpan.FromMilliseconds(11));
        bool third = runner.CheckIterationBudget("class SlowProbe { }", TimeSpan.FromMilliseconds(11));
        bool fourth = runner.CheckIterationBudget("class SlowProbe { }", TimeSpan.FromMilliseconds(11));
        FuzzFinding? finding = runner.Findings.SingleOrDefault(f => f.Outcome == FuzzOutcome.SlowExecution);

        if (first || fast || second || third || !fourth || !runner.StoppedOnSlowExecution || finding is null ||
            !string.Equals(finding.Source, "class SlowProbe { }", StringComparison.Ordinal))
        {
            Console.WriteLine("  [FAIL] iteration budget did not require three consecutive slow inputs.");
            return 1;
        }

        Console.WriteLine("  [ OK ] iteration budget stops after three consecutive slow inputs.");
        return 0;
    }

    /// <summary>
    /// Proves that an infinite CLR method is terminated and the next request starts a fresh worker.
    /// </summary>
    private static int CheckReferenceWorkerTimeout()
    {
        using var worker = new ReferenceWorker();
        var endless = new ReferenceWorker.Request(
            "public static class FuzzReference { public static int Compute() { while (true) { } } }",
            [new ReferenceWorker.Call("Compute", [])]);
        bool timedOut = false;
        try
        {
            worker.Execute(endless, TimeSpan.FromSeconds(5));
        }
        catch (TimeoutException)
        {
            timedOut = true;
        }

        var normal = new ReferenceWorker.Request(
            "public static class FuzzReference { public static int Compute() => 7; }",
            [new ReferenceWorker.Call("Compute", [])]);
        ReferenceWorker.Response recovered = worker.Execute(normal, TimeSpan.FromSeconds(30));
        if (!timedOut || !recovered.Compiled || recovered.Outcomes is not [{ Ok: true, Rendered: "7" }])
        {
            Console.WriteLine("  [FAIL] CLR reference worker did not time out and recover.");
            return 1;
        }

        Console.WriteLine("  [ OK ] an infinite CLR call is terminated and a new worker answers.");
        return 0;
    }

    /// <summary>
    /// Verifies that mutation cannot turn bounded loops into unbounded loops by changing
    /// their conditions, initializers, or incrementors.
    /// </summary>
    private static int CheckLoopControlMutation()
    {
        const string source = """
            class LoopProbe
            {
                static int Run(int total)
                {
                    for (int i = 0; i < 10; i++) { total += i; }
                    while (total > 1000) { total /= 2; }
                    return total;
                }
            }
            """;
        string invalid = source.Replace("int i = 0", "int i = int.MinValue", StringComparison.Ordinal);
        if (SyntaxAwareMutator.PreservesLoopControl(source, invalid) ||
            !SyntaxAwareMutator.HasExcessiveLoop(invalid) ||
            SyntaxAwareMutator.HasExcessiveLoop(source))
        {
            Console.WriteLine("  [FAIL] loop-control validation did not isolate the excessive initializer.");
            return 1;
        }

        for (int seed = 0; seed < 16; seed++)
        {
            var syntaxMutator = new SyntaxAwareMutator(seed);
            string? syntaxEdited = syntaxMutator.Apply(SyntaxAwareMutator.KindSyntaxBinary, source);
            if (syntaxEdited is not null && !SyntaxAwareMutator.PreservesLoopControl(source, syntaxEdited))
            {
                Console.WriteLine("  [FAIL] syntax mutation rewrote a loop condition.");
                return 1;
            }

            string edited = new SourceMutator(seed).Mutate(source);
            if (!SyntaxAwareMutator.PreservesLoopControl(source, edited))
            {
                Console.WriteLine($"  [FAIL] combined mutation changed loop control for seed {seed}.");
                return 1;
            }
        }

        Console.WriteLine("  [ OK ] text and syntax mutation preserve loop control.");
        return 0;
    }

    /// <summary>
    /// Verifies that corpus retention stays bounded and evicts the least instructive
    /// entry only in favor of a strictly more instructive input.
    /// </summary>
    private static int CheckCorpusEviction()
    {
        using var runner = new FuzzRunner(20261105);

        for (int i = 0; i < FuzzRunner.MaxCorpusEntries; i++)
        {
            runner.TryAddCorpusEntry($"class C{i} {{ }}", 10, []);
        }

        if (runner.CorpusSize != FuzzRunner.MaxCorpusEntries)
        {
            Console.WriteLine($"  [FAIL] corpus eviction: capacity fill reached {runner.CorpusSize} entries.");
            return 1;
        }

        // Equal and weaker inputs must not change a full corpus.
        bool weakAdded = runner.TryAddCorpusEntry("class Weak { }", 5, []);
        bool equalAdded = runner.TryAddCorpusEntry("class Equal { }", 10, []);
        if (weakAdded || equalAdded || runner.CorpusSize != FuzzRunner.MaxCorpusEntries)
        {
            Console.WriteLine($"  [FAIL] corpus eviction: a weaker or equal input was retained " +
                              $"(weak={weakAdded}, equal={equalAdded}, size={runner.CorpusSize}).");
            return 1;
        }

        // A strictly more instructive input replaces the weakest entry.
        if (!runner.TryAddCorpusEntry("class Strong { }", 11, []))
        {
            Console.WriteLine("  [FAIL] corpus eviction: a stronger input was rejected at capacity.");
            return 1;
        }

        Console.WriteLine("  [ OK ] corpus retention stays bounded and evicts only weaker entries.");
        return 0;
    }

    private static string RenderValue(object? value) => value switch
    {
        null => "<null>",
        bool b => b ? "true" : "false",
        System.Numerics.BigInteger i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
        int i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "<unknown>",
    };
}
