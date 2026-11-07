// Copyright (C) 2015-2026 The Neo Project.
//
// StrategyCoverageTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class StrategyCoverageTests
    {
        [TestMethod]
        public void UseIsNull_RewritesEqualityWithNull()
        {
            var (optimizedNef, _, _) = Strategies.ApplyPeephole(
                CreateNefFile([(byte)OpCode.PUSHNULL, (byte)OpCode.EQUAL, (byte)OpCode.RET]),
                CreateManifest(), null, PeepholeRule.UseIsNull);
            CollectionAssert.AreEqual(new[] { OpCode.ISNULL, OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void FoldNotInEqual_FoldsEqualityNegation()
        {
            var (optimizedNef, _, _) = Strategies.ApplyPeephole(
                CreateNefFile([(byte)OpCode.EQUAL, (byte)OpCode.NOT, (byte)OpCode.RET]),
                CreateManifest(), null, PeepholeRule.FoldNotInEqual);
            CollectionAssert.AreEqual(new[] { OpCode.NOTEQUAL, OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void FoldNotInJmp_FoldsNegatedConditionalJump()
        {
            var (optimizedNef, _, _) = Strategies.ApplyPeephole(
                CreateNefFile([(byte)OpCode.PUSH1, (byte)OpCode.NOT, (byte)OpCode.JMPIF, 0x02, (byte)OpCode.PUSH0, (byte)OpCode.RET]),
                CreateManifest(), null, PeepholeRule.FoldNotInJmp);
            CollectionAssert.AreEqual(new[] { OpCode.PUSH1, OpCode.JMPIFNOT, OpCode.PUSH0, OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void NopBufferPeephole_PreservesScriptLengthUntilCompaction()
        {
            NefFile nef = CreateNefFile([(byte)OpCode.PUSH1, (byte)OpCode.ADD, (byte)OpCode.RET]);
            int originalLength = nef.Script.Length;
            NefFile rewritten = Peephole.Rewrite(nef, CreateManifest(), PeepholeRule.All);
            Assert.AreEqual(originalLength, rewritten.Script.Length);
            CollectionAssert.AreEqual(new[] { OpCode.INC, OpCode.NOP, OpCode.RET }, GetOpCodes(rewritten));
        }

        [TestMethod]
        public void ReplaceJumpWithRet_RewritesJumpToReturn()
        {
            var (optimizedNef, _, _) = Strategies.ReplaceJumpWithRet(
                CreateNefFile([(byte)OpCode.JMP, 0x02, (byte)OpCode.RET]), CreateManifest(), null);
            CollectionAssert.AreEqual(new[] { OpCode.RET, OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void RemoveMultiRet_CollapsesAdjacentReturns()
        {
            var (optimizedNef, _, _) = Strategies.CollapseAdjacentReturns(
                CreateNefFile([(byte)OpCode.RET, (byte)OpCode.RET, (byte)OpCode.RET]), CreateManifest(), null);
            CollectionAssert.AreEqual(new[] { OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void RemoveUnreachable_PreservesTrailingReturn()
        {
            var (optimizedNef, _, _) = Strategies.RemoveUnreachable(
                CreateNefFile([(byte)OpCode.RET, (byte)OpCode.PUSH1, (byte)OpCode.RET]), CreateManifest(), null);
            CollectionAssert.AreEqual(new[] { OpCode.RET, OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void CollapseAdjacentReturns_PreservesMethodBoundaries()
        {
            var (optimizedNef, optimizedManifest, _) = Strategies.CollapseAdjacentReturns(
                CreateNefFile([(byte)OpCode.RET, (byte)OpCode.RET, (byte)OpCode.RET]), CreateManifest(0, 1), null);
            CollectionAssert.AreEqual(new[] { OpCode.RET, OpCode.RET }, GetOpCodes(optimizedNef));
            CollectionAssert.AreEqual(new[] { 0, 1 }, optimizedManifest.Abi.Methods.Select(static method => method.Offset).ToArray());
        }

        [TestMethod]
        public void CollapseAdjacentReturns_PreservesPushaMethodEntry()
        {
            byte[] script = [(byte)OpCode.RET, (byte)OpCode.RET, (byte)OpCode.PUSHA, 0xff, 0xff, 0xff, 0xff, (byte)OpCode.RET];
            var (optimizedNef, _, _) = Strategies.CollapseAdjacentReturns(CreateNefFile(script), CreateManifest(), null);
            CollectionAssert.AreEqual(script, optimizedNef.Script.ToArray());
        }

        [TestMethod]
        public void CollapseAdjacentReturns_IgnoresRemovedMethodEntries()
        {
            ContractManifest manifest = CreateManifest(0, 1);
            manifest.Abi.Methods[1].Name = "_initialize";
            ScriptGraph graph = ScriptGraph.Parse(
                CreateNefFile([(byte)OpCode.RET, (byte)OpCode.INITSSLOT, 0, (byte)OpCode.RET, (byte)OpCode.RET]), manifest);

            graph.RemoveUnusedInitialize();
            Assert.IsFalse(manifest.Abi.Methods.Any(static method => method.Name == "_initialize"));

            graph.CollapseAdjacentReturns();
            var (optimizedNef, _, _) = graph.Emit();
            CollectionAssert.AreEqual(new[] { OpCode.RET }, GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void CollapseAdjacentReturns_IgnoresUnreachableDebugMethodEntries()
        {
            JObject debugInfo = new()
            {
                ["methods"] = new JArray(new JObject { ["range"] = "1-1" })
            };
            ScriptGraph graph = ScriptGraph.Parse(
                CreateNefFile([(byte)OpCode.RET, (byte)OpCode.RET]), CreateManifest(), debugInfo);
            graph.CollapseAdjacentReturns();

            CollectionAssert.AreEqual(new[] { OpCode.RET }, graph.Instructions.Select(static instruction => instruction.OpCode).ToArray());
        }

        private static readonly OpCode[] OneOperandJumps = [OpCode.JMPIF, OpCode.JMPIFNOT, OpCode.JMPIF_L, OpCode.JMPIFNOT_L];

        private static readonly OpCode[] TwoOperandJumps =
        [
            OpCode.JMPEQ, OpCode.JMPNE, OpCode.JMPGT, OpCode.JMPGE, OpCode.JMPLT, OpCode.JMPLE,
            OpCode.JMPEQ_L, OpCode.JMPNE_L, OpCode.JMPGT_L, OpCode.JMPGE_L, OpCode.JMPLT_L, OpCode.JMPLE_L
        ];

        public static IEnumerable<object[]> AllConditionalJumps =>
            OneOperandJumps.Concat(TwoOperandJumps).Select(static op => new object[] { op });

        private static bool IsLong(OpCode op) => op >= OpCode.JMPIF_L && op <= OpCode.JMPLE_L && (op - OpCode.JMPIF_L) % 2 == 0;

        // Appends "jump to next instruction" followed by RET.
        private static byte[] FallThroughJump(byte[] prefix, OpCode op)
        {
            List<byte> script = [.. prefix, (byte)op];
            if (IsLong(op))
                script.AddRange([5, 0, 0, 0]);
            else
                script.Add(2);
            script.Add((byte)OpCode.RET);
            return [.. script];
        }

        private static (VMState State, string[] Stack) Run(byte[] script)
        {
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            VMState state = engine.Execute();
            string[] stack = engine.ResultStack.Select(static item =>
                item.Type + ":" + (item.IsNull ? "" : Convert.ToHexString(item.GetSpan()))).ToArray();
            return (state, stack);
        }

        private static byte[] OptimizeJumps(byte[] script)
        {
            var (nef, _, _) = Strategies.RemoveUnnecessaryJumps(CreateNefFile(script), CreateManifest(), null);
            return nef.Script.ToArray();
        }

        private static byte[] OptimizeAll(byte[] script)
        {
            var (nef, _, _) = Neo.Optimizer.Optimizer.Optimize(CreateNefFile(script), CreateManifest(), null,
                CompilationOptions.OptimizationType.All);
            return nef.Script.ToArray();
        }

        private static void AssertSameBehavior(byte[] script, params Func<byte[], byte[]>[] optimizers)
        {
            var expected = Run(script);
            foreach (var optimize in optimizers)
            {
                byte[] optimized = optimize(script);
                var actual = Run(optimized);
                Assert.AreEqual(expected.State, actual.State);
                Assert.AreSequenceEqual(expected.Stack, actual.Stack);
            }
        }

        private static OpCode[] OpCodesOf(byte[] script)
            => new Script(script).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray();

        [TestMethod]
        public void RemoveUnnecessaryJumps_JmpEqWithNullOperandStillFaults()
        {
            byte[] script = [(byte)OpCode.PUSHNULL, (byte)OpCode.PUSH0, (byte)OpCode.JMPEQ, 2, (byte)OpCode.RET];
            Assert.AreEqual(VMState.FAULT, Run(script).State);

            foreach (var optimize in new Func<byte[], byte[]>[] { OptimizeJumps, OptimizeAll })
            {
                byte[] optimized = optimize(script);
                Assert.AreEqual(VMState.FAULT, Run(optimized).State);
                Assert.DoesNotContain(OpCode.DROP, OpCodesOf(optimized));
            }
        }

        [TestMethod]
        [DataRow(OpCode.JMPIF)]
        [DataRow(OpCode.JMPIFNOT)]
        [DataRow(OpCode.JMPIF_L)]
        [DataRow(OpCode.JMPIFNOT_L)]
        public void RemoveUnnecessaryJumps_OversizedByteStringConditionStillFaults(OpCode op)
        {
            byte[] push = [(byte)OpCode.PUSHDATA1, 33, .. new byte[33]];
            byte[] script = FallThroughJump(push, op);
            Assert.AreEqual(VMState.FAULT, Run(script).State);

            foreach (var optimize in new Func<byte[], byte[]>[] { OptimizeJumps, OptimizeAll })
            {
                byte[] optimized = optimize(script);
                Assert.AreEqual(VMState.FAULT, Run(optimized).State);
                Assert.DoesNotContain(OpCode.DROP, OpCodesOf(optimized));
            }
        }

        [TestMethod]
        [DynamicData(nameof(AllConditionalJumps))]
        public void RemoveUnnecessaryJumps_PreservesConditionalJumpBehavior(OpCode op)
        {
            byte[] prefix = OneOperandJumps.Contains(op)
                ? [(byte)OpCode.PUSH1]
                : [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2];
            byte[] script = FallThroughJump(prefix, op);
            byte[] optimized = OptimizeJumps(script);
            Assert.IsTrue(
                OpCodesOf(optimized).Any(candidate => OneOperandJumps.Contains(candidate) || TwoOperandJumps.Contains(candidate)),
                "Conditional jump must remain because it performs operand conversion and can fault.");
            AssertSameBehavior(script, OptimizeJumps, OptimizeAll);
        }

        [TestMethod]
        [DynamicData(nameof(AllConditionalJumps))]
        public void RemoveUnnecessaryJumps_NullOperandBehaviorIsPreserved(OpCode op)
        {
            // JMPIF/JMPIFNOT accept null (falsy); comparison jumps must fault on null.
            byte[] prefix = OneOperandJumps.Contains(op)
                ? [(byte)OpCode.PUSHNULL]
                : [(byte)OpCode.PUSHNULL, (byte)OpCode.PUSHNULL];
            AssertSameBehavior(FallThroughJump(prefix, op), OptimizeJumps, OptimizeAll);
        }

        [TestMethod]
        [DataRow(OpCode.JMP, 2)]
        [DataRow(OpCode.JMP_L, 5)]
        public void RemoveUnnecessaryJumps_StillRemovesUnconditionalJumpToNextInstruction(OpCode op, int size)
        {
            List<byte> script = [(byte)OpCode.PUSH1, (byte)op];
            if (size == 5)
                script.AddRange([5, 0, 0, 0]);
            else
                script.Add(2);
            script.Add((byte)OpCode.RET);

            byte[] optimized = OptimizeJumps([.. script]);
            Assert.AreSequenceEqual(new[] { OpCode.PUSH1, OpCode.RET }, OpCodesOf(optimized));
            AssertSameBehavior([.. script], OptimizeJumps, OptimizeAll);
        }

        [TestMethod]
        public void RemoveUnnecessaryJumps_PreservesIncomingJumpToConditionalJump()
        {
            byte[] script =
            [
                (byte)OpCode.PUSH2,
                (byte)OpCode.PUSH2,
                (byte)OpCode.JMP, 4,            // -> JMPEQ
                (byte)OpCode.PUSH9,
                (byte)OpCode.RET,
                (byte)OpCode.JMPEQ, 2,          // -> next instruction
                (byte)OpCode.PUSH5,
                (byte)OpCode.RET
            ];

            byte[] optimized = OptimizeJumps(script);
            Assert.AreSequenceEqual(script, optimized);

            var nef = CreateNefFile(optimized);
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            Assert.IsNotNull(NefFile.Parse(nef.ToArray(), true));
            AssertSameBehavior(script, OptimizeJumps, OptimizeAll);
        }

        [TestMethod]
        public void NormalizeJumps_PreservesPushaPointerIdentity()
        {
            byte[] script =
            [
                (byte)OpCode.PUSHA, 12, 0, 0, 0,       // target A at 12
                (byte)OpCode.PUSHA, 11, 0, 0, 0,       // target B at 16
                (byte)OpCode.EQUAL,
                (byte)OpCode.RET,
                (byte)OpCode.JMP, 4,                   // A -> B
                (byte)OpCode.PUSH2,
                (byte)OpCode.RET,
                (byte)OpCode.PUSH1,                    // B
                (byte)OpCode.RET
            ];
            var expected = Run(script);
            var (normalizedNef, _, _) = Strategies.NormalizeJumps(CreateNefFile(script), CreateManifest(0, 14), null);
            var normalized = Run(normalizedNef.Script.ToArray());
            Assert.AreEqual(expected.State, normalized.State);
            Assert.AreSequenceEqual(expected.Stack, normalized.Stack);

            var (optimizedNef, _, _) = Neo.Optimizer.Optimizer.Optimize(
                CreateNefFile(script), CreateManifest(0, 14), null, CompilationOptions.OptimizationType.All);
            var optimized = Run(optimizedNef.Script.ToArray());
            Assert.AreEqual(expected.State, optimized.State);
            Assert.AreSequenceEqual(expected.Stack, optimized.Stack);
        }

        [TestMethod]
        public void NormalizeJumps_EndTryContinuationThroughJumpPreservesFinally()
        {
            byte[] script =
            [
                (byte)OpCode.TRY, 0, 8,        // FINALLY starts at address 8
                (byte)OpCode.PUSH1,
                (byte)OpCode.ENDTRY, 2,        // resume at the JMP at address 6
                (byte)OpCode.JMP, 4,           // continue at ADD at address 10
                (byte)OpCode.PUSH2,
                (byte)OpCode.ENDFINALLY,
                (byte)OpCode.ADD,
                (byte)OpCode.RET
            ];

            var expected = Run(script);
            Assert.AreEqual(VMState.HALT, expected.State);
            Assert.AreSequenceEqual(new[] { "Integer:03" }, expected.Stack);

            var (normalizedNef, _, _) = Strategies.NormalizeJumps(CreateNefFile(script), CreateManifest(), null);
            var normalizedScript = new Script(normalizedNef.Script.ToArray());
            var (endTryAddress, endTry) = normalizedScript.EnumerateInstructions()
                .Single(static item => item.instruction.OpCode is OpCode.ENDTRY or OpCode.ENDTRY_L);
            int continuation = endTryAddress + (endTry.OpCode == OpCode.ENDTRY ? endTry.TokenI8 : endTry.TokenI32);
            Assert.AreEqual(OpCode.ADD, normalizedScript.GetInstruction(continuation).OpCode);
            var normalized = Run(normalizedNef.Script.ToArray());
            Assert.AreEqual(expected.State, normalized.State);
            Assert.AreSequenceEqual(expected.Stack, normalized.Stack);

            var (optimizedNef, _, _) = Neo.Optimizer.Optimizer.Optimize(
                CreateNefFile(script), CreateManifest(), null, CompilationOptions.OptimizationType.All);
            var optimized = Run(optimizedNef.Script.ToArray());
            Assert.AreEqual(expected.State, optimized.State);
            Assert.AreSequenceEqual(expected.Stack, optimized.Stack);
        }

        [TestMethod]
        public void RemoveUnnecessaryJumps_DoesNotIncreaseStaticOpcodeCost()
        {
            // Static comparison (not a measured GAS test): legacy fee JMPEQ = 2, DROP = 2,
            // so JMPEQ -> DROP DROP would cost more. The optimizer must keep the instruction count.
            byte[] script = FallThroughJump([(byte)OpCode.PUSH1, (byte)OpCode.PUSH1], OpCode.JMPEQ);
            byte[] optimized = OptimizeJumps(script);
            Assert.HasCount(OpCodesOf(script).Length, OpCodesOf(optimized));
            Assert.AreEqual(0, OpCodesOf(optimized).Count(static o => o == OpCode.DROP));
        }

        private static OpCode[] GetOpCodes(NefFile nef)
            => new Script(nef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray();

        private static NefFile CreateNefFile(byte[] script)
        {
            return new NefFile
            {
                Compiler = "test",
                Source = "test.cs",
                Tokens = [],
                Script = script
            };
        }

        private static ContractManifest CreateManifest(params int[] methodOffsets)
        {
            if (methodOffsets.Length == 0) methodOffsets = [0];
            return new ContractManifest
            {
                Name = "TestContract",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods = methodOffsets.Select((offset, index) => new ContractMethodDescriptor
                    {
                        Name = index == 0 ? "main" : $"method{index}",
                        Offset = offset,
                        Parameters = [],
                        ReturnType = ContractParameterType.Any,
                        Safe = false
                    }).ToArray(),
                    Events = [],
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
