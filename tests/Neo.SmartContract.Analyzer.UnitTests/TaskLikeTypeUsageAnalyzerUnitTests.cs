// Copyright (C) 2015-2026 The Neo Project.
//
// TaskLikeTypeUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.TaskLikeTypeUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class TaskLikeTypeUsageAnalyzerUnitTests
{
    [TestMethod]
    public async Task TaskLikeMethodSignature_ShouldReportDiagnostic()
    {
        var test = """
                   using System.Threading.Tasks;

                   class Test
                   {
                       public {|#0:Task<int>|} Echo({|#1:Task<int>|} value) => value;
                       public {|#2:ValueTask|} Run({|#3:ValueTask<int>|} value) => default;
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(TaskLikeTypeUsageAnalyzer.DiagnosticId).WithLocation(0).WithArguments("System.Threading.Tasks.Task<int>"),
            VerifyCS.Diagnostic(TaskLikeTypeUsageAnalyzer.DiagnosticId).WithLocation(1).WithArguments("System.Threading.Tasks.Task<int>"),
            VerifyCS.Diagnostic(TaskLikeTypeUsageAnalyzer.DiagnosticId).WithLocation(2).WithArguments("System.Threading.Tasks.ValueTask"),
            VerifyCS.Diagnostic(TaskLikeTypeUsageAnalyzer.DiagnosticId).WithLocation(3).WithArguments("System.Threading.Tasks.ValueTask<int>"),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}
