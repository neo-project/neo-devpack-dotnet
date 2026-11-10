// Copyright (C) 2015-2026 The Neo Project.
//
// StructProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Testing;
using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Isolates the struct shapes that fail, and compares each against plain C#.
/// </summary>
/// <remarks>
/// A struct field is lowered to an indexed store into the field array. For a struct
/// nested inside another struct the compiler also has to store the inner struct back into
/// its parent, which is a second indexed store — a step that can be missed. Each shape is
/// compiled and executed on its own here so a failure can be attributed, and the result is
/// compared against the same method running on the CLR.
/// </remarks>
public static class StructProbe
{
    private static readonly (string Label, string Body)[] Cases =
    [
        ("flat struct fields",
            """
                    var p = new Point();
                    p.X = a;
                    p.Y = b;
                    return p.X * 2 + p.Y;
            """),

        ("struct copy",
            """
                    var p = new Point { X = a, Y = a + 1 };
                    var q = p;
                    q.X = 99;
                    return p.X + q.X;
            """),

        ("nested struct field write",
            """
                    var n = new Nested();
                    n.Origin.X = a;
                    n.Origin.Y = b;
                    n.Scale = 2;
                    return n.Origin.X * n.Scale + n.Origin.Y;
            """),

        ("nested struct read only",
            """
                    var n = new Nested();
                    n.Scale = 2;
                    return n.Origin.X + n.Scale;
            """),

        ("struct passed to method",
            """
                    var p = new Point { X = a, Y = b };
                    return Add(p, b);
            """),

        ("struct copy into array element",
            """
                    var p = new Point { X = a, Y = b };
                    var arr = new Point[1];
                    arr[0] = p;
                    p.X = 99;
                    return arr[0].X + p.X;
            """),

        ("struct argument mutated in callee",
            """
                    var p = new Point { X = a, Y = b };
                    return Overwrite(p) + p.X * 1000;
            """),

        ("struct field of a class",            """
                    var holder = new Holder { P = new Point { X = a, Y = b } };
                    var q = holder.P;
                    q.X = 99;
                    return holder.P.X + q.X;
            """),

        ("struct object initializer",
            """
                    var p = new Point { X = a, Y = b };
                    var s = new Nested { Origin = p, Scale = 2 };
                    p.X = 99;
                    return s.Origin.X * 1000 + p.X;
            """),
    ];

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of shapes that failed.</returns>
    public static int Run()
    {
        int failures = 0;

        foreach ((string label, string body) in Cases)
        {
            string source = $$"""
                using System;
                using System.Numerics;
                using Neo.SmartContract.Framework;

                public class StructProbeContract : SmartContract
                {
                    public struct Point
                    {
                        public int X;
                        public int Y;
                    }

                    public struct Nested
                    {
                        public Point Origin;
                        public int Scale;
                    }

                    private static int Add(Point p, int delta) { return p.X + p.Y + delta; }

                    private static int Overwrite(Point p) { p.X = 99; return p.X; }

                    public class Holder
                    {
                        public Point P;
                    }

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
                Console.WriteLine($"  {label,-32} => REJECTED ({outcome}) {finding?.Detail}");
                failures++;
                continue;
            }

            (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
            using var analyzer = new BehaviorDifferentialAnalyzer();
            (bool ok, string? error, Neo.VM.Types.StackItem? value) =
                analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), "probe", [7, 3]);

            string vmResult = ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})";

            // The same body on the CLR, to tell a compiler defect from a genuine fault.
            string referenceSource = $$"""
                using System;
                using System.Numerics;

                public static class StructReference
                {
                    public struct Point
                    {
                        public int X;
                        public int Y;
                    }

                    public struct Nested
                    {
                        public Point Origin;
                        public int Scale;
                    }

                    private static int Add(Point p, int delta) { return p.X + p.Y + delta; }

                    private static int Overwrite(Point p) { p.X = 99; return p.X; }

                    public class Holder
                    {
                        public Point P;
                    }

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
                    oracle.TryInvoke(referenceAssembly, "StructReference", "Probe", [7, 3]);
                clrResult = okRef ? refValue?.ToString() ?? "<null>" : $"THROW({refError})";
            }

            bool agrees = string.Equals(clrResult, vmResult, StringComparison.Ordinal);

            Console.WriteLine($"  {label,-32} => VM {vmResult,-40} C# {clrResult,-12} {(agrees ? "match" : "MISMATCH")}");

            if (!agrees)
            {
                Console.WriteLine();
                Console.WriteLine($"  --- disassembly for '{label}' ---");
                foreach (string line in context.CreateAssembly().Split('\n'))
                {
                    if (line.Contains("Probe", StringComparison.Ordinal) || line.StartsWith("0000", StringComparison.Ordinal))
                    {
                        Console.WriteLine($"  {line.TrimEnd()}");
                    }
                }
                Console.WriteLine();
            }

            if (!ok)
            {
                failures++;
            }
        }

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "STRUCT PROBE PASSED: every shape ran."
            : $"STRUCT PROBE: {failures} shape(s) faulted on the VM.");

        return failures;
    }
}
