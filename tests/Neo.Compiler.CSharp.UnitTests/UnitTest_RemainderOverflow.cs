// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RemainderOverflow.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RemainderOverflow
{
    private const string Source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Contract : SmartContract
{
    private class Holder
    {
        public int Field;
        public int Property { get; set; }
    }

    [DisplayName("checkedInt")]
    public static int CheckedInt(int dividend, int divisor) => checked(dividend % divisor);

    [DisplayName("uncheckedInt")]
    public static int UncheckedInt(int dividend, int divisor) => unchecked(dividend % divisor);

    [DisplayName("checkedLong")]
    public static long CheckedLong(long dividend, long divisor) => checked(dividend % divisor);

    [DisplayName("uncheckedLong")]
    public static long UncheckedLong(long dividend, long divisor) => unchecked(dividend % divisor);

    [DisplayName("compoundInt")]
    public static int CompoundInt(int dividend, int divisor)
    {
        dividend %= divisor;
        return dividend;
    }

    [DisplayName("compoundLong")]
    public static long CompoundLong(long dividend, long divisor)
    {
        dividend %= divisor;
        return dividend;
    }

    [DisplayName("safeDividend")]
    public static int SafeDividend(int divisor) => 42 % divisor;

    [DisplayName("safeDivisor")]
    public static int SafeDivisor(int dividend) => dividend % 3;

    [DisplayName("constantOverflow")]
    public static int ConstantOverflow() => int.MinValue % -1;

    [DisplayName("caughtArrayCompound")]
    public static int CaughtArrayCompound(int dividend, int divisor)
    {
        try
        {
            int[] values = [dividend];
            values[0] %= divisor;
            return 0;
        }
        catch
        {
            return 1;
        }
    }

    [DisplayName("caughtArrayDivide")]
    public static int CaughtArrayDivide(int dividend, int divisor)
    {
        try
        {
            int[] values = [dividend];
            values[0] /= divisor;
            return 0;
        }
        catch
        {
            return 1;
        }
    }

    [DisplayName("caughtFieldCompound")]
    public static int CaughtFieldCompound(int dividend, int divisor)
    {
        try
        {
            var holder = new Holder { Field = dividend };
            holder.Field %= divisor;
            return 0;
        }
        catch
        {
            return 1;
        }
    }

    [DisplayName("caughtPropertyDivide")]
    public static int CaughtPropertyDivide(int dividend, int divisor)
    {
        try
        {
            var holder = new Holder { Property = dividend };
            holder.Property /= divisor;
            return 0;
        }
        catch
        {
            return 1;
        }
    }

    [DisplayName("caughtOuterExpression")]
    public static int CaughtOuterExpression(int dividend, int divisor)
    {
        try
        {
            int[] values = [dividend];
            return 7 + (values[0] %= divisor);
        }
        catch
        {
            return 1;
        }
    }

    [DisplayName("callerOwnedOperand")]
    public static int CallerOwnedOperand(int dividend, int divisor) => 100 + SafeRemainder(dividend, divisor);

    [DisplayName("inlineThenMethodOverflow")]
    public static int InlineThenMethodOverflow(int safeDividend, int safeDivisor, int overflowDividend, int overflowDivisor) =>
        1000 + MethodWithInlineThenOverflow(safeDividend, safeDivisor, overflowDividend, overflowDivisor);

    private static int MethodWithInlineThenOverflow(int safeDividend, int safeDivisor, int overflowDividend, int overflowDivisor)
    {
        int prefix = 100 + SafeRemainder(safeDividend, safeDivisor);
        try
        {
            return prefix + overflowDividend % overflowDivisor;
        }
        catch
        {
            return prefix;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SafeRemainder(int dividend, int divisor)
    {
        try
        {
            return dividend % divisor;
        }
        catch
        {
            return 0;
        }
    }

    [DisplayName("compoundShort")]
    public static short CompoundShort(short dividend, short divisor)
    {
        dividend %= divisor;
        return dividend;
    }

    [DisplayName("binaryShort")]
    public static int BinaryShort(short dividend, short divisor) => dividend % divisor;

    [DisplayName("unsignedInt")]
    public static uint UnsignedInt(uint dividend, uint divisor) => dividend % divisor;
}
""";

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SignedRemainderOverflowThrowsInAllContexts(CompilationOptions.OptimizationType optimization)
    {
        var context = CompileContract(optimization);
        foreach (var method in new[]
                 {
                     "Contract.CheckedInt(",
                     "Contract.UncheckedInt(",
                     "Contract.CheckedLong(",
                     "Contract.UncheckedLong(",
                     "Contract.CompoundInt(",
                     "Contract.CompoundLong("
                 })
        {
            CollectionAssert.Contains(GetMethodOpCodes(context, method), OpCode.THROW);
        }

        var contract = Deploy(context);
        AssertOverflowFault(() => contract.CheckedInt(int.MinValue, -1));
        AssertOverflowFault(() => contract.UncheckedInt(int.MinValue, -1));
        AssertOverflowFault(() => contract.CheckedLong(long.MinValue, -1));
        AssertOverflowFault(() => contract.UncheckedLong(long.MinValue, -1));
        AssertOverflowFault(() => contract.CompoundInt(int.MinValue, -1));
        AssertOverflowFault(() => contract.CompoundLong(long.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.CaughtArrayCompound(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.CaughtArrayDivide(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.CaughtFieldCompound(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.CaughtPropertyDivide(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.CaughtOuterExpression(int.MinValue, -1));
        Assert.AreEqual(new BigInteger(100), contract.CallerOwnedOperand(int.MinValue, -1));
        Assert.AreEqual(new BigInteger(1101), contract.InlineThenMethodOverflow(10, 3, int.MinValue, -1));
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void OrdinaryRemaindersPreserveCSharpSemantics(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(CompileContract(optimization));

        Assert.AreEqual(new BigInteger(1), contract.CheckedInt(10, 3));
        Assert.AreEqual(new BigInteger(-1), contract.CheckedInt(-10, 3));
        Assert.AreEqual(new BigInteger(1), contract.UncheckedInt(10, -3));
        Assert.AreEqual(new BigInteger(-1), contract.UncheckedLong(-10, -3));
        Assert.AreEqual(BigInteger.Zero, contract.CompoundShort(short.MinValue, -1));
        Assert.AreEqual(BigInteger.Zero, contract.BinaryShort(short.MinValue, -1));
        Assert.AreEqual(new BigInteger(1), contract.UnsignedInt(uint.MaxValue, 2));
        Assert.AreEqual(BigInteger.Zero, contract.ConstantOverflow());
        Assert.ThrowsExactly<TestException>(() => contract.CheckedInt(1, 0));
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SafeConstantsSkipOverflowGuard(CompilationOptions.OptimizationType optimization)
    {
        var context = CompileContract(optimization);

        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeDividend("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeDivisor("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.ConstantOverflow("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.CompoundShort("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.BinaryShort("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.UnsignedInt("), OpCode.THROW);
    }

    private static CompilationContext CompileContract(CompilationOptions.OptimizationType optimization)
    {
        var context = TestHelper.CompileSingleContract(Source, new CompilationOptions
        {
            Optimize = optimization
        });
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        return context;
    }

    private static OpCode[] GetMethodOpCodes(CompilationContext context, string methodIdPrefix)
    {
        var methods = (JArray)context.CreateDebugInformation()["methods"]!;
        var method = methods
            .OfType<JObject>()
            .Single(m => m["id"]?.GetString().StartsWith(methodIdPrefix, StringComparison.Ordinal) == true);
        var range = method["range"]!.GetString().Split('-');
        var start = int.Parse(range[0]);
        var end = int.Parse(range[1]);

        return ((Script)context.CreateExecutable().Script)
            .EnumerateInstructions()
            .Where(i => i.address >= start && i.address <= end)
            .Select(i => i.instruction.OpCode)
            .ToArray();
    }

    private static RemainderOverflowContract Deploy(CompilationContext context)
    {
        var engine = new TestEngine(true);
        return engine.Deploy<RemainderOverflowContract>(context.CreateExecutable(), context.CreateManifest());
    }

    private static void AssertOverflowFault(Action action)
    {
        var exception = Assert.ThrowsExactly<TestException>(action);
        Assert.IsInstanceOfType<VMUnhandledException>(exception.InnerException);
        Assert.AreEqual("Overflow", ((VMUnhandledException)exception.InnerException).ExceptionObject.GetString());
    }

    public abstract class RemainderOverflowContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("checkedInt")]
        public abstract BigInteger? CheckedInt(BigInteger dividend, BigInteger divisor);

        [DisplayName("uncheckedInt")]
        public abstract BigInteger? UncheckedInt(BigInteger dividend, BigInteger divisor);

        [DisplayName("checkedLong")]
        public abstract BigInteger? CheckedLong(BigInteger dividend, BigInteger divisor);

        [DisplayName("uncheckedLong")]
        public abstract BigInteger? UncheckedLong(BigInteger dividend, BigInteger divisor);

        [DisplayName("compoundInt")]
        public abstract BigInteger? CompoundInt(BigInteger dividend, BigInteger divisor);

        [DisplayName("compoundLong")]
        public abstract BigInteger? CompoundLong(BigInteger dividend, BigInteger divisor);

        [DisplayName("safeDividend")]
        public abstract BigInteger? SafeDividend(BigInteger divisor);

        [DisplayName("safeDivisor")]
        public abstract BigInteger? SafeDivisor(BigInteger dividend);

        [DisplayName("constantOverflow")]
        public abstract BigInteger? ConstantOverflow();

        [DisplayName("caughtArrayCompound")]
        public abstract BigInteger? CaughtArrayCompound(BigInteger dividend, BigInteger divisor);

        [DisplayName("caughtArrayDivide")]
        public abstract BigInteger? CaughtArrayDivide(BigInteger dividend, BigInteger divisor);

        [DisplayName("caughtFieldCompound")]
        public abstract BigInteger? CaughtFieldCompound(BigInteger dividend, BigInteger divisor);

        [DisplayName("caughtPropertyDivide")]
        public abstract BigInteger? CaughtPropertyDivide(BigInteger dividend, BigInteger divisor);

        [DisplayName("caughtOuterExpression")]
        public abstract BigInteger? CaughtOuterExpression(BigInteger dividend, BigInteger divisor);

        [DisplayName("callerOwnedOperand")]
        public abstract BigInteger? CallerOwnedOperand(BigInteger dividend, BigInteger divisor);

        [DisplayName("inlineThenMethodOverflow")]
        public abstract BigInteger? InlineThenMethodOverflow(BigInteger safeDividend, BigInteger safeDivisor, BigInteger overflowDividend, BigInteger overflowDivisor);

        [DisplayName("compoundShort")]
        public abstract BigInteger? CompoundShort(BigInteger dividend, BigInteger divisor);

        [DisplayName("binaryShort")]
        public abstract BigInteger? BinaryShort(BigInteger dividend, BigInteger divisor);

        [DisplayName("unsignedInt")]
        public abstract BigInteger? UnsignedInt(BigInteger dividend, BigInteger divisor);
    }
}
