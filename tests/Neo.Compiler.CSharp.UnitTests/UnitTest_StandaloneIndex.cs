// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StandaloneIndex.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StandaloneIndex
{
    private static readonly CompilationOptions.OptimizationType[] OptimizationModes =
    [
        CompilationOptions.OptimizationType.None,
        CompilationOptions.OptimizationType.Basic,
        CompilationOptions.OptimizationType.All
    ];

    [DataTestMethod]
    [DataRow("""
    public static int Test()
    {
        Index start = ^2;
        return 0;
    }
    """, "^2")]
    [DataRow("""
    private static Index _start;

    public static int Test()
    {
        _start = ^3;
        return 0;
    }
    """, "^3")]
    [DataRow("""
    private static Index GetStart() => ^4;

    public static int Test() => GetStart().GetOffset(10);
    """, "^4")]
    [DataRow("""
    private static int Consume(Index value) => 0;

    public static int Test() => Consume(^5);
    """, "^5")]
    [DataRow("""
    private static int Consume(Index value) => 0;

    public static byte Test(byte[] values) => values[Consume(^6)];
    """, "^6")]
    [DataRow("""
    public static int Test()
    {
        (Index, int) pair = (^7, 0);
        return pair.Item2;
    }
    """, "^7")]
    [DataRow("""
    private sealed class Holder
    {
        public Holder(Index value) { }
    }

    public static int Test()
    {
        _ = new Holder(^9);
        return 0;
    }
    """, "^9")]
    public void StandaloneContextsReportSyntaxNotSupported(string members, string caret)
    {
        AssertStandaloneIndexRejected(BuildContract(members), caret);
    }

    [DataTestMethod]
    [DataRow("flag ? ^2 : fallback", "^2")]
    [DataRow("flag ? fallback : ^1", "^1")]
    public void StoredConditionalCaretInEitherArmIsRejected(string conditional, string caret)
    {
        string source = BuildContract($$"""
    private static int Store(bool flag, Index fallback)
    {
        Index stored = {{conditional}};
        return 0;
    }

    public static int Test() => 0;
    """);

        AssertStandaloneIndexRejected(source, caret);
    }

    [TestMethod]
    public void StandaloneDiagnosticIsConsistentAcrossOptimizationModes()
    {
        string source = BuildContract("""
    public static int Test()
    {
        Index start = ^2;
        return 0;
    }
    """);

        AssertStandaloneIndexRejected(source, "^2", OptimizationModes);
    }

    [TestMethod]
    public void InlineElementAndRangeUsesRemainValid()
    {
        const string source = """
using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("test")]
    public static int Test()
    {
        byte[] values = { 1, 2, 3, 4, 5 };
        int[] numbers = { 10, 20, 30 };
        string text = "abcde";
        bool flag = true;
        int selector = 0;

        byte byteElement = values[^2];
        int numberElement = numbers[^2];
        char stringElement = text[^2];
        byte[] fromTwo = values[^2..];
        byte[] toLast = values[..^1];
        byte[] middle = values[^3..^1];
        byte conditional = values[(flag ? ^2 : ^1)];
        byte selected = values[(selector switch { 0 => ^2, _ => ^1 })];
        byte parenthesized = values[((^2))];
        byte checkedIndex = values[checked(^2)];
        byte uncheckedIndex = values[unchecked(^2)];
        byte identityCast = values[(System.Index)(^2)];
        byte nullForgiving = values[(^2)!];
        byte conditionalBinding = values?[^2] ?? 0;
        string stringRange = text[^3..^1];

        return byteElement + numberElement + stringElement + fromTwo.Length + toLast.Length + middle.Length
            + conditional + selected + parenthesized + checkedIndex + uncheckedIndex
            + identityCast + nullForgiving + conditionalBinding + stringRange.Length;
    }
}
""";

        foreach (CompilationOptions.OptimizationType optimization in OptimizationModes)
        {
            CompilationContext context = Compile(source, optimization);
            string diagnostics = FormatDiagnostics(context);

            Assert.IsTrue(context.Success, $"{optimization}:{Environment.NewLine}{diagnostics}");
            Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);

            var engine = new TestEngine(true);
            var contract = engine.Deploy<ValidIndexContract>(context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(166), contract.Test(), optimization.ToString());
        }
    }

    private static string BuildContract(string members) => $$"""
using Neo.SmartContract.Framework;
using System;

public class Contract : SmartContract
{
{{members}}
}
""";

    private static void AssertStandaloneIndexRejected(string source, string caret,
        params CompilationOptions.OptimizationType[] optimizationModes)
    {
        int expectedStart = source.IndexOf(caret, StringComparison.Ordinal);
        Assert.IsTrue(expectedStart >= 0, $"Caret expression '{caret}' was not found in the source.");

        if (optimizationModes.Length == 0)
            optimizationModes = [CompilationOptions.OptimizationType.All];

        foreach (CompilationOptions.OptimizationType optimization in optimizationModes)
        {
            CompilationContext context = Compile(source, optimization);
            string diagnostics = FormatDiagnostics(context);
            var syntaxDiagnostics = context.Diagnostics.Where(d => d.Id == DiagnosticId.SyntaxNotSupported).ToArray();
            var otherErrors = context.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error && d.Id != DiagnosticId.SyntaxNotSupported)
                .ToArray();

            Assert.IsFalse(context.Success, $"{optimization}: compilation unexpectedly succeeded.");
            Assert.AreEqual(1, syntaxDiagnostics.Length, $"{optimization}:{Environment.NewLine}{diagnostics}");
            Assert.AreEqual(0, otherErrors.Length, $"{optimization}:{Environment.NewLine}{diagnostics}");
            Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);

            var diagnostic = syntaxDiagnostics[0];
            Assert.AreEqual(expectedStart, diagnostic.Location.SourceSpan.Start, optimization.ToString());
            Assert.AreEqual(caret.Length, diagnostic.Location.SourceSpan.Length, optimization.ToString());
            Assert.IsNotNull(diagnostic.Location.SourceTree, optimization.ToString());
            Assert.AreEqual(caret,
                diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan),
                optimization.ToString());
            StringAssert.Contains(diagnostic.GetMessage(), "Store the distance as an int");
            StringAssert.Contains(diagnostic.GetMessage(), "inline '^'");
        }
    }

    private static CompilationContext Compile(string source, CompilationOptions.OptimizationType optimization)
    {
        CompilationOptions options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        return TestHelper.CompileSingleContract(source, options);
    }

    private static string FormatDiagnostics(CompilationContext context) =>
        string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));

    public abstract class ValidIndexContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract BigInteger? Test();
    }
}
