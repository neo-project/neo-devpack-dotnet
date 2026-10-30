// Copyright (C) 2015-2026 The Neo Project.
//
// FamilyCoverageProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Checks that every generated feature family actually runs, not just compiles.
/// </summary>
/// <remarks>
/// A family whose source does not compile is silently skipped by the fuzzer: the run reports
/// "no defects" while that family contributes nothing. The same is true of a family whose ABI
/// methods all fault before reaching the interesting code. This probe generates a few contracts
/// per family, invokes every ABI method, and reports the instruction coverage each family
/// reaches, so a dead family shows up as a zero.
/// </remarks>
public static class FamilyCoverageProbe
{
    private const int ContractsPerFamily = 3;

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of families that never reached any instruction.</returns>
    public static int Run()
    {
        FeatureFamily[] families = (FeatureFamily[])Enum.GetValues(typeof(FeatureFamily));
        int dead = 0;

        Console.WriteLine($"  {"family",-20} {"compiled",8} {"methods",7} {"calls",6} {"ok",5} {"instr",7}");
        Console.WriteLine("  " + new string('-', 58));

        foreach (FeatureFamily family in families)
        {
            int compiled = 0;
            int methods = 0;
            int calls = 0;
            int succeeded = 0;
            int instructions = 0;
            string? lastError = null;
            string? compileError = null;

            for (int seed = 0; seed < ContractsPerFamily; seed++)
            {
                var generator = new SourceGenerator(seed * 7919 + (int)family * 131);
                (string contractSource, _) = generator.GeneratePair(family);

                using var harness = new CompilerHarness();
                (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(contractSource);
                if (outcome != FuzzOutcome.Clean || context is null)
                {
                    compileError ??= context?.Diagnostics
                        .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                        .Select(d => $"{d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan().StartLinePosition}")
                        .FirstOrDefault() ?? finding?.Detail ?? outcome.ToString();
                    continue;
                }

                compiled++;
                (Neo.SmartContract.NefFile nef, ContractManifest manifest, _) = context.CreateResults();

                var callList = new List<(string Method, object?[] Args)>();
                foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                {
                    if (method.Name.StartsWith('_'))
                        continue;
                    methods++;
                    callList.Add((method.Name, BuildArguments(method)));
                }

                if (callList.Count == 0)
                    continue;

                using var analyzer = new BehaviorDifferentialAnalyzer();
                (List<(bool Ok, string? Error, Neo.VM.Types.StackItem? Value)> results,
                 IReadOnlyCollection<int> coverage, _) = analyzer.ExecuteBatchWithCoverage(
                    new ArtifactSet(nef, manifest), callList);

                calls += results.Count;
                succeeded += results.Count(r => r.Ok);
                foreach (var result in results)
                    if (!result.Ok)
                    {
                        lastError = result.Error ?? "<null error>";
                        break;
                    }
                instructions += coverage.Count;
            }

            string flag = instructions == 0 ? "  <-- DEAD" : string.Empty;
            if (compiled < ContractsPerFamily && compileError is not null)
                Console.WriteLine($"      compile error: {compileError}");
            if (instructions == 0 && lastError is not null)
                Console.WriteLine($"      last error: {lastError}");
            Console.WriteLine($"  {family,-20} {compiled,8} {methods,7} {calls,6} {succeeded,5} {instructions,7}{flag}");

            if (instructions == 0)
                dead++;
        }

        Console.WriteLine();
        Console.WriteLine(dead == 0
            ? "FAMILY COVERAGE OK: every feature family executes code."
            : $"FAMILY COVERAGE: {dead} family(ies) never executed any instruction.");

        return dead;
    }

    /// <summary>
    /// Builds an argument list that matches the ABI parameter types.
    /// </summary>
    /// <remarks>
    /// The values only have to reach the method: a fault after the entry point still executes
    /// code, so the coverage count stays meaningful even for a shape the value does not fit.
    /// </remarks>
    private static object?[] BuildArguments(ContractMethodDescriptor method)
    {
        var args = new object?[method.Parameters.Length];
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = method.Parameters[i].Type switch
            {
                ContractParameterType.Integer => 7,
                ContractParameterType.Boolean => true,
                ContractParameterType.String => "abc",
                ContractParameterType.ByteArray => new byte[] { 1, 2, 3 },
                ContractParameterType.Hash160 => new byte[20],
                ContractParameterType.Hash256 => new byte[32],
                ContractParameterType.PublicKey => new byte[33],
                ContractParameterType.Array => new object?[] { 7, 3 },
                ContractParameterType.Map => new Dictionary<object, object?>(),
                _ => null,
            };
        }
        return args;
    }
}
