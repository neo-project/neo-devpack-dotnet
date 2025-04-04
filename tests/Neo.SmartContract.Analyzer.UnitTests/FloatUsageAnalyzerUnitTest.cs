// Copyright (C) 2015-2025 The Neo Project.
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
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Neo.SmartContract.Analyzer.FloatUsageAnalyzer,
        Neo.SmartContract.Analyzer.FloatUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class FloatUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task FloatUsageAnalyzer_ReplaceWithCommonKeyword()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestFloat(){ float a = (float)1.5;}
                                        }

                                        """;

            const string fixedCode = """

                                     public class TestClass
                                     {
                                         public void TestFloat(){ int a = (int)1.5;}
                                     }

                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 50).WithArguments("float");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FloatUsageAnalyzer_ReplaceWithVar()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestFloat(){ var a = 1.5F; }
                                        }

                                        """;

            const string fixedCode = """

                                     public class TestClass
                                     {
                                         public void TestFloat(){ int a = (int)1.5; }
                                     }

                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 42).WithArguments("float");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FloatUsageAnalyzer_ReplaceWithFloat()
        {
            const string originalCode = """

                                        public class TestClass
                                        {
                                            public void TestFloat(){ float a = 1.5F;}
                                        }

                                        """;

            const string fixedCode = """

                                     public class TestClass
                                     {
                                         public void TestFloat(){ int a = (int)1.5; }
                                     }

                                     """;

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 44).WithArguments("float");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

    }
}
