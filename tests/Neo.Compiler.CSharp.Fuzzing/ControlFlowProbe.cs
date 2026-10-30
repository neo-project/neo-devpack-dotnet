// Copyright (C) 2015-2026 The Neo Project.
//
// ControlFlowProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Checks the interaction of try/finally with loops and jumps.
/// </summary>
/// <remarks>
/// break/continue/return out of a protected region must reroute through the finally
/// blocks; the shapes where several of them combine are where the lowering can disagree
/// with C#.
/// </remarks>
public static class ControlFlowProbe
{
    private static readonly (string Label, string Helpers, string Body)[] Cases =
    [
        ("break out of try in loop", "",
            """
                    int total = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            if (i == a) break;
                            total += i;
                        }
                        catch (System.Exception)
                        {
                            total -= 100;
                        }
                    }
                    return total;
            """),

        ("return through finally", "",
            """
                    int tracked = 0;
                    try
                    {
                        tracked += 1;
                        return a + tracked;
                    }
                    finally
                    {
                        tracked += 50;
                    }
            """),

        ("continue through finally", "",
            """
                    int total = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        try
                        {
                            if (i == a) continue;
                            total += i;
                        }
                        finally
                        {
                            total += 1000;
                        }
                    }
                    return total;
            """),

        ("catch in loop", "",
            """
                    int total = 0;
                    for (int i = -2; i < 3; i++)
                    {
                        try
                        {
                            total += a / i;
                        }
                        catch (System.Exception)
                        {
                            total += 7;
                        }
                    }
                    return total;
            """),

        ("nested finally with break", "",
            """
                    int total = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            try
                            {
                                if (i == a) break;
                                total += 1;
                            }
                            finally
                            {
                                total += 10;
                            }
                        }
                        finally
                        {
                            total += 100;
                        }
                    }
                    return total;
            """),

        ("compound divide in try, no loop", "",
            """
                    int total = 0;
                    try
                    {
                        total += a / b;
                    }
                    catch (System.Exception)
                    {
                        total += 7;
                    }
                    return total;
            """),

        ("loop try with declaration", "",
            """
                    int total = 0;
                    for (int i = -1; i < 2; i++)
                    {
                        try
                        {
                            int r = a / i;
                            total += r;
                        }
                        catch (System.Exception)
                        {
                            total += 7;
                        }
                    }
                    return total;
            """),

        ("loop try with plain expression", "",
            """
                    int total = 0;
                    for (int i = -1; i < 2; i++)
                    {
                        try
                        {
                            total = total + a / i;
                        }
                        catch (System.Exception)
                        {
                            total += 7;
                        }
                    }
                    return total;
            """),
        new ("throw in helper caught by caller",
            """
                    private static int Divide(int x, int y) { return x / y; }
            """,
            """
                    try
                    {
                        return Divide(a, b) + 1;
                    }
                    catch (System.Exception)
                    {
                        return -1;
                    }
            """),

        new ("finally runs when helper throws",
            """
                    private static int Thrower(int x) { if (x == 0) throw new System.Exception("zero"); return x; }
            """,
            """
                    int total = 0;
                    try
                    {
                        total += Thrower(b) * 10;
                    }
                    catch (System.Exception)
                    {
                        total += 7;
                    }
                    finally
                    {
                        total += 100;
                    }
                    return total + a;
            """),

        new ("throw in lambda caught by enclosing try",
            "",
            """
                    Func<int, int> halve = x => 100 / x;
                    try
                    {
                        return halve(b);
                    }
                    catch (System.Exception)
                    {
                        return -1;
                    }
            """),

        new ("catch across nested calls",
            """
                    private static int Inner(int y) { return 10 / y; }
                    private static int Middle(int y) { return Inner(y) * 2; }
            """,
            """
                    try
                    {
                        return Middle(b);
                    }
                    catch (System.Exception)
                    {
                        return -5;
                    }
            """),
        new ("minimal catch+finally with exception",
            """
                    private static int Thrower(int x) { if (x == 0) throw new System.Exception("zero"); return x; }
            """,
            """
                    int total = 0;
                    try
                    {
                        total += Thrower(b);
                    }
                    catch (System.Exception)
                    {
                        total += 7;
                    }
                    finally
                    {
                        total += 100;
                    }
                    return total;
            """),

        new ("catch+finally without exception",
            """
                    private static int Thrower(int x) { if (x == 0) throw new System.Exception("zero"); return x; }
            """,
            """
                    int total = 0;
                    try
                    {
                        total += Thrower(b + 1);
                    }
                    catch (System.Exception)
                    {
                        total += 7;
                    }
                    finally
                    {
                        total += 100;
                    }
                    return total;
            """),

        new ("inline catch+finally with exception",
            "",
            """
                    int total = 0;
                    try
                    {
                        total += 100 / b;
                    }
                    catch (System.Exception)
                    {
                        total += 7;
                    }
                    finally
                    {
                        total += 1000;
                    }
                    return total;
            """),
    ];

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of shapes whose VM result disagrees with C#.</returns>
    public static int Run()
    {
        int mismatches = 0;

        foreach ((string label, string helpers, string body) in Cases)
        {
            string source = $$"""
                using System;
                using System.Numerics;
                using Neo.SmartContract.Framework;

                public class ControlFlowProbeContract : SmartContract
                {
                {{helpers}}
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
                Console.WriteLine($"  {label,-34} => REJECTED ({outcome}) {finding?.Detail}");
                continue;
            }

            (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
            using var analyzer = new BehaviorDifferentialAnalyzer();
            (bool ok, string? error, Neo.VM.Types.StackItem? value) =
                analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), "probe", [2, 0]);

            string vmResult = ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})";

            string referenceSource = $$"""
                using System;
                using System.Numerics;

                public static class ControlFlowReference
                {
                {{helpers}}
                    public static int Probe(int a, int b)
                    {
                {{body}}
                    }
                }
                """;

            using var oracle = new CrossLanguageOracle();
            Assembly? referenceAssembly = oracle.TryCompileReference(referenceSource);
            string clrResult = "<not compared>";

            if (referenceAssembly is not null)
            {
                (bool okRef, object? refValue, string? refError) =
                    oracle.TryInvoke(referenceAssembly, "ControlFlowReference", "Probe", [2, 0]);
                clrResult = okRef ? refValue?.ToString() ?? "<null>" : $"THROW({refError})";
            }

            bool agrees = string.Equals(clrResult, vmResult, StringComparison.Ordinal);
            if (!agrees)
                mismatches++;

            Console.WriteLine($"  {label,-34} => VM {vmResult,-24} C# {clrResult,-12} {(agrees ? "match" : "MISMATCH")}");
        }

        Console.WriteLine();
        Console.WriteLine(mismatches == 0
            ? "CONTROL FLOW PROBE PASSED: every interaction shape matches C#."
            : $"CONTROL FLOW PROBE: {mismatches} shape(s) disagree with C#.");

        return mismatches;
    }
}
