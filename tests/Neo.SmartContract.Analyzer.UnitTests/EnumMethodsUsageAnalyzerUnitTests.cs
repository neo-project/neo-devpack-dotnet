// Copyright (C) 2015-2026 The Neo Project.
//
// EnumMethodsUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.EnumMethodsUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class EnumMethodsUsageAnalyzerUnitTests
    {
        private const string DiagnosticId = EnumMethodsUsageAnalyzer.DiagnosticId;

        [TestMethod]
        public async Task UnsupportedEnumFormat_ReportsDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var result = Enum.Format(typeof(DayOfWeek), DayOfWeek.Monday, ""G"");
    }
}";

            var expected = VerifyCS.Diagnostic(DiagnosticId)
                .WithLocation(8, 22)  // Line 8, column 22
                .WithArguments("Format");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UnsupportedEnumGetUnderlyingType_ReportsDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var result = Enum.GetUnderlyingType(typeof(DayOfWeek));
    }
}";

            var expected = VerifyCS.Diagnostic(DiagnosticId)
                .WithSpan(8, 22, 8, 63)
                .WithArguments("GetUnderlyingType");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task SupportedEnumMethods_NoDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    void TestMethod()
    {
        var parsed = Enum.Parse(typeof(DayOfWeek), ""Monday"");
        var parsedIgnoreCase = Enum.Parse(typeof(DayOfWeek), ""monday"", true);
        var tryParsed = Enum.TryParse(typeof(DayOfWeek), ""Tuesday"", out var result);
        var tryParsedIgnoreCase = Enum.TryParse(typeof(DayOfWeek), ""tuesday"", true, out var result2);
        var names = Enum.GetNames(typeof(DayOfWeek));
        var values = Enum.GetValues(typeof(DayOfWeek));
        var isDefined = Enum.IsDefined(typeof(DayOfWeek), ""Wednesday"");
        var isDefinedValue = Enum.IsDefined(typeof(DayOfWeek), DayOfWeek.Wednesday);
        var name = Enum.GetName(typeof(DayOfWeek), DayOfWeek.Thursday);
        var genericParsed = Enum.Parse<DayOfWeek>(""Friday"");
        var genericParsedIgnoreCase = Enum.Parse<DayOfWeek>(""friday"", true);
        var genericTryParsed = Enum.TryParse<DayOfWeek>(""Saturday"", out var genericResult);
        var genericTryParsedIgnoreCase = Enum.TryParse<DayOfWeek>(""saturday"", true, out var genericResult2);
        var text = DayOfWeek.Monday.ToString();
        var hasFlag = DayOfWeek.Monday.HasFlag(DayOfWeek.Monday);
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnsupportedEnumOverloads_ReportDiagnostics()
        {
            var test = """
                       using System;

                       class TestClass
                       {
                           void TestMethod()
                           {
                               _ = {|#0:Enum.ToObject(typeof(DayOfWeek), 1)|};
                               _ = {|#1:DayOfWeek.Monday.ToString("G")|};
                               _ = {|#2:DayOfWeek.Monday.CompareTo(DayOfWeek.Tuesday)|};
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(DiagnosticId).WithLocation(0).WithArguments("ToObject"),
                VerifyCS.Diagnostic(DiagnosticId).WithLocation(1).WithArguments("ToString"),
                VerifyCS.Diagnostic(DiagnosticId).WithLocation(2).WithArguments("CompareTo")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
