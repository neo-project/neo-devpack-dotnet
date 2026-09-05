// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StaticInitializerOptimization.cs file belongs to the neo project and is free
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

[TestClass]
public class UnitTest_StaticInitializerOptimization
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, 0, false)]
    [DataRow(CompilationOptions.OptimizationType.None, 1, false)]
    [DataRow(CompilationOptions.OptimizationType.Experimental, 0, false)]
    [DataRow(CompilationOptions.OptimizationType.Experimental, 1, false)]
    [DataRow(CompilationOptions.OptimizationType.All, 0, false)]
    [DataRow(CompilationOptions.OptimizationType.All, 1, false)]
    [DataRow(CompilationOptions.OptimizationType.None, 0, true)]
    [DataRow(CompilationOptions.OptimizationType.None, 1, true)]
    [DataRow(CompilationOptions.OptimizationType.Experimental, 0, true)]
    [DataRow(CompilationOptions.OptimizationType.Experimental, 1, true)]
    [DataRow(CompilationOptions.OptimizationType.All, 0, true)]
    [DataRow(CompilationOptions.OptimizationType.All, 1, true)]
    public void ConditionalRewritePreservesStaticInitializer(
        CompilationOptions.OptimizationType optimization, int timeParity, bool computedRewrite)
    {
        string replacement = computedRewrite ? "(int)(Runtime.Time % 2) + 1" : "2";
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Services;

            public class Contract : SmartContract
            {
                private static int value = 1;

                static Contract()
                {
                    if (Runtime.Time % 2 != 0)
                        value = {{replacement}};
                }

                public static int Get() => value;
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        int currentParity = (int)((ulong)engine.PersistingBlock.Timestamp.TotalMilliseconds % 2);
        if (currentParity != timeParity)
            engine.PersistingBlock.Advance(TimeSpan.FromMilliseconds(1));

        var contract = engine.Deploy<StaticInitializerContract>(nef, manifest);
        Assert.AreEqual(new BigInteger(timeParity == 0 ? 1 : 2), contract.Get());
    }

    public abstract class StaticInitializerContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("get")]
        public abstract BigInteger? Get();
    }
}
