using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.SupportedStandardsAnalyzer,
    Neo.SmartContract.Analyzer.SupportedStandardsCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class SupportedStandardsAnalyzerUnitTest
    {
        private const string TestNamespace = """

                                             using System;

                                             public enum NepStandard
                                             {
                                                 // The NEP-11 standard is used for non-fungible tokens (NFTs).
                                                 // Defined at https://github.com/neo-project/proposals/blob/master/nep-11.mediawiki
                                                 Nep11,
                                                 // The NEP-17 standard is used for fungible tokens.
                                                 // Defined at https://github.com/neo-project/proposals/blob/master/nep-17.mediawiki
                                                 Nep17
                                             }

                                             [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
                                             public class SupportedStandardsAttribute : Attribute
                                             {
                                                 public SupportedStandardsAttribute(params string[] supportedStandards){}

                                                 public SupportedStandardsAttribute(params NepStandard[] supportedStandards){}
                                             }

                                             """;
        [TestMethod]
        public async Task SupportedStandardsAnalyzer_UnsupportedStandard_ShouldReportDiagnostic()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("NEP5")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 28).WithArguments("NEP5");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_Nep11Suggestion_ShouldReportDiagnostic()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("NEP11")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(NepStandard.Nep11)]");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_Nep17Suggestion_ShouldReportDiagnostic()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("NEP17")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(NepStandard.Nep17)]");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_UpdateNep11_ShouldFixCode()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("NEP11")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            const string fixedCode = TestNamespace + """

                                                     [SupportedStandards(NepStandard.Nep11)]
                                                     public class TestContract
                                                     {
                                                         public static void Main()
                                                         {
                                                         }
                                                     }
                                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(NepStandard.Nep11)]");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SupportedStandardsAnalyzer_UpdateNep17_ShouldFixCode()
        {
            const string originalCode = TestNamespace + """

                                                        [SupportedStandards("NEP17")]
                                                        public class TestContract
                                                        {
                                                            public static void Main()
                                                            {
                                                            }
                                                        }
                                                        """;

            const string fixedCode = TestNamespace + """

                                                     [SupportedStandards(NepStandard.Nep17)]
                                                     public class TestContract
                                                     {
                                                         public static void Main()
                                                         {
                                                         }
                                                     }
                                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(SupportedStandardsAnalyzer.DiagnosticId)
                .WithSpan(22, 2, 22, 29).WithArguments("Consider using [SupportedStandards(NepStandard.Nep17)]");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }
    }
}
