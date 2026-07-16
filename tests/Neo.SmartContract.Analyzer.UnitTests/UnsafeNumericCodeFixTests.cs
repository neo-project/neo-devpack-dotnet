// Copyright (C) 2015-2026 The Neo Project.
//
// UnsafeNumericCodeFixTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class UnsafeNumericCodeFixTests
    {
        private const string FixedPointGuidance =
            "Use an integer or BigInteger with an explicit application-defined scale for fixed-point arithmetic.";

        [TestMethod]
        public void FloatingPointDiagnostics_ShouldNotOfferAutomaticFixes()
        {
            HashSet<string> unsafeDiagnosticIds =
            [
                FloatUsageAnalyzer.DiagnosticId,
                DecimalUsageAnalyzer.DiagnosticId,
                DoubleUsageAnalyzer.DiagnosticId
            ];

            var unsafeFixes = typeof(FloatUsageAnalyzer).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(CodeFixProvider).IsAssignableFrom(type))
                .Select(type => (CodeFixProvider)Activator.CreateInstance(type)!)
                .SelectMany(provider => provider.FixableDiagnosticIds.Select(id => new
                {
                    ProviderName = provider.GetType().Name,
                    DiagnosticId = id
                }))
                .Where(fix => unsafeDiagnosticIds.Contains(fix.DiagnosticId))
                .Select(fix => $"{fix.ProviderName}: {fix.DiagnosticId}")
                .OrderBy(fix => fix, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(0, unsafeFixes.Length, string.Join(Environment.NewLine, unsafeFixes));
        }

        [TestMethod]
        public void FloatingPointDiagnostics_ShouldProvideFixedPointGuidance()
        {
            DiagnosticAnalyzer[] analyzers =
            [
                new FloatUsageAnalyzer(),
                new DecimalUsageAnalyzer(),
                new DoubleUsageAnalyzer()
            ];

            foreach (var analyzer in analyzers)
            {
                Assert.AreEqual(1, analyzer.SupportedDiagnostics.Length);
                var diagnostic = analyzer.SupportedDiagnostics[0];
                Assert.AreEqual(FixedPointGuidance, diagnostic.Description.ToString());
            }
        }
    }
}
