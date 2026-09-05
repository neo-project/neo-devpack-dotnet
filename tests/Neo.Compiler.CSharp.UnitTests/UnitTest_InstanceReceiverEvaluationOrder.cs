// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_InstanceReceiverEvaluationOrder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the accompanying
// file LICENSE in the main directory of the repository for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_InstanceReceiverEvaluationOrder
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        public class Contract : SmartContract
        {
            private static int counter;

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

            private class Holder
            {
                public Box Receiver;
            }

            private class Box
            {
                public int Value;
                public Box(int value) { Value = value; }
                public int Combine(int argument) => Value * 10 + argument;
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

    public abstract class ReceiverContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")]
        public abstract BigInteger? Run(BigInteger? scenario);
    }
}
