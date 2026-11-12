// Copyright (C) 2015-2026 The Neo Project.
//
// UInt160SetTest.cs file belongs to the neo project and is free
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
public class UInt160SetTest
{
    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static UInt160SetProxy Deploy()
        => new TestEngine(true).Deploy<UInt160SetProxy>(_compiled.nef, _compiled.manifest);

    private static void Merge(UInt160SetProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static UInt160 Item(int i)
    {
        var bytes = new byte[20];
        bytes[0] = 0x42;
        BitConverter.GetBytes(i).CopyTo(bytes, 1);
        return new UInt160(bytes);
    }

    private static void AssertStorage(UInt160SetProxy c, HashSet<UInt160> expected)
    {
        Assert.AreEqual(expected.Count, (int)c.Length()!.Value);
        var countKey = new byte[] { 0xE1, 0x00 };
        if (expected.Count == 0)
            Assert.IsFalse(c.Storage.Contains(countKey), "count key must be deleted when empty");
        else
            Assert.AreEqual(expected.Count, c.Storage.GetInteger(countKey));

        foreach (var e in expected)
        {
            Assert.IsTrue(c.Contains(e));
            Assert.IsTrue(c.Storage.Contains(new byte[] { 0xE1, 0x01 }.Concat(e.GetSpan().ToArray()).ToArray()));
        }

        var values = c.Values()!.Select(v => new UInt160(((Neo.VM.Types.ByteString)v).GetSpan())).ToList();
        Assert.AreEqual(expected.Count, values.Count);
        Assert.IsTrue(expected.SetEquals(values));
    }

    [TestMethod]
    public void Empty_Set()
    {
        var c = Deploy();
        Assert.AreEqual(0, c.Length());
        Assert.IsFalse(c.Contains(Item(1)));
        Assert.IsFalse(c.Remove(Item(1)));
        AssertStorage(c, []);
        Merge(c);
    }

    [TestMethod]
    public void Add_Unique_And_Duplicate()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var i = 0; i < 5; i++)
        {
            Assert.IsTrue(c.Add(Item(i)));
            expected.Add(Item(i));
        }
        var changes = c.TestEngine.Storage.Snapshot.GetChangeSet().Count();
        Assert.IsFalse(c.Add(Item(3)));
        Assert.AreEqual(changes, c.TestEngine.Storage.Snapshot.GetChangeSet().Count());
        AssertStorage(c, expected);
        Merge(c);
    }

    [TestMethod]
    public void Remove_Existing_And_Missing()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var i = 0; i < 6; i++)
        {
            c.Add(Item(i));
            expected.Add(Item(i));
        }

        foreach (var i in new[] { 0, 5, 2 })
        {
            Assert.IsTrue(c.Remove(Item(i)));
            expected.Remove(Item(i));
            AssertStorage(c, expected);
        }

        var changes = c.TestEngine.Storage.Snapshot.GetChangeSet().Count();
        Assert.IsFalse(c.Remove(Item(0)));
        Assert.AreEqual(changes, c.TestEngine.Storage.Snapshot.GetChangeSet().Count());

        foreach (var e in expected.ToList())
        {
            Assert.IsTrue(c.Remove(e));
            expected.Remove(e);
        }
        AssertStorage(c, []);
        Merge(c);
    }

    [TestMethod]
    public void Independent_Prefixes()
    {
        var c = Deploy();
        Assert.IsTrue(c.Add(Item(1)));
        Assert.IsTrue(c.AddB(Item(1)));
        Assert.IsTrue(c.AddB(Item(2)));
        Assert.IsTrue(c.RemoveB(Item(1)));
        Assert.IsTrue(c.Contains(Item(1)));
        Assert.IsFalse(c.ContainsB(Item(1)));
        Assert.IsTrue(c.ContainsB(Item(2)));
        Assert.AreEqual(1, c.Length());
        Assert.AreEqual(1, c.LengthB());
        AssertStorage(c, [Item(1)]);
        Merge(c);
    }

    [TestMethod]
    public void Large_Set()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var i = 0; i < 100; i++)
        {
            c.Add(Item(i));
            expected.Add(Item(i));
        }
        for (var i = 0; i < 100; i += 3)
        {
            c.Remove(Item(i));
            expected.Remove(Item(i));
        }
        AssertStorage(c, expected);
        Merge(c);
    }

    [TestMethod]
    public void Failure_Rolls_Back()
    {
        var c = Deploy();
        c.Add(Item(1));
        Assert.ThrowsExactly<TestException>(() => c.AddThenAbort(Item(2)));
        Assert.ThrowsExactly<TestException>(() => c.RemoveThenAbort(Item(1)));
        Assert.IsTrue(c.Contains(Item(1)));
        AssertStorage(c, [Item(1)]);
        Merge(c);
    }

    [TestMethod]
    public void Invalid_Value_And_Prefix_Fail()
    {
        var c = Deploy();
        foreach (var size in new[] { 19, 21 })
        {
            Assert.ThrowsExactly<TestException>(() => c.AddInvalid(new byte[size]));
            Assert.ThrowsExactly<TestException>(() => c.ContainsInvalid(new byte[size]));
            Assert.ThrowsExactly<TestException>(() => c.RemoveInvalid(new byte[size]));
        }
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
        var source = Path.Combine(repoRoot, "tests", "Neo.SmartContract.Framework.TestContracts", "Contract_UInt160Set.cs");

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

    public abstract class UInt160SetProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public TestEngine TestEngine { get; } = initialize.Engine;

        [DisplayName("add")] public abstract bool? Add(UInt160 value);
        [DisplayName("remove")] public abstract bool? Remove(UInt160 value);
        [DisplayName("contains")] public abstract bool? Contains(UInt160 value);
        [DisplayName("length")] public abstract BigInteger? Length();
        [DisplayName("values")] public abstract IList<object>? Values();

        [DisplayName("addB")] public abstract bool? AddB(UInt160 value);
        [DisplayName("removeB")] public abstract bool? RemoveB(UInt160 value);
        [DisplayName("containsB")] public abstract bool? ContainsB(UInt160 value);
        [DisplayName("lengthB")] public abstract BigInteger? LengthB();

        [DisplayName("addInvalid")] public abstract bool? AddInvalid(byte[] value);
        [DisplayName("containsInvalid")] public abstract bool? ContainsInvalid(byte[] value);
        [DisplayName("removeInvalid")] public abstract bool? RemoveInvalid(byte[] value);
        [DisplayName("addThenAbort")] public abstract void AddThenAbort(UInt160 value);
        [DisplayName("removeThenAbort")] public abstract void RemoveThenAbort(UInt160 value);
        [DisplayName("createWithPrefixLength")] public abstract BigInteger? CreateWithPrefixLength(BigInteger size);
    }
}
