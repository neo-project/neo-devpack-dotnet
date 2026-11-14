// Copyright (C) 2015-2026 The Neo Project.
//
// UInt160ListTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests.Services;

[TestClass]
public class UInt160ListTest
{
    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static UInt160ListProxy Deploy()
        => new TestEngine(true).Deploy<UInt160ListProxy>(_compiled.nef, _compiled.manifest);

    private static void Merge(UInt160ListProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static UInt160 Item(int i)
    {
        var bytes = new byte[20];
        bytes[0] = 0x42;
        BitConverter.GetBytes(i).CopyTo(bytes, 1);
        return new UInt160(bytes);
    }

    private static byte[] IntBytes(BigInteger i) => i.IsZero ? [] : i.ToByteArray();

    private static void AssertStorage(UInt160ListProxy c, List<UInt160> expected)
    {
        Assert.AreEqual(expected.Count, (int)c.Length()!.Value);
        var countKey = new byte[] { 0xE1, 0x00 };
        if (expected.Count == 0)
            Assert.IsFalse(c.Storage.Contains(countKey), "count key must be deleted when empty");
        else
            Assert.AreEqual(expected.Count, c.Storage.GetInteger(countKey));

        for (var i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i], c.At(i));
            Assert.IsTrue(c.Storage.TryGet(new byte[] { 0xE1, 0x01 }.Concat(IntBytes(i)).ToArray(), out var stored));
            CollectionAssert.AreEqual(expected[i].GetSpan().ToArray(), stored.ToArray());
        }
        Assert.IsFalse(c.Storage.Contains(new byte[] { 0xE1, 0x01 }.Concat(IntBytes(expected.Count)).ToArray()));
    }

    [TestMethod]
    public void Empty_List()
    {
        var c = Deploy();
        Assert.AreEqual(0, c.Length());
        Assert.ThrowsExactly<TestException>(() => c.At(0));
        Assert.ThrowsExactly<TestException>(() => c.RemoveAt(0));
        Assert.AreEqual(0, c.Values()!.Count);
        AssertStorage(c, []);
        Merge(c);
    }

    [TestMethod]
    public void Add_Allows_Duplicates_And_Keeps_Order()
    {
        var c = Deploy();
        var expected = new List<UInt160>();
        foreach (var i in new[] { 1, 2, 2, 3, 1 })
        {
            c.Add(Item(i));
            expected.Add(Item(i));
        }
        AssertStorage(c, expected);
        Assert.AreEqual(5, c.Values()!.Count);
        Merge(c);
    }

    [TestMethod]
    public void RemoveAt_First_Middle_Last_Uses_SwapAndPop()
    {
        var c = Deploy();
        var expected = new List<UInt160>();
        for (var i = 0; i < 6; i++)
        {
            c.Add(Item(i));
            expected.Add(Item(i));
        }

        Assert.AreEqual(Item(0), c.RemoveAt(0));
        expected[0] = expected[^1];
        expected.RemoveAt(expected.Count - 1);
        AssertStorage(c, expected);

        Assert.AreEqual(expected[2], c.RemoveAt(2));
        expected[2] = expected[^1];
        expected.RemoveAt(expected.Count - 1);
        AssertStorage(c, expected);

        Assert.AreEqual(expected[^1], c.RemoveAt(expected.Count - 1));
        expected.RemoveAt(expected.Count - 1);
        AssertStorage(c, expected);

        while (expected.Count > 0)
        {
            c.RemoveAt(0);
            expected.RemoveAt(expected.Count - 1);
            Assert.AreEqual(expected.Count, c.Length());
        }
        AssertStorage(c, []);
        Merge(c);
    }

    [TestMethod]
    public void Out_Of_Range_Fails()
    {
        var c = Deploy();
        c.Add(Item(1));
        Assert.ThrowsExactly<TestException>(() => c.At(1));
        Assert.ThrowsExactly<TestException>(() => c.At(-1));
        Assert.ThrowsExactly<TestException>(() => c.RemoveAt(1));
        Assert.ThrowsExactly<TestException>(() => c.RemoveAt(-1));
        AssertStorage(c, [Item(1)]);
        Merge(c);
    }

    [TestMethod]
    public void Independent_Prefixes()
    {
        var c = Deploy();
        c.Add(Item(1));
        c.AddB(Item(2));
        c.AddB(Item(3));
        Assert.AreEqual(1, c.Length());
        Assert.AreEqual(2, c.LengthB());
        Assert.AreEqual(Item(3), c.AtB(1));
        AssertStorage(c, [Item(1)]);
        Merge(c);
    }

    [TestMethod]
    public void Values_Iterator_Returns_All_Elements()
    {
        var c = Deploy();
        var expected = Enumerable.Range(0, 20).Select(Item).ToList();
        foreach (var e in expected) c.Add(e);

        var values = c.Values()!;
        Assert.AreEqual(expected.Count, values.Count);
        Assert.IsTrue(expected.ToHashSet().SetEquals(values.Select(v => new UInt160(((Neo.VM.Types.ByteString)v).GetSpan()))));
        Merge(c);
    }

    [TestMethod]
    public void Failure_Rolls_Back()
    {
        var c = Deploy();
        c.Add(Item(1));
        Assert.ThrowsExactly<TestException>(() => c.AddThenAbort(Item(2)));
        AssertStorage(c, [Item(1)]);
        Merge(c);
    }

    [TestMethod]
    public void Invalid_Value_And_Prefix_Fail()
    {
        var c = Deploy();
        Assert.ThrowsExactly<TestException>(() => c.AddInvalid(new byte[19]));
        Assert.ThrowsExactly<TestException>(() => c.AddInvalid(new byte[21]));
        Assert.AreEqual(0, c.Length());

        Assert.ThrowsExactly<TestException>(() => c.CreateWithPrefixLength(0));
        Assert.ThrowsExactly<TestException>(() => c.CreateWithPrefixLength(44));
        Assert.AreEqual(0, c.CreateWithPrefixLength(43));
        Merge(c);
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileContract()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        var source = Path.Combine(repoRoot, "tests", "Neo.SmartContract.Framework.TestContracts", "Contract_UInt160List.cs");

        var options = new CompilationOptions
        {
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true
        };
        var engine = new CompilationEngine(options);
        var contexts = engine.CompileSources(new CompilationSourceReferences { Projects = [frameworkProject] }, source);
        Assert.AreEqual(1, contexts.Count);
        var context = contexts[0];
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        var (nef, manifest, debugInfoJson) = context.CreateResults(repoRoot);
        return (nef, manifest, NeoDebugInfo.FromDebugInfoJson(debugInfoJson));
    }

    public abstract class UInt160ListProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("add")] public abstract void Add(UInt160 value);
        [DisplayName("removeAt")] public abstract UInt160? RemoveAt(BigInteger index);
        [DisplayName("length")] public abstract BigInteger? Length();
        [DisplayName("at")] public abstract UInt160? At(BigInteger index);
        [DisplayName("values")] public abstract IList<object>? Values();

        [DisplayName("addB")] public abstract void AddB(UInt160 value);
        [DisplayName("lengthB")] public abstract BigInteger? LengthB();
        [DisplayName("atB")] public abstract UInt160? AtB(BigInteger index);

        [DisplayName("addInvalid")] public abstract void AddInvalid(byte[] value);
        [DisplayName("addThenAbort")] public abstract void AddThenAbort(UInt160 value);
        [DisplayName("createWithPrefixLength")] public abstract BigInteger? CreateWithPrefixLength(BigInteger size);
    }
}
