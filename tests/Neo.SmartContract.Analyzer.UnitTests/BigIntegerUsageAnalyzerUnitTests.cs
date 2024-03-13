using System.Threading.Tasks;
using Xunit;

using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.BigIntegerUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{

    public class BigIntegerUsageAnalyzerUnitTests
    {
        [Fact]
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

        [Fact]
        public async Task UnsupportedBigIntegerProperty_ShouldReportDiagnostic()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = 42;
                               bool isPowerOfTwo = x.IsPowerOfTwo;
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(BigIntegerUsageAnalyzer.DiagnosticId)
                .WithLocation(9, 29)
                .WithArguments("IsPowerOfTwo");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
