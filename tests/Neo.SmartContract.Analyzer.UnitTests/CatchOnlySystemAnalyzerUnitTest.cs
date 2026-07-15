// Copyright (C) 2015-2026 The Neo Project.
//
// CatchOnlySystemAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
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
        catch (ArgumentException ex) { Console.WriteLine(ex.ParamName); }
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
        public void NC4027_ShouldNotHaveCodeFixProvider()
        {
            var providerNames = typeof(CatchOnlySystemExceptionAnalyzer).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(CodeFixProvider).IsAssignableFrom(type))
                .Select(type => (CodeFixProvider)Activator.CreateInstance(type)!)
                .Where(provider => provider.FixableDiagnosticIds.Contains(CatchOnlySystemExceptionAnalyzer.DiagnosticId))
                .Select(provider => provider.GetType().Name)
                .ToArray();

            Assert.AreEqual(0, providerNames.Length,
                $"NC4027 must not have an automatic code fix: {string.Join(", ", providerNames)}");
        }
    }
}
