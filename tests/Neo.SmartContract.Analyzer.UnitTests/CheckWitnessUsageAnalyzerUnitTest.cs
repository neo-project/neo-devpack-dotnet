// Copyright (C) 2015-2025 The Neo Project.
//
// CheckWitnessUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = Neo.SmartContract.Analyzer.UnitTests.TestHelper;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class CheckWitnessUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_UnverifiedResult_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main(UInt160 owner)
                                          {
                                              // Unverified CheckWitness result
                                              Runtime.CheckWitness(owner);
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<CheckWitnessUsageAnalyzer>();
            test.TestCode = sourceCode;

            // Our analyzer diagnostic - the main thing we're testing
            test.ExpectedDiagnostics.Add(new DiagnosticResult(CheckWitnessUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(9, 15, 9, 41)
                .WithArguments("The result of Runtime.CheckWitness(...) should be used in a condition, assigned to a variable, passed to ExecutionEngine.Assert, or otherwise utilized"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithAssert_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main(UInt160 owner)
                                          {
                                              // Verified with Assert
                                              Assert(Runtime.CheckWitness(owner));
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<CheckWitnessUsageAnalyzer>();
            test.TestCode = sourceCode;

            // No analyzer diagnostics expected - CheckWitness is properly used with Assert

            await test.RunAsync();
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithIfCondition_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main(UInt160 owner)
                                          {
                                              // Verified with if condition
                                              if (Runtime.CheckWitness(owner))
                                              {
                                                  // Authorized code
                                              }
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<CheckWitnessUsageAnalyzer>();
            test.TestCode = sourceCode;

            // No analyzer diagnostics expected - CheckWitness is properly used in if condition

            await test.RunAsync();
        }

        // Code fix tests are temporarily disabled due to framework compatibility issues
        // TODO: Re-enable and fix these tests once framework compatibility is resolved


    }
}
