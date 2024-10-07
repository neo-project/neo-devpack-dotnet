using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.StringMethodUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class StringMethodUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task SupportedStringMethod_ShouldNotReportDiagnostic()
        {
            var test = """

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               string x = "Hello";
                               int length = x.Length;
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
