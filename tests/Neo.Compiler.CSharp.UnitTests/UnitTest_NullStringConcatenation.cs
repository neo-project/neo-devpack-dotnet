// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullStringConcatenation.cs file belongs to the neo project and is free
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
public class UnitTest_NullStringConcatenation
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using Neo.SmartContract.Framework.Services;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            private static string? GetNull() => null;

            [DisplayName("right")]
            public static string Right(string? value) => "a" + value;

            [DisplayName("left")]
            public static string Left(string? value) => value + "b";

            [DisplayName("both")]
            public static string Both(string? x, string? y) => x + y;

            [DisplayName("compound")]
            public static string Compound(string? value)
            {
                string s = "a";
                s += value;
                return s;
            }

            [DisplayName("compoundOnNull")]
            public static string CompoundOnNull()
            {
                string? s = GetNull();
                s += "b";
                return s;
            }

            [DisplayName("fromMethod")]
            public static bool FromMethod() => "x" + GetNull() == "x";

            [DisplayName("describe")]
            public static string Describe(ByteString key)
            {
                string? label = Storage.Get(key);
                return "label: " + label;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullStringConcatenatesAsEmpty(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<ConcatContract>(
            context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("av", contract.Right("v"));
        Assert.AreEqual("vb", contract.Left("v"));
        Assert.AreEqual("av", contract.Compound("v"));
        Assert.AreEqual("a", contract.Right(null));
        Assert.AreEqual("b", contract.Left(null));
        Assert.AreEqual("", contract.Both(null, null));
        Assert.AreEqual("a", contract.Compound(null));
        Assert.AreEqual("b", contract.CompoundOnNull());
        Assert.IsTrue(contract.FromMethod());
        Assert.AreEqual("label: ", contract.Describe(new byte[] { 0x01 }));
    }

    public abstract class ConcatContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("right")]
        public abstract string? Right(string? value);

        [DisplayName("left")]
        public abstract string? Left(string? value);

        [DisplayName("both")]
        public abstract string? Both(string? x, string? y);

        [DisplayName("compound")]
        public abstract string? Compound(string? value);

        [DisplayName("compoundOnNull")]
        public abstract string? CompoundOnNull();

        [DisplayName("fromMethod")]
        public abstract bool FromMethod();

        [DisplayName("describe")]
        public abstract string? Describe(byte[] key);
    }
}
