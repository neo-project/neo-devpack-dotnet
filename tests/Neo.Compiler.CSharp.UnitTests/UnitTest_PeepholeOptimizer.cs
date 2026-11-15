// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_PeepholeOptimizer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using NeoOptimizer = Neo.Optimizer.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class PeepholeOptimizerTests
    {
        [TestMethod]
        public void Test_UseIncDec_RewritesConstantArithmetic()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH1);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            var (optimizedNef, _, _) = Strategies.ApplyPeephole(CreateNefFile(sb.ToArray()), CreateManifest(), null, PeepholeRule.UseIncDec);
            CollectionAssert.AreEqual(new[] { OpCode.INC, OpCode.RET },
                new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
        }

        [TestMethod]
        public void Test_UseNz_RewritesZeroNumericComparison()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH0);
            sb.Emit(OpCode.NUMEQUAL);
            sb.Emit(OpCode.RET);

            var (optimizedNef, _, _) = Strategies.ApplyPeephole(CreateNefFile(sb.ToArray()), CreateManifest(), null, PeepholeRule.UseNz);
            CollectionAssert.AreEqual(new[] { OpCode.NZ, OpCode.NOT, OpCode.RET },
                new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
        }

        [TestMethod]
        public void Test_RemoveDupDrop_RemovesRedundantAssignmentValue()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH1);
            sb.Emit(OpCode.DUP);
            sb.Emit(OpCode.STLOC0);
            sb.Emit(OpCode.DROP);
            sb.Emit(OpCode.RET);

            var (optimizedNef, _, _) = Strategies.ApplyPeephole(CreateNefFile(sb.ToArray()), CreateManifest(), null, PeepholeRule.RemoveDupDrop);
            CollectionAssert.AreEqual(new[] { OpCode.PUSH1, OpCode.STLOC0, OpCode.RET },
                new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
        }

        [TestMethod]
        public void Test_RemovePushDrop_RemovesAllPushKinds()
        {
            (string Name, byte[] Script, OpCode[] Expected)[] cases =
            [
                ("PUSHM1", [(byte)OpCode.PUSHM1, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSH0", [(byte)OpCode.PUSH0, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSH16", [(byte)OpCode.PUSH16, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSHT", [(byte)OpCode.PUSHT, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSHF", [(byte)OpCode.PUSHF, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSHNULL", [(byte)OpCode.PUSHNULL, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSHINT16", [(byte)OpCode.PUSHINT16, 0x01, 0x00, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSHDATA1", [(byte)OpCode.PUSHDATA1, 0x02, 0xAA, 0xBB, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("PUSHA", [(byte)OpCode.PUSHA, 0x00, 0x00, 0x00, 0x00, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("NEWARRAY0", [(byte)OpCode.NEWARRAY0, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("NEWSTRUCT0", [(byte)OpCode.NEWSTRUCT0, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("NEWMAP", [(byte)OpCode.NEWMAP, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("DEPTH", [(byte)OpCode.DEPTH, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.RET]),
                ("DUP", [(byte)OpCode.PUSH1, (byte)OpCode.DUP, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.PUSH1, OpCode.RET]),
                ("OVER", [(byte)OpCode.PUSH1, (byte)OpCode.PUSH2, (byte)OpCode.OVER, (byte)OpCode.DROP, (byte)OpCode.RET], [OpCode.PUSH1, OpCode.PUSH2, OpCode.RET]),
            ];

            foreach (var (name, script, expected) in cases)
            {
                var (optimizedNef, _, _) = Strategies.ApplyPeephole(
                    CreateNefFile(script), CreateManifest(), null, PeepholeRule.RemovePushDrop);
                CollectionAssert.AreEqual(expected, GetOpCodes(optimizedNef), name);
            }
        }

        [TestMethod]
        public void Test_RemovePushDrop_DoesNotCrossBasicBlockBoundary()
        {
            byte[] script = [(byte)OpCode.PUSH1, (byte)OpCode.DROP, (byte)OpCode.RET];
            NefFile rewritten = Peephole.Rewrite(CreateNefFile(script), CreateManifest(0, 1), PeepholeRule.RemovePushDrop);
            CollectionAssert.AreEqual(new[] { OpCode.PUSH1, OpCode.DROP, OpCode.RET }, GetOpCodes(rewritten));
        }

        [TestMethod]
        public void Test_RemovePushDrop_DoesNotRewriteWhenDropIsJumpTarget()
        {
            byte[] script = [(byte)OpCode.PUSH9, (byte)OpCode.JMP, 0x03 /* to DROP */, (byte)OpCode.PUSH1, (byte)OpCode.DROP, (byte)OpCode.RET];
            NefFile rewritten = Peephole.Rewrite(CreateNefFile(script), CreateManifest(), PeepholeRule.RemovePushDrop);
            CollectionAssert.AreEqual(script, rewritten.Script.ToArray());
        }

        /// <summary>
        /// Test that PUSH1 ADD pattern does not crash during optimization.
        /// This tests the operator precedence fix in peephole optimization.
        /// </summary>
        [TestMethod]
        public void Test_Push1_Add_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH1);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            // Optimize - should not throw
            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                // If we get here, the test passes
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that PUSH2 ADD pattern does not crash during optimization.
        /// This tests the operator precedence fix for value == 2 pattern.
        /// </summary>
        [TestMethod]
        public void Test_Push2_Add_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH2);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            // Optimize - should not throw
            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_IntegerPush2_Add_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSHINT8, new byte[] { 0x02 });
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that integer push with value 1 followed by ADD does not crash.
        /// This specifically tests the pattern: (pushInt.Contains && value==1) || PUSH1
        /// </summary>
        [TestMethod]
        public void Test_IntegerPush1_Add_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSHINT8, new byte[] { 0x01 });
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            // Optimize - should not throw
            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that PUSH0 NUMEQUAL pattern does not crash during optimization.
        /// This tests the operator precedence fix for value == 0 pattern.
        /// </summary>
        [TestMethod]
        public void Test_Push0_NumEqual_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH0);
            sb.Emit(OpCode.NUMEQUAL);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            // Optimize - should not throw
            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_IntegerPush0_NumEqual_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSHINT8, new byte[] { 0x00 });
            sb.Emit(OpCode.NUMEQUAL);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that PUSH1 SUB pattern does not crash during optimization.
        /// </summary>
        [TestMethod]
        public void Test_Push1_Sub_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH1);
            sb.Emit(OpCode.SUB);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            // Optimize - should not throw
            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that patterns with values other than 1, 2, 0 do not crash.
        /// </summary>
        [TestMethod]
        public void Test_Push3_Add_Optimization_DoesNotCrash()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH3);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();
            NefFile nef = CreateNefFile(script);
            var manifest = CreateManifest();

            // Optimize - should not throw
            try
            {
                var (optimizedNef, _, _) = NeoOptimizer.Optimize(nef, manifest, null, CompilationOptions.OptimizationType.Experimental);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
        }

        // INC INC / DEC DEC is cheaper before Huyao but more expensive after it, so PUSH2 stays.
        [TestMethod]
        public void Test_UseIncDec_Push2_IsNotRewritten()
        {
            foreach (var op in new[] { OpCode.ADD, OpCode.SUB })
            {
                OpCode[] original = [OpCode.PUSH2, op, OpCode.RET];
                using ScriptBuilder sb = new();
                foreach (var o in original) sb.Emit(o);

                var (optimizedNef, _, _) = Strategies.ApplyPeephole(
                    CreateNefFile(sb.ToArray()), CreateManifest(), null, PeepholeRule.UseIncDec);
                Assert.AreSequenceEqual(original, new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static i => i.instruction.OpCode).ToArray());
            }
        }

        [TestMethod]
        public void Test_UseIncDec_Push1_AndPushInt8_Of_One_StillRewritten()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH1);
            sb.Emit(OpCode.SUB);
            sb.Emit(OpCode.PUSHINT8, new byte[] { 1 });
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            var (optimizedNef, _, _) = Strategies.ApplyPeephole(CreateNefFile(sb.ToArray()), CreateManifest(), null, PeepholeRule.UseIncDec);
            Assert.AreSequenceEqual([OpCode.DEC, OpCode.INC, OpCode.RET],
                new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static i => i.instruction.OpCode).ToArray());
        }

        [TestMethod]
        public void Test_Optimize_Push2_Representations_PreserveExecution()
        {
            var maxInt = System.Numerics.BigInteger.Pow(2, 255) - 1;
            var minInt = -System.Numerics.BigInteger.Pow(2, 255);
            var starts = new[]
            {
                System.Numerics.BigInteger.Zero, -5, int.MaxValue, int.MinValue,
                maxInt - 2, maxInt - 1, maxInt, minInt, minInt + 2, minInt + 1
            };
            Action<ScriptBuilder>[] pushTwo =
            [
                static sb => sb.Emit(OpCode.PUSH2),
                static sb => sb.Emit(OpCode.PUSHINT8, new byte[] { 2 }),
                static sb => sb.Emit(OpCode.PUSHINT16, new byte[] { 2, 0 }),
            ];

            int faults = 0;
            foreach (var op in new[] { OpCode.ADD, OpCode.SUB })
            {
                var incDec = op == OpCode.ADD ? OpCode.INC : OpCode.DEC;
                foreach (var start in starts)
                {
                    foreach (var push in pushTwo)
                    {
                        using ScriptBuilder sb = new();
                        sb.EmitPush(start);
                        push(sb);
                        sb.Emit(op);
                        sb.Emit(OpCode.RET);
                        byte[] original = sb.ToArray();

                        var (optimizedNef, _, _) = NeoOptimizer.Optimize(CreateNefFile(original), CreateManifest(), null, CompilationOptions.OptimizationType.Experimental);
                        byte[] optimized = optimizedNef.Script.ToArray();

                        var ops = new Script(optimized).EnumerateInstructions().Select(static i => i.instruction.OpCode).ToArray();
                        for (int i = 0; i + 1 < ops.Length; i++)
                            Assert.IsFalse(ops[i] == incDec && ops[i + 1] == incDec, $"{incDec} {incDec} introduced for {start} {op}");

                        using ExecutionEngine a = new();
                        a.LoadScript(original);
                        using ExecutionEngine b = new();
                        b.LoadScript(optimized);
                        var stateA = a.Execute();
                        Assert.AreEqual(stateA, b.Execute(), $"State mismatch for {start} {op}");
                        if (stateA == VM.VMState.FAULT)
                        {
                            faults++;
                            continue;
                        }
                        Assert.AreEqual(a.ResultStack.Pop().GetInteger(), b.ResultStack.Pop().GetInteger());
                    }
                }
            }

            // Overflow cases: ADD from maxInt - 1, SUB from minInt + 1, in each encoding.
            Assert.AreEqual(12, faults);
        }

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

        private static OpCode[] GetOpCodes(NefFile nef) => new Script(nef.Script.ToArray())
            .EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray();

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
                    Events = []
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
