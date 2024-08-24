using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.BigIntegerUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{

    [TestClass]
    public class BigIntegerUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task SupportedBigIntegerMethod_ShouldNotReportDiagnostic()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = 42;
                               BigInteger y = 24;
                               BigInteger z = BigInteger.Add(x, y);
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
