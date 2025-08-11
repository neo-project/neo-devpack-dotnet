// Copyright (C) 2015-2025 The Neo Project.
//
// SystemThreadingUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.SystemThreadingUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class SystemThreadingUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task UsingSystemThreading_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Threading;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               // Some code
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                .WithLocation(1, 1)
                .WithArguments("System.Threading");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingSystemThreadingTasks_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Threading.Tasks;

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
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 1, 1, 29).WithArguments("System.Threading.Tasks"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 22, 1, 27).WithArguments("System.Threading")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingTaskClass_ShouldReportDiagnostic()
        {
            var test = """
                       using System;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               System.Threading.Tasks.Task task = System.Threading.Tasks.Task.CompletedTask;
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 28, 7, 33).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 34, 7, 39).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 64, 7, 69).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 70, 7, 75).WithArguments("System.Threading")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingAsyncMethod_ShouldReportDiagnostic()
        {
            var test = """
                       using System;
                       using System.Threading.Tasks;

                       class TestClass
                       {
                           public async Task TestMethodAsync()
                           {
                               await Task.Delay(100);
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(2, 1, 2, 29).WithArguments("System.Threading.Tasks"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(2, 22, 2, 27).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(6, 18, 6, 22).WithArguments("async keyword"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(8, 9, 8, 14).WithArguments("await keyword")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingThread_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Threading;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               Thread thread = new Thread(() => { });
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 1, 1, 23).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 9, 7, 15).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(7, 29, 7, 35).WithArguments("System.Threading")
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
        public async Task UsingCancellationToken_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Threading;

                       class TestClass
                       {
                           public void TestMethod(CancellationToken token)
                           {
                               // Some code
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(1, 1, 1, 23).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(5, 32, 5, 49).WithArguments("System.Threading")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task UsingFullyQualifiedThread_ShouldReportDiagnostic()
        {
            var test = """
                       class TestClass
                       {
                           public void TestMethod()
                           {
                               System.Threading.Thread thread = new System.Threading.Thread(() => { });
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(5, 9, 5, 25).WithArguments("System.Threading"),
                VerifyCS.Diagnostic(SystemThreadingUsageAnalyzer.DiagnosticId)
                    .WithSpan(5, 42, 5, 58).WithArguments("System.Threading")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}