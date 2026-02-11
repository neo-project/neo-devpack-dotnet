// Copyright (C) 2015-2026 The Neo Project.
//
// SystemMathUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.SystemMathUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class SystemMathUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task UnsupportedMathSqrt_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var result = Math.Sqrt(4.0);
    }
}";

            var expected = VerifyCS.Diagnostic(SystemMathUsageAnalyzer.DiagnosticId)
                .WithSpan(8, 22, 8, 36)
                .WithArguments("Sqrt");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UnsupportedMathCos_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var result = Math.Cos(1.0);
    }
}";

            var expected = VerifyCS.Diagnostic(SystemMathUsageAnalyzer.DiagnosticId)
                .WithSpan(8, 22, 8, 35)
                .WithArguments("Cos");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UnsupportedMathLog_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var result = Math.Log(10.0);
    }
}";

            var expected = VerifyCS.Diagnostic(SystemMathUsageAnalyzer.DiagnosticId)
                .WithSpan(8, 22, 8, 36)
                .WithArguments("Log");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task SupportedMathMethods_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var result = Math.Max(1, 2);
        var result2 = Math.Min(1, 2);
        var result3 = Math.Abs(-1);
        var result4 = Math.Sign(-5);
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
