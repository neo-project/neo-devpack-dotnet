// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableOperatorMatrix.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableOperatorMatrix
{
    private const string Source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : Neo.SmartContract.Framework.SmartContract
{
    [DisplayName("and")]
    public static bool? And(bool? left, bool? right) => left & right;

    [DisplayName("or")]
    public static bool? Or(bool? left, bool? right) => left | right;

    [DisplayName("coalesce")]
    public static int Coalesce(int? value, int fallback) => value ?? fallback;

    [DisplayName("nestedCoalesce")]
    public static int NestedCoalesce(int? first, int? second, int fallback) => first ?? second ?? fallback;
}
""";

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void LiftedBooleanOperatorsMatchTheCSharpTruthTable(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        var values = new bool?[] { null, false, true };
        var expectedAnd = new bool?[,]
        {
            { null, false, null },
            { false, false, false },
            { null, false, true }
        };
        var expectedOr = new bool?[,]
        {
            { null, null, true },
            { null, false, true },
            { true, true, true }
        };

        for (var left = 0; left < values.Length; left++)
        {
            for (var right = 0; right < values.Length; right++)
            {
                Assert.AreEqual(
                    expectedAnd[left, right],
                    contract.And(values[left], values[right]),
                    $"AND ({values[left]}, {values[right]})");
                Assert.AreEqual(
                    expectedOr[left, right],
                    contract.Or(values[left], values[right]),
                    $"OR ({values[left]}, {values[right]})");
            }
        }
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullCoalescingSelectsTheFirstNonNullValue(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);

        Assert.AreEqual(7, contract.Coalesce(null, 7));
        Assert.AreEqual(3, contract.Coalesce(3, 7));
        Assert.AreEqual(9, contract.NestedCoalesce(null, null, 9));
        Assert.AreEqual(5, contract.NestedCoalesce(null, 5, 9));
        Assert.AreEqual(2, contract.NestedCoalesce(2, 5, 9));
    }

    private static NullableOperatorContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var context = TestHelper.CompileSingleContract(Source, new CompilationOptions { Optimize = optimization });
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        return new TestEngine(true).Deploy<NullableOperatorContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class NullableOperatorContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("and")]
        public abstract bool? And(bool? left, bool? right);

        [DisplayName("or")]
        public abstract bool? Or(bool? left, bool? right);

        [DisplayName("coalesce")]
        public abstract int? Coalesce(int? value, int fallback);

        [DisplayName("nestedCoalesce")]
        public abstract int? NestedCoalesce(int? first, int? second, int fallback);
    }
}
