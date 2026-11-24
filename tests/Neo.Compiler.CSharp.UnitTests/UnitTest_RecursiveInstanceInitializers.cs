// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RecursiveInstanceInitializers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RecursiveInstanceInitializers
{
    [TestMethod]
    [DataRow("public class Helper { private object value = new Helper(); }")]
    [DataRow("public class Helper { private object value { get; } = new Helper(); }")]
    [DataRow("public class Helper { private Other value = new Other(); } public class Other { private Helper value = new Helper(); }")]
    [DataRow("public struct Helper { private object value = new Helper(); public Helper() { } }")]
    public void RecursiveInstanceInitializersReportDiagnostic(string declaration)
    {
        var context = TestHelper.CompileSingleContract($$"""
using Neo.SmartContract.Framework;

{{declaration}}

public class Contract : SmartContract
{
    public static object Test() => new Helper();
}
""");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.SyntaxNotSupported
            && d.ToString().Contains("Recursive instance field initializers")), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void NonrecursiveInstanceInitializerCompiles()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Helper { private object value = new Other(); }
public class Other { }

public class Contract : SmartContract
{
    public static object Test() => new Helper();
}
""");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));
        Assert.IsTrue(context.Success, diagnostics);
    }
}
