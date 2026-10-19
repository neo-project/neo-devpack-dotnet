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
    [TestMethod]
    public async Task DeferredFunctionsInsideLoop_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       void Build(int count)
                       {
                           var result = "";
                           for (int i = 0; i < count; i++)
                           {
                               System.Action append = () => result += "a";
                               System.Action anonymous = delegate { result += "b"; };
                               void Append() { result += "c"; }
                           }
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task LoopSetup_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       void Build(int count)
                       {
                           var result = "";
                           for (result += "a"; count > 0; count--) { }
                           for (string value = result += "b"; count > 0; count--) { }
                           foreach (var c in result += "c") { }
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task ForInitializerLocal_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       void Build(int count)
                       {
                           for (string result = ""; result.Length < count; {|#0:result += "a"|})
                               {|#1:result = result + "b"|};
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test,
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("result"),
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(1).WithArguments("result"));
    }

    [TestMethod]
    public async Task DifferentMemberReceivers_ShouldNotReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       public string Text { get; set; }
                       public string Field;
                       void Build(Test other, int count)
                       {
                           for (int i = 0; i < count; i++)
                           {
                               this.Text = other.Text + "a";
                               this.Field = other.Field + "a";
                           }
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task NestedConcatenationAndParameter_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(string result, int count)
                       {
                           for (int i = 0; i < count; i++)
                               {|#0:result = "[" + result + "]"|};
                           return result;
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test,
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("result"));
    }

    [TestMethod]
    public async Task InnerLoopSetup_RepeatedByOuterLoop_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       string Build(int count)
                       {
                           var result = "";
                           for (int i = 0; i < count; i++)
                               for ({|#0:result += "a"|}; false;) { }
                           return result;
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test,
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("result"));
    }

    [TestMethod]
    public async Task SameMemberReceiver_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       public string Text { get; set; }
                       public string Field;
                       void Build(int count)
                       {
                           for (int i = 0; i < count; i++)
                           {
                               {|#0:this.Text = Text + "a"|};
                               {|#1:Field = this.Field + "a"|};
                           }
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test,
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("this.Text"),
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(1).WithArguments("Field"));
    }

    [TestMethod]
    public async Task LoopInsideLocalFunction_ShouldReportDiagnostic()
    {
        var test = """
                   class Test
                   {
                       void Build(int count)
                       {
                           var result = "";
                           void Append()
                           {
                               while (result.Length < count)
                                   {|#0:result += "a"|};
                           }
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test,
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("result"));
    }

    [TestMethod]
    public async Task MemberAccesses_ShouldRequireMatchingStableReceivers()
    {
        var test = """
                   class Test
                   {
                       public string Text { get; set; }
                       static string Shared;
                       string this[int index] { get => ""; set { } }
                       Test Next() => new Test();
                       void Build(Test left, Test right, string[] values, int count)
                       {
                           for (int i = 0; i < count; i++)
                           {
                               {|#0:left.Text = left.Text + "a"|};
                               left.Text = right.Text + "a";
                               {|#1:Shared += "a"|};
                               Next().Text += "a";
                               this[i] = this[i] + "a";
                               values[i] += "a";
                           }
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test,
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(0).WithArguments("left.Text"),
            VerifyCS.Diagnostic(StringConcatenationInLoopAnalyzer.DiagnosticId).WithLocation(1).WithArguments("Shared"));
    }

    [TestMethod]
    public async Task UserDefinedReceiverConversion_ShouldNotAssumeSameInstance()
    {
        var test = """
                   class Item { public string Text; }
                   class Test
                   {
                       public static explicit operator Item(Test source) => new Item();
                       void Build(int count)
                       {
                           var source = new Test();
                           for (int i = 0; i < count; i++)
                               ((Item)source).Text = ((Item)source).Text + "a";
                       }
                   }
                   """;
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

}
