// Copyright (C) 2015-2026 The Neo Project.
//
// MultiContractProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM.Types;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Verifies that the multi-contract path actually performs cross-contract calls.
/// </summary>
/// <remarks>
/// A cross-contract family that silently fails to deploy the callee, or passes a zero
/// hash, would look exactly like a passing oracle: both builds fault the same way and the
/// differential reports nothing. This probe checks that the callee is deployed and that
/// the call returns the callee's value rather than a fault.
/// </remarks>
public static class MultiContractProbe
{
    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <param name="seed">Seed for the generator.</param>
    /// <returns>The number of failures; zero means the path works.</returns>
    public static int Run(int seed)
    {
        var generator = new SourceGenerator(seed);
        string source = generator.GenerateMultiContract();

        Console.WriteLine("=== generated multi-contract source ===");
        Console.WriteLine(source);

        using var harness = new CompilerHarness();

        (FuzzOutcome outcome, List<(string Name, CompilationContext Context)> contracts, FuzzFinding? finding) =
            harness.CompileAll(source);

        Console.WriteLine($"compile outcome: {outcome}");
        if (finding is not null)
        {
            Console.WriteLine($"finding: {finding.Detail}");
            return 1;
        }

        foreach ((string name, CompilationContext context) in contracts)
        {
            Console.WriteLine($"  contract '{name}': success={context.Success}, " +
                              $"methods={context.CreateManifest().Abi.Methods.Length}");
        }

        if (contracts.Count < 2)
        {
            Console.WriteLine("[FAIL] the source did not produce two contracts.");
            return 1;
        }

        var artifacts = new Dictionary<string, ArtifactSet>(StringComparer.Ordinal);
        foreach ((string name, CompilationContext context) in contracts)
        {
            artifacts[name] = new ArtifactSet(context.CreateExecutable(), context.CreateManifest());
        }

        var bundle = new BehaviorDifferentialAnalyzer.ArtifactBundle(artifacts);
        using var analyzer = new BehaviorDifferentialAnalyzer();

        // The ABI lower-cases only the first character, so the method names must be read
        // from the manifest rather than guessed: "ForwardTwice" becomes "forwardTwice",
        // not "forwardtwice".
        string[] callerMethods = artifacts["ContractA"].Manifest.Abi.Methods
            .Where(m => m.Parameters.Length == 3 &&
                        m.Parameters[2].Type == Neo.SmartContract.ContractParameterType.Hash160)
            .Select(m => m.Name)
            .ToArray();

        Console.WriteLine($"  cross-contract entry points: [{string.Join(", ", callerMethods)}]");

        var calls = callerMethods
            .Select(m => (Method: m, Args: new object?[]
            {
                2, 3, new BehaviorDifferentialAnalyzer.ContractReference("ContractB"),
            }))
            .ToList();

        (List<(bool Ok, string? Error, StackItem? Value)> results, IReadOnlyCollection<int> coverage,
         IReadOnlyDictionary<string, string> storage) =
            analyzer.ExecuteBundleWithCoverage(bundle, "ContractA", calls);

        int failures = 0;
        for (int i = 0; i < calls.Count; i++)
        {
            var result = results[i];
            string rendered = result.Ok
                ? BehaviorDifferentialAnalyzer.Render(result.Value!)
                : $"FAULT({result.Error})";

            Console.WriteLine($"  {calls[i].Method,-16} => {rendered}");

            if (!result.Ok)
            {
                Console.WriteLine($"    [FAIL] the cross-contract call faulted, so it never reached the callee.");
                failures++;
            }
        }

        Console.WriteLine($"  coverage          => {coverage.Count} instructions");
        Console.WriteLine($"  storage entries   => {storage.Count}");

        if (coverage.Count == 0)
        {
            Console.WriteLine("    [FAIL] no coverage was captured, so the call did not execute.");
            failures++;
        }

        // A cross-contract write must land in the callee's storage, which the aggregate
        // reports under a "ContractB:" prefix.
        bool wroteToCallee = storage.Keys.Any(k => k.StartsWith("ContractB:", StringComparison.Ordinal));
        Console.WriteLine($"  wrote to callee   => {wroteToCallee}");

        if (!wroteToCallee)
        {
            Console.WriteLine("    [FAIL] the callee's storage was not written, so the call did not reach it.");
            failures++;
        }

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "MULTI-CONTRACT PROBE PASSED: the callee is deployed and the cross-contract call reaches it."
            : $"MULTI-CONTRACT PROBE FAILED: {failures} check(s) failed.");

        return failures;
    }
}
