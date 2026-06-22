// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SwitchExpressionThrow.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_SwitchExpressionThrow
{
    [TestMethod]
    public void NonExhaustiveSwitchExpression_ThrowsDescriptiveMessage()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""classify"")]
    public static string Classify(int x) => x switch
    {
        1 => ""one"",
        2 => ""two"",
    };
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<SwitchExpressionThrowContract>(context.CreateExecutable(), context.CreateManifest());

        // A matched arm still works.
        Assert.AreEqual("one", contract.Classify(1));

        // An unmatched value throws a meaningful message rather than the governing value.
        var ex = Assert.ThrowsException<TestException>(() => contract.Classify(5));
        StringAssert.Contains(ex.InnerException?.Message ?? ex.Message, "No switch arm matched");
    }

    public abstract class SwitchExpressionThrowContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("classify")]
        public abstract string? Classify(BigInteger x);
    }
}
