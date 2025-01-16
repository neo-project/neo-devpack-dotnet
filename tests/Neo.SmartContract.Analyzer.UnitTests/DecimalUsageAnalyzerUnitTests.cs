// Copyright (C) 2015-2025 The Neo Project.
//
// DecimalUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Neo.SmartContract.Analyzer.DecimalUsageAnalyzer,
        Neo.SmartContract.Analyzer.DecimalUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class DecimalUsageAnalyzerUnitTests
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
                                  long d = (long)1.0m;
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                .WithLocation(6, 9)
                .WithArguments("System_Decimal", "decimal");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }

        [TestMethod]
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
                                  long d = (long)1.0;
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
