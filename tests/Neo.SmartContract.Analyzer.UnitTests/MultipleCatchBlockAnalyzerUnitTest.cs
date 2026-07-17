// Copyright (C) 2015-2026 The Neo Project.
//
// MultipleCatchBlockAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.MultipleCatchBlockAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class MultipleCatchBlockAnalyzerUnitTest
    {
        [TestMethod]
        public async Task MultipleCatchBlockAnalyzer_DetectMultipleCatchBlocks()
        {
            const string sourceCode = """
                                      using System;

                                      public class TestClass
                                      {
                                          public void TestMethod()
                                          {
                                              try
                                              {
                                                  // Some code that might throw an exception
                                              }
                                              catch (FormatException ex)
                                              {
                                                  // Handle general exception
                                              }
                                              catch (Exception ex)
                                              {
                                                  // Handle specific exception
                                              }
                                          }
                                      }
                                      """;

            var expected = Verifier.Diagnostic(MultipleCatchBlockAnalyzer.DiagnosticId)
                .WithSpan(15, 9, 15, 14)
                .WithArguments("2");

            await Verifier.VerifyAnalyzerAsync(sourceCode, expected);
        }

        [TestMethod]
        public async Task MultipleCatchBlockAnalyzer_AllowsSingleCatchBlock()
        {
            const string sourceCode = """
                                      using System;

                                      public class TestClass
                                      {
                                          public void TestMethod()
                                          {
                                              try
                                              {
                                              }
                                              catch (Exception)
                                              {
                                              }
                                          }
                                      }
                                      """;

            await Verifier.VerifyAnalyzerAsync(sourceCode);
        }

        [TestMethod]
        public async Task MultipleCatchBlockAnalyzer_ReportsSecondOfThreeCatchBlocks()
        {
            const string sourceCode = """
                                      using System;

                                      public class TestClass
                                      {
                                          public void TestMethod()
                                          {
                                              try
                                              {
                                              }
                                              catch (FormatException)
                                              {
                                              }
                                              catch (InvalidOperationException)
                                              {
                                              }
                                              catch (Exception)
                                              {
                                              }
                                          }
                                      }
                                      """;

            var expected = Verifier.Diagnostic(MultipleCatchBlockAnalyzer.DiagnosticId)
                .WithSpan(13, 9, 13, 14)
                .WithArguments("3");

            await Verifier.VerifyAnalyzerAsync(sourceCode, expected);
        }

        [TestMethod]
        public async Task MultipleCatchBlockAnalyzer_HandlesIncompleteSecondCatchBlock()
        {
            const string sourceCode = """
                                      using System;

                                      public class TestClass
                                      {
                                          public void TestMethod()
                                          {
                                              try
                                              {
                                              }
                                              catch (FormatException)
                                              {
                                              }
                                              catch (Exception)
                                      """;

            var test = new CSharpAnalyzerTest<MultipleCatchBlockAnalyzer, DefaultVerifier>
            {
                TestCode = sourceCode,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            test.ExpectedDiagnostics.Add(
                Verifier.Diagnostic(MultipleCatchBlockAnalyzer.DiagnosticId)
                    .WithSpan(13, 9, 13, 14)
                    .WithArguments("2"));

            await test.RunAsync();
        }
    }
}
