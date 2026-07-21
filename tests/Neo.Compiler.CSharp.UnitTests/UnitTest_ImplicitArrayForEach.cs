// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ImplicitArrayForEach.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ImplicitArrayForEach
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("sum")]
            public static int Sum()
            {
                int sum = 0;
                foreach (int value in new[] { 1, 2, 3 })
                {
                    sum += value;
                }

                return sum;
            }
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ImplicitArrayUsedDirectlyInForEach_ShouldCompileAndExecute(
        CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);

        Assert.IsTrue(
            context.Success,
            string.Join(Environment.NewLine, context.Diagnostics.Select(static item => item.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ImplicitArrayForEachContract>(
            context.CreateExecutable(),
            context.CreateManifest());

        Assert.AreEqual(new BigInteger(6), contract.Sum(), optimization.ToString());
    }

    [TestMethod]
    public void ContextualImplicitArray_ShouldResolveSourceArrayType()
    {
        var (model, expression) = CreateSemanticModel("""
            class Contract
            {
                void Test()
                {
                    foreach (int value in new[] { 1, 2, 3 }) { }
                }
            }
            """);
        TypeInfo typeInfo = model.GetTypeInfo(expression);

        Assert.IsTrue(typeInfo.Type is IArrayTypeSymbol);
        Assert.IsFalse(typeInfo.ConvertedType is IArrayTypeSymbol);

        IArrayTypeSymbol type = MethodConvert.ResolveImplicitArrayType(model, expression);

        Assert.AreEqual(SpecialType.System_Int32, type.ElementType.SpecialType);
    }

    [TestMethod]
    public void UnresolvedImplicitArray_ShouldReportUnsupportedSyntax()
    {
        var (model, expression) = CreateSemanticModel("""
            class Contract
            {
                void Test()
                {
                    var values = new[] { null, null };
                }
            }
            """);

        Assert.IsTrue(model.GetTypeInfo(expression).Type is IArrayTypeSymbol
        {
            ElementType.TypeKind: TypeKind.Error
        });

        var exception = Assert.ThrowsException<CompilationException>(
            () => MethodConvert.ResolveImplicitArrayType(model, expression));

        Assert.AreEqual(DiagnosticId.SyntaxNotSupported, exception.Diagnostic.Id);
        StringAssert.Contains(exception.Message, "Unable to determine the element type");
    }

    private static (SemanticModel Model, ImplicitArrayCreationExpressionSyntax Expression) CreateSemanticModel(
        string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "ImplicitArray.cs");
        var compilation = CSharpCompilation.Create(
            $"ImplicitArray_{Guid.NewGuid():N}",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var expression = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<ImplicitArrayCreationExpressionSyntax>()
            .Single();

        return (compilation.GetSemanticModel(syntaxTree), expression);
    }

    public abstract class ImplicitArrayForEachContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("sum")]
        public abstract BigInteger? Sum();
    }
}
