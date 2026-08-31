// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DivRemOverflowOptimization.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DivRemOverflowOptimization
{
    private const string Source = """
using Neo.SmartContract.Framework;
using System;
using System.Runtime.CompilerServices;

public class Contract : SmartContract
{
    public static object SafeInt(int divisor) => int.DivRem(42, divisor);
    public static object SafeLong(long divisor) => long.DivRem(42L, divisor);
    public static object SafeMathInt(int dividend) => Math.DivRem(dividend, 3);
    public static object SafeMathLong(long dividend) => Math.DivRem(dividend, 3L);
    public static object RuntimeInt(int dividend, int divisor) => int.DivRem(dividend, divisor);
    public static object RuntimeLong(long dividend, long divisor) => Math.DivRem(dividend, divisor);

    public static int CaughtInt(int dividend, int divisor)
    {
        try
        {
            return 7 + int.DivRem(dividend, divisor).Quotient;
        }
        catch
        {
            return 1;
        }
    }

    public static long CaughtMathLong(long dividend, long divisor)
    {
        try
        {
            return 7 + Math.DivRem(dividend, divisor).Quotient;
        }
        catch
        {
            return 1;
        }
    }

    public static int CallerInt(int dividend, int divisor) => 100 + SafeIntHelper(dividend, divisor);

    public static long CallerMathLong(long dividend, long divisor) => 100 + SafeMathLongHelper(dividend, divisor);

    public static int InlineThenMethodOverflow(int safeDividend, int safeDivisor, int overflowDividend, int overflowDivisor) =>
        1000 + MethodWithInlineThenOverflow(safeDividend, safeDivisor, overflowDividend, overflowDivisor);

    private static int MethodWithInlineThenOverflow(int safeDividend, int safeDivisor, int overflowDividend, int overflowDivisor)
    {
        int prefix = 100 + SafeIntHelper(safeDividend, safeDivisor);
        try
        {
            return prefix + int.DivRem(overflowDividend, overflowDivisor).Quotient;
        }
        catch
        {
            return prefix;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SafeIntHelper(int dividend, int divisor)
    {
        try
        {
            return int.DivRem(dividend, divisor).Quotient;
        }
        catch
        {
            return 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SafeMathLongHelper(long dividend, long divisor)
    {
        try
        {
            return Math.DivRem(dividend, divisor).Quotient;
        }
        catch
        {
            return 0;
        }
    }
}
""";

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SafeConstantsSkipOverflowGuard(CompilationOptions.OptimizationType optimization)
    {
        var context = CompileContract(optimization);

        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeInt("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeLong("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeMathInt("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeMathLong("), OpCode.THROW);
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void RuntimeArgumentsKeepOverflowGuard(CompilationOptions.OptimizationType optimization)
    {
        var context = CompileContract(optimization);

        CollectionAssert.Contains(GetMethodOpCodes(context, "Contract.RuntimeInt("), OpCode.THROW);
        CollectionAssert.Contains(GetMethodOpCodes(context, "Contract.RuntimeLong("), OpCode.THROW);
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CatchPathsPreserveCallerOwnedValues(CompilationOptions.OptimizationType optimization)
    {
        var context = CompileContract(optimization);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<DivRemOverflowContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(BigInteger.One, contract.CaughtInt(int.MinValue, -1));
        Assert.AreEqual(BigInteger.One, contract.CaughtMathLong(long.MinValue, -1));
        Assert.AreEqual(new BigInteger(100), contract.CallerInt(int.MinValue, -1));
        Assert.AreEqual(new BigInteger(100), contract.CallerMathLong(long.MinValue, -1));
        Assert.AreEqual(new BigInteger(1103), contract.InlineThenMethodOverflow(10, 3, int.MinValue, -1));
        Assert.AreEqual(new BigInteger(10), contract.CaughtInt(10, 3));
        Assert.AreEqual(new BigInteger(10), contract.CaughtMathLong(10, 3));
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

    public abstract class DivRemOverflowContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("caughtInt")]
        public abstract BigInteger? CaughtInt(BigInteger dividend, BigInteger divisor);

        [DisplayName("caughtMathLong")]
        public abstract BigInteger? CaughtMathLong(BigInteger dividend, BigInteger divisor);

        [DisplayName("callerInt")]
        public abstract BigInteger? CallerInt(BigInteger dividend, BigInteger divisor);

        [DisplayName("callerMathLong")]
        public abstract BigInteger? CallerMathLong(BigInteger dividend, BigInteger divisor);

        [DisplayName("inlineThenMethodOverflow")]
        public abstract BigInteger? InlineThenMethodOverflow(BigInteger safeDividend, BigInteger safeDivisor, BigInteger overflowDividend, BigInteger overflowDivisor);
    }
}
