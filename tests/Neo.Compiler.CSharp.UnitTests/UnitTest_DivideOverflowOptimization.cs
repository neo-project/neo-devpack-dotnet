// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DivideOverflowOptimization.cs file belongs to the neo project and is free
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
public class UnitTest_DivideOverflowOptimization
{
    private const string Source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("safeInt")]
    public static int SafeInt(int divisor) => 42 / divisor;

    [DisplayName("safeLong")]
    public static long SafeLong(long divisor) => 42L / divisor;

    [DisplayName("minInt")]
    public static int MinInt(int divisor) => int.MinValue / divisor;

    [DisplayName("minLong")]
    public static long MinLong(long divisor) => long.MinValue / divisor;

    [DisplayName("runtimeInt")]
    public static int RuntimeInt(int dividend, int divisor) => dividend / divisor;

    [DisplayName("runtimeLong")]
    public static long RuntimeLong(long dividend, long divisor) => dividend / divisor;
}
""";

    [TestMethod]
    public void SafeConstantDividendsSkipOverflowGuard()
    {
        var context = CompileContract();

        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeInt("), OpCode.THROW);
        CollectionAssert.DoesNotContain(GetMethodOpCodes(context, "Contract.SafeLong("), OpCode.THROW);

        var contract = Deploy(context);
        Assert.AreEqual(new BigInteger(-42), contract.SafeInt(-1));
        Assert.AreEqual(new BigInteger(-42), contract.SafeLong(-1));
    }

    [TestMethod]
    public void MinimumConstantDividendsKeepOverflowGuard()
    {
        var context = CompileContract();

        CollectionAssert.Contains(GetMethodOpCodes(context, "Contract.MinInt("), OpCode.THROW);
        CollectionAssert.Contains(GetMethodOpCodes(context, "Contract.MinLong("), OpCode.THROW);

        var contract = Deploy(context);
        Assert.ThrowsException<TestException>(() => contract.MinInt(-1));
        Assert.ThrowsException<TestException>(() => contract.MinLong(-1));
    }

    [TestMethod]
    public void RuntimeDividendsKeepOverflowGuard()
    {
        var context = CompileContract();

        CollectionAssert.Contains(GetMethodOpCodes(context, "Contract.RuntimeInt("), OpCode.THROW);
        CollectionAssert.Contains(GetMethodOpCodes(context, "Contract.RuntimeLong("), OpCode.THROW);

        var contract = Deploy(context);
        Assert.AreEqual(new BigInteger(20), contract.RuntimeInt(100, 5));
        Assert.AreEqual(new BigInteger(100), contract.RuntimeLong(1000, 10));
        Assert.ThrowsException<TestException>(() => contract.RuntimeInt(int.MinValue, -1));
        Assert.ThrowsException<TestException>(() => contract.RuntimeLong(long.MinValue, -1));
    }

    private static CompilationContext CompileContract()
    {
        var context = TestHelper.CompileSingleContract(Source, new CompilationOptions
        {
            Optimize = CompilationOptions.OptimizationType.None
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

    private static DivideOverflowContract Deploy(CompilationContext context)
    {
        var engine = new TestEngine(true);
        return engine.Deploy<DivideOverflowContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class DivideOverflowContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("safeInt")]
        public abstract BigInteger? SafeInt(BigInteger divisor);

        [DisplayName("safeLong")]
        public abstract BigInteger? SafeLong(BigInteger divisor);

        [DisplayName("minInt")]
        public abstract BigInteger? MinInt(BigInteger divisor);

        [DisplayName("minLong")]
        public abstract BigInteger? MinLong(BigInteger divisor);

        [DisplayName("runtimeInt")]
        public abstract BigInteger? RuntimeInt(BigInteger dividend, BigInteger divisor);

        [DisplayName("runtimeLong")]
        public abstract BigInteger? RuntimeLong(BigInteger dividend, BigInteger divisor);
    }
}
