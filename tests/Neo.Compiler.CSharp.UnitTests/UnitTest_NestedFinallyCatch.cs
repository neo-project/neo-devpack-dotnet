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
        using Neo.SmartContract.Framework;

        public class Cases : SmartContract
        {
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
    }
}
