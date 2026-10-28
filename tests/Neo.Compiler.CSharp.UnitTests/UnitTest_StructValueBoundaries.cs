// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StructValueBoundaries
{
    [TestMethod]
    [DataRow("var p = new Point { X = 7 }; object boxed = (object)p; p.X = 99; return ((Point)boxed).X * 1000 + p.X;", 7099)]
    [DataRow("var p = new Point { X = 7 }; var frame = new Frame { Origin = p }; var copy = frame with { Scale = 4 }; copy.Origin.X = 99; return frame.Origin.X * 1000 + copy.Origin.X;", 7099)]
    [DataRow("var items = new (Point, int)[] { (new Point { X = 7 }, 3) }; int sum = 0; foreach (var (point, count) in items) { items[0].Item1.X = 99; sum += point.X + count; } return sum;", 10)]
    [DataRow("var p = new Point { X = 7 }; Point? copy = p; p.X = 99; return ((Point)copy).X * 1000 + p.X;", 7099)]
    [DataRow("var values = new int[] { 7 }; var p = new References { Values = values }; object boxed = (object)p; values[0] = 99; return ((References)boxed).Values[0];", 99)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxShared(); Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedExpression(); Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Point? value = null; Point? copy = value; return copy == null ? 0 : 1;", 0)]
    [DataRow("Point? value = new Point { X = 7 }; object boxed = value; value = new Point { X = 99 }; return ((Point)boxed).X;", 7)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedProperty; Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedAccessor; Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedInline(); Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedInlineBlock(); Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedLambda(); Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; object boxed = BoxSharedBlockLambda(); Shared.X = 99; return ((Point)boxed).X * 1000 + Shared.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; var copy = GetSharedPoint(); copy.X = 99; return Shared.X * 1000 + copy.X;", 7099)]
    [DataRow("Shared = new Point { X = 7 }; GetSharedPoint().SetX(99); return Shared.X;", 7)]
    [DataRow("var p = new Point { X = 7 }; Point[] items = [p]; p.X = 99; return items[0].X * 1000 + p.X;", 7099)]
    [DataRow("var items = new Point[1]; return items[0].X;", 0)]
    [DataRow("var items = new Point[2]; items[0].X = 7; return items[1].X * 1000 + items[0].X;", 7)]
    [DataRow("var p = new Point { X = 7 }; int changed = Change(p); return changed * 1000 + p.X;", 99007)]
    [DataRow("var items = new Point[1, 1] { { new Point { X = 7 } } }; foreach (var p in items) { items[0, 0].X = 99; return p.X; } return -1;", 7)]
    [DataRow("Amount a = new Amount { Value = 7 }; int value = a; return value;", 7)]
    [DataRow("Amount a = new Amount { Value = 7 }; int value = 0; value = a; return value;", 7)]
    [DataRow("Amount a = new Amount { Value = 7 }; return ReadInt(a);", 7)]
    [DataRow("return ReturnInt();", 7)]
    [DataRow("return ReturnIntExpression();", 7)]
    [DataRow("Amount a = new Amount { Value = 7 }; Pair value = a; return value.First * 100 + value.Second;", 708)]
    [DataRow("Amount a = new Amount { Value = 7 }; Pair value = default; value = a; return value.First * 100 + value.Second;", 708)]
    [DataRow("Amount a = new Amount { Value = 7 }; return ReadPair(a);", 708)]
    [DataRow("Pair value = ReturnPair(); return value.First * 100 + value.Second;", 708)]
    [DataRow("Pair value = ReturnPairExpression(); return value.First * 100 + value.Second;", 708)]
    [DataRow("var items = new Point[1, 1]; return items[0, 0].X;", 0)]
    [DataRow("var items = new Frame[2, 2]; items[0, 0].Origin.X = 7; return items[1, 1].Origin.X * 1000 + items[0, 0].Origin.X;", 7)]
    [DataRow("var items = new Point[1, 1, 2]; items[0, 0, 0].X = 7; return items[0, 0, 1].X * 1000 + items[0, 0, 0].X;", 7)]
    [DataRow("var items = new Point[1, 0]; int count = 0; foreach (var item in items) count++; return count;", 0)]
    [DataRow("Amount a = new Amount { Value = 7 }; int value = (a); return value;", 7)]
    [DataRow("Amount a = new Amount { Value = 7 }; int value = (int)a; return value;", 7)]
    [DataRow("Amount? a = null; Pair? value = a; return value == null ? 0 : 1;", 0)]
    [DataRow("Amount? a = new Amount { Value = 7 }; Pair? value = a; return ((Pair)value).Second;", 8)]
    [DataRow("Conversions = 0; Amount a = new Amount { Value = 7 }; Pair value = a; return Conversions * 1000 + value.Second;", 1008)]
    [DataRow("Amount a = new Amount { Value = 7 }; Pair value = (Pair)a; return value.First * 100 + value.Second;", 708)]
    [DataRow("Amount a = new Amount { Value = 7 }; object boxed = a; a.Value = 99; return ((Amount)boxed).Value;", 7)]
    [DataRow("MutatingAmount a = new MutatingAmount { Value = 7 }; int result = a; return a.Value * 1000 + result;", 7007)]
    [DataRow("MutatingAmount a = new MutatingAmount { Value = 7 }; int result = (int)a; return a.Value * 1000 + result;", 7007)]
    [DataRow("MutatingAmount? a = new MutatingAmount { Value = 7 }; int? result = a; return ((MutatingAmount)a).Value * 1000 + (int)result;", 7007)]
    public void StructCopiesRemainIndependentAcrossBoundaries(string body, int expected)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.Basic, CompilationOptions.OptimizationType.All })
        {
            var source = $$"""
                using Neo.SmartContract.Framework;
                using System.ComponentModel;
                public class Contract : SmartContract
                {
                    public struct Point { public int X; public void SetX(int value) { X = value; } }
                    public struct References { public int[] Values; }
                    public record struct Frame { public Point Origin; public int Scale; }
                    public struct Amount
                    {
                        public int Value;
                        public static implicit operator int(Amount value) => value.Value;
                        public static implicit operator Pair(Amount value) { Conversions++; return new Pair { First = value.Value, Second = value.Value + 1 }; }
                    }
                    public struct MutatingAmount
                    {
                        public int Value;
                        public static implicit operator int(MutatingAmount value) { int result = value.Value; value.Value = 99; return result; }
                    }
                    public struct Pair { public int First; public int Second; }
                    private static int ReadInt(int value) => value;
                    private static int ReadPair(Pair value) => value.First * 100 + value.Second;
                    private static int ReturnInt() { Amount a = new Amount { Value = 7 }; return a; }
                    private static int ReturnIntExpression() => new Amount { Value = 7 };
                    private static Pair ReturnPair() { Amount a = new Amount { Value = 7 }; return a; }
                    private static Pair ReturnPairExpression() => new Amount { Value = 7 };
                    private static int Conversions;
                    private static Point Shared;
                    private static object BoxShared() { return Shared; }
                    private static object BoxSharedExpression() => Shared;
                    private static object BoxSharedProperty => Shared;
                    private static object BoxSharedAccessor { get { return Shared; } }
                    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                    private static object BoxSharedInline() => Shared;
                    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                    private static object BoxSharedInlineBlock() { return Shared; }
                    private static object BoxSharedLambda() { System.Func<object> f = () => Shared; return f(); }
                    private static object BoxSharedBlockLambda() { System.Func<object> f = () => { return Shared; }; return f(); }
                    private static Point GetSharedPoint() => Shared;
                    private static int Change(params Point[] points) { points[0].X = 99; return points[0].X; }
                    [DisplayName("run")]
                    public static int Run() { {{body}} }
                }
                """;
            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(source, options);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            var engine = new TestEngine(true);
            var contract = engine.Deploy<BoundaryContract>(context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(expected), contract.Run(), body + " / " + optimization);
        }
    }

    public abstract class BoundaryContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")] public abstract BigInteger? Run();
    }
}
