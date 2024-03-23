using System.Threading.Tasks;
using Xunit;

using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.SmartContractMethodNamingAnalyzerUnderline>;

namespace Neo.SmartContract.Analyzer.Test
{
    public class MethodNamingAnalyzerUnderlineUnitTests
    {
        [Fact]
        public async Task MethodNameStartingWithUnderscoreAndSameParameterCount_ShouldReportDiagnostic()
        {
            var test = """
                       public class TestContract
                       {
                           public static void _testMethod(int a) {}
                           public static void TestMethod(int a) {}
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(SmartContractMethodNamingAnalyzerUnderline.DiagnosticId)
                .WithLocation(3, 24)
                .WithArguments("_testMethod");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [Fact]
        public async Task MethodNameStartingWithUnderscoreButDifferentParameterCount_ShouldNotReportDiagnostic()
        {
            var test = """
                       public class TestContract
                       {
                           public static void _testMethod(int a) {}
                           public static void TestMethod(int a, int b) {}
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic().WithSpan(3, 24, 3, 35);
            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
