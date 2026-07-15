// Copyright (C) 2015-2026 The Neo Project.
//
// CollectionTypesUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
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
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.CollectionTypesUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class CollectionTypesUsageAnalyzerUnitTests
    {
        private const string TestNamespace = """
                                             using System.Collections.Generic;

                                                 public class Map<TKey, TValue>
                                                 {
                                                     public Map() { }
                                                 }

                                                 public class List<T>
                                                 {
                                                     public List() { }
                                                 }


                                             """;
        [TestMethod]
        public async Task UnsupportedDictionaryType_ShouldReportDiagnostic()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               Dictionary<int, string> dict = new Dictionary<int, string>();
                                           }
                                       }
                                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(18, 9)
                .WithArguments("System.Collections.Generic.Dictionary<TKey, TValue>", "Map<TKey, TValue>");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task UnsupportedStackType_ShouldReportDiagnostic()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               Stack<int> stack = new Stack<int>();
                                           }
                                       }
                                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(18, 9)
                .WithArguments("System.Collections.Generic.Stack<T>", "List<T>");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task UnsupportedSystemListType_ShouldReportDiagnostic()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
                                           }
                                       }
                                       """;

            var expectedDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(18, 9)
                .WithArguments("System.Collections.Generic.List<T>", "List<T>");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task SupportedCollectionType_ShouldNotReportDiagnostic()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               List<int> list = new List<int>();
                                               Map<int, string> map = new Map<int, string>();
                                           }
                                       }
                                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnsupportedCollectionSignatureTypes_ShouldReportDiagnostics()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public {|#0:Dictionary<int, string>|} GetItems({|#1:Stack<int>|} values) => null;
                                           public {|#2:System.Collections.Generic.List<int>|} Items { get; set; }
                                       }
                                       """;

            var returnDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("System.Collections.Generic.Dictionary<TKey, TValue>", "Map<TKey, TValue>");
            var parameterDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("System.Collections.Generic.Stack<T>", "List<T>");
            var propertyDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(2)
                .WithArguments("System.Collections.Generic.List<T>", "List<T>");

            await VerifyCS.VerifyAnalyzerAsync(test, returnDiagnostic, parameterDiagnostic, propertyDiagnostic);
        }

        [TestMethod]
        public void CollectionDiagnostic_ShouldNotOfferAutomaticCodeFixes()
        {
            var fixes = typeof(CollectionTypesUsageAnalyzer).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(CodeFixProvider).IsAssignableFrom(type))
                .Select(type => (CodeFixProvider)Activator.CreateInstance(type)!)
                .Where(provider => provider.FixableDiagnosticIds.Contains(CollectionTypesUsageAnalyzer.DiagnosticId))
                .Select(provider => provider.GetType().Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(0, fixes.Length, string.Join(Environment.NewLine, fixes));
        }
    }
}
