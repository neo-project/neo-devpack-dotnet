// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ByRefArgumentEvaluationOrder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the accompanying
// file LICENSE in the main directory of the repository for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Linq;
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
            private static Box tail;

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
            private static void Set(ref int value) => value = 4;
            private static int SetAndReturn(ref int value, int ignored)
            {
                value = 7;
                return 0;
            }
            private static int Walk(Box box, int depth)
            {
                if (depth == 0) return 0;
                return SetAndReturn(ref box.Value, Walk(tail, depth - 1));
            }

            private class Box
            {
                public int Value;
            }
            private class Holder
            {
                public int Value;

                public int Run()
                {
                    Value = 7;
                    return Update(Next(), ref Value, Next());
                }

                public int SetSelf()
                {
                    Set(ref Value);
                    return Value;
                }
            }
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
        "int result = WithOut(Next(), out field, Next()); return result * 10 + field;",
        "return holder.Run();",
        "return Update(1, ref holder.Value, 2) * 10 + holder.Value;",
        "Set(ref holder.Value); return holder.Value;",
        "return holder.SetSelf();",
        "var first = new Box { Value = 1 }; tail = new Box { Value = 2 }; Walk(first, 2); return first.Value * 10 + tail.Value;"
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
    [DataRow(CompilationOptions.OptimizationType.None, 17, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 17, 12)]
    [DataRow(CompilationOptions.OptimizationType.None, 18, 124)]
    [DataRow(CompilationOptions.OptimizationType.All, 18, 124)]
    [DataRow(CompilationOptions.OptimizationType.None, 19, 4)]
    [DataRow(CompilationOptions.OptimizationType.All, 19, 4)]
    [DataRow(CompilationOptions.OptimizationType.None, 20, 4)]
    [DataRow(CompilationOptions.OptimizationType.All, 20, 4)]
    [DataRow(CompilationOptions.OptimizationType.None, 21, 77)]
    [DataRow(CompilationOptions.OptimizationType.All, 21, 77)]
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

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.Basic, "ref")]
    [DataRow(CompilationOptions.OptimizationType.All, "ref")]
    [DataRow(CompilationOptions.OptimizationType.Basic, "out")]
    [DataRow(CompilationOptions.OptimizationType.All, "out")]
    public void SequentialParamsCallsReuseCapturedReceiverSlots(
        CompilationOptions.OptimizationType optimization, string refKind)
    {
        const int callCount = 300;
        var calls = string.Join(System.Environment.NewLine,
            Enumerable.Repeat($"Set({refKind} box.Value, box.Value, 0);", callCount));
        var source = $$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Run()
                {
                    var box = new Box();
                    {{calls}}
                    return box.Value;
                }

                private static void Set({{refKind}} int value, params int[] values)
                    => value = values[0] + 1;

                private class Box
                {
                    public int Value;
                }
            }
            """;

        AssertExecution(source, optimization, callCount);
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NestedParamsCallsKeepCapturedReceiversUntilWriteback(
        CompilationOptions.OptimizationType optimization)
    {
        const string source = """
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Run()
                {
                    var first = new Box { Value = 1 };
                    var second = new Box { Value = 2 };
                    var original = first;
                    Set(ref first.Value,
                        Set(ref second.Value, (first = new Box { Value = 3 }).Value),
                        second.Value);
                    return original.Value * 100 + second.Value * 10 + first.Value;
                }

                private static int Set(ref int value, params int[] values)
                {
                    value += values[0];
                    if (values.Length > 1) value += values[1];
                    return value;
                }

                private class Box
                {
                    public int Value;
                }
            }
            """;

        AssertExecution(source, optimization, 1153);
    }

    private static void AssertExecution(string source, CompilationOptions.OptimizationType optimization, int expected)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(source, options);
        Assert.IsTrue(context.Success, string.Join(System.Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true) { Fee = 1000_00000000 };
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
