// Copyright (C) 2015-2026 The Neo Project.
//
// CollectionWithElementUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.CollectionWithElementUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class CollectionWithElementUsageAnalyzerUnitTests
{
    [TestMethod]
    public async Task WithElement_ShouldReportDiagnostic()
    {
        var test = """
                   using System.Collections.Generic;

                   class Test
                   {
                       List<int> Create(int seed) => [{|#0:with(capacity: 10)|}, seed, seed + 1];
                   }
                   """;

        var expected = VerifyCS.Diagnostic(CollectionWithElementUsageAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("with(capacity: 10)");

        await VerifyPreviewAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task ExplicitCollectionElements_ShouldNotReportDiagnostic()
    {
        var test = """
                   using System.Collections.Generic;

                   class Test
                   {
                       List<int> Create(int first, int second) => [first, second];
                   }
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

    private sealed class PreviewAnalyzerTest : CSharpAnalyzerTest<Neo.SmartContract.Analyzer.CollectionWithElementUsageAnalyzer, DefaultVerifier>
    {
        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion.Preview);
        }
    }
}
