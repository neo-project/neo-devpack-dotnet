// Copyright (C) 2015-2026 The Neo Project.
//
// EnumUInt160SetTest.cs file belongs to the neo project and is free
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
public class EnumUInt160SetTest
{
    private const byte PrefixA = 0xE1;
    private static readonly byte[] PrefixB = [0xE2, 0x00];

    private static (NefFile nef, ContractManifest manifest) _compiled;

    public TestContext? TestContext { get; set; }

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static EnumUInt160SetProxy Deploy()
        => new TestEngine(true).Deploy<EnumUInt160SetProxy>(_compiled.nef, _compiled.manifest);

    private static UInt160 Item(int i)
    {
        var bytes = new byte[20];
        bytes[0] = 0x42;
        BitConverter.GetBytes(i).CopyTo(bytes, 1);
        return new UInt160(bytes);
    }

    private static byte[] IntBytes(BigInteger i) => i.IsZero ? [] : i.ToByteArray();

    private static byte[] Key(byte[] prefix, byte tag, byte[] data) => prefix.Concat([tag]).Concat(data).ToArray();

    /// <summary>
    /// Verifies the three storage structures are mutually consistent for the given set.
    /// </summary>
    private static void AssertConsistent(EnumUInt160SetProxy c, byte[] prefix, Func<BigInteger, UInt160?> at, BigInteger length, HashSet<UInt160> expected)
    {
        var storage = c.Storage;
        var countKey = prefix.Concat(new byte[] { 0x00 }).ToArray();

        if (length == 0)
            Assert.IsFalse(storage.Contains(countKey), "count key must be deleted when empty");
        else
            Assert.AreEqual(length, storage.GetInteger(countKey));

        Assert.AreEqual(expected.Count, (int)length);
        var seen = new HashSet<UInt160>();
        for (var i = BigInteger.Zero; i < length; i++)
        {
            var value = at(i)!;
            Assert.IsTrue(seen.Add(value), "duplicate element");
            Assert.IsTrue(expected.Contains(value));
            Assert.IsTrue(storage.TryGet(Key(prefix, 0x01, IntBytes(i)), out var stored));
            CollectionAssert.AreEqual(value.GetSpan().ToArray(), stored.ToArray());
            Assert.AreEqual(i + 1, storage.GetInteger(Key(prefix, 0x02, value.GetSpan().ToArray())));
        }

        // No stale entries beyond the end.
        Assert.IsFalse(storage.Contains(Key(prefix, 0x01, IntBytes(length))));
    }

    private static void AssertConsistentA(EnumUInt160SetProxy c, HashSet<UInt160> expected)
        => AssertConsistent(c, [PrefixA], c.At, c.Length()!.Value, expected);

    [TestMethod]
    public void Empty_Collection()
    {
        var c = Deploy();
        Assert.AreEqual(0, c.Length());
        Assert.IsFalse(c.Contains(Item(1)));
        Assert.IsFalse(c.Remove(Item(1)));
        Assert.ThrowsExactly<TestException>(() => c.At(0));
        Assert.AreEqual(0, c.Values()!.Count);
        AssertConsistentA(c, []);
    }

