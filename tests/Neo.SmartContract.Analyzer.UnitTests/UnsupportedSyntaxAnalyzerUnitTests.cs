// Copyright (C) 2015-2026 The Neo Project.
//
// UnsupportedSyntaxAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.UnsupportedSyntaxAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class UnsupportedSyntaxAnalyzerUnitTests
{
    [TestMethod]
    public async Task UnsafeCode_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       void M()
                       {
                           {|#0:unsafe
                           {
                               int value = 0;
                           }|}
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.UnsafeCodeRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task AnonymousMethod_IsFlagged()
    {
        var test = """
                   using System;

                   class Test
                   {
                       void M()
                       {
                           Action action = {|#0:delegate { }|};
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AnonymousMethodRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task AsyncAwait_IsFlagged()
    {
        var test = """
                   using System.Threading.Tasks;

                   class Test
                   {
                       public {|#0:async|} Task Foo()
                       {
                           {|#1:await Task.Delay(1)|};
                       }
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AsyncMethodRuleId).WithLocation(0),
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AwaitExpressionRuleId).WithLocation(1),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task QueryExpression_IsFlagged()
    {
        var test = """
                   using System.Linq;

                   class Test
                   {
                       void M()
                       {
                           var numbers = new[] { 1, 2, 3 };
                           var evens = {|#0:from n in numbers
                                         where n % 2 == 0
                                         select n|};
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.QueryExpressionRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task DynamicBinding_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       {|#0:dynamic|} value = null;
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.DynamicBindingRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task IteratorBlock_IsFlagged()
    {
        var test = """
                   using System.Collections.Generic;

                   class Test
                   {
                       IEnumerable<int> Run()
                       {
                           {|#0:yield return 1;|}
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.IteratorRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task PatternMatching_IsAllowed()
    {
        var test = """
                   class Test
                   {
                       bool IsInt(object value) => {|#0:value is int n|};
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task PatternMatching_InSwitchStatement_IsAllowed()
    {
        var test = """
                   class Test
                   {
                       int Parse(object value)
                       {
                           switch (value)
                           {
                               case {|#0:int number|}:
                                   return number;
                               default:
                                   return 0;
                           }
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task ExceptionFilter_IsFlagged()
    {
        var test = """
                   using System;

                   class Test
                   {
                       void M()
                       {
                           try
                           {
                           }
                           catch (Exception ex) {|#0:when (ex != null)|}
                           {
                           }
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.ExceptionFilterRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task RangeExpression_IsAllowed()
    {
        var test = """
                   class Test
                   {
                       int[] Slice(int[] items) => items[{|#0:1..^1|}];
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task LocalFunction_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       void Outer()
                       {
                           {|#0:void Inner()
                           {
                           }|}
                       }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.LocalFunctionRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task AwaitUsing_IsFlagged()
    {
        var test = """
                   using System;
                   using System.Threading.Tasks;

                   class Disposable : IAsyncDisposable
                   {
                       public ValueTask DisposeAsync() => new ValueTask();
                   }

                   class Test
                   {
                       public {|#1:async|} Task Run()
                       {
                           {|#0:await|} using var disposable = new Disposable();
                       }
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AwaitExpressionRuleId).WithLocation(0),
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AsyncMethodRuleId).WithLocation(1)
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task InParameter_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       void Run({|#0:in|} int value) { }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.RefReadonlyParameterRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task RefLocal_IsAllowed()
    {
        var test = """
                   class Test
                   {
                       void M(int[] items)
                       {
                           {|#0:ref int|} value = {|#1:ref items[0]|};
                       }
                   }
                   """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task AwaitForeach_IsFlagged()
    {
        var test = """
                   using System.Collections.Generic;
                   using System.Threading.Tasks;

                   class Test
                   {
                       public {|#0:async|} Task Run(IAsyncEnumerable<int> source)
                       {
                           {|#1:await|} foreach (var item in source)
                           {
                           }
                       }
                   }
                   """;

        var expected = new[]
        {
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AsyncMethodRuleId).WithLocation(0),
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.AwaitForEachRuleId).WithLocation(1),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task NativeInt_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       {|#0:nint|} Field;
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.NativeIntRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task GlobalUsing_IsFlagged()
    {
        var test = """
                   {|#0:global|} using System;
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.GlobalUsingRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task TopLevelStatement_IsFlagged()
    {
        var test = """
                   {|#0:System.Console.WriteLine("hi");|}
                   """;

        var expected = new DiagnosticResult[]
        {
            VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.TopLevelStatementRuleId).WithLocation(0),
            DiagnosticResult.CompilerError("CS8805").WithSpan(1, 1, 1, 32),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task FunctionPointer_IsFlagged()
    {
        var test = """
                   unsafe class Test
                   {
                       {|#0:delegate* managed<int, void>|} Ptr;
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.FunctionPointerRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task ListPattern_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       bool IsMatch(int[] values) => {|#0:values is [1, .. _]|};
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.ListPatternRuleId).WithSpan(3, 45, 3, 54);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task Utf8Literal_IsFlagged()
    {
        var test = """
                   using System;

                   class Test
                   {
                       ReadOnlySpan<byte> Data => {|#0:"neo"u8|};
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.Utf8LiteralRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task DefaultInterfaceMethod_IsFlagged()
    {
        var test = """
                   interface ITest
                   {
                       void {|#0:Foo|}() { }
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.DefaultInterfaceMethodRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task FileLocalType_IsFlagged()
    {
        var test = """
                   {|#0:file|} class LocalType {}
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.FileLocalTypeRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task RefReadonlyParameter_IsFlagged()
    {
        var test = """
                   class Test
                   {
                       void M({|#0:ref|} readonly int value) {}
                   }
                   """;

        var expected = VerifyCS.Diagnostic(UnsupportedSyntaxAnalyzer.RefReadonlyParameterRuleId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}
