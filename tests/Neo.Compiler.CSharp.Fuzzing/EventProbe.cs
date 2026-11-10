// Copyright (C) 2015-2026 The Neo Project.
//
// EventProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Checks the argument order of the notifications a contract emits.
/// </summary>
/// <remarks>
/// <para>
/// The compiler packs the notification arguments into an array and calls
/// <c>System.Runtime.Notify</c>. Nothing on chain validates that order, so a mistake only shows
/// up in the off-chain listener that reads the event. The probe executes the contract and
/// compares the captured notification against the order the arguments were written in.
/// </para>
/// <para>
/// Byte strings are rendered as hex, integers as decimal, and arrays as a list, which is enough
/// to pin the order of mixed argument types.
/// </para>
/// </remarks>
public static class EventProbe
{
    private static readonly (string Label, string Declarations, string Body, string Expected)[] Cases =
    [
        ("runtime debug notification",
            """
                    // no extra declarations
            """,
            """
                    Runtime.Debug("hello");
                    return 0;
            """,
            "Debug:[0x68656C6C6F]"),

        ("event with five mixed arguments",
            """
                    public static event Action<int, string, bool, byte[], long> Many;
            """,
            """
                    Many(a, "x", true, new byte[] { 9 }, (long)a * b);
                    return 0;
            """,
            "Many:[7,0x78,true,0x09,21]"),

        ("declared event arguments",
            """
                    public static event Action<int, string, int> Fired;
            """,
            """
                    Fired(a, "text", b);
                    return 0;
            """,
            "Fired:[7,0x74657874,3]"),

        ("two events keep their order",
            """
                    public static event Action<int> First;
                    public static event Action<int> Second;
            """,
            """
                    First(a);
                    Second(b);
                    return 0;
            """,
            "First:[7]|Second:[3]"),

        ("event with a long and a byte array",
            """
                    public static event Action<long, byte[]> Payload;
            """,
            """
                    Payload(a * 1000000000L, new byte[] { 0x10, 0x20 });
                    return 0;
            """,
            "Payload:[7000000000,0x1020]"),
    ];

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of shapes whose notification payloads differ from the expected order.</returns>
    public static int Run()
    {
        int mismatches = 0;

        foreach ((string label, string declarations, string body, string expected) in Cases)
        {
            string source = $$"""
                using System;
                using System.Numerics;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Services;

                public class EventProbeContract : SmartContract
                {
                {{declarations}}

                    public static int Probe(int a, int b)
                    {
                {{body}}
                    }
                }
                """;

            using var harness = new CompilerHarness();
            (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);
            if (outcome != FuzzOutcome.Clean || context is null)
            {
                string reason = context?.Diagnostics
                    .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                    .Select(d => $"{d.Id}: {d.GetMessage()}")
                    .FirstOrDefault() ?? finding?.Detail ?? "no diagnostics";
                Console.WriteLine($"  {label,-40} => REJECTED {reason}");
                mismatches++;
                continue;
            }

            (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
            using var analyzer = new BehaviorDifferentialAnalyzer();
            (bool ok, string? error, _, IReadOnlyList<string> notifications) =
                analyzer.ExecuteWithNotifications(new ArtifactSet(nef, manifest), "probe", [7, 3]);

            if (!ok)
            {
                Console.WriteLine($"  {label,-40} => FAULT({error})");
                mismatches++;
                continue;
            }

            // The deploy handler emits its own notification; only the ones from the call matter.
            string actual = string.Join("|", notifications.Where(n => !n.StartsWith("Deploy:", StringComparison.Ordinal)));
            bool agrees = string.Equals(actual, expected, StringComparison.Ordinal);
            Console.WriteLine($"  {label,-40} => {actual,-40} expected {expected,-40} {(agrees ? "match" : "MISMATCH")}");

            if (!agrees)
                mismatches++;
        }

        Console.WriteLine();
        Console.WriteLine(mismatches == 0
            ? "EVENT PROBE PASSED: every notification keeps the argument order."
            : $"EVENT PROBE: {mismatches} shape(s) emit arguments in a different order.");

        return mismatches;
    }
}
