// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RiscVParity.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RiscVParity
{
    private static readonly string TestContractsPath = Path.GetFullPath(
        Path.Combine("..", "..", "..", "..", "Neo.Compiler.CSharp.TestContracts",
            "Neo.Compiler.CSharp.TestContracts.csproj"));

    /// <summary>
    /// Representative contracts covering different language features.
    /// At least 10 contracts verified individually for structure.
    /// </summary>
    private static readonly string[] KeyContracts =
    {
        "Contract1",
        "Contract_BigInteger",
        "Contract_Boolean",
        "Contract_Array",
        "Contract_Concat",
        "Contract_Enum",
        "Contract_ByteArrayAssignment",
        "Contract_BinaryExpression",
        "Contract_Assignment",
        "Contract_Switch",
        "Contract_Integer",
        "Contract_Math",
    };

    private static CompilationEngine? _riscvEngine;
    private static IReadOnlyList<CompilationContext>? _riscvContexts;

    [ClassInitialize]
    public static void Init(TestContext _)
    {
        RiscVTestHelper.Initialize();

        var riscvOptions = new CompilationOptions
        {
            Target = CompilationTarget.RiscV,
            Nullable = NullableContextOptions.Annotations,
        };
        _riscvEngine = new CompilationEngine(riscvOptions);
        _riscvContexts = _riscvEngine.CompileProject(TestContractsPath);
    }

    [TestMethod]
    public void AllContracts_CompileForRiscV()
    {
        // Compile with NeoVM target to get the contract list
        var options = new CompilationOptions
        {
            Nullable = NullableContextOptions.Annotations,
        };
        var engine = new CompilationEngine(options);
        var contexts = engine.CompileProject(TestContractsPath);

        // Every NeoVM contract should also produce Rust source for RISC-V
        foreach (var neoCtx in contexts.Where(c => c.Success))
        {
            var riscvCtx = _riscvContexts!.FirstOrDefault(c => c.ContractName == neoCtx.ContractName);
            Assert.IsNotNull(riscvCtx, $"{neoCtx.ContractName}: should have RISC-V compilation context");
            Assert.IsTrue(riscvCtx.Success, $"{neoCtx.ContractName}: RISC-V compilation should succeed");
            Assert.IsNotNull(riscvCtx.GeneratedRustSource, $"{neoCtx.ContractName}: should generate Rust source");
            Assert.IsTrue(riscvCtx.GeneratedRustSource!.Contains("fn dispatch("),
                $"{neoCtx.ContractName}: generated Rust should contain dispatch function");
        }
    }

    [TestMethod]
    [TestCategory("RiscV")]
    public void KeyContracts_GenerateValidRustStructure()
    {
        var missing = new List<string>();
        foreach (var name in KeyContracts)
        {
            var ctx = _riscvContexts!.FirstOrDefault(c => c.ContractName == name);
            if (ctx == null || !ctx.Success || ctx.GeneratedRustSource == null)
            {
                missing.Add(name);
                continue;
            }
            var rust = ctx.GeneratedRustSource;

            // Must have dispatch function
            Assert.IsTrue(rust.Contains("fn dispatch("),
                $"{name}: missing dispatch function");

            // Must import the runtime
            Assert.IsTrue(rust.Contains("neo_riscv_rt"),
                $"{name}: missing neo_riscv_rt import");

            // Must have at least one PolkaVM entry point export
            Assert.IsTrue(rust.Contains("pub extern") || rust.Contains("pub fn "),
                $"{name}: no exported functions generated");

            // Must not contain obvious compilation artifacts
            Assert.IsFalse(rust.Contains("todo!()"),
                $"{name}: contains unimplemented todo!() macro");
            Assert.IsFalse(rust.Contains("unimplemented!()"),
                $"{name}: contains unimplemented!() macro");
        }

        Assert.IsTrue(missing.Count == 0,
            $"Failed to compile for RISC-V: {string.Join(", ", missing)}");
    }

    [TestMethod]
    [TestCategory("RiscV")]
    public void KeyContracts_GenerateValidCargoToml()
    {
        foreach (var name in KeyContracts)
        {
            var ctx = _riscvContexts!.FirstOrDefault(c => c.ContractName == name);
            if (ctx == null || !ctx.Success) continue;

            Assert.IsNotNull(ctx.GeneratedCargoToml,
                $"{name}: missing Cargo.toml");
            var cargo = ctx.GeneratedCargoToml!;

            Assert.IsTrue(cargo.Contains("[package]"),
                $"{name}: Cargo.toml missing [package] section");
            Assert.IsTrue(cargo.Contains("neo-riscv-rt"),
                $"{name}: Cargo.toml missing neo-riscv-rt dependency");
            Assert.IsTrue(cargo.Contains("[profile.release]"),
                $"{name}: Cargo.toml missing [profile.release] section");
        }
    }

    [TestMethod]
    [TestCategory("RiscV")]
    public void KeyContracts_DispatchMapsPublicMethods()
    {
        // Verify the dispatch function routes to the correct methods
        // by checking that known method names appear in the generated source
        var methodChecks = new Dictionary<string, string[]>
        {
            ["Contract1"] = new[] { "unitTest_001", "testVoid", "testArgs1", "testArgs2" },
            ["Contract_BigInteger"] = new[] { "testPow", "testAdd", "testCompare" },
            ["Contract_Boolean"] = new[] { "testBooleanOr" },
            ["Contract_Array"] = new[] { "testIntArray", "testDefaultArray" },
            ["Contract_Concat"] = new[] { "testStringAdd1", "testStringAdd2" },
        };

        foreach (var (contractName, methods) in methodChecks)
        {
            var ctx = _riscvContexts!.FirstOrDefault(c => c.ContractName == contractName);
            if (ctx == null || !ctx.Success || ctx.GeneratedRustSource == null) continue;
            var rust = ctx.GeneratedRustSource;

            foreach (var method in methods)
            {
                Assert.IsTrue(rust.Contains(method),
                    $"{contractName}: dispatch should reference method '{method}'");
            }
        }
    }

    [TestMethod]
    [TestCategory("RiscV")]
    public void RustSource_HasNoSyntaxErrors_InBasicPatterns()
    {
        // Basic structural checks on generated Rust for common anti-patterns
        foreach (var ctx in _riscvContexts!.Where(c => c.Success && c.GeneratedRustSource != null).Take(15))
        {
            var rust = ctx.GeneratedRustSource!;

            // Balanced braces
            int braces = 0;
            foreach (char ch in rust)
            {
                if (ch == '{') braces++;
                else if (ch == '}') braces--;
            }
            Assert.AreEqual(0, braces,
                $"{ctx.ContractName}: unbalanced braces in generated Rust");

            // No empty dispatch body
            Assert.IsFalse(rust.Contains("fn dispatch(") && rust.Contains("fn dispatch() {\n}"),
                $"{ctx.ContractName}: dispatch function has empty body");
        }
    }
}
