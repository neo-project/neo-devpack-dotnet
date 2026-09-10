// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_InstanceReceiverEvaluationOrder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the accompanying
// file LICENSE in the main directory of the repository for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_InstanceReceiverEvaluationOrder
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;

        public class Contract : SmartContract
        {
            private static int counter;
            private static string stringMarker;
            private static Box target;
            private static Func<int, int> delegateReceiver;

            public static int Run(int scenario)
            {
                counter = 1;
                var receiver = new Box(1);
                switch (scenario)
                {
                    case 0:
                        return receiver.Combine((receiver = new Box(2)).Value);
                    case 1:
                        return GetReceiver().Combine(counter);
                    case 2:
                        var holder = new Holder { Receiver = receiver };
                        return holder.Receiver.Combine((holder.Receiver = new Box(2)).Value);
                    case 3:
                        return GetReceiver().Pair(second: counter++, first: counter++);
                    case 4:
                        return GetReceiver().Pair(second: counter);
                    case 5:
                        return GetReceiver().Many(counter++, counter++);
                    case 6:
                        return GetReceiver().Many(new[] { counter++, counter++ });
                    case 7:
                        return GetReceiver().Many();
                    case 8:
                        int result = GetReceiver().Write(out counter);
                        return result * 10 + counter;
                    case 10:
                        return Receiver.Combine(counter);
                    case 11:
                        stringMarker = "b";
                        return GetStringReceiver().StartsWith(GetStringMarker()) ? 1 : 0;
                    case 12:
                        target = new Box(1);
                        return target.Read(new Token());
                    case 13:
                        target = new Box(1);
                        return target.Read(new());
                    case 14:
                        return GetDelegateReceiver().Invoke(counter);
                    case 15:
                        delegateReceiver = First;
                        return delegateReceiver.Invoke(ReplaceDelegate());
                    default:
                        return receiver.Combine(counter);
                }
            }

            private static Box Receiver => GetReceiver();

            private static Box GetReceiver()
            {
                counter = 2;
                return new Box(1);
            }

            private static string GetStringReceiver()
            {
                stringMarker = "a";
                return "abc";
            }

            private static string GetStringMarker() => stringMarker;

            private static Func<int, int> GetDelegateReceiver()
            {
                counter = 2;
                return First;
            }

            private static int First(int value) => 10 + value;
            private static int Second(int value) => 20 + value;

            private static int ReplaceDelegate()
            {
                delegateReceiver = Second;
                return 2;
            }

            private class Token
            {
                public Token() { target = new Box(2); }
            }

            private class Holder
            {
                public Box Receiver;
            }

            private class Box
            {
                public int Value;
                public Box(int value) { Value = value; }
                public int Combine(int argument) => Value * 10 + argument;
                public int Read(Token unused) => Value;
                public int Pair(int first = 9, int second = 8) => Value * 100 + first * 10 + second;
                public int Many(params int[] values)
                {
                    return Value * 100 + (values.Length > 0 ? values[0] * 10 : 0)
                        + (values.Length > 1 ? values[1] : 0);
                }
                public int Write(out int value)
                {
                    value = 3;
                    return Value;
                }
            }
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, 0, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 0, 12)]
    [DataRow(CompilationOptions.OptimizationType.None, 1, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 1, 12)]
    [DataRow(CompilationOptions.OptimizationType.None, 2, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 2, 12)]
    [DataRow(CompilationOptions.OptimizationType.None, 3, 132)]
    [DataRow(CompilationOptions.OptimizationType.All, 3, 132)]
    [DataRow(CompilationOptions.OptimizationType.None, 4, 192)]
    [DataRow(CompilationOptions.OptimizationType.All, 4, 192)]
    [DataRow(CompilationOptions.OptimizationType.None, 5, 123)]
    [DataRow(CompilationOptions.OptimizationType.All, 5, 123)]
    [DataRow(CompilationOptions.OptimizationType.None, 6, 123)]
    [DataRow(CompilationOptions.OptimizationType.All, 6, 123)]
    [DataRow(CompilationOptions.OptimizationType.None, 7, 100)]
    [DataRow(CompilationOptions.OptimizationType.All, 7, 100)]
    [DataRow(CompilationOptions.OptimizationType.None, 8, 13)]
    [DataRow(CompilationOptions.OptimizationType.All, 8, 13)]
    [DataRow(CompilationOptions.OptimizationType.None, 9, 11)]
    [DataRow(CompilationOptions.OptimizationType.All, 9, 11)]
    [DataRow(CompilationOptions.OptimizationType.None, 10, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 10, 12)]
    [DataRow(CompilationOptions.OptimizationType.None, 11, 1)]
    [DataRow(CompilationOptions.OptimizationType.All, 11, 1)]
    [DataRow(CompilationOptions.OptimizationType.None, 12, 1)]
    [DataRow(CompilationOptions.OptimizationType.All, 12, 1)]
    [DataRow(CompilationOptions.OptimizationType.None, 13, 1)]
    [DataRow(CompilationOptions.OptimizationType.All, 13, 1)]
    [DataRow(CompilationOptions.OptimizationType.None, 14, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 14, 12)]
    [DataRow(CompilationOptions.OptimizationType.None, 15, 12)]
    [DataRow(CompilationOptions.OptimizationType.All, 15, 12)]
    public void ReceiverIsEvaluatedBeforeArgumentValues(
        CompilationOptions.OptimizationType optimization, int scenario, int expected)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(System.Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<ReceiverContract>(nef, manifest);

        Assert.AreEqual(new BigInteger(expected), contract.Run(scenario));
    }

    [TestMethod]
    public void SequentialSpecialCallsReuseReceiverTemporarySlots()
    {
        string calls = string.Join(Environment.NewLine,
            Enumerable.Repeat("GetStringReceiver().StartsWith(GetStringMarker());", 256));
        string source = $$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                private static string marker;

                public static int Run()
                {
                    marker = "b";
                    {{calls}}
                    return 1;
                }

                private static string GetStringReceiver()
                {
                    marker = "a";
                    return "abc";
                }

                private static string GetStringMarker() => marker;
            }
            """;

        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = CompilationOptions.OptimizationType.All;
        var context = TestHelper.CompileSingleContract(source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
    }

    public abstract class ReceiverContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")]
        public abstract BigInteger? Run(BigInteger? scenario);
    }
}
