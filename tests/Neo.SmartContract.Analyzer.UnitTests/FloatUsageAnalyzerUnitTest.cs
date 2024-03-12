using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Neo.SmartContract.Analyzer.FloatUsageAnalyzer,
        Neo.SmartContract.Analyzer.FloatUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public class FloatUsageAnalyzerUnitTest
    {
        [Fact]
        public async Task FloatUsageAnalyzer_ReplaceWithCommonKeyword()
        {
            const string originalCode = @"
public class TestClass
{
    public void TestFloat(){ float a = (float)1.5;}
}
";

            const string fixedCode = @"
public class TestClass
{
    public void TestFloat(){ int a = (int)1.5;}
}
";

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 50).WithArguments("float");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }


        [Fact]
        public async Task FloatUsageAnalyzer_ReplaceWithVar()
        {
            const string originalCode = @"
public class TestClass
{
    public void TestFloat(){ var a = 1.5F; }
}
";

            const string fixedCode = @"
public class TestClass
{
    public void TestFloat(){ int a = (int)1.5; }
}
";

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 42).WithArguments("float");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }


        [Fact]
        public async Task FloatUsageAnalyzer_ReplaceWithFloat()
        {
            const string originalCode = @"
public class TestClass
{
    public void TestFloat(){ float a = 1.5F;}
}
";

            const string fixedCode = @"
public class TestClass
{
    public void TestFloat(){ int a = (int)1.5; }
}
";

            var expectedDiagnostic = Verifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithSpan(4, 30, 4, 44).WithArguments("float");

            await Verifier.VerifyCodeFixAsync(originalCode, expectedDiagnostic, fixedCode).ConfigureAwait(false);
        }

    }
}
