// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DisplayNameDiagnostics.cs file belongs to the neo project and is free
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
public class UnitTest_DisplayNameDiagnostics
{
    [TestMethod]
    public void EmptyContractDisplayNameReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using System.ComponentModel;

[DisplayName("")]
public class Contract : SmartContract
{
    public static int Test() => 0;
}
""");

        AssertHasDiagnostic(context, DiagnosticId.InvalidArgument);
    }

    [TestMethod]
    public void EmptyMethodDisplayNameReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("")]
    public static int Test() => 0;
}
""");

        AssertHasDiagnostic(context, DiagnosticId.InvalidMethodName);
    }

    [TestMethod]
    public void EmptyEventDisplayNameReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("")]
    public static event Action<byte[]> Transferred = default!;

    public static void Test()
    {
        Transferred(new byte[] { 1 });
    }
}
""");

        AssertHasDiagnostic(context, DiagnosticId.InvalidArgument);
    }

    private static void AssertHasDiagnostic(CompilationContext context, string diagnosticId)
    {
        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == diagnosticId), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }
}