    [TestMethod]
    public void Add_Unique_Elements()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var i = 0; i < 5; i++)
        {
            Assert.IsTrue(c.Add(Item(i)));
            expected.Add(Item(i));
            Assert.IsTrue(c.Contains(Item(i)));
            Assert.AreEqual(i + 1, c.Length());
            Assert.AreEqual(Item(i), c.At(i));
        }
        AssertConsistentA(c, expected);
        Assert.AreEqual(5, c.Values()!.Count);
    }

    [TestMethod]
    public void Add_Duplicate_Is_NoOp()
    {
        var c = Deploy();
        Assert.IsTrue(c.Add(Item(1)));
        var changes = c.TestEngine.Storage.Snapshot.GetChangeSet().Count();
        Assert.IsFalse(c.Add(Item(1)));
        Assert.AreEqual(changes, c.TestEngine.Storage.Snapshot.GetChangeSet().Count());
        Assert.AreEqual(1, c.Length());
        AssertConsistentA(c, [Item(1)]);
    }

    [TestMethod]
    public void Remove_First_Middle_Last()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var i = 0; i < 5; i++) { c.Add(Item(i)); expected.Add(Item(i)); }

        // First: last element (4) moves into index 0.
        Assert.IsTrue(c.Remove(Item(0)));
        expected.Remove(Item(0));
        Assert.AreEqual(Item(4), c.At(0));
        AssertConsistentA(c, expected);

        // Middle.
        Assert.IsTrue(c.Remove(Item(2)));
        expected.Remove(Item(2));
        Assert.IsFalse(c.Contains(Item(2)));
        AssertConsistentA(c, expected);

        // Last element in the current order.
        var last = c.At(c.Length()!.Value - 1)!;
        Assert.IsTrue(c.Remove(last));
        expected.Remove(last);
        AssertConsistentA(c, expected);

        // Remove everything.
        foreach (var e in expected.ToArray())
        {
            Assert.IsTrue(c.Remove(e));
            expected.Remove(e);
            AssertConsistentA(c, expected);
        }
        Assert.AreEqual(0, c.Length());
    }

    [TestMethod]
    public void Remove_Missing_Is_NoOp()
    {
        var c = Deploy();
        c.Add(Item(1));
        var changes = c.TestEngine.Storage.Snapshot.GetChangeSet().Count();
        Assert.IsFalse(c.Remove(Item(2)));
        Assert.AreEqual(changes, c.TestEngine.Storage.Snapshot.GetChangeSet().Count());
        AssertConsistentA(c, [Item(1)]);
    }

    [TestMethod]
    public void At_OutOfRange_Fails()
    {
        var c = Deploy();
        c.Add(Item(1));
        Assert.AreEqual(Item(1), c.At(0));
        Assert.ThrowsExactly<TestException>(() => c.At(1));
        Assert.ThrowsExactly<TestException>(() => c.At(-1));
    }

    [TestMethod]
    public void Independent_Prefixes()
    {
        var c = Deploy();
        c.Add(Item(1));
        c.Add(Item(2));
        c.AddB(Item(2));
        c.AddB(Item(3));

        Assert.IsTrue(c.Contains(Item(1)));
        Assert.IsFalse(c.ContainsB(Item(1)));
        Assert.IsTrue(c.ContainsB(Item(3)));
        Assert.IsFalse(c.Contains(Item(3)));

        Assert.IsTrue(c.RemoveB(Item(2)));
        Assert.IsTrue(c.Contains(Item(2)));
        Assert.AreEqual(2, c.Length());
        Assert.AreEqual(1, c.LengthB());

        AssertConsistentA(c, [Item(1), Item(2)]);
        AssertConsistent(c, PrefixB, c.AtB, c.LengthB()!.Value, [Item(3)]);
    }

    [TestMethod]
    public void Repeated_Add_Remove_Cycles()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var round = 0; round < 4; round++)
        {
            for (var i = 0; i < 6; i++)
            {
                c.Add(Item(i));
                expected.Add(Item(i));
            }
            for (var i = 0; i < 6; i += 2)
            {
                Assert.IsTrue(c.Remove(Item(i)));
                expected.Remove(Item(i));
            }
            AssertConsistentA(c, expected);
        }
        foreach (var e in expected.ToArray())
        {
            c.Remove(e);
            expected.Remove(e);
        }
        AssertConsistentA(c, expected);
    }

    [TestMethod]
    public void Values_Iterator_Returns_All_Elements()
    {
        var c = Deploy();
        var expected = Enumerable.Range(0, 10).Select(Item).ToHashSet();
        foreach (var e in expected) c.Add(e);
        c.Remove(Item(3));
        expected.Remove(Item(3));

        var values = c.Values()!;
        Assert.AreEqual(expected.Count, values.Count);
        Assert.IsTrue(expected.SetEquals(values.Select(v => new UInt160(((Neo.VM.Types.ByteString)v).GetSpan()))));
    }

    [TestMethod]
    public void Large_Collection()
    {
        var c = Deploy();
        var expected = new HashSet<UInt160>();
        for (var i = 0; i < 300; i++)
        {
            Assert.IsTrue(c.Add(Item(i)));
            expected.Add(Item(i));
        }
        Assert.AreEqual(300, c.Length());
        Assert.AreEqual(Item(299), c.At(299));
        for (var i = 0; i < 300; i += 3)
        {
            Assert.IsTrue(c.Remove(Item(i)));
            expected.Remove(Item(i));
        }
        Assert.AreEqual(200, c.Length());
        AssertConsistentA(c, expected);
    }

    [TestMethod]
    public void Failure_Rolls_Back()
    {
        var c = Deploy();
        c.Add(Item(1));
        c.Add(Item(2));

        Assert.ThrowsExactly<TestException>(() => c.AddThenAbort(Item(9)));
        Assert.IsFalse(c.Contains(Item(9)));
        AssertConsistentA(c, [Item(1), Item(2)]);

        Assert.ThrowsExactly<TestException>(() => c.RemoveThenAbort(Item(1)));
        Assert.IsTrue(c.Contains(Item(1)));
        AssertConsistentA(c, [Item(1), Item(2)]);
    }

    [TestMethod]
    public void Invalid_Value_And_Prefix_Fail()
    {
        var c = Deploy();
        Assert.ThrowsExactly<TestException>(() => c.AddInvalid(new byte[19]));
        Assert.ThrowsExactly<TestException>(() => c.AddInvalid(new byte[21]));
        Assert.AreEqual(0, c.Length());

        Assert.ThrowsExactly<TestException>(() => c.CreateWithPrefixLength(0));
        Assert.ThrowsExactly<TestException>(() => c.CreateWithPrefixLength(EnumUInt160Set_MaxPrefix + 1));
        Assert.AreEqual(0, c.CreateWithPrefixLength(EnumUInt160Set_MaxPrefix));
    }

    private const int EnumUInt160Set_MaxPrefix = 43;

    [TestMethod]
    public void Gas_Measurements()
    {
        foreach (var size in new[] { 0, 1, 10, 100, 500 })
        {
            var c = Deploy();
            for (var i = 0; i < size; i++) c.Add(Item(i));

            var add = Measure(c, () => c.Add(Item(100000)));
            var contains = Measure(c, () => c.Contains(Item(100000)));
            var at = size == 0 ? 0 : Measure(c, () => c.At(size / 2));
            var removeLast = Measure(c, () => c.Remove(Item(100000)));
            var removeFirst = size == 0 ? 0 : Measure(c, () => c.Remove(Item(0)));
            var length = Measure(c, () => c.Length());

            TestContext?.WriteLine($"size={size,4} add={add} contains={contains} at={at} length={length} removeLast={removeLast} removeFirst={removeFirst} (datoshi)");
        }
    }

    private static long Measure(EnumUInt160SetProxy c, Action action)
    {
        action();
        return c.TestEngine.FeeConsumed.Value;
    }

    private static (NefFile nef, ContractManifest manifest) CompileContract()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        var source = Path.Combine(repoRoot, "tests", "Neo.SmartContract.Framework.TestContracts", "Contract_EnumUInt160Set.cs");

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
        return (context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class EnumUInt160SetProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public TestEngine TestEngine { get; } = initialize.Engine;

        [DisplayName("add")] public abstract bool? Add(UInt160 value);
        [DisplayName("remove")] public abstract bool? Remove(UInt160 value);
        [DisplayName("contains")] public abstract bool? Contains(UInt160 value);
        [DisplayName("length")] public abstract BigInteger? Length();
        [DisplayName("at")] public abstract UInt160? At(BigInteger index);
        [DisplayName("values")] public abstract IList<object>? Values();

        [DisplayName("addB")] public abstract bool? AddB(UInt160 value);
        [DisplayName("removeB")] public abstract bool? RemoveB(UInt160 value);
        [DisplayName("containsB")] public abstract bool? ContainsB(UInt160 value);
        [DisplayName("lengthB")] public abstract BigInteger? LengthB();
        [DisplayName("atB")] public abstract UInt160? AtB(BigInteger index);

        [DisplayName("addInvalid")] public abstract bool? AddInvalid(byte[] value);
        [DisplayName("addThenAbort")] public abstract void AddThenAbort(UInt160 value);
        [DisplayName("removeThenAbort")] public abstract void RemoveThenAbort(UInt160 value);
        [DisplayName("createWithPrefixLength")] public abstract BigInteger? CreateWithPrefixLength(BigInteger size);
    }
}
