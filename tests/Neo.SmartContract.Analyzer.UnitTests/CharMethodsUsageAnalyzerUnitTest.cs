// Copyright (C) 2015-2026 The Neo Project.
//
// CharMethodsUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.CharMethodsUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class CharMethodsUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task UnsupportedCharCompareTo_ShouldReportDiagnostic()
        {
            var test = @"
class TestClass
{
    void TestMethod()
    {
        char c = 'a';
        var result = c.CompareTo('b');
    }
}";

            var expected = VerifyCS.Diagnostic(CharMethodsUsageAnalyzer.DiagnosticId)
                .WithSpan(7, 22, 7, 38)
                .WithArguments("CompareTo");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UnsupportedCharGetHashCode_ShouldReportDiagnostic()
        {
            var test = @"
class TestClass
{
    void TestMethod()
    {
        char c = 'a';
        var result = c.GetHashCode();
    }
}";

            var expected = VerifyCS.Diagnostic(CharMethodsUsageAnalyzer.DiagnosticId)
                .WithLocation(7, 22)
                .WithArguments("GetHashCode");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UnsupportedCharIsNumber_ShouldReportDiagnostic()
        {
            var test = @"
class TestClass
{
    void TestMethod()
    {
        var result = char.IsNumber('5');
    }
}";

            var expected = VerifyCS.Diagnostic(CharMethodsUsageAnalyzer.DiagnosticId)
                .WithSpan(6, 22, 6, 40)
                .WithArguments("IsNumber");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task SupportedCharMethods_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    void TestMethod()
    {
        char c = 'a';
        var result = char.IsLetter(c);
        var result2 = char.IsDigit(c);
        var result3 = char.IsWhiteSpace(c);
        var result4 = char.ToLower(c);
        var result5 = char.ToUpper(c);
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
