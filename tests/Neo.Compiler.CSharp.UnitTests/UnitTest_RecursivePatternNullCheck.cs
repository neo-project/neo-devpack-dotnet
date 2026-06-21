// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RecursivePatternNullCheck.cs file belongs to the neo project and is free
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
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RecursivePatternNullCheck
{
    [TestMethod]
    public void TypelessPropertyPattern_NullScrutinee_YieldsFalseInsteadOfFaulting()
    {
        // A typeless property pattern `box is { Value: 5 }` must null-check the scrutinee:
        // a null value never matches. Previously the property getter was invoked on the
        // null reference, faulting the VM instead of yielding false.
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    public class Box { public int Value { get; set; } }

    [DisplayName(""match"")]
    public static bool Match(bool useNull)
    {
        Box? box = useNull ? null : new Box { Value = 5 };
        return box is { Value: 5 };
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<RecursivePatternNullCheckContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsFalse(contract.Match(true)!.Value, "A null scrutinee must not match the property pattern.");
        Assert.IsTrue(contract.Match(false)!.Value, "A matching property value must match the pattern.");
    }

    public abstract class RecursivePatternNullCheckContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("match")]
        public abstract bool? Match(bool useNull);
    }
}
