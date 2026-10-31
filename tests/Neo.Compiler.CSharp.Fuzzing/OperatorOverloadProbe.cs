// Copyright (C) 2015-2026 The Neo Project.
//
// OperatorOverloadProbe.cs file belongs to the neo project and is free
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
/// Checks how user-defined operators on structs are compiled.
/// </summary>
/// <remarks>
/// The binary-expression converter emits a raw VM opcode for every operator token, so a
/// user-defined <c>operator==</c> is compiled to structural EQUAL instead of a call to the
/// operator method. When the operator's semantics differ from structural equality (or from
/// what the raw opcode does to two struct values) the compiled contract silently disagrees
/// with C#. Arithmetic operators are rejected instead, with an integer-range diagnostic that
/// does not mention the actual cause.
/// </remarks>
public static class OperatorOverloadProbe
{
    private sealed record Case(string Label, string StructDeclaration, string Body, string Args);

    private static readonly Case[] Cases =
    [
        new Case("operator== compares X only",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static bool operator ==(Vec a, Vec b) { return a.X == b.X; }
                        public static bool operator !=(Vec a, Vec b) { return a.X != b.X; }
                    }
            """,
            """
                    Vec p = new Vec(7, 3);
                    Vec q = new Vec(7, 9);
                    return p == q ? 1 : 0;
            """,
            "0"),

        new Case("operator!= compares X only",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static bool operator ==(Vec a, Vec b) { return a.X == b.X; }
                        public static bool operator !=(Vec a, Vec b) { return a.X != b.X; }
                    }
            """,
            """
                    Vec p = new Vec(7, 3);
                    Vec q = new Vec(7, 9);
                    return p != q ? 1 : 0;
            """,
            "0"),

