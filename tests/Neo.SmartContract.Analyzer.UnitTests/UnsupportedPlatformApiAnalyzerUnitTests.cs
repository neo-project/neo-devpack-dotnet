// Copyright (C) 2015-2026 The Neo Project.
//
// UnsupportedPlatformApiAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.UnsupportedPlatformApiAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class UnsupportedPlatformApiAnalyzerUnitTests
{
    [TestMethod]
    public async Task UnsupportedPlatformNamespaces_ShouldReportDiagnostic()
    {
        var test = """
                   {|#0:using System.IO;|}
                   {|#1:using System.Net;|}
                   {|#2:using System.Reflection;|}
                   {|#3:using System.Threading;|}

                   class Test
                   {
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(0).WithArguments("System.IO"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(1).WithArguments("System.Net"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(2).WithArguments("System.Reflection"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(3).WithArguments("System.Threading"),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task UnsupportedPlatformTypes_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       void Run()
                       {
                           var memory = new {|#0:System.IO.MemoryStream|}();
                           {|#1:System.Console|}.WriteLine("neo");
                           var now = {|#2:System.DateTime|}.UtcNow;
                           var id = {|#3:System.Guid|}.NewGuid();
                           var random = new {|#4:System.Random|}();
                       }
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(0).WithArguments("System.IO.MemoryStream"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(1).WithArguments("System.Console"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(2).WithArguments("System.DateTime"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(3).WithArguments("System.Guid"),
            VerifyCS.Diagnostic(UnsupportedPlatformApiAnalyzer.DiagnosticId).WithLocation(4).WithArguments("System.Random"),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}
