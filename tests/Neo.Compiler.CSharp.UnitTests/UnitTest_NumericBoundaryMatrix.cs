// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NumericBoundaryMatrix.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NumericBoundaryMatrix
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("addLongUnchecked")]
            public static long AddLongUnchecked(long value, long delta) => unchecked(value + delta);

            [DisplayName("addLongChecked")]
            public static long AddLongChecked(long value, long delta) => checked(value + delta);

            [DisplayName("addULongUnchecked")]
            public static ulong AddULongUnchecked(ulong value, ulong delta) => unchecked(value + delta);

            [DisplayName("addULongChecked")]
            public static ulong AddULongChecked(ulong value, ulong delta) => checked(value + delta);

            [DisplayName("subULongUnchecked")]
            public static ulong SubULongUnchecked(ulong value, ulong delta) => unchecked(value - delta);

            [DisplayName("subULongChecked")]
            public static ulong SubULongChecked(ulong value, ulong delta) => checked(value - delta);

            [DisplayName("divideInt")]
            public static int DivideInt(int value, int divisor) => value / divisor;

            [DisplayName("remainderInt")]
            public static int RemainderInt(int value, int divisor) => value % divisor;

            [DisplayName("divideLong")]
            public static long DivideLong(long value, long divisor) => value / divisor;

            [DisplayName("remainderLong")]
            public static long RemainderLong(long value, long divisor) => value % divisor;
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SignedAndUnsigned64BitArithmeticMatchCSharpBoundaries(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<NumericBoundaryContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(long.MinValue), contract.AddLongUnchecked(long.MaxValue, 1));
        Assert.AreEqual(new BigInteger(long.MaxValue), contract.AddLongUnchecked(long.MinValue, -1));
        Assert.ThrowsExactly<TestException>(() => contract.AddLongChecked(long.MaxValue, 1));
        Assert.ThrowsExactly<TestException>(() => contract.AddLongChecked(long.MinValue, -1));

        Assert.AreEqual(BigInteger.Zero, contract.AddULongUnchecked(ulong.MaxValue, 1));
        Assert.AreEqual(new BigInteger(ulong.MaxValue), contract.SubULongUnchecked(0, 1));
        Assert.ThrowsExactly<TestException>(() => contract.AddULongChecked(ulong.MaxValue, 1));
        Assert.ThrowsExactly<TestException>(() => contract.SubULongChecked(0, 1));

        Assert.AreEqual(new BigInteger(-2), contract.DivideInt(-7, 3));
        Assert.AreEqual(new BigInteger(-1), contract.RemainderInt(-7, 3));
        Assert.AreEqual(new BigInteger(-2), contract.DivideInt(7, -3));
        Assert.AreEqual(new BigInteger(1), contract.RemainderInt(7, -3));
        Assert.AreEqual(new BigInteger(long.MinValue), contract.DivideLong(long.MinValue, 1));
        Assert.AreEqual(BigInteger.Zero, contract.RemainderLong(long.MinValue, 1));
        Assert.AreEqual(new BigInteger(long.MinValue + 1), contract.DivideLong(long.MinValue + 1, 1));
        Assert.AreEqual(new BigInteger(-1), contract.RemainderLong(long.MinValue + 1, 2));
    }

    public abstract class NumericBoundaryContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("addLongUnchecked")] public abstract BigInteger? AddLongUnchecked(BigInteger value, BigInteger delta);
        [DisplayName("addLongChecked")] public abstract BigInteger? AddLongChecked(BigInteger value, BigInteger delta);
        [DisplayName("addULongUnchecked")] public abstract BigInteger? AddULongUnchecked(BigInteger value, BigInteger delta);
        [DisplayName("addULongChecked")] public abstract BigInteger? AddULongChecked(BigInteger value, BigInteger delta);
        [DisplayName("subULongUnchecked")] public abstract BigInteger? SubULongUnchecked(BigInteger value, BigInteger delta);
        [DisplayName("subULongChecked")] public abstract BigInteger? SubULongChecked(BigInteger value, BigInteger delta);
        [DisplayName("divideInt")] public abstract BigInteger? DivideInt(BigInteger value, BigInteger divisor);
        [DisplayName("remainderInt")] public abstract BigInteger? RemainderInt(BigInteger value, BigInteger divisor);
        [DisplayName("divideLong")] public abstract BigInteger? DivideLong(BigInteger value, BigInteger divisor);
        [DisplayName("remainderLong")] public abstract BigInteger? RemainderLong(BigInteger value, BigInteger divisor);
    }
}
