// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_FloatingPointDefaults.cs file belongs to the neo project and is free
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
public class UnitTest_FloatingPointDefaults
{
    [DataTestMethod]
    [DataRow("default(float)")]
    [DataRow("default(double)")]
    [DataRow("default(decimal)")]
    [DataRow("default(System.Half)")]
    public void ExplicitFloatingPointDefaults_ReportFloatingPointDiagnostic(string expression)
    {
        AssertFloatingPointDiagnostic(CompileDefault(expression));
    }

    [DataTestMethod]
    [DataRow("float")]
    [DataRow("double")]
    [DataRow("decimal")]
    [DataRow("System.Half")]
    public void ContextualFloatingPointDefaults_ReportFloatingPointDiagnostic(string typeName)
    {
        AssertFloatingPointDiagnostic(CompileContextualDefault(typeName));
    }

    [TestMethod]
    public void IntegerDefaults_CompileSuccessfully()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Test()
                {
                    return (int)default(int) + (int)((int)default);
                }
            }
            """);

        Assert.IsTrue(context.Success, GetDiagnostics(context));
    }

    private static CompilationContext CompileDefault(string expression)
    {
        return TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Test()
                {
                    return (int){{expression}};
                }
            }
            """);
    }

    private static CompilationContext CompileContextualDefault(string typeName)
    {
        return TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Test()
                {
                    return (int)(({{typeName}})default);
                }
            }
            """);
    }

    private static void AssertFloatingPointDiagnostic(CompilationContext context)
    {
        var diagnostics = GetDiagnostics(context);
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(
            context.Diagnostics.Any(d =>
                d.Id == DiagnosticId.FloatingPointNumber &&
                d.GetMessage() == "Floating-point numbers are not supported."),
            diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    private static string GetDiagnostics(CompilationContext context)
    {
        return string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
    }
}
