// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_UnsupportedAbiTypes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class UnitTest_UnsupportedAbiTypes
{
    [TestMethod]
    public void PublicMethods_WithFloatingPointAbiTypes_FailCompilation()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public class Contract : SmartContract
{
[Safe]
public static double EchoDouble(double value) => value;

[Safe]
public static float EchoFloat(float value) => value;

[Safe]
public static decimal EchoDecimal(decimal value) => value;
}
""");

        Assert.IsFalse(context.Success, "Floating-point types must not be accepted in contract ABI methods.");
        StringAssert.Contains(GetDiagnostics(context), "not supported");
    }

    [TestMethod]
    public void PublicMethods_WithTaskAbiTypes_FailCompilation()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public class Contract : SmartContract
{
[Safe]
public static System.Threading.Tasks.Task EchoTask(System.Threading.Tasks.Task value) => value;

[Safe]
public static System.Threading.Tasks.Task<int> EchoGenericTask(System.Threading.Tasks.Task<int> value) => value;

[Safe]
public static System.Threading.Tasks.ValueTask EchoValueTask(System.Threading.Tasks.ValueTask value) => value;
}
""");

        Assert.IsFalse(context.Success, "Task-like types must not be accepted in contract ABI methods.");
        StringAssert.Contains(GetDiagnostics(context), "not supported");
    }

    private static string GetDiagnostics(CompilationContext context)
    {
        return string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
    }
}
