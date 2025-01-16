// Copyright (C) 2015-2024 The Neo Project.
//
// MethodNamingAnalyzerUnderlineUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.SmartContractMethodNamingAnalyzerUnderline>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class MethodNamingAnalyzerUnderlineUnitTests
    {
        [TestMethod]
        public async Task MethodNameStartingWithUnderscoreAndSameParameterCount_ShouldReportDiagnostic()
        {
            var test = """
                       public class TestContract
                       {
                           public static void _testMethod(int a) {}
                           public static void TestMethod(int a) {}
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(SmartContractMethodNamingAnalyzerUnderline.DiagnosticId)
                .WithLocation(3, 24)
                .WithArguments("_testMethod");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task MethodNameStartingWithUnderscoreButDifferentParameterCount_ShouldNotReportDiagnostic()
        {
            var test = """
                       public class TestContract
                       {
                           public static void _testMethod(int a) {}
                           public static void TestMethod(int a, int b) {}
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic().WithSpan(3, 24, 3, 35);
            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
