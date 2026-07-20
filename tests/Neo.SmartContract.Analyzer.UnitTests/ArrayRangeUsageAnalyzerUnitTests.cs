// Copyright (C) 2015-2026 The Neo Project.
//
// ArrayRangeUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.ArrayRangeUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class ArrayRangeUsageAnalyzerUnitTests
{
    [TestMethod]
    public async Task GeneralArrayRange_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       int[] Slice(int[] values) => values[{|#0:1..^1|}];
                   }
                   """;

        var expected = VerifyCS.Diagnostic(ArrayRangeUsageAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("int[]");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task ByteArrayAndStringRanges_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       byte[] SliceBytes(byte[] values) => values[1..^1];
                       string SliceString(string value) => value[1..^1];
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task UserDefinedRangeIndexer_ShouldNotReportDiagnostic()
    {
        var test = """
                   using System;

                   class RangeContainer
                   {
                       public int this[Range range] => 0;
                   }

                   class Test
                   {
                       int Read(RangeContainer values) => values[1..^1];
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
