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

    private const string ConditionalMethodSource = """
using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSegment(this string receiver, string segment) => receiver.Contains(segment);
}

public class Contract : SmartContract
{
    [Flags]
    public enum SampleFlags { None = 0, First = 1, Second = 2 }

    [DisplayName("contains")]
    public static bool Contains(string? value) => value?.Contains("neo") == true;

    [DisplayName("startsWith")]
    public static bool StartsWith(string? value, string prefix) => value?.StartsWith(prefix) == true;

    [DisplayName("endsWith")]
    public static bool EndsWith(string? value, string suffix) => value?.EndsWith(suffix) == true;

    [DisplayName("indexOf")]
    public static int IndexOf(string? value) => value?.IndexOf("neo") ?? -1;

    [DisplayName("lastIndexOf")]
    public static int LastIndexOf(string? value) => value?.LastIndexOf(":") ?? -1;

    [DisplayName("replace")]
    public static string? Replace(string? value) => value?.Replace("a", "X");

    [DisplayName("insert")]
    public static string? Insert(string? value) => value?.Insert(1, "X");

    [DisplayName("substring")]
    public static string? Substring(string? value) => value?.Substring(1, 2);

    [DisplayName("toLower")]
    public static string? ToLower(string? value) => value?.ToLower();

    [DisplayName("nestedContains")]
    public static bool NestedContains(string? value) => value?.ToLower()?.Contains("neo") == true;

    [DisplayName("hasFlag")]
    public static bool HasFlag(bool present, int mask)
    {
        SampleFlags? flags = present ? (SampleFlags)mask : null;
        return flags?.HasFlag(SampleFlags.Second) == true;
    }

    [DisplayName("append")]
    public static string Append(bool present, string suffix)
    {
        StringBuilder? builder = present ? new StringBuilder("neo") : null;
        builder?.Append(suffix);
        return builder is null ? "" : builder.ToString();
    }

    [DisplayName("inlineExtension")]
    public static bool InlineExtension(string? value) => value?.HasSegment("neo") == true;
}
""";

    [TestMethod]
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

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ConditionalInstanceMethodsPreserveReceiverAndArguments(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(ConditionalMethodSource, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ConditionalMethodContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsFalse(contract.Contains(null));
        Assert.IsFalse(contract.Contains("a"));
        Assert.IsTrue(contract.Contains("super_neo"));

        Assert.IsFalse(contract.StartsWith(null, "neo"));
        Assert.IsFalse(contract.StartsWith("", "neo"));
        Assert.IsTrue(contract.StartsWith("neo_vm", "neo"));
        Assert.IsFalse(contract.EndsWith(null, ".neo"));
        Assert.IsFalse(contract.EndsWith("", ".neo"));
        Assert.IsTrue(contract.EndsWith("wallet.neo", ".neo"));

        Assert.AreEqual(new BigInteger(-1), contract.IndexOf(null));
        Assert.AreEqual(new BigInteger(6), contract.IndexOf("super_neo"));
        Assert.AreEqual(new BigInteger(-1), contract.LastIndexOf(null));
        Assert.AreEqual(new BigInteger(3), contract.LastIndexOf("a:b:c"));
        Assert.IsNull(contract.Replace(null));
        Assert.AreEqual("XbcXbc", contract.Replace("abcabc"));
        Assert.IsNull(contract.Insert(null));
        Assert.AreEqual("aXbc", contract.Insert("abc"));
        Assert.IsNull(contract.Substring(null));
        Assert.AreEqual("bc", contract.Substring("abcd"));
        Assert.IsNull(contract.ToLower(null));
        Assert.AreEqual("neo", contract.ToLower("NEO"));
        Assert.IsFalse(contract.NestedContains(null));
        Assert.IsTrue(contract.NestedContains("SUPER_NEO"));

        Assert.IsFalse(contract.HasFlag(false, 3));
        Assert.IsFalse(contract.HasFlag(true, 1));
        Assert.IsTrue(contract.HasFlag(true, 3));
        Assert.AreEqual("", contract.Append(false, "-n3"));
        Assert.AreEqual("neo-n3", contract.Append(true, "-n3"));
        Assert.IsFalse(contract.InlineExtension(null));
        Assert.IsFalse(contract.InlineExtension("a"));
        Assert.IsTrue(contract.InlineExtension("super_neo"));
    }

    public abstract class NullableConditionalAccessContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("nestedValue")] public abstract BigInteger? NestedValue(bool hasRoot, bool hasChild);
        [DisplayName("nestedValueOrDefault")] public abstract BigInteger? NestedValueOrDefault(bool hasRoot, bool hasChild);
        [DisplayName("fallbackEvaluation")] public abstract BigInteger? FallbackEvaluation(bool hasRoot, bool hasChild);
    }

    public abstract class ConditionalMethodContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("contains")] public abstract bool Contains(string? value);
        [DisplayName("startsWith")] public abstract bool StartsWith(string? value, string prefix);
        [DisplayName("endsWith")] public abstract bool EndsWith(string? value, string suffix);
        [DisplayName("indexOf")] public abstract BigInteger? IndexOf(string? value);
        [DisplayName("lastIndexOf")] public abstract BigInteger? LastIndexOf(string? value);
        [DisplayName("replace")] public abstract string? Replace(string? value);
        [DisplayName("insert")] public abstract string? Insert(string? value);
        [DisplayName("substring")] public abstract string? Substring(string? value);
        [DisplayName("toLower")] public abstract string? ToLower(string? value);
        [DisplayName("nestedContains")] public abstract bool NestedContains(string? value);
        [DisplayName("hasFlag")] public abstract bool HasFlag(bool present, int mask);
        [DisplayName("append")] public abstract string Append(bool present, string suffix);
        [DisplayName("inlineExtension")] public abstract bool InlineExtension(string? value);
    }
}
