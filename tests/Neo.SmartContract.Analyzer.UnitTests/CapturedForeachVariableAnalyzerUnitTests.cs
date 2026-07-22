// Copyright (C) 2015-2026 The Neo Project.
//
// CapturedForeachVariableAnalyzerUnitTests.cs file belongs to the neo project and is free
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
    Neo.SmartContract.Analyzer.CapturedForeachVariableAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class CapturedForeachVariableAnalyzerUnitTests
{
    [TestMethod]
    public async Task LambdaCapturingForeachVariable_ShouldReportDiagnostic()
    {
        var test = """
                   using System;
                   using System.Collections.Generic;

                   class Test
                   {
                       List<Func<int>> Capture(int[] values)
                       {
                           var callbacks = new List<Func<int>>();
                           foreach (int value in values)
                           {
                               callbacks.Add(() => {|#0:value|});
                           }

                           return callbacks;
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(CapturedForeachVariableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("value");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task RepeatedReferences_ShouldReportOneDiagnosticPerLambda()
    {
        var test = """
                   using System;
                   using System.Collections.Generic;

                   class Test
                   {
                       List<Func<int>> Capture(int[] values)
                       {
                           var callbacks = new List<Func<int>>();
                           foreach (int value in values)
                           {
                               callbacks.Add(() => {|#0:value|} + value);
                           }

                           return callbacks;
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(CapturedForeachVariableAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("value");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task LambdaWithoutForeachCapture_ShouldNotReportDiagnostic()
    {
        var test = """
                   using System;
                   using System.Collections.Generic;

                   class Test
                   {
                       List<Func<int>> Create(int[] values)
                       {
                           var callbacks = new List<Func<int>>();
                           foreach (int value in values)
                           {
                               callbacks.Add(() => 42);
                           }

                           return callbacks;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task LambdaOutsideForeach_ShouldNotReportDiagnostic()
    {
        var test = """
                   using System;

                   class Test
                   {
                       Func<int> Capture(int value) => () => value;
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task NameofForeachVariable_ShouldNotReportDiagnostic()
    {
        var test = """
                   using System;
                   using System.Collections.Generic;

                   class Test
                   {
                       List<Func<int>> Create(int[] values)
                       {
                           var callbacks = new List<Func<int>>();
                           foreach (int value in values)
                           {
                               callbacks.Add(() => nameof(value).Length);
                           }

                           return callbacks;
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
