// Copyright (C) 2015-2026 The Neo Project.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NestedFinallyCatch
{
    private const string Source = """
        using System;
        using System.Runtime.CompilerServices;
        using Neo.SmartContract.Framework;

        public class Cases : SmartContract
        {
            private static int s_recorded;
            private static int s_finallyRuns;

            public static int ReturnFromTry()
            {
                try { return 1; }
                finally { try { throw new Exception("inner"); } catch { } }
            }

            public static string ReturnFromCatch()
            {
                try { throw new Exception("outer"); }
                catch { return "recovered"; }
                finally { try { throw new Exception("inner"); } catch { } }
            }

            public static int ReturnWithCaughtDivInFinally(int divisor)
            {
                try { return 42; }
                finally { try { _ = 1 / divisor; } catch { } }
            }

            public static int ReturnWithCaughtDivRemInFinally(int divisor)
            {
                try { return 42; }
                finally { try { _ = Math.DivRem(1, divisor); } catch { } }
            }

            public static int ReturnWithCaughtSubstringInFinally(int index)
            {
                try { return 42; }
                finally { try { _ = "abc".Substring(index); } catch { } }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int ClampPositive(int x)
            {
                if (x < 0) return 0;
                return x;
            }

            public static int InlineInsideTryNormalPath(int x)
            {
                s_finallyRuns = 0;
                try
                {
                    int value = ClampPositive(x);
                    if (s_finallyRuns != 0) return -999;
                    return value + 10;
                }
                finally { s_finallyRuns++; }
            }

            public static int InlineInsideTryPreservesCatch(int x)
            {
                try
                {
                    _ = ClampPositive(x);
                    throw new Exception("after inline return");
                }
                catch { return 77; }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int InlineDefaultWithFinallyDiv(int divisor)
            {
                try { return 5; }
                finally { try { _ = 1 / divisor; } catch { } }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int CaptureDefaultAndAbortIfZero(int divisor)
            {
                int value = InlineDefaultWithFinallyDiv(divisor);
                s_recorded = value;
                if (divisor == 0) throw new Exception("stop");
                return value;
            }

            public static int OuterExpressionIsolation(int divisor)
            {
                s_recorded = 0;
                try { _ = 999999 + CaptureDefaultAndAbortIfZero(divisor); }
                catch { }
                return s_recorded;
            }

        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void PendingReturnSurvivesInnerCatch(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(1), contract.ReturnFromTry());
        Assert.AreEqual("recovered", contract.ReturnFromCatch());
        Assert.AreEqual(new BigInteger(42), contract.ReturnWithCaughtDivInFinally(0));
        Assert.AreEqual(new BigInteger(42), contract.ReturnWithCaughtDivRemInFinally(0));
        Assert.AreEqual(new BigInteger(42), contract.ReturnWithCaughtSubstringInFinally(5));
        Assert.AreEqual(new BigInteger(15), contract.InlineInsideTryNormalPath(5));
        Assert.AreEqual(new BigInteger(10), contract.InlineInsideTryNormalPath(-3));
        Assert.AreEqual(new BigInteger(77), contract.InlineInsideTryPreservesCatch(5));
        Assert.AreEqual(new BigInteger(5), contract.OuterExpressionIsolation(1));
        Assert.AreEqual(new BigInteger(5), contract.OuterExpressionIsolation(0));
    }

    private static CasesContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        return new TestEngine(true).Deploy<CasesContract>(nef, manifest);
    }

    public abstract class CasesContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("returnFromTry")] public abstract BigInteger? ReturnFromTry();
        [DisplayName("returnFromCatch")] public abstract string? ReturnFromCatch();
        [DisplayName("returnWithCaughtDivInFinally")] public abstract BigInteger? ReturnWithCaughtDivInFinally(BigInteger divisor);
        [DisplayName("returnWithCaughtDivRemInFinally")] public abstract BigInteger? ReturnWithCaughtDivRemInFinally(BigInteger divisor);
        [DisplayName("returnWithCaughtSubstringInFinally")] public abstract BigInteger? ReturnWithCaughtSubstringInFinally(BigInteger index);
        [DisplayName("inlineInsideTryNormalPath")] public abstract BigInteger? InlineInsideTryNormalPath(BigInteger x);
        [DisplayName("inlineInsideTryPreservesCatch")] public abstract BigInteger? InlineInsideTryPreservesCatch(BigInteger x);
        [DisplayName("outerExpressionIsolation")] public abstract BigInteger? OuterExpressionIsolation(BigInteger divisor);
    }
}
