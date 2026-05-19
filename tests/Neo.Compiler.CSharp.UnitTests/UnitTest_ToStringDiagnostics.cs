// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ToStringDiagnostics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ToStringDiagnostics
{
    [TestMethod]
    public void MapToStringReportsUnsupportedToStringType()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test()
    {
        var map = new Map<string, string>();
        map["name"] = "My NFT";
        return map.ToString();
    }
}
""");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidToStringType), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
        StringAssert.Contains(diagnostics, "StdLib.Serialize");
    }

    [TestMethod]
    public void StringToStringStillCompiles()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test()
    {
        return "Jimmy".ToString();
    }
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    [TestMethod]
    public void ObjectToStringStillCompiles()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(object value)
    {
        return value.ToString();
    }
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    [TestMethod]
    public void FrameworkValueToStringTypesStillCompile()
    {
        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(ByteString value)
    {
        return value.ToString();
    }
}
""");

        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(UInt160 value)
    {
        return value.ToString();
    }
}
""");

        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(UInt256 value)
    {
        return value.ToString();
    }
}
""");

        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(ECPoint value)
    {
        return value.ToString();
    }
}
""");
    }

    [TestMethod]
    public void ObjectToStringGuardHandlesDefensiveCases()
    {
        var methodConvertType = typeof(CompilationEngine).Assembly.GetType("Neo.Compiler.MethodConvert")!;
        var handler = methodConvertType.GetMethod("HandleObjectToString", BindingFlags.NonPublic | BindingFlags.Static)!;
        var objectToStringSymbol = GetObjectToStringSymbol();

        var exception = Assert.ThrowsException<TargetInvocationException>(() =>
            handler.Invoke(null, [null, null, objectToStringSymbol, null, null]));

        Assert.IsInstanceOfType(exception.InnerException, typeof(CompilationException));
        Assert.AreEqual(DiagnosticId.InvalidToStringType, ((CompilationException)exception.InnerException!).Diagnostic.Id);

        var isSupported = methodConvertType.GetMethod("IsSupportedObjectToStringType", BindingFlags.NonPublic | BindingFlags.Static)!;
        Assert.IsFalse((bool)isSupported.Invoke(null, [null])!);
        Assert.IsTrue((bool)isSupported.Invoke(null, [CreateCompilation("").GetSpecialType(SpecialType.System_Char)])!);
    }

    private static void AssertCompiles(string source)
    {
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    private static IMethodSymbol GetObjectToStringSymbol()
    {
        const string source = "class Contract { string Test(object value) => value.ToString(); }";
        var compilation = CreateCompilation(source);
        var invocation = compilation.SyntaxTrees.Single()
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single();

        return (IMethodSymbol)compilation.GetSemanticModel(invocation.SyntaxTree).GetSymbolInfo(invocation).Symbol!;
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        return CSharpCompilation.Create(
            assemblyName: $"ToStringDiagnostics_{Guid.NewGuid():N}",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source)],
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
    }
}
