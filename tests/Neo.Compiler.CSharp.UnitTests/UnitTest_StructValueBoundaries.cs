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
