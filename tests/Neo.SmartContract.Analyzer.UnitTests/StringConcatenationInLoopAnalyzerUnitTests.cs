// Copyright (C) 2015-2026 The Neo Project.
//
// StringConcatenationInLoopAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.StringConcatenationInLoopAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class StringConcatenationInLoopAnalyzerUnitTests
{
    [TestMethod]
    public async Task CompoundAssignmentInForLoop_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(int count)
                       {
                           var result = "";
                           for (int i = 0; i < count; i++)
                               {|#0:result += i|};
                           return result;
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("result");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task CompoundAssignmentInOtherLoops_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string While(string[] values)
                       {
                           var result = "";
                           int index = 0;
                           while (index < values.Length)
                           {
                               {|#0:result += values[index]|};
                               index++;
                           }

                           do
                           {
                               {|#1:result += "-"|};
                           }
                           while (false);

                           foreach (var value in values)
                               {|#2:result += value|};

                           return result;
                       }
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("result"),
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(1).WithArguments("result"),
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(2).WithArguments("result"),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task SelfConcatenationAssignment_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(string[] values)
                       {
                           var result = "";
                           foreach (var value in values)
                               {|#0:result = result + value|};
                           return result;
                       }

                       string Prepend(string[] values)
                       {
                           var result = "";
                           foreach (var value in values)
                               {|#1:result = value + result|};
                           return result;
                       }
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("result"),
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(1).WithArguments("result"),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task ConcatenationOutsideLoop_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(string value)
                       {
                           var result = "";
                           result += value;
                           result = result + value;
                           return result;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task LocalDeclaredInsideLoop_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(string[] values)
                       {
                           var output = "";
                           foreach (var value in values)
                           {
                               var current = "";
                               current += value;
                               output = current;
                           }
                           return output;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task UnrelatedConcatenationInsideLoop_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(string left, string right, int count)
                       {
                           var result = "";
                           for (int i = 0; i < count; i++)
                               result = left + right;
                           return result;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task NonStringCompoundAssignmentInsideLoop_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       int Sum(int count)
                       {
                           var total = 0;
                           for (int i = 0; i < count; i++)
                               total += i;
                           return total;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
