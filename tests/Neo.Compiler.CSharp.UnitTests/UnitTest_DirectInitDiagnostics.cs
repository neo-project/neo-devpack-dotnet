// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DirectInitDiagnostics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DirectInitDiagnostics
{
    [TestMethod]
    public void InvalidDirectUInt256LiteralReportsInvalidInitialValue()
    {
        var context = CompileSingleContractWithUInt256Literal("abc");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidInitialValue), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void WrongLengthDirectUInt256LiteralReportsInvalidInitialValue()
    {
        var context = CompileSingleContractWithUInt256Literal("abcd");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidInitialValue), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    private static CompilationContext CompileSingleContractWithUInt256Literal(string literal)
    {
        return TestHelper.CompileSingleContract($$"""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    private static readonly UInt256 Hash = "{{literal}}";

    public static UInt256 Test()
    {
        return Hash;
    }
}
""");
    }
}
