// Copyright (C) 2015-2025 The Neo Project.
//
// VerifyStorageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public static class VerifyStorageAnalyzer
    {
        public static CSharpAnalyzerTest<TAnalyzer, XUnitVerifier> CreateAnalyzerTest<TAnalyzer>()
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            return StoragePatternAnalyzerTestHelper.CreateAnalyzerTest<TAnalyzer>();
        }

        public static CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier> CreateCodeFixTest<TAnalyzer, TCodeFix>()
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            return StoragePatternAnalyzerTestHelper.CreateCodeFixTest<TAnalyzer, TCodeFix>();
        }
    }
}
