// Copyright (C) 2015-2024 The Neo Project.
//
// DoubleUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
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
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Neo.SmartContract.Analyzer.DoubleUsageAnalyzer,
        Neo.SmartContract.Analyzer.DoubleUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class DoubleUsageAnalyzerUnitTest
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
