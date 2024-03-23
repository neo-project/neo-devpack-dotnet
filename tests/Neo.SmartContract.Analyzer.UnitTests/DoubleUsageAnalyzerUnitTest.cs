using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Neo.SmartContract.Analyzer.DoubleUsageAnalyzer,
        Neo.SmartContract.Analyzer.DoubleUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public class DoubleUsageAnalyzerUnitTest
    {
        [Fact]
        public async Task DoubleUsageAnalyzer_ReplaceWithCommonKeyword()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestDouble(){ double a = (double)1.5;}
                                        }

                                        """;

            const string fixedCode = """

                                     public class TestClass
                                     {
                                         public void TestDouble(){ long a = (long)1.5;}
                                     }

                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 31, 4, 53).WithArguments("double");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }


        [Fact]
        public async Task DoubleUsageAnalyzer_ReplaceWithVar()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestDouble(){ var a = 1.5D; }
                                        }

                                        """;

            const string fixedCode = """

                                     public class TestClass
                                     {
                                         public void TestDouble(){ long a = (long)1.5; }
                                     }

                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 31, 4, 43).WithArguments("double");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }


        [Fact]
        public async Task DoubleUsageAnalyzer_ReplaceWithDouble()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestDouble(){ double a = 1.5D;}
                                        }

                                        """;

            const string fixedCode = """

                                     public class TestClass
                                     {
                                         public void TestDouble(){ long a = (long)1.5; }
                                     }

                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 31, 4, 46).WithArguments("double");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

    }
}
