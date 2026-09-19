// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_FoldConstantConditionalJump.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.VM;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_FoldConstantConditionalJump
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        public class Contract : SmartContract
        {
            public static int WhileTrue(int x)
            {
                while (true)
                {
                    x++;
                    if (x > 10) break;
                }
                return x;
            }

            public static int DoWhileTrue(int x)
            {
                do { x += 2; } while (true);
            }

            public static int IfNever()
            {
                int v = 1;
                if (false) { v = 2; }
                return v;
            }
        }
        """;

    [TestMethod]
    public void ConstantConditionalPairsAreFolded()
    {
        var ctx = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(ctx.Success, string.Join("\n", ctx.Diagnostics));
        var (nef, manifest, _) = Neo.Optimizer.Optimizer.Optimize(
            ctx.CreateExecutable(), ctx.CreateManifest(), null, CompilationOptions.OptimizationType.All);

        Script optimizedScript = nef.Script;
        List<string> ops = optimizedScript.EnumerateInstructions().Select(kv => kv.instruction.OpCode.ToString()).ToList();
        for (int i = 0; i + 1 < ops.Count; i++)
        {
            bool isConstantPush = ops[i] is "PUSH0" or "PUSH1" or "PUSHF" or "PUSHT";
            bool isConditionalJump = ops[i + 1] is "JMPIF" or "JMPIFNOT" or "JMPIF_L" or "JMPIFNOT_L";
            Assert.IsFalse(isConstantPush && isConditionalJump,
                $"constant push followed by conditional jump was not folded at index {i}: {ops[i]} {ops[i + 1]}");
        }
        // while (true) folds to nothing at the loop head and do..while (true) folds to a
        // backward JMP, so both loop forms keep executing correctly.
        CollectionAssert.DoesNotContain(ops, "PUSHT");
        Assert.IsTrue(ops.Contains("JMP"));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<FoldedContract>(nef, manifest);
        Assert.AreEqual(11, contract.WhileTrue(5));
        Assert.AreEqual(1, contract.IfNever());
    }

    public abstract class FoldedContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("whileTrue")]
        public abstract int WhileTrue(int x);

        [DisplayName("doWhileTrue")]
        public abstract int DoWhileTrue(int x);

        [DisplayName("ifNever")]
        public abstract int IfNever();
    }
}