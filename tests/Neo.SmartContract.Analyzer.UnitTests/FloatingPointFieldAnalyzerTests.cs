// Copyright (C) 2015-2026 The Neo Project.
//
// FloatingPointFieldAnalyzerTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FloatVerifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.FloatUsageAnalyzer>;
using DoubleVerifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.DoubleUsageAnalyzer>;
using DecimalVerifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.DecimalUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class FloatingPointFieldAnalyzerTests
{
    [TestMethod]
    [DataRow("public float first, second;")]
    [DataRow("public static readonly float first = 1.5f;")]
    [DataRow("public const float first = 1.5f;")]
    [DataRow("public float[][] first;")]
    [DataRow("public System.Collections.Generic.List<float?> first;")]
    public Task FloatFields_AreReportedOnce(string declaration)
        => FloatVerifier.VerifyAnalyzerAsync("class Contract { {|#0:" + declaration + "|} }",
            FloatVerifier.Diagnostic().WithLocation(0).WithArguments("float"));

    [TestMethod]
    [DataRow("public double first, second;")]
    [DataRow("public static readonly double first = 1.5d;")]
    [DataRow("public const double first = 1.5d;")]
    [DataRow("public double[][] first;")]
    [DataRow("public System.Collections.Generic.List<double?> first;")]
    public Task DoubleFields_AreReportedOnce(string declaration)
        => DoubleVerifier.VerifyAnalyzerAsync("class Contract { {|#0:" + declaration + "|} }",
            DoubleVerifier.Diagnostic().WithLocation(0).WithArguments("double"));

    [TestMethod]
    [DataRow("public decimal first, second;")]
    [DataRow("public static readonly decimal first = 1.5m;")]
    [DataRow("public const decimal first = 1.5m;")]
    [DataRow("public decimal[][] first;")]
    [DataRow("public System.Collections.Generic.List<decimal?> first;")]
    public Task DecimalFields_AreReportedOnce(string declaration)
        => DecimalVerifier.VerifyAnalyzerAsync("class Contract { {|#0:" + declaration + "|} }",
            DecimalVerifier.Diagnostic().WithLocation(0).WithArguments("System_Decimal", "decimal"));

    [TestMethod]
    public async Task IntegralFields_DoNotReportFloatingPointErrors()
    {
        const string source = "class Contract { public int first = 1, second; public long[] values; }";
        await FloatVerifier.VerifyAnalyzerAsync(source);
        await DoubleVerifier.VerifyAnalyzerAsync(source);
        await DecimalVerifier.VerifyAnalyzerAsync(source);
    }
}
