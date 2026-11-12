// Copyright (C) 2015-2026 The Neo Project.
//
// UnionDeclarationUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.UnionDeclarationUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class UnionDeclarationUsageAnalyzerUnitTests
{
    [TestMethod]
    public async Task UnionDeclaration_ShouldReportDiagnostic()
    {
        var test = """
                   namespace System.Runtime.CompilerServices
                   {
                       [System.AttributeUsage(System.AttributeTargets.Struct)]
                       public class UnionAttribute : System.Attribute { }

                       public interface IUnion { }
                   }

                   class Cat { }
                   class Dog { }

                   {|#0:union Pet(Cat, Dog);|}
                   """;

        var expected = VerifyCS.Diagnostic(UnionDeclarationUsageAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("Pet");

        await VerifyPreviewAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task RegularStructDeclaration_ShouldNotReportDiagnostic()
    {
        var test = """
                   struct Pet
                   {
                       public Cat Value;
                   }

                   class Cat { }
                   """;

        await VerifyPreviewAnalyzerAsync(test);
    }

    private static Task VerifyPreviewAnalyzerAsync(string test, params DiagnosticResult[] expected)
    {
        var verifier = new PreviewAnalyzerTest
        {
            TestCode = test
        };
        verifier.ExpectedDiagnostics.AddRange(expected);
        return verifier.RunAsync();
    }

    private sealed class PreviewAnalyzerTest : CSharpAnalyzerTest<Neo.SmartContract.Analyzer.UnionDeclarationUsageAnalyzer, DefaultVerifier>
    {
        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion.Preview);
        }
    }
}
