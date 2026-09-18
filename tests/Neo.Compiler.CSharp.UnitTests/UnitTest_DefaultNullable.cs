// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DefaultNullable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_DefaultNullable : DebugAndTestBase<Contract_DefaultNullable>
    {
        [TestMethod]
        public void TestDefaultLiteral()
        {
            Assert.IsTrue(Contract.TestDefaultLiteral());
            Assert.IsFalse(Contract.TestHasValue());
            Assert.AreEqual(42, Contract.TestNullCoalescing());
            Assert.IsNull(Contract.TestReturnDefault());
            Assert.IsTrue(Contract.TestComparison(null));
            Assert.IsFalse(Contract.TestComparison(42));
        }

        [TestMethod]
        public void TestDefaultExpression()
        {
            Assert.IsTrue(Contract.TestDefaultExpression());
            Assert.IsFalse(Contract.TestHasValueExpression());
            Assert.AreEqual(42, Contract.TestNullCoalescingExpression());
            Assert.IsNull(Contract.TestReturnDefaultExpression());
            Assert.IsTrue(Contract.TestComparisonExpression(null));
            Assert.IsFalse(Contract.TestComparisonExpression(42));
        }

        [TestMethod]
        public void TestDefaultExpressionWithoutTypeReportsDiagnostic()
        {
            var (model, expression, converter) = CreateDefaultConverter("System");
            Assert.IsNull(model.GetTypeInfo(expression.Type).Type);

            var method = typeof(MethodConvert).GetMethod("ConvertDefaultExpression", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var invocation = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(converter, [model, expression]));
            Assert.IsInstanceOfType<CompilationException>(invocation.InnerException);
            var exception = (CompilationException)invocation.InnerException!;
            StringAssert.Contains(exception.Message, "Cannot determine type for default expression. Ensure the expression has a valid type.");
            Assert.AreEqual(0, converter.Instructions.Count);
        }

        [TestMethod]
        [DataRow("int?", OpCode.PUSHNULL)]
        [DataRow("Value", OpCode.NEWSTRUCT0)]
        [DataRow("T", OpCode.NEWSTRUCT0)]
        public void TestDefaultValueTypeDistinguishesNullableAndStruct(string typeName, OpCode expected)
        {
            var (model, expression, converter) = CreateDefaultConverter(typeName);
            Assert.IsFalse(model.GetDiagnostics().Any(p => p.Severity == DiagnosticSeverity.Error));
            var type = model.GetTypeInfo(expression.Type).Type!;
            Assert.IsTrue(type.IsValueType);
            Assert.AreEqual(typeName != "T", type is INamedTypeSymbol);

            var method = typeof(MethodConvert).GetMethod("ConvertDefaultExpression", BindingFlags.Instance | BindingFlags.NonPublic)!;
            method.Invoke(converter, [model, expression]);

            CollectionAssert.AreEqual(new[] { expected }, converter.Instructions.Select(p => p.OpCode).ToArray());
        }

        private static (SemanticModel Model, DefaultExpressionSyntax Expression, MethodConvert Converter) CreateDefaultConverter(string typeName)
        {
            var tree = CSharpSyntaxTree.ParseText($$"""
                public struct Value { }
                public class Contract
                {
                    public static {{typeName}} Main<T>() where T : struct => default({{typeName}});
                }
                """);
            var compilation = CSharpCompilation.Create("DefaultExpressions", [tree],
                [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var model = compilation.GetSemanticModel(tree);
            var expression = tree.GetRoot().DescendantNodes().OfType<DefaultExpressionSyntax>().Single();
            var symbol = (IMethodSymbol)model.GetDeclaredSymbol(expression.FirstAncestorOrSelf<MethodDeclarationSyntax>()!)!;
            var context = new CompilationContext(new CompilationEngine(new CompilationOptions()), symbol.ContainingType);
            return (model, expression, new MethodConvert(context, symbol));
        }

        [TestMethod]
        public void TestNotEqualNull()
        {
            Assert.IsFalse(Contract.TestNotEqualNull());
            Assert.IsFalse(Contract.TestNotEqualNullExpression());
        }
    }
}
