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
    [DataRow("var p = new Point { X = 7 }; Point[] items = [p]; p.X = 99; return items[0].X * 1000 + p.X;", 7099)]
    [DataRow("var items = new Point[1]; return items[0].X;", 0)]
    [DataRow("var items = new Point[2]; items[0].X = 7; return items[1].X * 1000 + items[0].X;", 7)]
    [DataRow("var p = new Point { X = 7 }; int changed = Change(p); return changed * 1000 + p.X;", 99007)]
    [DataRow("var items = new Point[1, 1] { { new Point { X = 7 } } }; foreach (var p in items) { items[0, 0].X = 99; return p.X; } return -1;", 7)]
    public void StructCopiesRemainIndependentAcrossBoundaries(string body, int expected)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.All })
        {
            var source = $$"""
                using Neo.SmartContract.Framework;
                using System.ComponentModel;
                public class Contract : SmartContract
                {
                    public struct Point { public int X; }
                    public struct References { public int[] Values; }
                    public record struct Frame { public Point Origin; public int Scale; }
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
