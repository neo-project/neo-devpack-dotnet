// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ShiftCountMasking.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ShiftCountMasking
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;
        using System.Numerics;

        public class Contract : SmartContract
        {
            private static int _evaluations;

            [DisplayName("intLeftChecked")]
            public static int IntLeftChecked(int value, int count) => checked(value << count);

            [DisplayName("intRightChecked")]
            public static int IntRightChecked(int value, int count) => checked(value >> count);

            [DisplayName("longLeftChecked")]
            public static long LongLeftChecked(long value, int count) => checked(value << count);

            [DisplayName("longRightChecked")]
            public static long LongRightChecked(long value, int count) => checked(value >> count);

            [DisplayName("intLeftUnchecked")]
            public static int IntLeftUnchecked(int value, int count) => unchecked(value << count);

            [DisplayName("nullableIntLeftChecked")]
            public static int? NullableIntLeftChecked(int? value, int count) => checked(value << count);

            [DisplayName("nullableLongRightChecked")]
            public static long? NullableLongRightChecked(long? value, int count) => checked(value >> count);

            [DisplayName("nullableBothLeft")]
            public static int? NullableBothLeft(int? value, int? count) => value << count;

            [DisplayName("nullableBothRight")]
            public static int? NullableBothRight(int? value, int? count) => value >> count;

            [DisplayName("nullLeftEvaluationCount")]
            public static int NullLeftEvaluationCount()
            {
                _evaluations = 0;
                int? value = null;
                _ = value << Evaluate();
                return _evaluations;
            }

            private static int Evaluate() => ++_evaluations;

            [DisplayName("compoundInt")]
            public static int CompoundInt(int value, int count)
            {
                checked { value <<= count; }
                return value;
            }

            [DisplayName("compoundByte")]
            public static byte CompoundByte(byte value, int count)
            {
                checked { value <<= count; }
                return value;
            }

            [DisplayName("compoundLongRight")]
            public static long CompoundLongRight(long value, int count)
            {
                checked { value >>= count; }
                return value;
            }

            [DisplayName("bigIntegerLeft")]
            public static BigInteger BigIntegerLeft(BigInteger value, int count) => checked(value << count);

            [DisplayName("compoundBigInteger")]
            public static BigInteger CompoundBigInteger(BigInteger value, int count)
            {
                checked { value <<= count; }
                return value;
            }
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void FixedWidthShiftCounts_MatchCSharpSemantics(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics.Select(static p => p.ToString()))}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ShiftCountContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(2), contract.IntLeftChecked(1, 33), optimization.ToString());
        Assert.AreEqual(new BigInteger(-4), contract.IntRightChecked(-8, 33), optimization.ToString());
        Assert.AreEqual(new BigInteger(int.MinValue), contract.IntLeftChecked(1, -1), optimization.ToString());
        Assert.AreEqual(new BigInteger(2), contract.LongLeftChecked(1, 65), optimization.ToString());
        Assert.AreEqual(new BigInteger(-4), contract.LongRightChecked(-8, 65), optimization.ToString());
        Assert.AreEqual(new BigInteger(long.MinValue), contract.LongLeftChecked(1, -1), optimization.ToString());
        Assert.AreEqual(new BigInteger(2), contract.IntLeftUnchecked(1, 33), optimization.ToString());
        Assert.AreEqual(new BigInteger(2), contract.NullableIntLeftChecked(1, 33), optimization.ToString());
        Assert.AreEqual(new BigInteger(-4), contract.NullableLongRightChecked(-8, 65), optimization.ToString());
        Assert.IsNull(contract.NullableIntLeftChecked(null, 33), optimization.ToString());
        Assert.IsNull(contract.NullableLongRightChecked(null, 65), optimization.ToString());
        Assert.AreEqual(new BigInteger(2), contract.NullableBothLeft(1, 33), optimization.ToString());
        Assert.IsNull(contract.NullableBothLeft(1, null), optimization.ToString());
        Assert.IsNull(contract.NullableBothLeft(null, null), optimization.ToString());
        Assert.AreEqual(new BigInteger(-4), contract.NullableBothRight(-8, 33), optimization.ToString());
        Assert.IsNull(contract.NullableBothRight(-8, null), optimization.ToString());
        Assert.IsNull(contract.NullableBothRight(null, 33), optimization.ToString());
        Assert.AreEqual(BigInteger.One, contract.NullLeftEvaluationCount(), optimization.ToString());
        Assert.AreEqual(new BigInteger(2), contract.CompoundInt(1, 33), optimization.ToString());
        Assert.AreEqual(BigInteger.Zero, contract.CompoundByte(0, 8), optimization.ToString());
        Assert.AreEqual(new BigInteger(2), contract.CompoundByte(1, 33), optimization.ToString());
        Assert.ThrowsException<TestException>(() => contract.CompoundByte(1, 8), optimization.ToString());
        Assert.AreEqual(new BigInteger(-4), contract.CompoundLongRight(-8, 65), optimization.ToString());
        Assert.AreEqual(BigInteger.One << 33, contract.BigIntegerLeft(1, 33), optimization.ToString());
        Assert.AreEqual(BigInteger.One << 33, contract.CompoundBigInteger(1, 33), optimization.ToString());
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void LiftedShift_WithNullableLeft_StillAppliesFixedWidthMask(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(static p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ShiftCountContract>(context.CreateExecutable(), context.CreateManifest());

        // 33 should be masked to 1 for int shifts (C# semantics)
        Assert.AreEqual(new BigInteger(2), contract.NullableBothLeft(1, 33));
        Assert.AreEqual(new BigInteger(-4), contract.NullableBothRight(-8, 33));

        // null propagation must remain intact
        Assert.IsNull(contract.NullableBothLeft(null, 33));
        Assert.IsNull(contract.NullableBothLeft(1, null));
        Assert.IsNull(contract.NullableBothRight(null, 33));
        Assert.IsNull(contract.NullableBothRight(-8, null));
        Assert.IsNull(contract.NullableBothLeft(null, null));
        Assert.IsNull(contract.NullableBothRight(null, null));
    }

    public abstract class ShiftCountContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("intLeftChecked")]
        public abstract BigInteger? IntLeftChecked(BigInteger? value, BigInteger? count);

        [DisplayName("intRightChecked")]
        public abstract BigInteger? IntRightChecked(BigInteger? value, BigInteger? count);

        [DisplayName("longLeftChecked")]
        public abstract BigInteger? LongLeftChecked(BigInteger? value, BigInteger? count);

        [DisplayName("longRightChecked")]
        public abstract BigInteger? LongRightChecked(BigInteger? value, BigInteger? count);

        [DisplayName("intLeftUnchecked")]
        public abstract BigInteger? IntLeftUnchecked(BigInteger? value, BigInteger? count);

        [DisplayName("nullableIntLeftChecked")]
        public abstract BigInteger? NullableIntLeftChecked(BigInteger? value, BigInteger? count);

        [DisplayName("nullableLongRightChecked")]
        public abstract BigInteger? NullableLongRightChecked(BigInteger? value, BigInteger? count);

        [DisplayName("nullableBothLeft")]
        public abstract BigInteger? NullableBothLeft(BigInteger? value, BigInteger? count);

        [DisplayName("nullableBothRight")]
        public abstract BigInteger? NullableBothRight(BigInteger? value, BigInteger? count);

        [DisplayName("nullLeftEvaluationCount")]
        public abstract BigInteger? NullLeftEvaluationCount();

        [DisplayName("compoundInt")]
        public abstract BigInteger? CompoundInt(BigInteger? value, BigInteger? count);

        [DisplayName("compoundByte")]
        public abstract BigInteger? CompoundByte(BigInteger? value, BigInteger? count);

        [DisplayName("compoundLongRight")]
        public abstract BigInteger? CompoundLongRight(BigInteger? value, BigInteger? count);

        [DisplayName("bigIntegerLeft")]
        public abstract BigInteger? BigIntegerLeft(BigInteger? value, BigInteger? count);

        [DisplayName("compoundBigInteger")]
        public abstract BigInteger? CompoundBigInteger(BigInteger? value, BigInteger? count);
    }
}
