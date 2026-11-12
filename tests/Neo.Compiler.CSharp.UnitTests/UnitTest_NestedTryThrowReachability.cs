// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NestedTryThrowReachability.cs file belongs to the neo project and is free
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
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/neo-project/neo-devpack-dotnet/issues/2290
/// </summary>
[TestClass]
public class UnitTest_NestedTryThrowReachability
{
    private const string Source = """
        using System;
        using Neo.SmartContract.Framework;

        public class Cases : SmartContract
        {
            public static int ThrowInFinallyReplaces()
            {
                int log = 0;
                try
                {
                    try { throw new Exception("a"); }
                    finally { log = 5; }
                }
                catch { log = log * 10 + 1; }
                return log;
            }

            public static int NestedTryFinally()
            {
                int log = 0;
                try
                {
                    try { log = 1; }
                    finally { log = log * 10 + 2; }
                }
                finally { log = log * 10 + 3; }
                return log;
            }

            public static int NestedTryFinallyWithThrow()
            {
                int log = 0;
                try
                {
                    try
                    {
                        try { throw new Exception("a"); }
                        finally { log = 1; }
                    }
                    finally { log = log * 10 + 2; }
                }
                catch { log = log * 10 + 3; }
                return log;
            }

            public static int ThrowInCatch()
            {
                int log = 0;
                try
                {
                    try { throw new Exception("a"); }
                    catch { log = 1; throw new Exception("b"); }
                }
                catch { log = log * 10 + 2; }
                return log;
            }

            public static int ThrowInCatchWithFinally()
            {
                int log = 0;
                try
                {
                    try { throw new Exception("a"); }
                    catch { log = 1; throw new Exception("b"); }
                    finally { log = log * 10 + 2; }
                }
                catch { log = log * 10 + 3; }
                return log;
            }

            public static int ThrowInFinally()
            {
                int log = 0;
                try
                {
                    try { log = 1; }
                    finally { log = log * 10 + 2; throw new Exception("b"); }
                }
                catch { log = log * 10 + 3; }
                return log;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ThrowInFinallyReplacesReturns51(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(51), contract.ThrowInFinallyReplaces());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NestedTryFinallyRunsBothFinallyBlocks(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(123), contract.NestedTryFinally());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NestedTryFinallyWithThrowRunsFinallyThenCatch(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(123), contract.NestedTryFinallyWithThrow());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ThrowInCatchIsCaughtByOuterCatch(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(12), contract.ThrowInCatch());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ThrowInCatchRunsFinallyBeforeOuterCatch(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(123), contract.ThrowInCatchWithFinally());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ThrowInFinallyIsCaughtByOuterCatch(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(123), contract.ThrowInFinally());
    }

    private static CasesContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        return engine.Deploy<CasesContract>(nef, manifest);
    }

    public abstract class CasesContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("throwInFinallyReplaces")] public abstract BigInteger? ThrowInFinallyReplaces();
        [DisplayName("nestedTryFinally")] public abstract BigInteger? NestedTryFinally();
        [DisplayName("nestedTryFinallyWithThrow")] public abstract BigInteger? NestedTryFinallyWithThrow();
        [DisplayName("throwInCatch")] public abstract BigInteger? ThrowInCatch();
        [DisplayName("throwInCatchWithFinally")] public abstract BigInteger? ThrowInCatchWithFinally();
        [DisplayName("throwInFinally")] public abstract BigInteger? ThrowInFinally();
    }
}
