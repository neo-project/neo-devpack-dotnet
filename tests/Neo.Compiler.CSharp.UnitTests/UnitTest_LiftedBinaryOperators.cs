// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LiftedBinaryOperators.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_LiftedBinaryOperators
{
    private const string Source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;
using System.Numerics;

public class Contract : SmartContract
{
    [System.Flags]
    public enum Options
    {
        None = 0,
        First = 1,
        Second = 2
    }

    private static int _evaluations;

    [DisplayName("add")]
    public static int? Add(int? left, int? right) => left + right;

    [DisplayName("subtract")]
    public static int? Subtract(int? left, int? right) => left - right;

    [DisplayName("multiply")]
    public static int? Multiply(int? left, int? right) => left * right;

    [DisplayName("divide")]
    public static int? Divide(int? left, int? right) => left / right;

    [DisplayName("remainder")]
    public static int? Remainder(int? left, int? right) => left % right;

    [DisplayName("and")]
    public static int? And(int? left, int? right) => left & right;

    [DisplayName("or")]
    public static int? Or(int? left, int? right) => left | right;

    [DisplayName("xor")]
    public static int? Xor(int? left, int? right) => left ^ right;

    [DisplayName("addByte")]
    public static int? AddByte(byte? left, byte? right) => left + right;

    [DisplayName("addLong")]
    public static long? AddLong(long? left, long? right) => left + right;

    [DisplayName("addUInt")]
    public static uint? AddUInt(uint? left, uint? right) => left + right;

    [DisplayName("addULong")]
    public static ulong? AddULong(ulong? left, ulong? right) => left + right;

    [DisplayName("addBigInteger")]
    public static BigInteger? AddBigInteger(BigInteger? left, BigInteger? right) => left + right;

    [DisplayName("orOptions")]
    public static Options? OrOptions(Options? left, Options? right) => left | right;

    [DisplayName("outerAdd")]
    public static int? OuterAdd(int? left, int? right) => 10 + (left + right);

    [DisplayName("checkedAdd")]
    public static int? CheckedAdd(int? left, int? right) => checked(left + right);

    [DisplayName("addAssign")]
    public static int? AddAssign(int? left, int? right)
    {
        left += right;
        return left;
    }

    [DisplayName("subtractAssign")]
    public static int? SubtractAssign(int? left, int? right)
    {
        left -= right;
        return left;
    }

    [DisplayName("multiplyAssign")]
    public static int? MultiplyAssign(int? left, int? right)
    {
        left *= right;
        return left;
    }

    [DisplayName("divideAssign")]
    public static int? DivideAssign(int? left, int? right)
    {
        left /= right;
        return left;
    }

    [DisplayName("remainderAssign")]
    public static int? RemainderAssign(int? left, int? right)
    {
        left %= right;
        return left;
    }

    [DisplayName("andAssign")]
    public static int? AndAssign(int? left, int? right)
    {
        left &= right;
        return left;
    }

    [DisplayName("orAssign")]
    public static int? OrAssign(int? left, int? right)
    {
        left |= right;
        return left;
    }

    [DisplayName("xorAssign")]
    public static int? XorAssign(int? left, int? right)
    {
        left ^= right;
        return left;
    }

    [DisplayName("addBigIntegerAssign")]
    public static BigInteger? AddBigIntegerAssign(BigInteger? left, BigInteger? right)
    {
        left += right;
        return left;
    }

    [DisplayName("lessThan")]
    public static bool LessThan(int? left, int? right) => left < right;

    [DisplayName("lessThanOrEqual")]
    public static bool LessThanOrEqual(int? left, int? right) => left <= right;

    [DisplayName("greaterThan")]
    public static bool GreaterThan(int? left, int? right) => left > right;

    [DisplayName("greaterThanOrEqual")]
    public static bool GreaterThanOrEqual(int? left, int? right) => left >= right;

    [DisplayName("equal")]
    public static bool Equal(int? left, int? right) => left == right;

    [DisplayName("notEqual")]
    public static bool NotEqual(int? left, int? right) => left != right;

    [DisplayName("boolAnd")]
    public static bool? BoolAnd(bool? left, bool? right) => left & right;

    [DisplayName("boolOr")]
    public static bool? BoolOr(bool? left, bool? right) => left | right;

    [DisplayName("boolXor")]
    public static bool? BoolXor(bool? left, bool? right) => left ^ right;

    [DisplayName("boolAndAssign")]
    public static bool? BoolAndAssign(bool? left, bool? right)
    {
        left &= right;
        return left;
    }

    [DisplayName("boolOrAssign")]
    public static bool? BoolOrAssign(bool? left, bool? right)
    {
        left |= right;
        return left;
    }

    [DisplayName("nullLeftEvaluationCount")]
    public static int NullLeftEvaluationCount()
    {
        _evaluations = 0;
        int? left = null;
        _ = left + Evaluate();
        return _evaluations;
    }

    [DisplayName("nullRightEvaluationCount")]
    public static int NullRightEvaluationCount()
    {
        _evaluations = 0;
        int? right = null;
        _ = Evaluate() + right;
        return _evaluations;
    }

    [DisplayName("nullCompoundEvaluationCount")]
    public static int NullCompoundEvaluationCount()
    {
        _evaluations = 0;
        int? left = null;
        left += Evaluate();
        return _evaluations;
    }

    private static int Evaluate() => ++_evaluations;
}
""";

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullableIntegralOperatorsPropagateNull(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);

        AssertNullPropagation(contract.Add);
        AssertNullPropagation(contract.Subtract);
        AssertNullPropagation(contract.Multiply);
        AssertNullPropagation(contract.Divide);
        AssertNullPropagation(contract.Remainder);
        AssertNullPropagation(contract.And);
        AssertNullPropagation(contract.Or);
        AssertNullPropagation(contract.Xor);
        AssertNullPropagation(contract.AddByte);
        AssertNullPropagation(contract.AddLong);
        AssertNullPropagation(contract.AddUInt);
        AssertNullPropagation(contract.AddULong);
        AssertNullPropagation(contract.AddBigInteger);
        AssertNullPropagation(contract.OrOptions);
        AssertNullPropagation(contract.OuterAdd);
        AssertNullPropagation(contract.CheckedAdd);
        Assert.AreEqual(new BigInteger(5), contract.Add(2, 3));
        Assert.AreEqual(new BigInteger(-1), contract.Subtract(2, 3));
        Assert.AreEqual(new BigInteger(6), contract.Multiply(2, 3));
        Assert.AreEqual(new BigInteger(2), contract.Divide(7, 3));
        Assert.AreEqual(BigInteger.One, contract.Remainder(7, 3));
        Assert.AreEqual(new BigInteger(2), contract.And(6, 3));
        Assert.AreEqual(new BigInteger(7), contract.Or(6, 3));
        Assert.AreEqual(new BigInteger(5), contract.Xor(6, 3));
        Assert.AreEqual(new BigInteger(3), contract.AddByte(1, 2));
        Assert.AreEqual(new BigInteger(3), contract.AddLong(1, 2));
        Assert.AreEqual(new BigInteger(3), contract.AddUInt(1, 2));
        Assert.AreEqual(new BigInteger(3), contract.AddULong(1, 2));
        Assert.AreEqual(new BigInteger(3), contract.AddBigInteger(1, 2));
        Assert.AreEqual(new BigInteger(3), contract.OrOptions(1, 2));
        Assert.AreEqual(new BigInteger(15), contract.OuterAdd(2, 3));
        AssertFaultInteger(() => contract.CheckedAdd(int.MaxValue, 1), (BigInteger)int.MaxValue + 1);
        AssertOverflowFault(() => contract.Divide(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.NullLeftEvaluationCount());
        Assert.AreEqual(BigInteger.One, contract.NullRightEvaluationCount());
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullableIntegralCompoundAssignmentsPropagateNull(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);

        AssertNullPropagation(contract.AddAssign);
        AssertNullPropagation(contract.SubtractAssign);
        AssertNullPropagation(contract.MultiplyAssign);
        AssertNullPropagation(contract.DivideAssign);
        AssertNullPropagation(contract.RemainderAssign);
        AssertNullPropagation(contract.AndAssign);
        AssertNullPropagation(contract.OrAssign);
        AssertNullPropagation(contract.XorAssign);
        AssertNullPropagation(contract.AddBigIntegerAssign);
        Assert.AreEqual(new BigInteger(5), contract.AddAssign(2, 3));
        Assert.AreEqual(new BigInteger(-1), contract.SubtractAssign(2, 3));
        Assert.AreEqual(new BigInteger(6), contract.MultiplyAssign(2, 3));
        Assert.AreEqual(new BigInteger(2), contract.DivideAssign(7, 3));
        Assert.AreEqual(BigInteger.One, contract.RemainderAssign(7, 3));
        Assert.AreEqual(new BigInteger(2), contract.AndAssign(6, 3));
        Assert.AreEqual(new BigInteger(7), contract.OrAssign(6, 3));
        Assert.AreEqual(new BigInteger(5), contract.XorAssign(6, 3));
        Assert.AreEqual(new BigInteger(5), contract.AddBigIntegerAssign(2, 3));
        AssertOverflowFault(() => contract.DivideAssign(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.NullCompoundEvaluationCount());
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullableRelationalOperatorsReturnFalseForNull(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);

        Assert.IsFalse(contract.LessThan(null, 2));
        Assert.IsFalse(contract.LessThan(2, null));
        Assert.IsFalse(contract.LessThanOrEqual(null, null));
        Assert.IsFalse(contract.GreaterThan(null, 2));
        Assert.IsFalse(contract.GreaterThan(2, null));
        Assert.IsFalse(contract.GreaterThanOrEqual(null, null));
        Assert.IsTrue(contract.LessThan(1, 2));
        Assert.IsTrue(contract.LessThanOrEqual(2, 2));
        Assert.IsTrue(contract.GreaterThan(2, 1));
        Assert.IsTrue(contract.GreaterThanOrEqual(2, 2));

        Assert.IsTrue(contract.Equal(null, null));
        Assert.IsFalse(contract.Equal(null, 2));
        Assert.IsTrue(contract.NotEqual(null, 2));
        Assert.IsFalse(contract.NotEqual(null, null));
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullableBooleanBitwiseOperatorsUseThreeValuedLogic(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);

        Assert.IsFalse(contract.BoolAnd(false, null));
        Assert.IsFalse(contract.BoolAnd(null, false));
        Assert.IsNull(contract.BoolAnd(true, null));
        Assert.IsNull(contract.BoolAnd(null, true));
        Assert.IsNull(contract.BoolAnd(null, null));

        Assert.IsTrue(contract.BoolOr(true, null));
        Assert.IsTrue(contract.BoolOr(null, true));
        Assert.IsNull(contract.BoolOr(false, null));
        Assert.IsNull(contract.BoolOr(null, false));
        Assert.IsNull(contract.BoolOr(null, null));

        Assert.IsNull(contract.BoolXor(true, null));
        Assert.IsNull(contract.BoolXor(null, false));
        Assert.IsFalse(contract.BoolXor(true, true));
        Assert.IsTrue(contract.BoolXor(true, false));

        Assert.IsFalse(contract.BoolAndAssign(false, null));
        Assert.IsNull(contract.BoolAndAssign(true, null));
        Assert.IsTrue(contract.BoolOrAssign(true, null));
        Assert.IsNull(contract.BoolOrAssign(false, null));
    }

    private static LiftedBinaryContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(static p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<LiftedBinaryContract>(context.CreateExecutable(), context.CreateManifest());
    }

    private static void AssertNullPropagation(Func<BigInteger?, BigInteger?, BigInteger?> operation)
    {
        Assert.IsNull(operation(null, 2));
        Assert.IsNull(operation(2, null));
        Assert.IsNull(operation(null, null));
    }

    private static void AssertOverflowFault(Action operation)
    {
        var vmException = GetVmUnhandledException(operation);
        Assert.AreEqual("Overflow", vmException.ExceptionObject.GetString());
    }

    private static void AssertFaultInteger(Action operation, BigInteger expected)
    {
        var vmException = GetVmUnhandledException(operation);
        Assert.AreEqual(expected, vmException.ExceptionObject.GetInteger());
    }

    private static VMUnhandledException GetVmUnhandledException(Action operation)
    {
        var exception = Assert.ThrowsExactly<TestException>(operation);
        Assert.IsInstanceOfType<VMUnhandledException>(exception.InnerException);
        return (VMUnhandledException)exception.InnerException;
    }

    public abstract class LiftedBinaryContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("add")]
        public abstract BigInteger? Add(BigInteger? left, BigInteger? right);

        [DisplayName("subtract")]
        public abstract BigInteger? Subtract(BigInteger? left, BigInteger? right);

        [DisplayName("multiply")]
        public abstract BigInteger? Multiply(BigInteger? left, BigInteger? right);

        [DisplayName("divide")]
        public abstract BigInteger? Divide(BigInteger? left, BigInteger? right);

        [DisplayName("remainder")]
        public abstract BigInteger? Remainder(BigInteger? left, BigInteger? right);

        [DisplayName("and")]
        public abstract BigInteger? And(BigInteger? left, BigInteger? right);

        [DisplayName("or")]
        public abstract BigInteger? Or(BigInteger? left, BigInteger? right);

        [DisplayName("xor")]
        public abstract BigInteger? Xor(BigInteger? left, BigInteger? right);

        [DisplayName("addByte")]
        public abstract BigInteger? AddByte(BigInteger? left, BigInteger? right);

        [DisplayName("addLong")]
        public abstract BigInteger? AddLong(BigInteger? left, BigInteger? right);

        [DisplayName("addUInt")]
        public abstract BigInteger? AddUInt(BigInteger? left, BigInteger? right);

        [DisplayName("addULong")]
        public abstract BigInteger? AddULong(BigInteger? left, BigInteger? right);

        [DisplayName("addBigInteger")]
        public abstract BigInteger? AddBigInteger(BigInteger? left, BigInteger? right);

        [DisplayName("orOptions")]
        public abstract BigInteger? OrOptions(BigInteger? left, BigInteger? right);

        [DisplayName("outerAdd")]
        public abstract BigInteger? OuterAdd(BigInteger? left, BigInteger? right);

        [DisplayName("checkedAdd")]
        public abstract BigInteger? CheckedAdd(BigInteger? left, BigInteger? right);

        [DisplayName("addAssign")]
        public abstract BigInteger? AddAssign(BigInteger? left, BigInteger? right);

        [DisplayName("subtractAssign")]
        public abstract BigInteger? SubtractAssign(BigInteger? left, BigInteger? right);

        [DisplayName("multiplyAssign")]
        public abstract BigInteger? MultiplyAssign(BigInteger? left, BigInteger? right);

        [DisplayName("divideAssign")]
        public abstract BigInteger? DivideAssign(BigInteger? left, BigInteger? right);

        [DisplayName("remainderAssign")]
        public abstract BigInteger? RemainderAssign(BigInteger? left, BigInteger? right);

        [DisplayName("andAssign")]
        public abstract BigInteger? AndAssign(BigInteger? left, BigInteger? right);

        [DisplayName("orAssign")]
        public abstract BigInteger? OrAssign(BigInteger? left, BigInteger? right);

        [DisplayName("xorAssign")]
        public abstract BigInteger? XorAssign(BigInteger? left, BigInteger? right);

        [DisplayName("addBigIntegerAssign")]
        public abstract BigInteger? AddBigIntegerAssign(BigInteger? left, BigInteger? right);

        [DisplayName("lessThan")]
        public abstract bool LessThan(BigInteger? left, BigInteger? right);

        [DisplayName("lessThanOrEqual")]
        public abstract bool LessThanOrEqual(BigInteger? left, BigInteger? right);

        [DisplayName("greaterThan")]
        public abstract bool GreaterThan(BigInteger? left, BigInteger? right);

        [DisplayName("greaterThanOrEqual")]
        public abstract bool GreaterThanOrEqual(BigInteger? left, BigInteger? right);

        [DisplayName("equal")]
        public abstract bool Equal(BigInteger? left, BigInteger? right);

        [DisplayName("notEqual")]
        public abstract bool NotEqual(BigInteger? left, BigInteger? right);

        [DisplayName("boolAnd")]
        public abstract bool? BoolAnd(bool? left, bool? right);

        [DisplayName("boolOr")]
        public abstract bool? BoolOr(bool? left, bool? right);

        [DisplayName("boolXor")]
        public abstract bool? BoolXor(bool? left, bool? right);

        [DisplayName("boolAndAssign")]
        public abstract bool? BoolAndAssign(bool? left, bool? right);

        [DisplayName("boolOrAssign")]
        public abstract bool? BoolOrAssign(bool? left, bool? right);

        [DisplayName("nullLeftEvaluationCount")]
        public abstract BigInteger? NullLeftEvaluationCount();

        [DisplayName("nullRightEvaluationCount")]
        public abstract BigInteger? NullRightEvaluationCount();

        [DisplayName("nullCompoundEvaluationCount")]
        public abstract BigInteger? NullCompoundEvaluationCount();
    }
}
