// Copyright (C) 2015-2026 The Neo Project.
//
// FloatUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.FloatUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class FloatUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task FloatUsageAnalyzer_ExplicitCast_ShouldReportDiagnostic()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestFloat(){ float a = (float)1.5;}
                                        }

                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 50).WithArguments("float");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FloatUsageAnalyzer_InferredType_ShouldReportDiagnostic()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestFloat(){ var a = 1.5F; }
                                        }

                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 42).WithArguments("float");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FloatUsageAnalyzer_ExplicitType_ShouldReportDiagnostic()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestFloat(){ float a = 1.5F;}
                                        }

                                        """;

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 44).WithArguments("float");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FloatUsageAnalyzer_MethodSignature_ShouldReportDiagnostic()
        {
            const string test = """

                                public class TestClass
                                {
                                    public {|#0:float|} TestFloat({|#1:float|} value) => value;
                                }

                                """;

            var returnDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("float");
            var parameterDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("float");

            await Verifier.VerifyAnalyzerAsync(test, returnDiagnostic, parameterDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FloatUsageAnalyzer_Property_ShouldReportDiagnostic()
        {
            const string test = """

                                public class TestClass
                                {
                                    public {|#0:float|} Value { get; set; }
                                }

                                """;

            var propertyDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("float");

            await Verifier.VerifyAnalyzerAsync(test, propertyDiagnostic).ConfigureAwait(false);
        }
    }
}
