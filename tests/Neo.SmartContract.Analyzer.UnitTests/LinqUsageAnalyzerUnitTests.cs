// Copyright (C) 2015-2025 The Neo Project.
//
// LinqUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.LinqUsageAnalyzer,
    Neo.SmartContract.Analyzer.LinqUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class LinqUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task LinqUsage_ShouldReportDiagnostic()
        {
            var test = """
                       using System;
                       using System.Linq;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               var numbers = new int[] { 1, 2, 3, 4, 5 };
                               var evenNumbers = numbers.Where(x => x % 2 == 0);
                           }
                       }
                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                .WithLocation(2, 1)
                .WithArguments("System.Linq");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task LinqUsage_ShouldChangeTo_NeoLinq()
        {
            var test = """
                       using System;
                       using System.Linq;

                       namespace Neo.SmartContract.Framework.Linq
                       {
                           public static class LinqExtensions
                           {}
                       }

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               var numbers = new int[] { 1, 2, 3, 4, 5 };
                           }
                       }
                       """;

            var fixtest = """
                          using System;
                          using Neo.SmartContract.Framework.Linq;

                          namespace Neo.SmartContract.Framework.Linq
                          {
                              public static class LinqExtensions
                              {}
                          }

                          class TestClass
                          {
                              public void TestMethod()
                              {
                                  var numbers = new int[] { 1, 2, 3, 4, 5 };
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(LinqUsageAnalyzer.DiagnosticId)
                .WithLocation(2, 1)
                .WithArguments("System.Linq");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }
    }
}
