// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StackItemComparisonMatrix.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StackItemComparisonMatrix
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;
        using System.ComponentModel;
        using System.Numerics;

        public class Contract : SmartContract
        {
            [DisplayName("compareInt")]
            public static int CompareInt(int left, int right) => left < right ? -1 : left > right ? 1 : 0;

            [DisplayName("compareLong")]
            public static int CompareLong(long left, long right) => left < right ? -1 : left > right ? 1 : 0;

            [DisplayName("equalBigInteger")]
            public static bool EqualBigInteger(BigInteger left, BigInteger right) => left == right;

            [DisplayName("equalString")]
            public static bool EqualString(string? left, string? right) => left == right;

            [DisplayName("equalBool")]
            public static bool EqualBool(bool left, bool right) => left == right;
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ComparisonsMatchCSharpAcrossSupportedValueKinds(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var contract = new TestEngine(true).Deploy<ComparisonMatrixContract>(
            context.CreateExecutable(), context.CreateManifest());

        foreach (var (left, right) in new[]
        {
            (int.MinValue, int.MinValue),
            (int.MinValue, -1),
            (-1, 0),
            (0, 1),
            (int.MaxValue, int.MaxValue),
            (int.MaxValue, int.MinValue)
        })
        {
            int expected = left < right ? -1 : left > right ? 1 : 0;
            Assert.AreEqual(expected, contract.CompareInt(left, right), $"int {left}, {right}, {optimization}");
        }

        foreach (var (left, right) in new[]
        {
            (long.MinValue, long.MinValue),
            (long.MinValue, -1L),
            (-1L, 0L),
            (0L, 1L),
            (long.MaxValue, long.MaxValue),
            (long.MaxValue, long.MinValue)
        })
        {
            int expected = left < right ? -1 : left > right ? 1 : 0;
            Assert.AreEqual(expected, contract.CompareLong(left, right), $"long {left}, {right}, {optimization}");
        }

        foreach (var (left, right) in new[]
        {
            (BigInteger.MinusOne, BigInteger.MinusOne),
            (BigInteger.MinusOne, BigInteger.Zero),
            (BigInteger.Zero, BigInteger.One),
            (BigInteger.Parse("123456789012345678901234567890"), BigInteger.Parse("123456789012345678901234567891"))
        })
        {
            Assert.AreEqual(left == right, contract.EqualBigInteger(left, right), $"BigInteger {left}, {right}, {optimization}");
        }

        foreach (var (left, right) in new (string?, string?)[]
        {
            (null, null),
            (null, "neo"),
            ("neo", null),
            ("", ""),
            ("neo", "neo"),
            ("neo", "Neo"),
            ("neo é", "neo é")
        })
        {
            Assert.AreEqual(left == right, contract.EqualString(left, right), $"string {left}, {right}, {optimization}");
        }

        foreach (var (left, right) in new[] { (false, false), (false, true), (true, false), (true, true) })
            Assert.AreEqual(left == right, contract.EqualBool(left, right), $"bool {left}, {right}, {optimization}");
    }

    public abstract class ComparisonMatrixContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("compareInt")] public abstract BigInteger? CompareInt(BigInteger left, BigInteger right);
        [DisplayName("compareLong")] public abstract BigInteger? CompareLong(BigInteger left, BigInteger right);
        [DisplayName("equalBigInteger")] public abstract bool? EqualBigInteger(BigInteger left, BigInteger right);
        [DisplayName("equalString")] public abstract bool? EqualString(string? left, string? right);
        [DisplayName("equalBool")] public abstract bool? EqualBool(bool left, bool right);
    }
}
