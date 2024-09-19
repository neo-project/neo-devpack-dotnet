using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.SystemDiagnosticsUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class SystemDiagnosticsUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task UsingSystemDiagnostics_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Diagnostics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               // Some code
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                .WithLocation(1, 1)
                .WithArguments("System.Diagnostics");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingSystemDiagnosticsSubNamespace_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Diagnostics.Tracing;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               // Some code
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 1, 1, 34).WithArguments("System.Diagnostics.Tracing"),
                VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 26, 1, 33).WithArguments("System.Diagnostics")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingSystemDiagnosticsClass_ShouldReportDiagnostic()
        {
            var test = """
                       using System;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 28, 7, 37).WithArguments("System.Diagnostics"),
                VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 73, 7, 82).WithArguments("System.Diagnostics")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingOtherNamespace_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System;
                       using System.Collections.Generic;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               List<int> list = new List<int>();
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UsingConditionalAttribute_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Diagnostics;

                       class TestClass
                       {
                           [Conditional("ALPHA")]
                           public void TestMethod()
                           {
                               // Some code
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 1, 1, 26).WithArguments("System.Diagnostics"),
                VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                    .WithSpan(5, 6, 5, 17).WithArguments("System.Diagnostics")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingFullyQualifiedConditionalAttribute_ShouldReportDiagnostic()
        {
            var test = """
                       class TestClass
                       {
                           [System.Diagnostics.Conditional("ALPHA")]
                           public void TestMethod()
                           {
                               // Some code
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                .WithSpan(3, 25, 3, 36)
                .WithArguments("System.Diagnostics");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
