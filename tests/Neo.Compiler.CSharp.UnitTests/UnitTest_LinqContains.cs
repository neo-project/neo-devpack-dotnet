// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LinqContains.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FrameworkLinq = scfx::Neo.SmartContract.Framework.Linq.LinqExtensions;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_LinqContains
{
    private const string LegacyImplementation = """
        private static bool LegacyContains<T>(System.Collections.Generic.IEnumerable<T> source, T value)
        {
            return LinqExtensions.Any(source, s => s!.Equals(value));
        }
        """;

    private static readonly Lazy<CompilationContext> LegacyIntegers = new(() => Compile(true, "int"));
    private static readonly Lazy<CompilationContext> DirectIntegers = new(() => Compile(false, "int"));
    private static readonly Lazy<CompilationContext> LegacyStrings = new(() => Compile(true, "string?"));
    private static readonly Lazy<CompilationContext> DirectStrings = new(() => Compile(false, "string?"));

    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void Contains_ClrEqualityAndNulls_AreUnchanged()
    {
        AssertClrEquivalent<int>(null, 1);
        AssertClrEquivalent(Array.Empty<int>(), 1);
        AssertClrEquivalent(new[] { 1, 2, 2, 3 }, 1);
        AssertClrEquivalent(new[] { 1, 2, 2, 3 }, 2);
        AssertClrEquivalent(new[] { 1, 2, 2, 3 }, 3);
        AssertClrEquivalent(new[] { 1, 2, 2, 3 }, 4);
        AssertClrEquivalent<string?>(null, null);
        AssertClrEquivalent(Array.Empty<string?>(), null);
        AssertClrEquivalent(new string?[] { null }, null);
        AssertClrEquivalent(new string?[] { null }, "a");
        AssertClrEquivalent(new string?[] { "a", null }, "a");
        AssertClrEquivalent(new string?[] { "a", null }, "b");
        AssertClrEquivalent(new string?[] { "a", "b" }, null);
        AssertClrEquivalent<int?>(new int?[] { null, 1 }, null);
        AssertClrEquivalent<int?>(new int?[] { null, 1 }, 1);

        var reference = new object();
        AssertClrEquivalent(new[] { reference }, reference);
        AssertClrEquivalent(new[] { reference }, new object());
        AssertClrEquivalent(new[] { new EqualityValue(7) }, new EqualityValue(7));
        AssertClrEquivalent(new[] { new EqualityValue(7) }, new EqualityValue(8));
        AssertClrEquivalent(new[] { (1, 2) }, (1, 2));
        AssertClrEquivalent(new[] { (1, 2) }, (1, 3));

        Assert.ThrowsExactly<NullReferenceException>(() => FrameworkLinq.Contains(new string?[] { null }, null));
        Assert.IsTrue(FrameworkLinq.Contains(new string?[] { "a", null }, "a"));
    }

    [TestMethod]
    public void Contains_NeoVmEqualityAndNulls_AreUnchanged()
    {
        var legacyEngine = new TestEngine(true);
        var directEngine = new TestEngine(true);
        var legacy = Deploy(legacyEngine, LegacyStrings.Value);
        var direct = Deploy(directEngine, DirectStrings.Value);
        (IList<object>? Source, object? Value)[] cases =
        [
            (null, "a"), (null, null),
            (new List<object>(), "a"), (new List<object>(), null),
            (new List<object> { "a" }, "a"), (new List<object> { "a" }, "b"),
            (new List<object> { "a", "b", "b", "c" }, "a"),
            (new List<object> { "a", "b", "b", "c" }, "b"),
            (new List<object> { "a", "b", "b", "c" }, "c"),
            (new List<object> { "a", "b", "b", "c" }, "d"),
            (new List<object> { null! }, null), (new List<object> { null! }, "a"),
            (new List<object> { "a", null! }, "a"),
            (new List<object> { "a", null! }, "b"),
            (new List<object> { "a", "b" }, null),
            (new List<object> { null!, "a" }, "a")
        ];
        foreach (var (source, value) in cases)
        {
            var before = CaptureVm(() => legacy.Contains(source, value));
            var after = CaptureVm(() => direct.Contains(source, value));
            Assert.AreEqual(before, after, $"Source={FormatSource(source)}, value={value ?? "null"}");
            TestContext.WriteLine($"Source={FormatSource(source)}, value={value ?? "null"}: {after}");
        }
        var fault = Assert.ThrowsExactly<TestException>(() => direct.Contains(null, "a"));
        StringAssert.Contains(fault.InnerException?.Message ?? fault.Message, "source is null");
        Assert.IsFalse(direct.Contains(new List<object>(), null));
        Assert.IsTrue(direct.Contains(new List<object> { "a", null! }, "a"));
        var legacyIntegers = Deploy(new TestEngine(true), LegacyIntegers.Value);
        var directIntegers = Deploy(new TestEngine(true), DirectIntegers.Value);
        foreach (var source in new IList<object>?[] { null, new List<object>(), new List<object> { 1, 2, 2, 3 } })
        {
            foreach (var target in new[] { 1, 2, 3, 4 })
            {
                Assert.AreEqual(CaptureVm(() => legacyIntegers.Contains(source, target)),
                    CaptureVm(() => directIntegers.Contains(source, target)),
                    $"Source={FormatSource(source)}, value={target}");
            }
        }
    }

    [TestMethod]
    public void Contains_NeoVmReferenceAndStructureEquality_AreUnchanged()
    {
        const string methods = """
            private class Person { public int Age; }
            private struct PersonValue { public int Age; }
            public static bool Reference(bool same)
            {
                var first = new Person { Age = 7 };
                var target = same ? first : new Person { Age = 7 };
                return __SEARCH__(new[] { first }, target);
            }
            public static bool Structure(bool same)
            {
                var first = new PersonValue { Age = 7 };
                var target = new PersonValue { Age = same ? 7 : 8 };
                return __SEARCH__(new[] { first }, target);
            }
            """;
        var legacy = Deploy(new TestEngine(true), Compile(true, "int", methods));
        var direct = Deploy(new TestEngine(true), Compile(false, "int", methods));
        foreach (var same in new[] { true, false })
        {
            Assert.AreEqual(same, legacy.Reference(same));
            Assert.AreEqual(legacy.Reference(same), direct.Reference(same));
            Assert.AreEqual(same, legacy.Structure(same));
            Assert.AreEqual(legacy.Structure(same), direct.Structure(same));
        }
    }

    [TestMethod]
    [DataRow(1, false)]
    [DataRow(10, false)]
    [DataRow(100, false)]
    [DataRow(1, true)]
    [DataRow(10, true)]
    [DataRow(100, true)]
    public void Contains_Gas_IsLowerThanLegacy(int size, bool strings)
    {
        var legacyEngine = new TestEngine(true);
        var directEngine = new TestEngine(true);
        var legacy = Deploy(legacyEngine, strings ? LegacyStrings.Value : LegacyIntegers.Value);
        var direct = Deploy(directEngine, strings ? DirectStrings.Value : DirectIntegers.Value);
        var source = Enumerable.Range(0, size).Select(i => strings ? (object)i.ToString(System.Globalization.CultureInfo.InvariantCulture) : i).ToList();
        foreach (var (position, target) in new[] { ("first", 0), ("middle", size / 2), ("last", size - 1), ("missing", -1) })
        {
            object value = strings ? target.ToString(System.Globalization.CultureInfo.InvariantCulture) : target;
            Assert.AreEqual(target >= 0, legacy.Contains(source, value));
            long before = legacyEngine.FeeConsumed.Value;
            Assert.AreEqual(target >= 0, direct.Contains(source, value));
            long after = directEngine.FeeConsumed.Value;
            Assert.IsTrue(after < before, $"GAS regression: size={size}, {position}, legacy={before}, direct={after}");
            TestContext.WriteLine($"{(strings ? "string" : "int")},{size},{position},{before},{after},{before - after}");
        }
    }

    [TestMethod]
    public void Contains_EmptyCollectionGas_IsLowerThanLegacy()
    {
        var legacyEngine = new TestEngine(true);
        var directEngine = new TestEngine(true);
        Assert.IsFalse(Deploy(legacyEngine, LegacyIntegers.Value).Contains(new List<object>(), 1));
        long before = legacyEngine.FeeConsumed.Value;
        Assert.IsFalse(Deploy(directEngine, DirectIntegers.Value).Contains(new List<object>(), 1));
        long after = directEngine.FeeConsumed.Value;
        Assert.IsTrue(after < before);
        TestContext.WriteLine($"empty,{before},{after},{before - after}");
    }

    [TestMethod]
    public void Contains_BytecodeAndNef_AreSmallerThanLegacy()
    {
        foreach (var (name, before, after) in new[]
        {
            ("int", LegacyIntegers.Value, DirectIntegers.Value),
            ("string", LegacyStrings.Value, DirectStrings.Value)
        })
        {
            var legacy = before.CreateExecutable();
            var direct = after.CreateExecutable();
            var legacyInstructions = new Script(legacy.Script.ToArray()).EnumerateInstructions().ToArray();
            var directInstructions = new Script(direct.Script.ToArray()).EnumerateInstructions().ToArray();
            Assert.IsTrue(directInstructions.Length < legacyInstructions.Length);
            Assert.IsTrue(direct.Script.Length < legacy.Script.Length);
            Assert.IsTrue(direct.ToArray().Length < legacy.ToArray().Length);
            Assert.IsTrue(legacyInstructions.Any(item => item.instruction.OpCode == OpCode.CALLA));
            Assert.IsFalse(directInstructions.Any(item => item.instruction.OpCode == OpCode.CALLA));
            TestContext.WriteLine($"{name}: instructions {legacyInstructions.Length}->{directInstructions.Length}; script {legacy.Script.Length}->{direct.Script.Length}; NEF {legacy.ToArray().Length}->{direct.ToArray().Length}");
            TestContext.WriteLine("Legacy: " + string.Join(" ", legacyInstructions.Select(item => $"{item.address}:{item.instruction.OpCode}")));
            TestContext.WriteLine("Direct: " + string.Join(" ", directInstructions.Select(item => $"{item.address}:{item.instruction.OpCode}")));
        }
    }

    private static CompilationContext Compile(bool legacy, string elementType, string methods = "")
    {
        const string template = """
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Linq;
            using System.ComponentModel;
            public class Contract : SmartContract
            {
                [DisplayName("contains")]
                public static bool Contains(__TYPE__[] source, __TYPE__ value) => __SEARCH__(source, value);
                __LEGACY__
                __METHODS__
            }
            """;
        var source = template.Replace("__TYPE__", elementType)
            .Replace("__LEGACY__", legacy ? LegacyImplementation : "")
            .Replace("__METHODS__", methods)
            .Replace("__SEARCH__", legacy ? "LegacyContains" : "LinqExtensions.Contains");
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        return context;
    }

    private static ContainsContract Deploy(TestEngine engine, CompilationContext context)
        => engine.Deploy<ContainsContract>(context.CreateExecutable(), context.CreateManifest());

    private static void AssertClrEquivalent<T>(IEnumerable<T>? source, T value)
    {
        var before = CaptureClr(() => FrameworkLinq.Any(source!, item => item!.Equals(value)));
        var after = CaptureClr(() => FrameworkLinq.Contains(source!, value));
        Assert.AreEqual(before, after);
    }

    private static Outcome CaptureClr(Func<bool> action)
    {
        try { return new Outcome(action(), null, null); }
        catch (Exception exception) { return new Outcome(null, exception.GetType().FullName, exception.Message); }
    }

    private static Outcome CaptureVm(Func<bool?> action)
    {
        try { return new Outcome(action(), null, null); }
        catch (TestException exception)
        {
            var cause = exception.InnerException ?? exception;
            return new Outcome(null, cause.GetType().FullName, cause.Message);
        }
    }

    private static string FormatSource(IList<object>? source)
        => source is null ? "null" : "[" + string.Join(",", source.Select(item => item ?? "null")) + "]";

    private sealed record Outcome(bool? Result, string? ExceptionType, string? Message);

    private sealed class EqualityValue(int value)
    {
        public override bool Equals(object? obj) => obj is EqualityValue other && value == other.Value;
        private int Value => value;
        public override int GetHashCode() => value;
    }

    public abstract class ContainsContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("contains")]
        public abstract bool? Contains(IList<object>? source, object? value);
        [DisplayName("reference")]
        public abstract bool? Reference(bool same);
        [DisplayName("structure")]
        public abstract bool? Structure(bool same);
    }
}
