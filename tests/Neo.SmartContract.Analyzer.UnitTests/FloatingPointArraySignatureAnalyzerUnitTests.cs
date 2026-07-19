// Copyright (C) 2015-2026 The Neo Project.
//
// FloatingPointArraySignatureAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DecimalVerifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.DecimalUsageAnalyzer>;
using DoubleVerifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.DoubleUsageAnalyzer>;
using FloatVerifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.FloatUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class FloatingPointArraySignatureAnalyzerUnitTests
    {
        [TestMethod]
        public async Task DoubleArrayReturnType_ShouldReportDiagnostic()
        {
            const string test = """
                                public class TestClass
                                {
                                    public {|#0:double[]|} Values() => new double[1];
                                }
                                """;

            var expected = DoubleVerifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("double");

            await DoubleVerifier.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task JaggedDoubleArrayReturnType_ShouldReportDiagnostic()
        {
            const string test = """
                                public class TestClass
                                {
                                    public {|#0:double[][]|} Values() => new double[1][];
                                }
                                """;

            var expected = DoubleVerifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("double");

            await DoubleVerifier.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FloatArrayParameter_ShouldReportDiagnostic()
        {
            const string test = """
                                public class TestClass
                                {
                                    public void SetValues({|#0:float[]|} values) { }
                                }
                                """;

            var expected = FloatVerifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("float");

            await FloatVerifier.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task DecimalArrayProperty_ShouldReportDiagnostic()
        {
            const string test = """
                                public class TestClass
                                {
                                    public {|#0:decimal[]|} Values { get; set; }
                                }
                                """;

            var expected = DecimalVerifier.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("System_Decimal", "decimal");

            await DecimalVerifier.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task IntegerArraySignatures_ShouldNotReportDiagnostics()
        {
            const string test = """
                                public class TestClass
                                {
                                    public int[] Values { get; set; }
                                    public int[] Copy(int[] values) => values;
                                }
                                """;

            await DoubleVerifier.VerifyAnalyzerAsync(test);
            await FloatVerifier.VerifyAnalyzerAsync(test);
            await DecimalVerifier.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task NestedFloatingPointSignatureTypes_ShouldReportDiagnostics()
        {
            const string doubleTest = """
                                      public class TestClass
                                      {
                                          public {|#0:System.Tuple<int, double>|} Values() => null;
                                      }
                                      """;
            const string floatTest = """
                                     public class Wrapper<T> { }
                                     public class TestClass
                                     {
                                         public void SetValues({|#0:Wrapper<float>|} values) { }
                                     }
                                     """;
            const string decimalTest = """
                                       public class TestClass
                                       {
                                           public {|#0:System.Tuple<decimal>|} Values { get; set; }
                                       }
                                       """;

            await DoubleVerifier.VerifyAnalyzerAsync(
                doubleTest,
                DoubleVerifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("double"));
            await FloatVerifier.VerifyAnalyzerAsync(
                floatTest,
                FloatVerifier.Diagnostic(FloatUsageAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("float"));
            await DecimalVerifier.VerifyAnalyzerAsync(
                decimalTest,
                DecimalVerifier.Diagnostic(DecimalUsageAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("System_Decimal", "decimal"));
        }

        [TestMethod]
        public async Task NestedFloatingPointLocalTypes_ShouldReportDiagnostics()
        {
            const string test = """
                                public class Wrapper<T> { }
                                public class TestClass
                                {
                                    public void Test()
                                    {
                                        {|#0:Wrapper<double> value = null|};
                                    }
                                }
                                """;

            var expected = DoubleVerifier.Diagnostic(DoubleUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("double");

            await DoubleVerifier.VerifyAnalyzerAsync(test, expected);
        }
    }
}
