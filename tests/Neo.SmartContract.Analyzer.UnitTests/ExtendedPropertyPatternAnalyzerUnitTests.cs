// Copyright (C) 2015-2026 The Neo Project.
//
// ExtendedPropertyPatternAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.ExtendedPropertyPatternAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class ExtendedPropertyPatternAnalyzerUnitTests
{
    [TestMethod]
    public async Task ExtendedPropertyPattern_ShouldReportDiagnostic()
    {
        var test = """
                   class Inner { public int Value { get; set; } }
                   class Holder { public Inner Inner { get; set; } = new(); }

                   class Test
                   {
                       bool Match(Holder holder) => holder is { {|#0:Inner.Value|}: 5 };
                   }
                   """;

        var expected = VerifyCS.Diagnostic(ExtendedPropertyPatternAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("Inner.Value");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task NestedPropertyPattern_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Inner { public int Value { get; set; } }
                   class Holder { public Inner Inner { get; set; } = new(); }

                   class Test
                   {
                       bool Match(Holder holder) => holder is { Inner: { Value: 5 } };
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
