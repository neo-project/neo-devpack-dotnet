using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.StaticFieldInitializationAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class StaticFieldInitializationAnalyzerUnitTest
    {
        [TestMethod]
        public async Task ValidUInt256Initialization_NoDiagnostic()
        {
            const string code = """
                                 public abstract class UInt256
                                 {
                                     public static extern implicit operator UInt256(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly UInt256 validUInt256 = "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";
                                 }
                                 """;

            await Verifier.VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task InvalidUInt256Initialization_ReportsDiagnostic()
        {
            const string code = """
                                 public abstract class UInt256
                                 {
                                     public static extern implicit operator UInt256(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly UInt256 invalidUInt256 = "invalid";
                                 }
                                 """;

            var expectedDiagnostic = Verifier.Diagnostic("NC4023")
                .WithSpan(7, 37, 7, 63).WithArguments("UInt256 must be initialized with a 64-character hex string.");

            await Verifier.VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task ValidUInt160HexInitialization_NoDiagnostic()
        {
            const string code = """
                                 public abstract class UInt160
                                 {
                                     public static extern implicit operator UInt160(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly UInt160 validUInt160 = "1a4fe29ad7db232a493e5b990fb1da7af0c7b989";
                                 }
                                 """;

            await Verifier.VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task ValidUInt160AddressInitialization_NoDiagnostic()
        {
            const string code = """
                                 public abstract class UInt160
                                 {
                                     public static extern implicit operator UInt160(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly UInt160 validUInt160 = "NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq";
                                 }
                                 """;

            await Verifier.VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task InvalidUInt160Initialization_ReportsDiagnostic()
        {
            const string code = """
                                 public abstract class UInt160
                                 {
                                     public static extern implicit operator UInt160(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly UInt160 invalidUInt160 = "invalid";
                                 }
                                 """;

            var expectedDiagnostic = Verifier.Diagnostic("NC4023")
                .WithSpan(7, 37, 7, 63).WithArguments("UInt160 must be initialized with a 40-character hex string or a 34-character string starting with 'N'.");

            await Verifier.VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task ValidECPointInitialization_NoDiagnostic()
        {
            const string code = """
                                 public abstract class ECPoint
                                 {
                                     public static extern implicit operator ECPoint(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly ECPoint validECPoint = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";
                                 }
                                 """;

            await Verifier.VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task InvalidECPointInitialization_ReportsDiagnostic()
        {
            const string code = """
                                 public abstract class ECPoint
                                 {
                                     public static extern implicit operator ECPoint(string value);
                                 }
                                 public class Test
                                 {
                                     private static readonly ECPoint invalidECPoint = "invalid";
                                 }
                                 """;

            var expectedDiagnostic = Verifier.Diagnostic("NC4023")
                .WithSpan(7, 37, 7, 63).WithArguments("ECPoint must be initialized with a 66-character hex string.");

            await Verifier.VerifyAnalyzerAsync(code, expectedDiagnostic);
        }
    }
}
