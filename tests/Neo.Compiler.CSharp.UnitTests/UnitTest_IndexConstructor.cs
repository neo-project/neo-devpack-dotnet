// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_IndexConstructor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_IndexConstructor
{
    private const string ValidControlSource = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("userDefinedIndex")]
            public static int UserDefinedIndex() => new Index(7).Value;

            [DisplayName("ordinaryObject")]
            public static int OrdinaryObject() => new Box(9).Value;

            [DisplayName("arrayFromEnd")]
            public static int ArrayFromEnd()
            {
                int[] values = { 4, 5, 6 };
                return values[^2];
            }

            [DisplayName("stringFromEnd")]
            public static char StringFromEnd() => "neo"[^1];

            [DisplayName("byteRange")]
            public static byte[] ByteRange()
            {
                byte[] values = { 1, 2, 3, 4 };
                return values[1..3];
            }

            [DisplayName("stringRange")]
            public static string StringRange() => "neo"[1..3];
        }

        public class Index
        {
            public int Value;

            public Index(int value)
            {
                Value = value;
            }
        }

        public class Box
        {
            public int Value;

            public Box(int value)
            {
                Value = value;
            }
        }
        """;

    public static IEnumerable<object[]> GetUnsupportedConstructionCases()
    {
        yield return new object[]
        {
            "new Index()",
            "public static int Invalid() { Index index = new Index(); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(2)",
            "public static int Invalid() { Index index = new Index(2); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(2, false)",
            "public static int Invalid() { Index index = new Index(2, false); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(2, true)",
            "public static int Invalid() { Index index = new Index(2, true); return index.Value; }"
        };
        yield return new object[]
        {
            "new()",
            "public static int Invalid() { Index index = new(); return index.Value; }"
        };
        yield return new object[]
        {
            "new(2)",
            "public static int Invalid() { Index index = new(2); return index.Value; }"
        };
        yield return new object[]
        {
            "new(2, true)",
            "public static int Invalid() { Index index = new(2, true); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(value: 2)",
            "public static int Invalid() { Index index = new Index(value: 2); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(fromEnd: true, value: 2)",
            "public static int Invalid() { Index index = new Index(fromEnd: true, value: 2); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(value, fromEnd)",
            "public static int Invalid(int value, bool fromEnd) { Index index = new Index(value, fromEnd); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(value, fromEnd)",
            "public static int Invalid(int seed) { int value = seed + 1; bool fromEnd = seed > 0; Index index = new Index(value, fromEnd); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(value, true)",
            "public static int Invalid(int input) { dynamic value = input; Index index = new Index(value, true); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(-2, true)",
            "public static int Invalid() { Index index = new Index(-2, true); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(value)",
            "public static int Invalid(int seed) { int value = -seed; Index index = new Index(value); return index.Value; }"
        };
        yield return new object[]
        {
            "new Index(1, true)",
            "public static int Invalid() { int[] values = { 1, 2, 3 }; return values[new Index(1, true)]; }"
        };
    }

    [DataTestMethod]
    [DynamicData(nameof(GetUnsupportedConstructionCases), DynamicDataSourceType.Method)]
    public void FrameworkIndexConstruction_ReportsSingleLocatedDiagnostic(string expression, string member)
    {
        string source = $$"""
            using Neo.SmartContract.Framework;
            using System;

            public class Contract : SmartContract
            {
                {{member}}
            }
            """;

        CompilationContext context = TestHelper.CompileSingleContract(source);

        AssertUnsupportedConstruction(context, source, expression);
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void FrameworkIndexConstruction_DiagnosticIsOptimizationIndependent(CompilationOptions.OptimizationType optimization)
    {
        const string expression = "new Index(2, true)";
        const string source = """
            using Neo.SmartContract.Framework;
            using System;

            public class Contract : SmartContract
            {
                public static int Invalid()
                {
                    Index index = new Index(2, true);
                    return index.Value;
                }
            }
            """;

        CompilationContext context = TestHelper.CompileSingleContract(source, CreateOptions(optimization));

        AssertUnsupportedConstruction(context, source, expression);
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void RelatedObjectAndIndexForms_CompileAndExecute(CompilationOptions.OptimizationType optimization)
    {
        CompilationContext context = TestHelper.CompileSingleContract(ValidControlSource, CreateOptions(optimization));
        string diagnostics = FormatDiagnostics(context);
        Assert.IsTrue(context.Success, diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.SyntaxNotSupported), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);

        var engine = new TestEngine(true);
        ValidControlContract contract = engine.Deploy<ValidControlContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(7), contract.UserDefinedIndex());
        Assert.AreEqual(new BigInteger(9), contract.OrdinaryObject());
        Assert.AreEqual(new BigInteger(5), contract.ArrayFromEnd());
        Assert.AreEqual(new BigInteger('o'), contract.StringFromEnd());
        byte[]? byteRange = contract.ByteRange();
        Assert.IsNotNull(byteRange);
        CollectionAssert.AreEqual(new byte[] { 2, 3 }, byteRange);
        Assert.AreEqual("eo", contract.StringRange());
    }

    [TestMethod]
    public void SourceDefinedSystemIndexShadow_CompilesAndExecutes()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("shadowIndex")]
                public static int ShadowIndex() => new global::System.Index(11).Value;
            }

            namespace System
            {
                public class Index
                {
                    public int Value;

                    public Index(int value)
                    {
                        Value = value;
                    }
                }
            }
            """;

        CompilationContext context = TestHelper.CompileSingleContract(source);
        string diagnostics = FormatDiagnostics(context);
        Assert.IsTrue(context.Success, diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.SyntaxNotSupported), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);

        var engine = new TestEngine(true);
        SystemIndexShadowContract contract = engine.Deploy<SystemIndexShadowContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(new BigInteger(11), contract.ShadowIndex());
    }

    private static CompilationOptions CreateOptions(CompilationOptions.OptimizationType optimization)
    {
        CompilationOptions options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        return options;
    }

    private static void AssertUnsupportedConstruction(CompilationContext context, string source, string expression)
    {
        string diagnostics = FormatDiagnostics(context);
        Assert.IsFalse(context.Success, diagnostics);

        var unsupported = context.Diagnostics.Where(d => d.Id == DiagnosticId.SyntaxNotSupported).ToArray();
        Assert.AreEqual(1, unsupported.Length, diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);

        var diagnostic = unsupported[0];
        Assert.IsTrue(diagnostic.Location.IsInSource, diagnostics);
        Assert.IsNotNull(diagnostic.Location.SourceTree, diagnostics);

        int expectedStart = source.IndexOf(expression, StringComparison.Ordinal);
        Assert.AreNotEqual(-1, expectedStart, $"Expression '{expression}' was not found in the source.");
        Assert.AreEqual(expectedStart, diagnostic.Location.SourceSpan.Start, diagnostics);
        Assert.AreEqual(expression.Length, diagnostic.Location.SourceSpan.Length, diagnostics);
        Assert.AreEqual(expression, diagnostic.Location.SourceTree.GetText().ToString(diagnostic.Location.SourceSpan), diagnostics);
        StringAssert.Contains(diagnostic.GetMessage(), "System.Index construction is not supported.");
        StringAssert.Contains(diagnostic.GetMessage(), "Use an int index, or inline '^' in an element or range access.");
    }

    private static string FormatDiagnostics(CompilationContext context) =>
        string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));

    public abstract class ValidControlContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("userDefinedIndex")]
        public abstract BigInteger? UserDefinedIndex();

        [DisplayName("ordinaryObject")]
        public abstract BigInteger? OrdinaryObject();

        [DisplayName("arrayFromEnd")]
        public abstract BigInteger? ArrayFromEnd();

        [DisplayName("stringFromEnd")]
        public abstract BigInteger? StringFromEnd();

        [DisplayName("byteRange")]
        public abstract byte[]? ByteRange();

        [DisplayName("stringRange")]
        public abstract string? StringRange();
    }

    public abstract class SystemIndexShadowContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("shadowIndex")]
        public abstract BigInteger? ShadowIndex();
    }
}
