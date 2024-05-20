using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.SmartContractMethodNamingAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class MethodNamingAnalyzerUnitTests
    {
        [TestMethod]
        public async Task MethodsWithSameNameAndParamCount_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Transfer(byte[] to, BigInteger amount) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodsWithSameNameButDifferentParamCount_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Transfer(byte[] to) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodsWithDifferentNames_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Withdraw(byte[] to, BigInteger amount) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
