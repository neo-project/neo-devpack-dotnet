// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_VariableSlotAllocation.cs file belongs to the neo project and is free
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
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_VariableSlotAllocation
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("single")]
            public static int Single(object value)
            {
                byte[] bytes = new byte[] { 1, 2, 3 };
                _ = bytes[..(value is int end ? end + 1 : (end = 0) + 1)];
                return end;
            }

            [DisplayName("pair")]
            public static int Pair(object startValue, object endValue)
            {
                byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                _ = bytes[(startValue is int start ? start + 1 : (start = 0))..
                    (endValue is int end ? end + 1 : (end = bytes.Length - 1) + 1)];
                return start * 10 + end;
            }
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ReleasedAnonymousSlots_DoNotOverwriteLivePatternLocals(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics.Select(static p => p.ToString()))}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<VariableSlotContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(1), contract.Single(1), optimization.ToString());
        Assert.AreEqual(new BigInteger(14), contract.Pair(1, 4), optimization.ToString());
    }

    public abstract class VariableSlotContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("single")]
        public abstract BigInteger? Single(object value);

        [DisplayName("pair")]
        public abstract BigInteger? Pair(object startValue, object endValue);
    }
}
