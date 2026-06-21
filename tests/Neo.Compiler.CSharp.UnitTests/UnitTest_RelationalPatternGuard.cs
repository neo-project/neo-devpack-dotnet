// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RelationalPatternGuard.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RelationalPatternGuard
{
    [TestMethod]
    public void RelationalPattern_NullableScrutinee_IsRejected()
    {
        // `int? n; n is > 5` would fault the VM when n is null, instead of yielding false.
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static bool Test(int? n) => n is > 5;
}";

        var context = TestHelper.CompileSingleContract(source);

        Assert.IsFalse(context.Success, "Relational pattern on a nullable scrutinee must be rejected.");
        Assert.IsTrue(
            context.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error && d.GetMessage().Contains("Relational patterns")),
            "Expected the relational-pattern compatibility diagnostic.");
    }

    [TestMethod]
    public void RelationalPattern_ObjectScrutinee_IsRejected()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static bool Test(object value) => value is > 5;
}";

        var context = TestHelper.CompileSingleContract(source);

        Assert.IsFalse(context.Success, "Relational pattern on an object scrutinee must be rejected.");
        Assert.IsTrue(
            context.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error && d.GetMessage().Contains("Relational patterns")),
            "Expected the relational-pattern compatibility diagnostic.");
    }

    [TestMethod]
    public void RelationalPattern_ObjectScrutineeWithNumericTypePattern_Compiles()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static bool Test(object value) => value is int and > 5;
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<RelationalPatternGuardContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsTrue(contract.Test(10)!.Value);
        Assert.IsFalse(contract.Test(3)!.Value);
    }

    [TestMethod]
    public void RelationalPattern_NumericScrutinee_Compiles()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static bool Test(int value) => value is > 5;
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<RelationalPatternGuardContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsTrue(contract.Test(10)!.Value);
        Assert.IsFalse(contract.Test(3)!.Value);
    }

    public abstract class RelationalPatternGuardContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract bool? Test(BigInteger value);
    }
}