        new Case("operator== compares all fields",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static bool operator ==(Vec a, Vec b) { return a.X == b.X && a.Y == b.Y; }
                        public static bool operator !=(Vec a, Vec b) { return a.X != b.X || a.Y != b.Y; }
                    }
            """,
            """
                    Vec p = new Vec(7, 3);
                    Vec q = new Vec(7, 3);
                    Vec r = new Vec(7, 4);
                    return (p == q ? 1 : 0) + (p == r ? 2 : 0);
            """,
            "0"),

        new Case("operator< compares X only",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static bool operator <(Vec a, Vec b) { return a.X < b.X; }
                        public static bool operator >(Vec a, Vec b) { return a.X > b.X; }
                    }
            """,
            """
                    Vec p = new Vec(7, 3);
                    Vec q = new Vec(9, 1);
                    return p < q ? 1 : 0;
            """,
            "0"),

        new Case("implicit conversion from int",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static implicit operator Vec(int x) { return new Vec(x, x + 1); }
                    }
            """,
            """
                    Vec v = 7;
                    return v.X * 100 + v.Y;
            """,
            "0"),

        new Case("explicit conversion to int",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static explicit operator int(Vec v) { return v.X * 1000 + v.Y; }
                    }
            """,
            """
                    Vec v = new Vec(7, 3);
                    return (int)v;
            """,
            "0"),

        new Case("operator+ on structs",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static Vec operator +(Vec a, Vec b) { return new Vec(a.X + b.X, a.Y + b.Y); }
                    }
            """,
            """
                    Vec p = new Vec(7, 3);
                    Vec q = new Vec(1, 2);
                    Vec c = p + q;
                    return c.X * 100 + c.Y;
            """,
            "0"),

        new Case("operator- is order sensitive",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static Vec operator -(Vec a, Vec b) { return new Vec(a.X - b.X, a.Y - b.Y); }
                    }
            """,
            """
                    Vec p = new Vec(9, 4);
                    Vec q = new Vec(3, 1);
                    Vec c = p - q;
                    return c.X * 100 + c.Y;
            """,
            "0"),

        new Case("compound assignment through operator",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static Vec operator +(Vec a, Vec b) { return new Vec(a.X + b.X, a.Y + b.Y); }
                        public static Vec operator *(Vec a, int k) { return new Vec(a.X * k, a.Y * k); }
                    }
            """,
            """
                    Vec v = new Vec(7, 3);
                    v += new Vec(1, 2);
                    v *= 2;
                    return v.X * 100 + v.Y;
            """,
            "0"),

        new Case("postfix increment uses operator",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static Vec operator ++(Vec a) { return new Vec(a.X + 1, a.Y + 10); }
                    }
            """,
            """
                    Vec v = new Vec(7, 3);
                    Vec old = v++;
                    return old.X * 10000 + old.Y * 100 + v.X * 10 + v.Y;
            """,
            "0"),

        new Case("prefix decrement uses operator",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static Vec operator --(Vec a) { return new Vec(a.X - 1, a.Y - 5); }
                    }
            """,
            """
                    Vec v = new Vec(7, 30);
                    Vec now = --v;
                    return now.X * 1000 + now.Y + v.X;
            """,
            "0"),

        new Case("compound assignment on array element",
            """
                    public struct Vec
                    {
                        public int X;
                        public int Y;
                        public Vec(int x, int y) { X = x; Y = y; }
                        public static Vec operator +(Vec a, Vec b) { return new Vec(a.X + b.X, a.Y + b.Y); }
                    }
            """,
            """
                    Vec[] arr = new Vec[] { new Vec(7, 3), new Vec(1, 1) };
                    arr[0] += new Vec(1, 2);
                    return arr[0].X * 100 + arr[0].Y + arr[1].X;
            """,
            "0"),
    ];

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of cases whose VM result disagrees with C#.</returns>
    public static int Run()
    {
        int mismatches = 0;

        foreach (Case c in Cases)
        {
            string source = $$"""
                using System;
                using System.Numerics;
                using Neo.SmartContract.Framework;

                public class OperatorProbeContract : SmartContract
                {
                {{c.StructDeclaration}}

                    public static int Probe(int a, int b)
                    {
                {{c.Body}}
                    }
                }
                """;

            using var harness = new CompilerHarness();
            (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

            if (outcome != FuzzOutcome.Clean || context is null)
            {
                Console.WriteLine($"  {c.Label,-34} => REJECTED ({outcome}) {finding?.Detail}");
                continue;
            }

            (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
            using var analyzer = new BehaviorDifferentialAnalyzer();
            (bool ok, string? error, Neo.VM.Types.StackItem? value) =
                analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), "probe", [7, 3]);

            string vmResult = ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})";

            string referenceSource = $$"""
                using System;
                using System.Numerics;

                public static class OperatorReference
                {
                {{c.StructDeclaration}}

                    public static int Probe(int a, int b)
                    {
                {{c.Body}}
                    }
                }
                """;

            using var oracle = new CrossLanguageOracle();
            Assembly? referenceAssembly = oracle.TryCompileReference(referenceSource);
            string clrResult = "<not compared>";

            if (referenceAssembly is not null)
            {
                (bool okRef, object? refValue, string? refError) =
                    oracle.TryInvoke(referenceAssembly, "OperatorReference", "Probe", [7, 3]);
                clrResult = okRef ? refValue?.ToString() ?? "<null>" : $"THROW({refError})";
            }

            bool agrees = string.Equals(clrResult, vmResult, StringComparison.Ordinal);
            if (!agrees)
                mismatches++;

            Console.WriteLine($"  {c.Label,-34} => VM {vmResult,-24} C# {clrResult,-12} {(agrees ? "match" : "MISMATCH")}");

            if (!agrees)
            {
                Console.WriteLine();
                Console.WriteLine($"  --- disassembly for '{c.Label}' ---");
                foreach (string line in context.CreateAssembly().Split('\n'))
                {
                    if (line.Contains("Probe", StringComparison.Ordinal) || line.StartsWith("0000", StringComparison.Ordinal))
                    {
                        Console.WriteLine($"  {line.TrimEnd()}");
                    }
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine();
        Console.WriteLine(mismatches == 0
            ? "OPERATOR PROBE PASSED: every user-defined operator shape matches C#."
            : $"OPERATOR PROBE: {mismatches} shape(s) disagree with C#.");

        return mismatches;
    }
}
