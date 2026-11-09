// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CapturedConditionalLocals.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CapturedConditionalLocals
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;

        public class Contract : SmartContract
        {
            public static bool Guard(bool useCallback)
            {
                bool paused = true;
                Func<bool> callback = null;
                if (useCallback) callback = () => paused;
                if (paused) return false;
                return true;
            }

            public static bool Parameter(bool useCallback, bool paused)
            {
                Func<bool> callback = null;
                if (useCallback) callback = () => paused;
                return paused;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ConditionalCaptureKeepsOneStorageOnEveryPath(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<ConditionalContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(false, contract.Guard(false));
        Assert.AreEqual(false, contract.Guard(true));
        Assert.AreEqual(false, contract.Guard(false));
        Assert.AreEqual(true, contract.Parameter(false, true));
        Assert.AreEqual(false, contract.Parameter(true, false));
        Assert.AreEqual(true, contract.Parameter(false, true));
    }

    public abstract class ConditionalContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("guard")] public abstract bool? Guard(bool useCallback);
        [DisplayName("parameter")] public abstract bool? Parameter(bool useCallback, bool paused);
    }
}
