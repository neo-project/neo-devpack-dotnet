// Copyright (C) 2015-2026 The Neo Project.
//
// TransferRiscVTests.cs file belongs to the neo project and is free
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

namespace Example.SmartContract.Transfer.UnitTests;

[TestClass]
[TestCategory("RiscV")]
public class TransferRiscVTests
{
    private static readonly string ContractProjectPath = Path.GetFullPath(
        Path.Combine("..", "..", "..", "..", "Example.SmartContract.Transfer",
            "Example.SmartContract.Transfer.csproj"));

    [TestMethod]
    public void Transfer_CompilesForRiscV()
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
        Assert.IsTrue(ctx.Success, "Transfer contract should compile successfully for RISC-V");
        Assert.AreEqual("SampleTransferContract", ctx.ContractName);
    }

    [TestMethod]
    public void Transfer_NeoVmAndRiscV_HaveSameContractName()
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
