// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_OperatorBoundaries
{
    [TestMethod]
    [DataRow("return (+new Number(7)).X;", 17)]
    [DataRow("return (-new Number(7)).X;", 27)]
    [DataRow("return (~new Number(7)).X;", 37)]
    [DataRow("return !new Number(0) ? 1 : 0;", 1)]
    [DataRow("Number? n = null; return (+n) is null ? 1 : 0;", 1)]
    [DataRow("Number? n = new Number(7); return ((Number)(-n)).X;", 27)]
    [DataRow("Number? a = null, b = new Number(2); var c = a + b; return c is null ? 1 : 0;", 1)]
    [DataRow("Number? a = new Number(3), b = new Number(4); return ((Number)(a + b)).X;", 7)]
    [DataRow("Number? a = null, b = null; return a == b ? 1 : 0;", 1)]
    [DataRow("Number? a = null, b = new Number(2); return a == b ? 1 : 0;", 0)]
    [DataRow("Number? a = null, b = new Number(2); return a < b ? 1 : 0;", 0)]
    [DataRow("Number? n = null; n++; return n is null ? 1 : 0;", 1)]
    [DataRow("Number n = new Number(0) && Right(); return Hits * 100 + n.X;", 0)]
    [DataRow("Number n = new Number(1) || Right(); return Hits * 100 + n.X;", 1)]
    [DataRow("Number n = new Number(1) && Right(); return Hits * 100 + n.X;", 103)]
    [DataRow("return Accept(7);", 57)]
    [DataRow("return Make().X;", 57)]
    [DataRow("Number[] values = new Number[] { 7 }; return values[0].X;", 57)]
    [DataRow("return new Holder().Value.X;", 57)]
    [DataRow("Number n = (7); return Conversions * 100 + n.X;", 157)]
    [DataRow("var holder = new Holder(); holder.Value = 8; return holder.Value.X;", 58)]
    public void OperatorBodiesAndNullableSemanticsArePreserved(string body, int expected)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.All })
        {
            var source = $$"""
                using Neo.SmartContract.Framework;
                using System.ComponentModel;
                #pragma warning disable CS0660, CS0661
                public class Contract : SmartContract
                {
                    public struct Number
                    {
                        public int X;
                        public Number(int x) { X = x; }
                        public static implicit operator Number(int x) { Conversions++; return new Number(x + 50); }
                        public static Number operator +(Number n) => new Number(n.X + 10);
                        public static Number operator -(Number n) => new Number(n.X + 20);
                        public static Number operator ~(Number n) => new Number(n.X + 30);
                        public static bool operator !(Number n) => n.X == 0;
                        public static Number operator ++(Number n) => new Number(n.X + 1);
                        public static Number operator +(Number a, Number b) => new Number(a.X + b.X);
                        public static bool operator ==(Number a, Number b) => a.X == b.X;
                        public static bool operator !=(Number a, Number b) => a.X != b.X;
                        public static bool operator <(Number a, Number b) => a.X < b.X;
                        public static bool operator >(Number a, Number b) => a.X > b.X;
                        public static bool operator true(Number n) => n.X > 0;
                        public static bool operator false(Number n) => n.X <= 0;
                        public static Number operator &(Number a, Number b) => new Number(a.X + b.X);
                        public static Number operator |(Number a, Number b) => new Number(a.X + b.X);
                    }
                    public class Holder { public Number Value = 7; }
                    private static int Conversions;
                    private static int Accept(Number value) => value.X;
                    private static Number Make() => 7;
                    private static int Hits;
                    private static Number Right() { Hits++; return new Number(2); }
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
