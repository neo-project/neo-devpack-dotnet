// Copyright (C) 2015-2025 The Neo Project.
//
// CatchOnlySystemAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;


namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class CatchSystemExceptionTests
    {
        string testCode = @"
using System;

class Program
{
    static void Main()
    {
        try { }
        catch (ArgumentException ex) { }
    }
}";

        string fixedCode = @"
using System;

class Program
{
    static void Main()
    {
        try { }
        catch (System.Exception ex) { }
    }
}";

        string codeWithoutExceptionType = @"
using System;

class Program
{
    static void Main()
    {
        try { }
        catch { }
    }
}";

        string codeWithCorrectExceptionType = @"
using System;

class Program
{
    static void Main()
    {
        try { }
        catch (Exception e) { }
    }
}";

        DiagnosticResult expectedDiagnostic = DiagnosticResult
            .CompilerWarning(CatchOnlySystemExceptionAnalyzer.DiagnosticId)
            .WithSpan(9, 16, 9, 33);

        [TestMethod]
        public async Task TestAnalyzer()
        {
            var test = new CSharpAnalyzerTest<CatchOnlySystemExceptionAnalyzer, DefaultVerifier>
            {
                TestCode = testCode
            };

            test.ExpectedDiagnostics.AddRange([expectedDiagnostic]);
            await test.RunAsync();

            test = new CSharpAnalyzerTest<CatchOnlySystemExceptionAnalyzer, DefaultVerifier>
            {
                TestCode = codeWithoutExceptionType
            };
            // no ExpectedDiagnostics
            await test.RunAsync();

            test = new CSharpAnalyzerTest<CatchOnlySystemExceptionAnalyzer, DefaultVerifier>
            {
                TestCode = codeWithCorrectExceptionType
            };
            // no ExpectedDiagnostics
            await test.RunAsync();
        }

        [TestMethod]
        public async Task TestCodeFix()
        {
            var test = new CSharpCodeFixTest<CatchOnlySystemExceptionAnalyzer, CatchOnlySystemExceptionCodeFixProvider, DefaultVerifier>
            {
                TestCode = testCode,
                FixedCode = fixedCode
            };

            test.ExpectedDiagnostics.AddRange([expectedDiagnostic]);
            await test.RunAsync();
        }
    }
}
