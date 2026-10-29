// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableConditionalBoundaries.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableConditionalBoundaries
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("boolCoalesce")]
            public static int BoolCoalesce(bool? value) => (value ?? false) ? 11 : 22;

            [DisplayName("conditionalValue")]
            public static int ConditionalValue(bool choose, int? value) => choose ? value ?? 7 : 3;

            [DisplayName("nullableConditional")]
            public static int? NullableConditional(bool choose, int? value) => choose ? value : null;

            [DisplayName("nestedConditional")]
            public static int NestedConditional(bool? first, int? value) =>
                (first ?? true) ? value ?? 8 : 4;
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullableConditionalExpressionsMatchCSharp(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);

        Assert.AreEqual(11, contract.BoolCoalesce(true));
        Assert.AreEqual(22, contract.BoolCoalesce(false));
        Assert.AreEqual(22, contract.BoolCoalesce(null));

        Assert.AreEqual(-1, contract.ConditionalValue(true, -1));
        Assert.AreEqual(7, contract.ConditionalValue(true, null));
        Assert.AreEqual(3, contract.ConditionalValue(false, 99));

        Assert.AreEqual(new BigInteger(5), contract.NullableConditional(true, 5));
        Assert.IsNull(contract.NullableConditional(true, null));
        Assert.IsNull(contract.NullableConditional(false, 5));

        Assert.AreEqual(-2, contract.NestedConditional(true, -2));
        Assert.AreEqual(8, contract.NestedConditional(null, null));
        Assert.AreEqual(4, contract.NestedConditional(false, 9));
    }

    private static NullableConditionalContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");
        return new TestEngine(true).Deploy<NullableConditionalContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class NullableConditionalContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("boolCoalesce")] public abstract int? BoolCoalesce(bool? value);
        [DisplayName("conditionalValue")] public abstract int? ConditionalValue(bool choose, int? value);
        [DisplayName("nullableConditional")] public abstract BigInteger? NullableConditional(bool choose, BigInteger? value);
        [DisplayName("nestedConditional")] public abstract int? NestedConditional(bool? first, int? value);
    }
}
