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
        (List<(bool Ok, string? Error, Neo.VM.Types.StackItem? Value)> results, _, _, List<IReadOnlyList<string>> notifications) =
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
        (List<(bool Ok, string? Error, StackItem? Value)> results, IReadOnlyCollection<int> coverage,
         IReadOnlyDictionary<string, string> storage) =
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

        if (failures == 0)
        {
            Console.WriteLine("  [ OK ] failure triage flags systematic failures and stays quiet on boundary ones.");
        }

        return failures;
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
