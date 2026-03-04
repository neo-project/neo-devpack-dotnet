// Copyright (C) 2015-2026 The Neo Project.
//
// VolatileKeywordUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.VolatileKeywordUsageAnalyzer,
    Neo.SmartContract.Analyzer.VolatileKeywordRemovalCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class VolatileKeywordUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task VolatileField_ShouldReportDiagnostic()
        {
            var test = """
                       class TestClass
                       {
                           private volatile int _counter;
                       }
                       """;

            var expected = Verifier.Diagnostic(VolatileKeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(3, 26, 3, 34)
                .WithArguments("field declaration");
            await Verifier.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task VolatileField_CodeFix_RemovesVolatile()
        {
            var test = """
                       class TestClass
                       {
                           private volatile int _counter;
                       }
                       """;

            var fixedCode = """
                            class TestClass
                            {
                                private int _counter;
                            }
                            """;

            var expected = Verifier.Diagnostic(VolatileKeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(3, 26, 3, 34)
                .WithArguments("field declaration");
            await Verifier.VerifyCodeFixAsync(test, expected, fixedCode);
        }

        [TestMethod]
        public async Task NonVolatileField_ShouldNotReportDiagnostic()
        {
            var test = """
                       class TestClass
                       {
                           private int _counter;
                       }
                       """;

            await Verifier.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task StaticField_ShouldNotReportDiagnostic()
        {
            var test = """
                       class TestClass
                       {
                           private static int _counter;
                       }
                       """;

            await Verifier.VerifyAnalyzerAsync(test);
        }
    }
}
