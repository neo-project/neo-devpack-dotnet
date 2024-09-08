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
