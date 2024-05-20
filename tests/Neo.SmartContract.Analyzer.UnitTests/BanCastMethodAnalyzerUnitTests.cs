using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.BanCastMethodAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class BanCastMethodAnalyzerUnitTests
    {
        [TestMethod]
        public async Task CastMethod_ShouldReportDiagnostic()
        {
            var test = """
                       using System;
                       using System.Collections.Generic;
                       using System.Linq;

                       namespace TestNamespace
                       {
                           public class TestClass
                           {
                               public void TestMethod()
                               {
                                   var numbers = new[] { 1, 2, 3, 4 };
                                   var doubledNumbers = numbers.Cast<int>().Select(x => x * 2);
                               }
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(BanCastMethodAnalyzer.DiagnosticId)
                .WithLocation(12, 42);

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
