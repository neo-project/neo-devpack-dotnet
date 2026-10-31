// Copyright (C) 2015-2026 The Neo Project.
//
// StorageMapProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Isolates which <c>StorageMap</c> operation fails.
/// </summary>
/// <remarks>
/// The fuzzer reported "Out of stack bounds: 0/0" for every method of the StorageMap
/// family, which points at the generated code rather than at the test input. Each
/// operation is compiled on its own here, so a construct the compiler rejects cannot mask
/// the behaviour of the others, and each is compared against the raw <c>Storage</c> call
/// that the Storage family already exercises successfully.
/// </remarks>
public static class StorageMapProbe
{
    private static readonly (string Label, string Body)[] Cases =
    [
        // Baseline: the raw storage API.
        ("raw Storage.Put/Get",
            """
                    Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, a);
                    var v = Storage.Get(Storage.CurrentContext, new byte[] { 0x01 });
                    return v == null ? -1 : (int)(BigInteger)v;
            """),

        ("StorageMap construction",
            """
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x02 });
                    return a;
            """),

        ("StorageMap.Put",
            """
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x03 });
                    map.Put(new byte[] { 0x01 }, a);
                    return a;
            """),

        ("StorageMap.Get (missing key)",
            """
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x04 });
                    var v = map.Get(new byte[] { 0x01 });
                    return v == null ? a : (int)(BigInteger)v;
            """),

        // Control group: a type declared in the contract itself, whose constructor is
        // NOT extern. If this works, the failure is specific to extern constructors
        // rather than to object creation in general.
        ("custom class ctor (control)",
            """
                    var probe = new CustomProbe(a);
                    return probe.Value;
            """),

        // The single-argument StorageMap constructor, which also extern.
        ("StorageMap single-arg ctor",
            """
                    var map = new StorageMap(new byte[] { 0x06 });
                    return a;
            """),

        // The shape the repository's own Contract_Foreach uses: chained, no local.
        ("StorageMap.Find chained",
            """
                    var tokens = new StorageMap(Storage.CurrentContext, new byte[] { 0x07 })
                        .Find(FindOptions.KeysOnly | FindOptions.RemovePrefix);
                    var count = 0;
                    foreach (var item in tokens) { count = count + 1; }
                    return count + a;
            """),

        ("StorageMap indexer",
            """
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x05 });
                    ByteString value = "probe";
                    map[new byte[] { 0x01 }] = value;
                    var v = map[new byte[] { 0x01 }];
                    return v == null ? -1 : v.Length;
            """),
    ];

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of operations that faulted or were rejected.</returns>
    public static int Run()
    {
        int failures = 0;

        foreach ((string label, string body) in Cases)
        {
            string source = $$"""
                using System;
                using System.Numerics;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Services;

                public class CustomProbe
                {
                    public int Value;
                    public CustomProbe(int value) { Value = value; }
                }

                public class MapProbe : SmartContract
                {
                    public static int Probe(int a)
                    {
                {{body}}
                    }
                }
                """;

            using var harness = new CompilerHarness();
            (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

            if (outcome != FuzzOutcome.Clean || context is null)
            {
                Console.WriteLine($"  {label,-34} => REJECTED ({outcome})");
                if (finding is not null)
                {
                    Console.WriteLine($"      {finding.Detail}");
                }
                else if (context is not null)
                {
                    foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                    {
                        Console.WriteLine($"      {diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                }
                failures++;
                continue;
            }

            (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
            using var analyzer = new BehaviorDifferentialAnalyzer();
            (bool ok, string? error, Neo.VM.Types.StackItem? value) =
                analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), "probe", [7]);

            if (ok)
            {
                Console.WriteLine($"  {label,-34} => {BehaviorDifferentialAnalyzer.Render(value!)}");
            }
            else
            {
                Console.WriteLine($"  {label,-34} => FAULT: {error}");
                failures++;
            }
        }

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "STORAGE MAP PROBE PASSED: every operation ran."
            : $"STORAGE MAP PROBE FAILED: {failures} operation(s) did not work.");

        return failures;
    }
}
