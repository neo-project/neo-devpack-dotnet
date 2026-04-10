// Copyright (C) 2015-2026 The Neo Project.
//
// HelloWorldRiscVTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System.IO;

namespace Example.SmartContract.HelloWorld.UnitTests;

/// <summary>
/// Verifies that the HelloWorld contract compiles successfully for RISC-V target.
/// These tests validate the dual-target compilation path without requiring a native library.
/// </summary>
[TestClass]
[TestCategory("RiscV")]
public class HelloWorldRiscVTests
{
    private static readonly string ContractProjectPath = Path.GetFullPath(
        Path.Combine("..", "..", "..", "..", "Example.SmartContract.HelloWorld",
            "Example.SmartContract.HelloWorld.csproj"));

    [TestMethod]
    public void HelloWorld_CompilesForRiscV()
    {
        var options = new Neo.Compiler.CompilationOptions
        {
            Target = CompilationTarget.RiscV,
            Nullable = NullableContextOptions.Annotations,
        };
        var engine = new CompilationEngine(options);
        var contexts = engine.CompileProject(ContractProjectPath);

        Assert.AreEqual(1, contexts.Count);
        var ctx = contexts[0];
        Assert.IsTrue(ctx.Success, "HelloWorld should compile successfully for RISC-V");
        Assert.AreEqual("SampleHelloWorld", ctx.ContractName);
    }

    [TestMethod]
    public void HelloWorld_NeoVmAndRiscV_HaveSameContractName()
    {
        var neovmOptions = new Neo.Compiler.CompilationOptions
        {
            Nullable = NullableContextOptions.Annotations,
        };
        var neovmEngine = new CompilationEngine(neovmOptions);
        var neovmContexts = neovmEngine.CompileProject(ContractProjectPath);

        var riscvOptions = new Neo.Compiler.CompilationOptions
        {
            Target = CompilationTarget.RiscV,
            Nullable = NullableContextOptions.Annotations,
        };
        var riscvEngine = new CompilationEngine(riscvOptions);
        var riscvContexts = riscvEngine.CompileProject(ContractProjectPath);

        Assert.AreEqual(neovmContexts[0].ContractName, riscvContexts[0].ContractName,
            "NeoVM and RISC-V should produce the same contract name");
    }
}
