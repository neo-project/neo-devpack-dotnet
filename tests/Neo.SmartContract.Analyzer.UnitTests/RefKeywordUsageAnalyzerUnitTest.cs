// Copyright (C) 2015-2026 The Neo Project.
//
// RefKeywordUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.RefKeywordUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class RefKeywordUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task InParameterInMethod_ShouldNotReportDuplicateDiagnostic()
        {
            var test = @"
class TestClass
{
    public void TestMethod(in int value)
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task RefReadonlyParameterInMethod_ShouldNotReportDuplicateDiagnostic()
        {
            var test = @"
class TestClass
{
    public void TestMethod(ref readonly int value)
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task InArgumentInInvocation_ShouldReportDiagnostic()
        {
            var test = @"
class TestClass
{
    public void Callee(in int value) { }
    public void Caller()
    {
        int x = 5;
        Callee(in x);
    }
}";

            var expected = VerifyCS.Diagnostic(RefKeywordUsageAnalyzer.DiagnosticId)
                .WithLocation(8, 16)
                .WithArguments("method invocation ('in' argument)");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InArgumentInDelegateInvocation_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private delegate int ReadIn(in int value);

    public int Caller(int value)
    {
        ReadIn read = (in current) => current;
        return read(in value);
    }
}";

            await VerifyPreviewAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task RegularRefParameter_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    public void TestMethod(ref int value)
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ArrayElementRefArgument_ShouldReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static void Increment(ref int value) => value++;

    public static void Caller(int[] values)
    {
        Increment({|#0:ref values[0]|});
    }
}";

            var expected = VerifyCS.Diagnostic(RefKeywordUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("array element argument; bind it to a ref local before forwarding");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task SpanElementRefArgument_ShouldReportDiagnostic()
        {
            var test = @"
using System;

class TestClass
{
    private static void Increment(ref int value) => value++;

    public static void Caller()
    {
        Span<int> values = new int[] { 1, 2, 3 };
        Increment({|#0:ref values[1]|});
    }
}";

            var expected = VerifyCS.Diagnostic(RefKeywordUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("Span element argument; bind it to a ref local before forwarding");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ArrayElementOutArgument_ShouldReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static void Reset(out int value) => value = 0;

    public static void Caller(int[] values)
    {
        Reset({|#0:out values[0]|});
    }
}";

            var expected = VerifyCS.Diagnostic(RefKeywordUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("array element argument; bind it to a ref local before forwarding");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task RefLocalArgument_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static void Increment(ref int value) => value++;

    public static void Caller(int[] values)
    {
        ref int value = ref values[0];
        Increment(ref value);
    }
}";

            await VerifyPreviewAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OutParameter_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    public void TestMethod(out int value)
    {
        value = 0;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task NoRefKeyword_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    public void TestMethod(int value)
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        private static Task VerifyPreviewAnalyzerAsync(string test)
        {
            var verifier = new PreviewAnalyzerTest
            {
                TestCode = test
            };
            return verifier.RunAsync();
        }

        private sealed class PreviewAnalyzerTest : CSharpAnalyzerTest<RefKeywordUsageAnalyzer, DefaultVerifier>
        {
            protected override ParseOptions CreateParseOptions()
            {
                return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion.Preview);
            }
        }
    }
}
