// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ParseInvocationDiagnostics.cs file belongs to the neo project and is free
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
public class UnitTest_ParseInvocationDiagnostics
{
    [TestMethod]
    public void EmptyECPointParseReportsInvalidInitialValue()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static ECPoint Test()
    {
        return ECPoint.Parse("");
    }
}
""");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidInitialValue), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void MalformedECPointParseReportsInvalidInitialValue()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static ECPoint Test()
    {
        return ECPoint.Parse("\uFFFD");
    }
}
""");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidInitialValue), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }
}
