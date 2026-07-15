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
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
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
            var test = """
                       class TestClass
                       {
                           void TestMethod()
                           {
                               char value = 'a';
                               _ = value.ToString();
                               _ = char.IsDigit(value);
                               _ = char.IsLetter(value);
                               _ = char.IsWhiteSpace(value);
                               _ = char.IsLower(value);
                               _ = char.ToLower(value);
                               _ = char.IsUpper(value);
                               _ = char.ToUpper(value);
                               _ = char.IsPunctuation(value);
                               _ = char.IsSymbol(value);
                               _ = char.IsControl(value);
                               _ = char.IsSurrogate(value);
                               _ = char.IsHighSurrogate(value);
                               _ = char.IsLowSurrogate(value);
                               _ = char.GetNumericValue(value);
                               _ = char.IsLetterOrDigit(value);
                               _ = char.ToLowerInvariant(value);
                               _ = char.ToUpperInvariant(value);
                               _ = char.Parse("a");
                               _ = char.TryParse("a", out var parsed);
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnsupportedCharOverloads_ShouldReportDiagnostics()
        {
            var test = """
                       using System.Globalization;

                       class TestClass
                       {
                           void TestMethod()
                           {
                               char value = 'a';
                               _ = {|#0:char.IsDigit("5", 0)|};
                               _ = {|#1:char.ToLower(value, CultureInfo.InvariantCulture)|};
                               _ = {|#2:value.ToString(CultureInfo.InvariantCulture)|};
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(CharMethodsUsageAnalyzer.DiagnosticId).WithLocation(0).WithArguments("IsDigit"),
                VerifyCS.Diagnostic(CharMethodsUsageAnalyzer.DiagnosticId).WithLocation(1).WithArguments("ToLower"),
                VerifyCS.Diagnostic(CharMethodsUsageAnalyzer.DiagnosticId).WithLocation(2).WithArguments("ToString")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
