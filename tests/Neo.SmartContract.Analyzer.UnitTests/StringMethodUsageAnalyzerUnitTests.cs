// Copyright (C) 2015-2026 The Neo Project.
//
// StringMethodUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.StringMethodUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class StringMethodUsageAnalyzerUnitTests
    {
        [TestMethod]
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

        [TestMethod]
        public async Task PadLeftAndPadRight_ShouldNotReportDiagnostic()
        {
            // The compiler supports string.PadLeft/PadRight (all overloads), so the
            // analyzer must not flag them as unsupported.
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               string x = "Hello";
                               string a = x.PadLeft(10);
                               string b = x.PadLeft(10, '*');
                               string c = x.PadRight(10);
                               string d = x.PadRight(10, '*');
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnsupportedStringMethod_ShouldReportDiagnostic()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               string x = "Hello";
                               string y = x.Normalize();
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(StringMethodUsageAnalyzer.DiagnosticId)
                .WithSpan(7, 20, 7, 33)
                .WithArguments("Normalize");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
