// Copyright (C) 2015-2026 The Neo Project.
//
// InitialValueAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyFix = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.CodeFixVerifier<
    Neo.SmartContract.Analyzer.InitialValueAnalyzer,
    Neo.SmartContract.Analyzer.InitialValueCodeFixProvider>;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.InitialValueAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class InitialValueAnalyzerUnitTest
    {
        [TestMethod]
        public async Task FieldWithoutAttribute_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static readonly string _address = ""hello"";
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldWithRegularInitializer_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static readonly int _value = 42;
    private static readonly string _name = ""test"";
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldWithNonTargetAttribute_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

[AttributeUsage(AttributeTargets.Field)]
class MyCustomAttribute : Attribute
{
    public MyCustomAttribute(string value) { }
}

class TestClass
{
    [MyCustom(""some_value"")]
    private static readonly string _address = default!;
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldWithInitialValueAttributeAndNonDefaultInit_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

[AttributeUsage(AttributeTargets.Field)]
class InitialValueAttribute : Attribute
{
    public InitialValueAttribute(string value) { }
}

class TestClass
{
    [InitialValue(""some_value"")]
    private static readonly string _address = ""actual_value"";
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ParseFix_ShouldTargetDiagnosedVariableAndPreserveSiblings()
        {
            var test = """
                       namespace Neo.SmartContract.Framework
                       {
                           public sealed class UInt160
                           {
                               public static UInt160 Parse(string value) => new();
                               public static implicit operator UInt160(string value) => new();
                           }
                       }

                       class TestClass
                       {
                           private static readonly Neo.SmartContract.Framework.UInt160
                               first = Neo.SmartContract.Framework.UInt160.Parse("first"),
                               second = {|#0:"second"|};
                       }
                       """;

            var fixedSource = """
                              namespace Neo.SmartContract.Framework
                              {
                                  public sealed class UInt160
                                  {
                                      public static UInt160 Parse(string value) => new();
                                      public static implicit operator UInt160(string value) => new();
                                  }
                              }

                              class TestClass
                              {
                                  private static readonly Neo.SmartContract.Framework.UInt160
                                      first = Neo.SmartContract.Framework.UInt160.Parse("first"),
                                      second = Neo.SmartContract.Framework.UInt160.Parse("second");
                              }
                              """;

            var expected = VerifyFix.Diagnostic(InitialValueAnalyzer.ParseDiagnosticId)
                .WithLocation(0)
                .WithArguments("UInt160");

            await VerifyFix.VerifyCodeFixAsync(test, expected, fixedSource);
        }

        [TestMethod]
        public async Task InitialValueFix_ShouldNotBeOfferedForMultipleVariables()
        {
            const string source = """
                                  class TestClass
                                  {
                                      private static readonly string first = default!, second = default!;
                                  }
                                  """;

            using var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
            var document = workspace.AddDocument(project.Id, "Test.cs", SourceText.From(source));
            var root = await document.GetSyntaxRootAsync();
            var field = root!.DescendantNodes().OfType<FieldDeclarationSyntax>().Single();
            var descriptor = new InitialValueAnalyzer().SupportedDiagnostics
                .Single(rule => rule.Id == InitialValueAnalyzer.DiagnosticId);
            var diagnostic = Diagnostic.Create(descriptor, field.GetLocation(), "InitialValue");
            List<CodeAction> actions = [];
            var context = new CodeFixContext(
                document,
                diagnostic,
                (action, _) => actions.Add(action),
                CancellationToken.None);

            await new InitialValueCodeFixProvider().RegisterCodeFixesAsync(context);

            Assert.AreEqual(0, actions.Count);
        }
    }
}
