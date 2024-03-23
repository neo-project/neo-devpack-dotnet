using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Neo.SmartContract.Analyzer.DecimalUsageAnalyzer,
        Neo.SmartContract.Analyzer.DecimalUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public class DecimalUsageAnalyzerUnitTests
    {
        [Fact]
        public async Task DecimalVariable_ShouldReportDiagnostic()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               decimal d = 1.0m;
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                .WithLocation(6, 9)
                .WithArguments("System_Decimal", "decimal");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [Fact]
        public async Task DoubleVariable_ShouldReportDiagnostic()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               double d = 1.0;
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                .WithLocation(6, 9)
                .WithArguments("System_Double", "double");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [Fact]
        public async Task DecimalVariable_ShouldChangeTo_LongVariable()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               decimal d = 1.0m;
                           }
                       }
                       """;

            var fixtest = """

                          class TestClass
                          {
                              public void TestMethod()
                              {
                                  long d = 1.0m;
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                .WithLocation(6, 9)
                .WithArguments("System_Decimal", "decimal");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }

        [Fact]
        public async Task DoubleVariable_ShouldChangeTo_LongVariable()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               double d = 1.0;
                           }
                       }
                       """;

            var fixtest = """

                          class TestClass
                          {
                              public void TestMethod()
                              {
                                  long d = 1.0;
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                .WithLocation(6, 9)
                .WithArguments("System_Double", "double");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }
    }
}
