// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RiscVEndToEnd.cs file belongs to the neo project and is free
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
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
[TestCategory("RiscV")]
public class UnitTest_RiscVEndToEnd
{
    /// <summary>
    /// Path to test contracts project, resolved the same way as TestCleanup.cs.
    /// From bin/Debug/net10.0 we go ../../../../Neo.Compiler.CSharp.TestContracts/
    /// </summary>
    private static readonly string TestContractsPath = Path.GetFullPath(
        Path.Combine("..", "..", "..", "..", "Neo.Compiler.CSharp.TestContracts",
            "Neo.Compiler.CSharp.TestContracts.csproj"));

    [TestMethod]
    public void TestCompileContract_RiscV_GeneratesRust()
    {
        Assert.IsTrue(File.Exists(TestContractsPath),
            $"Test contracts project not found at: {TestContractsPath}");

        var options = new CompilationOptions
        {
            Target = CompilationTarget.RiscV,
            Nullable = NullableContextOptions.Annotations,
        };
        var engine = new CompilationEngine(options);
        var contexts = engine.CompileProject(TestContractsPath);

        // Find Contract_Assignment -- a simple contract with straightforward methods
        var ctx = contexts.FirstOrDefault(c => c.ContractName == "Contract_Assignment");
        Assert.IsNotNull(ctx, "Contract_Assignment should be found among compiled contexts");

        Assert.IsTrue(ctx.Success,
            $"Compilation should succeed. Diagnostics: {string.Join("; ", ctx.Diagnostics.Select(d => d.ToString()))}");

        Assert.IsNotNull(ctx.GeneratedRustSource,
            "GeneratedRustSource should be populated for RiscV target");

        var rust = ctx.GeneratedRustSource!;

        // Verify Rust file structure
        Assert.IsTrue(rust.Contains("use neo_riscv_rt::Context;"),
            "Should import Context");
        Assert.IsTrue(rust.Contains("fn dispatch(ctx: &mut Context, method: &str)"),
            "Should have dispatch function");

        // Verify at least one method was translated
        Assert.IsTrue(rust.Contains("fn method_"),
            "Should have at least one compiled method");

        // Verify the dispatch table references the contract's exported methods
        Assert.IsTrue(rust.Contains("\"testAssignment\"") || rust.Contains("\"TestAssignment\""),
            $"Should contain a dispatch entry for testAssignment/TestAssignment. Dispatch section:\n{rust.Substring(rust.IndexOf("fn dispatch(ctx"))}");

    }
}
