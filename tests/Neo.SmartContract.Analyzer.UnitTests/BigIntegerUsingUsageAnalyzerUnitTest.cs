// Copyright (C) 2015-2024 The Neo Project.
//
// BigIntegerUsingUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.BigIntegerUsingUsageAnalyzer,
    Neo.SmartContract.Analyzer.BigIntegerUsingUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class BigIntegerUsingUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task BigIntegerUsingUsageAnalyzer_IncorrectUsing_ShouldReportDiagnostic()
        {
            const string originalCode = """
                                        using BigInteger = System.Int64;

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                BigInteger value = 42;
                                            }
                                        }
                                        """;
            var expectedDiagnostic = Verifier.Diagnostic(BigIntegerUsingUsageAnalyzer.DiagnosticId)
                .WithSpan(1, 1, 1, 33).WithArguments("using BigInteger = System.Int64;");

            await Verifier.VerifyAnalyzerAsync(originalCode, expectedDiagnostic).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task BigIntegerUsingUsageAnalyzer_CorrectUsing_ShouldNotReportDiagnostic()
        {
            const string originalCode = """
                                        using BigInteger = System.Numerics.BigInteger;

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                BigInteger value = 42;
                                            }
                                        }
                                        """;
            await Verifier.VerifyAnalyzerAsync(originalCode).ConfigureAwait(false);
        }
    }
}
