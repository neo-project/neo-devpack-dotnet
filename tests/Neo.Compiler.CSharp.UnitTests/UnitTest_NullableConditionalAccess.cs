// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableConditionalAccess.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableConditionalAccess
{
    private const string Source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    private sealed class Node
    {
        public Node? Child { get; set; }
        public int Value { get; set; }
    }

    private static int _fallbackCalls;

    [DisplayName("nestedValue")]
    public static int? NestedValue(bool hasRoot, bool hasChild)
    {
        Node? root = hasRoot ? new Node { Child = hasChild ? new Node { Value = 42 } : null } : null;
        return root?.Child?.Value;
    }

    [DisplayName("nestedValueOrDefault")]
    public static int NestedValueOrDefault(bool hasRoot, bool hasChild)
    {
        Node? root = hasRoot ? new Node { Child = hasChild ? new Node { Value = 42 } : null } : null;
        return root?.Child?.Value ?? Fallback();
    }

    [DisplayName("fallbackEvaluation")]
    public static int FallbackEvaluation(bool hasRoot, bool hasChild)
    {
        _fallbackCalls = 0;
        Node? root = hasRoot ? new Node { Child = hasChild ? new Node { Value = 42 } : null } : null;
        _ = root?.Child?.Value ?? Fallback();
        return _fallbackCalls;
    }

    private static int Fallback()
    {
        _fallbackCalls++;
        return 99;
    }
}
""";

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ConditionalAccessAndCoalescingMatchCSharp(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<NullableConditionalAccessContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsNull(contract.NestedValue(false, false));
        Assert.IsNull(contract.NestedValue(true, false));
        Assert.AreEqual(new BigInteger(42), contract.NestedValue(true, true));

        Assert.AreEqual(new BigInteger(99), contract.NestedValueOrDefault(false, false));
        Assert.AreEqual(new BigInteger(99), contract.NestedValueOrDefault(true, false));
        Assert.AreEqual(new BigInteger(42), contract.NestedValueOrDefault(true, true));

        Assert.AreEqual(BigInteger.One, contract.FallbackEvaluation(false, false));
        Assert.AreEqual(BigInteger.One, contract.FallbackEvaluation(true, false));
        Assert.AreEqual(BigInteger.Zero, contract.FallbackEvaluation(true, true));

    }

    public abstract class NullableConditionalAccessContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("nestedValue")] public abstract BigInteger? NestedValue(bool hasRoot, bool hasChild);
        [DisplayName("nestedValueOrDefault")] public abstract BigInteger? NestedValueOrDefault(bool hasRoot, bool hasChild);
        [DisplayName("fallbackEvaluation")] public abstract BigInteger? FallbackEvaluation(bool hasRoot, bool hasChild);
    }
}
