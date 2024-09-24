using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
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
        var tryParsed = Enum.TryParse(typeof(DayOfWeek), ""Tuesday"", out var result);
        var names = Enum.GetNames(typeof(DayOfWeek));
        var values = Enum.GetValues(typeof(DayOfWeek));
        var isDefined = Enum.IsDefined(typeof(DayOfWeek), ""Wednesday"");
        var name = Enum.GetName(typeof(DayOfWeek), DayOfWeek.Thursday);
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
