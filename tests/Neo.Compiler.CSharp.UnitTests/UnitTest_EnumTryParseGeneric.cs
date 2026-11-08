// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_EnumTryParseGeneric.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_EnumTryParseGeneric
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void TryParsePreservesStackAndAssignsResult(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private enum TestEnum { Value1 = 1, Value2 = 2 }

                [DisplayName("test")]
                public static object[] Test(string value, bool ignoreCase)
                {
                    bool success = Enum.TryParse<TestEnum>(value, ignoreCase, out var result);
                    int continuation = (int)result + 10;
                    return new object[] { success, (int)result, continuation };
                }
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<EnumTryParseContract>(context.CreateExecutable(), context.CreateManifest());
        foreach (bool ignoreCase in new[] { false, true })
        {
            foreach (var (value, expected) in new[]
            {
                ("Value1", 1), ("Value2", 2), ("value2", ignoreCase ? 2 : 0), ("missing", 0), ("", 0)
            })
            {
                var result = contract.Test(value, ignoreCase);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Length);
                Assert.AreEqual(expected != 0, ((StackItem)result[0]!).GetBoolean());
                Assert.AreEqual(expected, ((StackItem)result[1]!).GetInteger());
                Assert.AreEqual(expected + 10, ((StackItem)result[2]!).GetInteger());
            }
        }
    }

    public abstract class EnumTryParseContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract object?[]? Test(string value, bool ignoreCase);
    }
}
