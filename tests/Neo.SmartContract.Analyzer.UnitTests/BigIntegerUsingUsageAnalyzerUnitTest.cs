// Copyright (C) 2015-2026 The Neo Project.
//
// BigIntegerUsingUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.CodeFixVerifier<
    Neo.SmartContract.Analyzer.BigIntegerUsingUsageAnalyzer,
    Neo.SmartContract.Analyzer.BigIntegerUsingUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class BigIntegerUsingUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task BigIntegerUsingUsageAnalyzer_IncorrectUsing_ShouldReportDiagnostic()
        {
            const string originalCode = """
                                        using BigInteger = System.Int64;

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                BigInteger value = 42;
                                            }
                                        }
                                        """;
            var expectedDiagnostic = Verifier.Diagnostic(BigIntegerUsingUsageAnalyzer.DiagnosticId)
                .WithSpan(1, 1, 1, 33).WithArguments("using BigInteger = System.Int64;");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task BigIntegerUsingUsageAnalyzer_CorrectUsing_ShouldNotReportDiagnostic()
        {
            const string originalCode = """
                                        using BigInteger = System.Numerics.BigInteger;

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                BigInteger value = 42;
                                            }
                                        }
                                        """;
            await Verifier.VerifyAnalyzerAsync(originalCode).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task BigIntegerUsingUsageCodeFix_ShouldPreserveTrivia()
        {
            const string originalCode = """
                                        {|#0:using BigInteger = System.Int64;|} // keep

                                        public class TestClass
                                        {
                                            public BigInteger GetValue() => 42;
                                        }
                                        """;
            const string fixedCode = """
                                     using BigInteger = System.Numerics.BigInteger; // keep

                                     public class TestClass
                                     {
                                         public BigInteger GetValue() => 42;
                                     }
                                     """;
            var expectedDiagnostic = Verifier.Diagnostic(BigIntegerUsingUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("using BigInteger = System.Int64;");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task BigIntegerUsingUsageCodeFix_ShouldNotChangeGlobalUsingScope()
        {
            const string diagnosticSource = """
                                            {|#0:global using BigInteger = System.Int64;|}

                                            public class TestClass
                                            {
                                                public BigInteger GetValue() => 42;
                                            }
                                            """;
            const string source = """
                                  global using BigInteger = System.Int64;

                                  public class TestClass
                                  {
                                      public BigInteger GetValue() => 42;
                                  }
                                  """;
            var expectedDiagnostic = Verifier.Diagnostic(BigIntegerUsingUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("global using BigInteger = System.Int64;");

            await Verifier.VerifyAnalyzerAsync(diagnosticSource, expectedDiagnostic).ConfigureAwait(false);

            using var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("TestProject", LanguageNames.CSharp)
                .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var document = workspace.AddDocument(project.Id, "Test.cs", SourceText.From(source));
            var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);
            Assert.IsNotNull(root);
            var usingDirective = root.DescendantNodes().OfType<UsingDirectiveSyntax>().Single();
            var descriptor = new DiagnosticDescriptor(
                BigIntegerUsingUsageAnalyzer.DiagnosticId,
                "Incorrect BigInteger using usage",
                "Incorrect BigInteger using usage",
                "Usage",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);
            var diagnostic = Diagnostic.Create(descriptor, usingDirective.GetLocation());
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (action, _) => actions.Add(action),
                CancellationToken.None);

            await new BigIntegerUsingUsageCodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

            Assert.AreEqual(0, actions.Count);
        }
    }
}
