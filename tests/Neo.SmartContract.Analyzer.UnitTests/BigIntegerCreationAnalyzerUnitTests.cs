using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.BigIntegerCreationAnalyzer,
    Neo.SmartContract.Analyzer.BigIntegerCreationCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class BigIntegerCreationAnalyzerUnitTests
    {
        [TestMethod]
        public async Task BigIntegerCreationWithInt_ShouldReportDiagnostic()
        {
            var test = @"
using System.Numerics;

class TestClass
{
    public void TestMethod()
    {
        BigInteger x = new BigInteger(42);
    }
}";

            var expectedDiagnostic = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(8, 24)
                .WithMessage("Use of new BigInteger(int) is not allowed, please use BigInteger x = 0;");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task BigIntegerCreationWithInt_ShouldNotReportDiagnostic()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = 42;
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BigIntegerCreationWithInt_ShouldReplaceWithDirectAssignment()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = new BigInteger(42);
                           }
                       }
                       """;

            var fixtest = """

                          using System.Numerics;

                          class TestClass
                          {
                              public void TestMethod()
                              {
                                  BigInteger x = 42;
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(8, 24)
                .WithMessage("Use of new BigInteger(int) is not allowed, please use BigInteger x = 0;");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }
    }
}
