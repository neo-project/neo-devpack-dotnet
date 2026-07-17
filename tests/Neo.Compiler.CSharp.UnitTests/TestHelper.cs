// Copyright (C) 2015-2026 The Neo Project.
//
// TestHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests;

public static class TestHelper
{
    private static readonly Lazy<(MetadataReference Framework, MetadataReference[] All)> CompilerReferences = new(CreateCompilerReferences);

    public static CompilationOptions CreateDefaultOptions()
    {
        return new CompilationOptions
        {
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true
        };
    }

    public static CompilationContext CompileSingleContract(string sourceCode, CompilationOptions? options = null)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var engine = new CompilationEngine(options ?? CreateDefaultOptions());
            var references = CompilerReferences.Value;
            var contexts = engine.Compile([tempFile], references.All, references.Framework);

            Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
            return contexts[0];
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private static (MetadataReference Framework, MetadataReference[] All) CreateCompilerReferences()
    {
        var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        var frameworkCompilation = new CompilationEngine(CreateDefaultOptions()).GetCompilation(frameworkProject);
        var frameworkReference = frameworkCompilation.ToMetadataReference();
        return (frameworkReference, [.. frameworkCompilation.References, frameworkReference]);
    }
}
