// Copyright (C) 2015-2026 The Neo Project.
//
// BitOperationsUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.BitOperationsUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class BitOperationsUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task UnsupportedMethods_ShouldReportDiagnostics()
        {
            var test = """
                using System.Numerics;

                class TestClass
                {
                    void TestMethod()
                    {
                        var trailing32 = {|#0:BitOperations.TrailingZeroCount(1u)|};
                        var trailing64 = {|#1:BitOperations.TrailingZeroCount(1ul)|};
                    }
                }
                """;

            var expected0 = VerifyCS.Diagnostic(BitOperationsUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("BitOperations.TrailingZeroCount(uint)");
            var expected1 = VerifyCS.Diagnostic(BitOperationsUsageAnalyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("BitOperations.TrailingZeroCount(ulong)");
            await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1);
        }

        [TestMethod]
        public async Task SupportedRegisteredOverloads_ShouldNotReportDiagnostics()
        {
            var test = """
                using System.Numerics;

                class TestClass
                {
                    void TestMethod(uint value32, ulong value64, int offset)
                    {
                        _ = BitOperations.Log2(value32);
                        _ = BitOperations.Log2(value64);
                        _ = BitOperations.PopCount(value32);
                        _ = BitOperations.PopCount(value64);
                        _ = BitOperations.LeadingZeroCount(value32);
                        _ = BitOperations.LeadingZeroCount(value64);
                        _ = BitOperations.RotateLeft(value32, offset);
                        _ = BitOperations.RotateLeft(value64, offset);
                        _ = BitOperations.RotateRight(value32, offset);
                        _ = BitOperations.RotateRight(value64, offset);
                    }
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task AliasesAndStaticImports_ShouldStillReportDiagnostics()
        {
            var test = """
                using Bits = System.Numerics.BitOperations;
                using static System.Numerics.BitOperations;

                class TestClass
                {
                    void TestMethod()
                    {
                        _ = {|#0:Bits.TrailingZeroCount(1u)|};
                        _ = {|#1:TrailingZeroCount(1ul)|};
                    }
                }
                """;

            var expected0 = VerifyCS.Diagnostic(BitOperationsUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("BitOperations.TrailingZeroCount(uint)");
            var expected1 = VerifyCS.Diagnostic(BitOperationsUsageAnalyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("BitOperations.TrailingZeroCount(ulong)");

            await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1);
        }

        [TestMethod]
        public async Task UserDefinedTypeWithSameName_ShouldNotReportDiagnostic()
        {
            var test = """
                class BitOperations
                {
                    public static int TrailingZeroCount(uint value) => 0;
                }

                class TestClass
                {
                    void TestMethod()
                    {
                        _ = BitOperations.TrailingZeroCount(1u);
                    }
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
