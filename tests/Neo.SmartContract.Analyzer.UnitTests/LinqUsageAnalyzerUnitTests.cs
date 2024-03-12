using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.LinqUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    public class LinqUsageAnalyzerUnitTests
    {
        [Fact]
        public async Task LINQMethodUsage_ShouldReportDiagnostic()
        {
            var test = @"
using System;
using System.Linq;
using System.Collections.Generic;

class TestClass
{
    public void TestMethod()
    {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
        var evenNumbers = numbers.Where(n => n % 2 == 0).ToList();
    }
}";

            DiagnosticResult[] expectedDiagnostics = new[]
            {
                VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                    .WithLocation(11, 35) // Location of the 'Where' method usage
                    .WithArguments("Where"),
                VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                    .WithLocation(11, 58) // Location of the 'ToList' method usage
                    .WithArguments("ToList"),
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostics);
        }

        [Fact]
        public async Task LINQQuerySyntaxUsage_ShouldReportDiagnostic()
        {
            var test = @"
using System;
using System.Linq;
using System.Collections.Generic;

class TestClass
{
    public void TestMethod()
    {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
        var evenNumbers = from n in numbers
                          where n % 2 == 0
                          select n;
    }
}";

            var expectedDiagnostic = VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                // Update the WithLocation call to match the actual start column (27)
                .WithLocation(11, 27)
                .WithArguments("query");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
