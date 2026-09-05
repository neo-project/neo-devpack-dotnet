// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ByRefArgumentEvaluationOrder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the accompanying
// file LICENSE in the main directory of the repository for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ByRefArgumentEvaluationOrder
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        public class Contract : SmartContract
        {
            private static int counter;
            private static int field;

            public static int Run()
            {
                int value = 0;
                counter = 0;
                field = 0;
                var holder = new Holder();
                // SCENARIO
            }

            private static int Next() => ++counter;
            private static int Forward(int value) => Combine(value++, ref value, value++);
            private static Holder NextHolder()
            {
                counter++;
                return new Holder();
            }
            private static int Combine(int first, ref int value, int last)
                => first * 100 + value * 10 + last;
            private static int Update(int first, ref int value, int last)
            {
                value = 4;
                return first * 10 + last;
            }
            private static int WithParams(int first, ref int value, params int[] remaining)
            {
                value = 4;
                return first * 100 + (remaining.Length > 0 ? remaining[0] * 10 : 0)
                    + (remaining.Length > 1 ? remaining[1] : 0);
            }
            private static int WithOptional(ref int value, int first = 9, int last = 8)
            {
                return first * 100 + value * 10 + last;
            }
            private static int WithOut(int first, out int value, int last)
            {
                value = 4;
                return first * 100 + last;
            }
            private class Holder { public int Value; }
        }
        """;

    private static readonly string[] Scenarios =
    [
        "return Combine(value++, ref value, value++);",
        "return Combine(last: value++, value: ref value, first: value++);",
        "return Combine(field++, ref field, field++);",
        "return Combine(holder.Value++, ref holder.Value, holder.Value++);",
        "var original = holder; int result = Update(Next(), ref holder.Value, (holder = new Holder { Value = 9 }).Value); return result * 100 + original.Value * 10 + holder.Value;",
        "return WithParams(Next(), ref value, Next(), Next()) * 10 + value;",
        "return WithParams(Next(), ref value, new[] { Next(), Next() }) * 10 + value;",
        "return WithOptional(value: ref value, last: Next());",
        "value = 7; return WithOut(Next(), out value, value) * 10 + value;",
        "return WithOut(Next(), out int produced, Next()) * 10 + produced;",
        "return WithOut(Next(), out _, Next());",
        "int output = WithOut(Next(), out holder.Value, Next()); return output * 10 + holder.Value;",
        "return WithParams(Next(), ref value) * 10 + value;",
        "return Forward(0);",
        "return Update(Next(), ref NextHolder().Value, Next()) * 10 + counter;",
        "return WithParams(remaining: new[] { Next(), Next() }, value: ref value, first: Next()) * 10 + value;",
        "int result = WithOut(Next(), out field, Next()); return result * 10 + field;"
    ];

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, 0, 21)]
    [DataRow(CompilationOptions.OptimizationType.All, 0, 21)]
    [DataRow(CompilationOptions.OptimizationType.None, 1, 120)]
    [DataRow(CompilationOptions.OptimizationType.All, 1, 120)]
    [DataRow(CompilationOptions.OptimizationType.None, 2, 21)]
    [DataRow(CompilationOptions.OptimizationType.All, 2, 21)]
    [DataRow(CompilationOptions.OptimizationType.None, 3, 21)]
    [DataRow(CompilationOptions.OptimizationType.All, 3, 21)]
    [DataRow(CompilationOptions.OptimizationType.None, 4, 1949)]
    [DataRow(CompilationOptions.OptimizationType.All, 4, 1949)]
    [DataRow(CompilationOptions.OptimizationType.None, 5, 1234)]
    [DataRow(CompilationOptions.OptimizationType.All, 5, 1234)]
    [DataRow(CompilationOptions.OptimizationType.None, 6, 1234)]
    [DataRow(CompilationOptions.OptimizationType.All, 6, 1234)]
    [DataRow(CompilationOptions.OptimizationType.None, 7, 901)]
    [DataRow(CompilationOptions.OptimizationType.All, 7, 901)]
    [DataRow(CompilationOptions.OptimizationType.None, 8, 1074)]
    [DataRow(CompilationOptions.OptimizationType.All, 8, 1074)]
    [DataRow(CompilationOptions.OptimizationType.None, 9, 1024)]
    [DataRow(CompilationOptions.OptimizationType.All, 9, 1024)]
    [DataRow(CompilationOptions.OptimizationType.None, 10, 102)]
    [DataRow(CompilationOptions.OptimizationType.All, 10, 102)]
    [DataRow(CompilationOptions.OptimizationType.None, 11, 1024)]
    [DataRow(CompilationOptions.OptimizationType.All, 11, 1024)]
    [DataRow(CompilationOptions.OptimizationType.None, 12, 1004)]
    [DataRow(CompilationOptions.OptimizationType.All, 12, 1004)]
    [DataRow(CompilationOptions.OptimizationType.None, 13, 21)]
    [DataRow(CompilationOptions.OptimizationType.All, 13, 21)]
    [DataRow(CompilationOptions.OptimizationType.None, 14, 133)]
    [DataRow(CompilationOptions.OptimizationType.All, 14, 133)]
    [DataRow(CompilationOptions.OptimizationType.None, 15, 3124)]
    [DataRow(CompilationOptions.OptimizationType.All, 15, 3124)]
    [DataRow(CompilationOptions.OptimizationType.None, 16, 1024)]
    [DataRow(CompilationOptions.OptimizationType.All, 16, 1024)]
    public void ValueArgumentsAndByRefTargetsFollowSourceOrder(
        CompilationOptions.OptimizationType optimization, int scenario, int expected)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source.Replace("// SCENARIO", Scenarios[scenario]), options);
        Assert.IsTrue(context.Success, string.Join(System.Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<ByRefContract>(nef, manifest);

        Assert.AreEqual(new BigInteger(expected), contract.Run());
    }

    public abstract class ByRefContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")]
        public abstract BigInteger? Run();
    }
}
