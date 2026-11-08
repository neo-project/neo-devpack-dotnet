// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SemanticDifferentialMatrix.cs file belongs to the neo project and is free
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
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_SemanticDifferentialMatrix
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("add")]
            public static int Add(int left, int right) => left + right;

            [DisplayName("remainder")]
            public static int Remainder(int left, int right) => left % right;

            [DisplayName("select")]
            public static int Select(bool condition, int whenTrue, int whenFalse) => condition ? whenTrue : whenFalse;

            [DisplayName("contains")]
            public static bool Contains(string value, char character) => value.Contains(character);

            [DisplayName("arrayLength")]
            public static int ArrayLength() => new int[2, 3].Length;

            [DisplayName("arrayLengthRank3")]
            public static int ArrayLengthRank3() => new int[2, 3, 4].Length;

            [DisplayName("arrayLengthEmptyOuter")]
            public static int ArrayLengthEmptyOuter() => new int[0, 3].Length;

            [DisplayName("arrayLengthEmptyInner")]
            public static int ArrayLengthEmptyInner() => new int[2, 0, 3].Length;
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SupportedOperationsMatchCSharp(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<SemanticDifferentialContract>(context.CreateExecutable(), context.CreateManifest());

        foreach (var (left, right) in new[] { (-7, 3), (7, -3), (0, 5), (12, 4) })
        {
            Assert.AreEqual(new BigInteger(left + right), contract.Add(left, right));
            Assert.AreEqual(new BigInteger(left % right), contract.Remainder(left, right));
            Assert.AreEqual(new BigInteger(left), contract.Select(true, left, right));
            Assert.AreEqual(new BigInteger(right), contract.Select(false, left, right));
        }

        foreach (var (value, character) in new[] { ("neo", 'n'), ("neo", 'x'), ("", 'n') })
        {
            Assert.AreEqual(value.Contains(character), contract.Contains(value, character));
        }

        Assert.AreEqual(new BigInteger(6), contract.ArrayLength());
        Assert.AreEqual(new BigInteger(24), contract.ArrayLengthRank3());
        Assert.AreEqual(BigInteger.Zero, contract.ArrayLengthEmptyOuter());
        Assert.AreEqual(BigInteger.Zero, contract.ArrayLengthEmptyInner());
    }

    public abstract class SemanticDifferentialContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("add")]
        public abstract BigInteger? Add(BigInteger left, BigInteger right);

        [DisplayName("remainder")]
        public abstract BigInteger? Remainder(BigInteger left, BigInteger right);

        [DisplayName("select")]
        public abstract BigInteger? Select(bool condition, BigInteger left, BigInteger right);

        [DisplayName("contains")]
        public abstract bool? Contains(string value, char character);

        [DisplayName("arrayLength")]
        public abstract BigInteger? ArrayLength();

        [DisplayName("arrayLengthRank3")]
        public abstract BigInteger? ArrayLengthRank3();

        [DisplayName("arrayLengthEmptyOuter")]
        public abstract BigInteger? ArrayLengthEmptyOuter();

        [DisplayName("arrayLengthEmptyInner")]
        public abstract BigInteger? ArrayLengthEmptyInner();
    }
}
