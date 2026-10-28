// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_TupleMutation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_TupleMutation
{
    [TestMethod]
    [DataRow("var t = (amount, 2, 3, 4); t.Item2 += 5;", 7734)]
    [DataRow("var t = (first: amount, 2, 3, 4); t.Item2 *= 5;", 8034)]
    [DataRow("var t = (amount + 0, 2, 3, 4); t.Item2 += 5;", 7734)]
    [DataRow("var t = (amount, 2, 3, 4); ++t.Item3;", 7244)]
    [DataRow("var t = (amount, 2, 3, 4); t.Item3++;", 7244)]
    [DataRow("var t = (amount, 2, 3, 4); --t.Item4;", 7233)]
    [DataRow("var t = (amount, 2, 3, 4); t.Item4--;", 7233)]
    [DataRow("var t = (first: amount, second: 2, third: 3, fourth: 4); t.second += 5;", 7734)]
    [DataRow("var t = (first: amount, second: 2, third: 3, fourth: 4); t.Item2 += 5;", 7734)]
    public void MutationsPreserveElementPositions(string body, int expected)
    {
        Verify(body + " return Check(t);", expected);
    }

    [TestMethod]
    [DataRow("var t = (amount, 2); t.Item2 += 5; return t.Item2;", 7)]
    [DataRow("var t = (amount, 2, 3); t.Item3++; return t.Item3;", 4)]
    [DataRow("var t = (amount, 2, 3, 4); var old = t.Item2++; return old * 10000 + Check(t);", 27334)]
    [DataRow("var t = (amount, 2, 3, 4); var value = ++t.Item2; return value * 10000 + Check(t);", 37334)]
    [DataRow("var t = (amount, 2, 3, 4); Add(ref t.Item2); return Check(t);", 7734)]
    [DataRow("var t = (amount, (string)null, 3); t.Item2 ??= \"ok\"; return t.Item2 == \"ok\" ? t.Item3 : -1;", 3)]
    public void MutationBoundariesUseTupleLayout(string body, int expected) => Verify(body, expected);

    private static void Verify(string body, int expected)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.Basic, CompilationOptions.OptimizationType.All })
        {
            var source = $$"""
                using Neo.SmartContract.Framework;
                using System.ComponentModel;
                public class Contract : SmartContract
                {
                    private static int Check((int, int, int, int) t) => t.Item1 * 1000 + t.Item2 * 100 + t.Item3 * 10 + t.Item4;
                    private static void Add(ref int value) { value += 5; }
                    [DisplayName("run")]
                    public static int Run(int amount) { {{body}} }
                }
                """;
            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(source, options);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            var engine = new TestEngine(true);
            var contract = engine.Deploy<TupleContract>(context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(expected), contract.Run(7), body + " / " + optimization);
        }
    }

    public abstract class TupleContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")] public abstract BigInteger? Run(BigInteger? amount);
    }
}
