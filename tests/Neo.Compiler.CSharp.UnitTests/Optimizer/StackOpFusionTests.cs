// Copyright (C) 2015-2026 The Neo Project.
//
// StackOpFusionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class StackOpFusionTests
    {
        [TestMethod]
        [DataRow(OpCode.SWAP, OpCode.DROP, OpCode.NIP, 2)]
        [DataRow(OpCode.TUCK, OpCode.DROP, OpCode.SWAP, 2)]
        [DataRow(OpCode.SWAP, OpCode.OVER, OpCode.TUCK, 2)]
        public void Fusion_RewritesEquivalentSequence(OpCode first, OpCode second, OpCode expected, int itemCount)
        {
            byte[] prefix = itemCount == 2
                ? [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2]
                : [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2, (byte)OpCode.PUSH3];
            byte[] original = [.. prefix, (byte)first, (byte)second, (byte)OpCode.RET];
            byte[] optimized = Apply(original);
            byte[] expectedScript = [.. prefix, (byte)expected, (byte)OpCode.RET];
            CollectionAssert.AreEqual(expectedScript, optimized);
            Assert.IsLessThan(original.Length, optimized.Length);
            AssertSameBehavior(original, optimized);
        }

        [TestMethod]
        [DataRow(OpCode.SWAP, OpCode.DROP, 2)]
        [DataRow(OpCode.TUCK, OpCode.DROP, 2)]
        [DataRow(OpCode.SWAP, OpCode.OVER, 2)]
        [DataRow(OpCode.SWAP, OpCode.ROT, 3)]
        public void Fusion_DoesNotRewriteWhenSecondInstructionIsJumpTarget(OpCode first, OpCode second, int itemCount)
        {
            byte[] prefix = itemCount == 2
                ? [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2]
                : [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2, (byte)OpCode.PUSH3];
            byte[] script = [.. prefix, (byte)OpCode.JMP, 0x03, (byte)first, (byte)second, (byte)OpCode.RET];
            NefFile rewritten = Peephole.Rewrite(CreateNefFile(script), CreateManifest(), PeepholeRule.StackOpFusion);
            CollectionAssert.AreEqual(script, rewritten.Script.ToArray());
        }

        [TestMethod]
        [DataRow(OpCode.SWAP, OpCode.DROP, OpCode.NIP, 2)]
        [DataRow(OpCode.TUCK, OpCode.DROP, OpCode.SWAP, 2)]
        [DataRow(OpCode.SWAP, OpCode.OVER, OpCode.TUCK, 2)]
        [DataRow(OpCode.SWAP, OpCode.ROT, OpCode.REVERSE3, 3)]
        public void Fusion_IsIdempotent(OpCode first, OpCode second, OpCode expected, int itemCount)
        {
            byte[] prefix = itemCount == 2
                ? [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2]
                : [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2, (byte)OpCode.PUSH3];
            byte[] once = Apply([.. prefix, (byte)first, (byte)second, (byte)OpCode.RET]);
            byte[] twice = Apply(once);
            CollectionAssert.AreEqual(once, twice);
            Assert.Contains(expected, new Script(once).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
        }

        [TestMethod]
        public void Fusion_IsEnabledByAllOptimizationLevel()
        {
            byte[] original = [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2, (byte)OpCode.SWAP, (byte)OpCode.DROP, (byte)OpCode.RET];
            var (nef, _, _) = Neo.Optimizer.Optimizer.Optimize(
                CreateNefFile(original), CreateManifest(), null, CompilationOptions.OptimizationType.All);
            CollectionAssert.AreEqual(
                new[] { OpCode.PUSH1, OpCode.PUSH2, OpCode.NIP, OpCode.RET },
                new Script(nef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
        }

        private static byte[] Apply(byte[] script)
        {
            var (nef, _, _) = Strategies.ApplyPeephole(CreateNefFile(script), CreateManifest(), null, PeepholeRule.StackOpFusion);
            return nef.Script.ToArray();
        }

        private static void AssertSameBehavior(byte[] original, byte[] optimized)
        {
            var expected = Run(original);
            var actual = Run(optimized);
            Assert.AreEqual(VMState.HALT, expected.State);
            Assert.AreEqual(VMState.HALT, actual.State);
            CollectionAssert.AreEqual(expected.Stack, actual.Stack);
        }

        private static (VMState State, string[] Stack) Run(byte[] script)
        {
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            VMState state = engine.Execute();
            string[] stack = engine.ResultStack.Select(static item => $"{item.Type}:{item.GetInteger()}").ToArray();
            return (state, stack);
        }

        private static NefFile CreateNefFile(byte[] script) => new() { Compiler = "test", Source = "test.cs", Tokens = [], Script = script };

        private static ContractManifest CreateManifest() => new()
        {
            Name = "TestContract",
            Groups = [],
            SupportedStandards = [],
            Abi = new ContractAbi
            {
                Methods =
                [
                    new ContractMethodDescriptor
                    {
                        Name = "main",
                        Offset = 0,
                        Parameters = [],
                        ReturnType = ContractParameterType.Any,
                        Safe = false
                    }
                ],
                Events = [],
            },
            Permissions = [],
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create()
        };
    }
}
