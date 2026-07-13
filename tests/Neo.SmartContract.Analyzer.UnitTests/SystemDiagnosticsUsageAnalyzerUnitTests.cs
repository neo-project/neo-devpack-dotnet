// Copyright (C) 2015-2026 The Neo Project.
//
// SystemDiagnosticsUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
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
        public async Task UsingCodeAnalysisAttribute_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Diagnostics.CodeAnalysis;

                       class TestClass
                       {
                           [DoesNotReturn]
                           public void Throw() => throw new System.Exception();
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UsingFullyQualifiedCodeAnalysisAttribute_ShouldNotReportDiagnostic()
        {
            var test = """
                       class TestClass
                       {
                           [System.Diagnostics.CodeAnalysis.DoesNotReturn]
                           public void Throw() => throw new System.Exception();
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UsingAliasedCodeAnalysisAttribute_ShouldNotReportDiagnostic()
        {
            var test = """
                       using DoesNotReturn = System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute;

                       class TestClass
                       {
                           [DoesNotReturn]
                           public void Throw() => throw new System.Exception();
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UsingCodeAnalysisTypeAsRuntimeValue_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Diagnostics.CodeAnalysis;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               {|#0:DoesNotReturnAttribute|} value = new();
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("System.Diagnostics.CodeAnalysis");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingAliasedCodeAnalysisTypeAsRuntimeValue_ShouldReportDiagnostic()
        {
            var test = """
                       using DoesNotReturn = System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               {|#0:DoesNotReturn|} value = new();
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(SystemDiagnosticsUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("System.Diagnostics.CodeAnalysis");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
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
