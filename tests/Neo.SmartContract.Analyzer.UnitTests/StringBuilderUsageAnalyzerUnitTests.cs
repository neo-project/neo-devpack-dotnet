// Copyright (C) 2015-2026 The Neo Project.
//
// StringBuilderUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.StringBuilderUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class StringBuilderUsageAnalyzerUnitTests
{
    [TestMethod]
    public async Task SupportedMembers_ShouldNotReportDiagnostics()
    {
        var test = """
                   using System.Text;

                   class TestClass
                   {
                       void Test()
                       {
                           var sb = new StringBuilder();
                           var other = new StringBuilder();
                           var result = sb.Append("neo")
                                           .Append(' ')
                                           .Append(other)
                                           .AppendLine()
                                           .AppendLine("runtime")
                                           .ToString();
                           sb.Clear();
                           var length = sb.Length;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task UnsupportedMethod_ShouldReportDiagnostic()
    {
        var test = """
                   using System.Text;

                   class TestClass
                   {
                       void Test()
                       {
                           var sb = new StringBuilder();
                           sb.Replace("a", "b");
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(StringBuilderUsageAnalyzer.DiagnosticId)
            .WithSpan(8, 9, 8, 29)
            .WithArguments("StringBuilder.Replace(string, string?)");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task UnsupportedAppendParameter_ShouldReportDiagnostic()
    {
        var test = """
                   using System.Text;

                   class TestClass
                   {
                       void Test()
                       {
                           var sb = new StringBuilder();
                           sb.Append(42);
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(StringBuilderUsageAnalyzer.DiagnosticId)
            .WithSpan(8, 9, 8, 22)
            .WithArguments("StringBuilder.Append(int)");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task LengthSetter_ShouldReportDiagnostic()
    {
        var test = """
                   using System.Text;

                   class TestClass
                   {
                       void Test()
                       {
                           var sb = new StringBuilder();
                           sb.Length = 5;
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(StringBuilderUsageAnalyzer.DiagnosticId)
            .WithSpan(8, 9, 8, 22)
            .WithArguments("Length (set)");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}
