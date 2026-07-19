// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringBuilderNullAppend.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringBuilderNullAppend
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;
        using System.Text;

        public class Contract : SmartContract
        {
            private static int _evaluationCount;

            private static StringBuilder? GetNullBuilder()
            {
                _evaluationCount++;
                return null;
            }

            [DisplayName("appendNull")]
            public static string AppendNull()
            {
                var builder = new StringBuilder("neo");
                StringBuilder? value = null;
                var returned = builder.Append(value);
                returned.Append("-vm");
                return builder.ToString();
            }

            [DisplayName("evaluateOnce")]
            public static int EvaluateOnce()
            {
                _evaluationCount = 0;
                var builder = new StringBuilder("neo");
                builder.Append(GetNullBuilder());
                return _evaluationCount;
            }

            [DisplayName("appendValues")]
            public static string AppendValues()
            {
                var builder = new StringBuilder("neo");
                builder.Append(new StringBuilder());
                builder.Append(new StringBuilder("-vm"));
                return builder.ToString();
            }
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void AppendStringBuilder_HandlesNullWithoutChangingOtherValues(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics.Select(static p => p.ToString()))}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<StringBuilderNullContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("neo-vm", contract.AppendNull(), optimization.ToString());
        Assert.AreEqual(new BigInteger(1), contract.EvaluateOnce(), optimization.ToString());
        Assert.AreEqual("neo-vm", contract.AppendValues(), optimization.ToString());
    }

    public abstract class StringBuilderNullContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("appendNull")]
        public abstract string? AppendNull();

        [DisplayName("evaluateOnce")]
        public abstract BigInteger? EvaluateOnce();

        [DisplayName("appendValues")]
        public abstract string? AppendValues();
    }
}
