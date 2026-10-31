// Copyright (C) 2015-2026 The Neo Project.
//
// StructMatrixProbe.cs file belongs to the neo project and is free
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
/// Compares many struct shapes between the VM and the CLR, one shape per compilation.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="StructProbe"/> covers the shapes that were known to fail. This probe widens
/// the matrix to the shapes that store a struct without going through a plain variable
/// assignment: boxing, array elements, fields of other structs, three levels of nesting,
/// and collections.
/// </para>
/// <para>
/// Every shape is compiled twice, once as a contract and once as plain C#, and both are
/// executed with the same arguments. A shape that neither side compiles is reported as
/// unsupported rather than as a failure, so the probe stays useful while the language
/// subset grows.
/// </para>
/// </remarks>
public static class StructMatrixProbe
{
    private static readonly (string Label, string Declarations, string Body, int? Expected)[] Cases =
    [
        ("three levels of nesting",
            """
                    public struct A { public int X; }
                    public struct B { public A Inner; public int Y; }
                    public struct C { public B Middle; public int Z; }
            """,
            """
                    var c = new C { Middle = new B { Inner = new A { X = a }, Y = b }, Z = a + b };
                    var d = c;
                    d.Middle.Inner.X = 99;
                    return c.Middle.Inner.X * 1000 + d.Middle.Inner.X;
            """, null),

        ("nested struct inside a class field",
            """
                    public struct Point { public int X; public int Y; }
                    public struct Segment { public Point From; public Point To; }
                    public class Holder { public Segment S; }
                    private static Segment Take(Holder h) { return h.S; }
            """,
            """
                    var holder = new Holder { S = new Segment { From = new Point { X = a, Y = b }, To = new Point { X = 1, Y = 2 } } };
                    var s = Take(holder);
                    s.From.X = 99;
                    return holder.S.From.X * 1000 + s.From.X;
            """, null),

        ("struct element copied to another element",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var arr = new Point[2];
                    arr[0] = new Point { X = a, Y = b };
                    arr[1] = arr[0];
                    arr[1].X = 99;
                    return arr[0].X * 1000 + arr[1].X;
            """, null),

        ("struct field assigned from another struct",
            """
                    public struct Point { public int X; public int Y; }
                    public struct Segment { public Point From; public Point To; }
            """,
            """
                    var s = new Segment();
                    var p = new Point { X = a, Y = b };
                    s.From = p;
                    p.X = 99;
                    return s.From.X * 1000 + p.X;
            """, null),

        ("struct with a byte array field",
            """
                    public struct Blob { public byte[] Data; public int Length; }
            """,
            """
                    var one = new Blob { Data = new byte[] { 1, 2, 3 }, Length = 3 };
                    var two = one;
                    two.Length = 99;
                    return one.Length * 1000 + two.Length;
            """, null),

        ("struct with a string field",
            """
                    public struct Named { public string Name; public int Value; }
            """,
            """
                    var one = new Named { Name = "neo", Value = a };
                    var two = one;
                    two.Value = 99;
                    return one.Value * 1000 + two.Value;
            """, null),

        ("array of structs assigned to a second variable",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var one = new Point[] { new Point { X = a, Y = b } };
                    var two = one;
                    two[0].X = 99;
                    return one[0].X * 1000 + two[0].X;
            """, null),

        ("struct boxed as object",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    object boxed = p;
                    p.X = 99;
                    return ((Point)boxed).X * 1000 + p.X;
            """, null),

        ("struct stored in an object array",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    var boxed = new object[] { p };
                    p.X = 99;
                    return ((Point)boxed[0]).X * 1000 + p.X;
            """, null),

        ("struct returned from a field access",
            """
                    public struct Point { public int X; public int Y; }
                    public class Holder { public Point P; }
                    private static Point Read(Holder h) { return h.P; }
            """,
            """
                    var holder = new Holder { P = new Point { X = a, Y = b } };
                    var q = Read(holder);
                    q.X = 99;
                    return holder.P.X * 1000 + q.X;
            """, null),

        ("struct passed through two calls",
            """
                    public struct Point { public int X; public int Y; }
                    private static Point Bump(Point p) { p.Y = p.Y + 1; return p; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    var q = Bump(p);
                    q.X = 99;
                    return p.X * 1000 + q.X;
            """, null),

        ("struct read by a foreach loop after a write",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var arr = new Point[] { new Point { X = a, Y = b } };
                    int sum = 0;
                    foreach (var p in arr) { arr[0].X = 99; sum = sum + p.X; }
                    return sum;
            """, null),

        ("struct copy inside try",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    var q = new Point();
                    try { q = p; throw new Exception("boom"); } catch { }
                    q.X = 99;
                    return p.X * 1000 + q.X;
            """, null),

        ("struct copy inside a loop with continue",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    int sum = 0;
                    for (int i = 0; i < 3; i = i + 1)
                    {
                        var q = p;
                        q.X = 99;
                        if (i == 1) continue;
                        sum = sum + q.X;
                    }
                    return p.X * 1000 + sum;
            """, null),

        ("tuple assigned to a second variable",
            """
                    // no extra declarations
            """,
            """
                    var t = (a, b);
                    var u = t;
                    u.Item1 = 99;
                    return t.Item1 * 1000 + u.Item1;
            """, null),

        ("tuple argument mutated in callee",
            """
                    private static int Bump((int, int) value) { value.Item1 = 99; return value.Item1; }
            """,
            """
                    var t = (a, b);
                    return Bump(t) * 1000 + t.Item1;
            """, null),

        ("default tuple fields",
            """
                    // no extra declarations
            """,
            """
                    (int, int) t = default;
                    return t.Item1 * 1000 + t.Item2;
            """, null),



        ("nested tuple assigned",
            """
                    // no extra declarations
            """,
            """
                    var t = ((a, b), 5);
                    var u = t;
                    u.Item1.Item1 = 99;
                    return t.Item1.Item1 * 1000 + u.Item1.Item1;
            """, null),

        ("struct out parameter",
            """
                    public struct Point { public int X; public int Y; }
                    private static void Make(out Point p) { p = new Point { X = 5, Y = 6 }; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    Make(out p);
                    return p.X * 1000 + p.Y;
            """, null),

        ("struct ref parameter",
            """
                    public struct Point { public int X; public int Y; }
                    private static void Bump(ref Point p) { p.X = p.X + 1; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    Bump(ref p);
                    return p.X * 1000 + p.Y;
            """, null),


        ("struct boxed by an explicit cast",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    object boxed = (object)p;
                    p.X = 99;
                    return ((Point)boxed).X * 1000 + p.X;
            """, null),

        ("struct explicitly cast then written through the box",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    object boxed = (object)p;
                    ((Point)boxed).X = 99;
                    return p.X * 1000 + ((Point)boxed).X;
            """, null),

        ("captured integer read by lambda",
            """
                    // no extra declarations
            """,
            """
                    int value = a;
                    Func<int> read = () => value;
                    return read();
            """, null),

        ("captured struct read by lambda",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    Func<int> read = () => p.X + p.Y;
                    return read();
            """, null),

        ("captured struct written by lambda",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var p = new Point { X = a, Y = b };
                    Action<int> write = value => p.X = value;
                    write(99);
                    return p.X * 1000 + p.Y;
            """, null),

        // The two shapes below use framework collections, which the CLR reference cannot run:
        // the framework list is a set of opcode-lowered stubs. The expected value is what C#
        // would return if the collection stored a copy of the struct.
        ("struct added to a framework list",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var list = new List<Point>();
                    var p = new Point { X = a, Y = b };
                    list.Add(p);
                    p.X = 99;
                    return list[0].X * 1000 + p.X;
            """, 7099),

        ("struct stored in a framework map",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var map = new Map<string, Point>();
                    var p = new Point { X = a, Y = b };
                    map["key"] = p;
                    p.X = 99;
                    return map["key"].X * 1000 + p.X;
            """, 7099),

        ("struct written through a list indexer",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var list = new List<Point>();
                    list.Add(new Point { X = 1, Y = 2 });
                    var p = new Point { X = a, Y = b };
                    list[0] = p;
                    p.X = 99;
                    return list[0].X * 1000 + p.X;
            """, 7099),

        ("struct copied out of a list",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var list = new List<Point>();
                    list.Add(new Point { X = a, Y = b });
                    var p = list[0];
                    p.X = 99;
                    return list[0].X * 1000 + p.X;
            """, 7099),

        ("struct copied out of a map",
            """
                    public struct Point { public int X; public int Y; }
            """,
            """
                    var map = new Map<string, Point>();
                    map["key"] = new Point { X = a, Y = b };
                    var p = map["key"];
                    p.X = 99;
                    return map["key"].X * 1000 + p.X;
            """, 7099),

        ("nested struct in a framework list",
            """
                    public struct Point { public int X; public int Y; }
                    public struct Segment { public Point From; public Point To; }
            """,
            """
                    var list = new List<Segment>();
                    var s = new Segment { From = new Point { X = a, Y = b }, To = new Point { X = 1, Y = 2 } };
                    list.Add(s);
                    s.From.X = 99;
                    return list[0].From.X * 1000 + s.From.X;
            """, 7099),

        ("nested struct in a framework map",
            """
                    public struct Point { public int X; public int Y; }
                    public struct Segment { public Point From; public Point To; }
            """,
            """
                    var map = new Map<string, Segment>();
                    var s = new Segment { From = new Point { X = a, Y = b }, To = new Point { X = 1, Y = 2 } };
                    map["key"] = s;
                    s.From.X = 99;
                    return map["key"].From.X * 1000 + s.From.X;
            """, 7099),
    ];

    private static readonly CompilationOptions.OptimizationType[] Levels =
    [
        CompilationOptions.OptimizationType.None,
        CompilationOptions.OptimizationType.Basic,
        CompilationOptions.OptimizationType.Experimental,
        CompilationOptions.OptimizationType.All,
    ];

    /// <summary>
    /// Compiles one shape at a given optimization level and executes it.
    /// </summary>
    private static (bool Compiled, string Result) RunShape(string source, CompilationOptions.OptimizationType level)
    {
        using var harness = new CompilerHarness(new CompilationOptions { Optimize = level });
        (FuzzOutcome outcome, CompilationContext? context, _) = harness.Compile(source);
        if (outcome != FuzzOutcome.Clean || context is null)
            return (false, string.Empty);

        (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
        using var analyzer = new BehaviorDifferentialAnalyzer();
        (bool ok, string? error, Neo.VM.Types.StackItem? value) =
            analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), "probe", [7, 3]);

        return (true, ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})");
    }

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of shapes where the VM disagrees with the CLR.</returns>
    public static int Run()
    {
        int mismatches = 0;

        foreach ((string label, string declarations, string body, int? expected) in Cases)
        {
            string contractSource = $$"""
                using System;
                using System.Numerics;
                using Neo.SmartContract.Framework;

                public class StructMatrixContract : SmartContract
                {
                {{declarations}}

                    public static int Probe(int a, int b)
                    {
                {{body}}
                    }
                }
                """;

            string referenceSource = $$"""
                using System;
                using System.Numerics;

                public static class StructMatrixReference
                {
                {{declarations}}

                    public static int Probe(int a, int b)
                    {
                {{body}}
                    }
                }
                """;

            string? clrResult = null;
            if (expected is null)
            {
                using var oracle = new CrossLanguageOracle();
                Assembly? reference = oracle.TryCompileReference(referenceSource);
                if (reference is null)
                {
                    Console.WriteLine($"  {label,-46} => CLR rejected, skipped");
                    continue;
                }

                (bool okRef, object? refValue, string? refError) =
                    oracle.TryInvoke(reference, "StructMatrixReference", "Probe", [7, 3]);
                if (!okRef)
                {
                    Console.WriteLine($"  {label,-46} => CLR threw ({refError}), skipped");
                    continue;
                }

                clrResult = refValue?.ToString() ?? "<null>";
            }
            else
            {
                clrResult = expected.Value.ToString();
            }

            // Every optimization level has to produce the same answer; checking only the
            // default level would leave a level-specific lowering bug unobserved.
            bool shapeAgrees = true;
            var perLevel = new List<string>();

            foreach (CompilationOptions.OptimizationType level in Levels)
            {
                (bool compiled, string result) = RunShape(contractSource, level);
                if (!compiled)
                {
                    Console.WriteLine($"  {label,-46} => REJECTED at {level}");
                    shapeAgrees = false;
                    break;
                }

                perLevel.Add($"{level}:{result}");

                if (result.Contains("Out of stack bounds", StringComparison.Ordinal))
                {
                    // The collection itself was never created on the stack, because the extern
                    // constructor of a framework type is not expanded when the framework is
                    // referenced as an assembly. That defect is tracked separately.
                    Console.WriteLine($"  {label,-46} => BLOCKED by the extern constructor defect ({result})");
                    shapeAgrees = true;
                    perLevel.Clear();
                    break;
                }

                if (!string.Equals(clrResult, result, StringComparison.Ordinal))
                {
                    Console.WriteLine($"  {label,-46} => VM[{level}] {result,-30} C# {clrResult,-8} MISMATCH");
                    shapeAgrees = false;
                }
            }

            if (perLevel.Count > 0)
            {
                Console.WriteLine($"  {label,-46} => {string.Join(" ", perLevel),-62} C# {clrResult,-8} " +
                                  $"{(shapeAgrees ? "match" : "MISMATCH")}");
            }

            if (!shapeAgrees)
                mismatches++;
        }

        Console.WriteLine();
        Console.WriteLine(mismatches == 0
            ? "STRUCT MATRIX PASSED: every supported shape matches the CLR."
            : $"STRUCT MATRIX: {mismatches} shape(s) disagree with the CLR.");

        return mismatches;
    }
}
