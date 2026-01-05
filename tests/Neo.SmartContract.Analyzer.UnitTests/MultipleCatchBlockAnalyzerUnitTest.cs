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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.MultipleCatchBlockAnalyzer>;

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
                .WithSpan(7, 9, 18, 10)
                .WithArguments("2");

            await Verifier.VerifyAnalyzerAsync(sourceCode, expected);
        }
    }
}
