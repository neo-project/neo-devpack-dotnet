using System.Threading.Tasks;
using Xunit;

using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.StringMethodUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    public class StringMethodUsageAnalyzerUnitTests
    {
        [Fact]
        public async Task SupportedStringMethod_ShouldNotReportDiagnostic()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               string x = "Hello";
                               int length = x.Length;
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task UnsupportedStringMethod_ShouldReportDiagnostic()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               string x = "Hello";
                               string y = x.ToUpper();
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(StringMethodUsageAnalyzer.DiagnosticId)
                .WithLocation(7, 20)
                .WithArguments("ToUpper");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
