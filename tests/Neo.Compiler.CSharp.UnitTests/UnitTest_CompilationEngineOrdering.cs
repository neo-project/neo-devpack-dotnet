// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CompilationEngineOrdering.cs file belongs to the neo project and is free
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
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CompilationEngineOrdering
{
    [TestMethod]
    public void CompileSources_ReturnsContractsInPreparedOrder()
    {
        var contextNames = CompileContractNames("""
using Neo.SmartContract.Framework;

public class FirstContract : SmartContract
{
    public static int Main() => 1;
}

public class SecondContract : SmartContract
{
    public static int Main() => 2;
}
""");

        CollectionAssert.AreEqual(new[] { "FirstContract", "SecondContract" }, contextNames);
    }

    [TestMethod]
    public void CompileSources_ReturnsOnlySmartContractContexts()
    {
        var contextNames = CompileContractNames("""
using Neo.SmartContract.Framework;

public class ContractHelper
{
    public static int Value() => 42;
}

public class FirstContract : SmartContract
{
    public static int Main() => ContractHelper.Value();
}

public class SecondContract : SmartContract
{
    public static int Main() => ContractHelper.Value();
}
""");

        CollectionAssert.AreEqual(new[] { "FirstContract", "SecondContract" }, contextNames);
    }

    private static string[] CompileContractNames(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };

            var engine = new CompilationEngine(options);
            var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var contexts = engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile);

            Assert.IsTrue(contexts.All(context => context.Success), string.Join(Environment.NewLine, contexts.SelectMany(context => context.Diagnostics).Select(diagnostic => diagnostic.ToString())));
            return contexts.Select(context => context.ContractName ?? string.Empty).ToArray();
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
