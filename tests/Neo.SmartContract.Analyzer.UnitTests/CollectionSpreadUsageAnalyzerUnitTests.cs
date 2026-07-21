// Copyright (C) 2015-2026 The Neo Project.
//
// CollectionSpreadUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.CollectionSpreadUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class CollectionSpreadUsageAnalyzerUnitTests
{
    [TestMethod]
    public async Task SpreadElement_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       int[] Clone(int[] values) => [{|#0:..values|}];
                   }
                   """;

        var expected = VerifyCS.Diagnostic(CollectionSpreadUsageAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("..values");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task ExplicitCollectionElements_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       int[] Create(int first, int second) => [first, second];
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
