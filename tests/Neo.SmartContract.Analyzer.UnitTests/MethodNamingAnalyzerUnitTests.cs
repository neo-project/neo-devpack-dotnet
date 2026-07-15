// Copyright (C) 2015-2026 The Neo Project.
//
// MethodNamingAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.SmartContractMethodNamingAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class MethodNamingAnalyzerUnitTests
    {
        [TestMethod]
        public async Task MethodsWithSameNameAndParamCount_ShouldReportDiagnostic()
        {
            var test = """
                       namespace Neo.SmartContract.Framework
                       {
                           public class SmartContract { }
                       }

                       public class TestContract : Neo.SmartContract.Framework.SmartContract
                       {
                           {|#0:public void Transfer(int value) { }|}
                           {|#1:public void Transfer(string value) { }|}
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SmartContractMethodNamingAnalyzer.DiagnosticId).WithLocation(0).WithArguments("Transfer"),
                VerifyCS.Diagnostic(SmartContractMethodNamingAnalyzer.DiagnosticId).WithLocation(1).WithArguments("Transfer"),
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
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

        [TestMethod]
        public void MethodNamingDiagnostics_ShouldNotOfferAutomaticCodeFixes()
        {
            var fixes = typeof(SmartContractMethodNamingAnalyzer).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(CodeFixProvider).IsAssignableFrom(type))
                .Select(type => (CodeFixProvider)Activator.CreateInstance(type)!)
                .SelectMany(provider => provider.FixableDiagnosticIds
                    .Where(id => id is SmartContractMethodNamingAnalyzer.DiagnosticId or SmartContractMethodNamingAnalyzerUnderline.DiagnosticId)
                    .Select(id => $"{provider.GetType().Name}: {id}"))
                .OrderBy(fix => fix, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(0, fixes.Length, string.Join(Environment.NewLine, fixes));
        }
    }
}
