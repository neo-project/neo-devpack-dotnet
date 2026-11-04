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
using Neo.Compiler;
using Neo.Compiler.ControlFlow;
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

            var (optimizedNef, _, _) = Neo.Optimizer.Peephole.UseIncDec(CreateNefFile(sb.ToArray()), CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.INC, OpCode.RET },
                new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
        }

        [TestMethod]
        public void Test_UseNz_RewritesZeroNumericComparison()
        {
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.PUSH0);
            sb.Emit(OpCode.NUMEQUAL);
            sb.Emit(OpCode.RET);

            var (optimizedNef, _, _) = Neo.Optimizer.Peephole.UseNz(CreateNefFile(sb.ToArray()), CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.NZ, OpCode.NOT, OpCode.RET },
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

            var (optimizedNef, _, _) = Neo.Optimizer.Peephole.RemoveDupDrop(CreateNefFile(sb.ToArray()), CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.PUSH1, OpCode.STLOC0, OpCode.RET },
                new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray());
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

                var (optimizedNef, _, _) = Neo.Optimizer.Peephole.UseIncDec(CreateNefFile(sb.ToArray()), CreateManifest());
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

            var (optimizedNef, _, _) = Neo.Optimizer.Peephole.UseIncDec(CreateNefFile(sb.ToArray()), CreateManifest());
            Assert.AreSequenceEqual(
                [OpCode.DEC, OpCode.INC, OpCode.RET],
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
                Tokens = Array.Empty<MethodToken>(),
                Script = script
            };
        }

        private static ContractManifest CreateManifest()
        {
            return new ContractManifest
            {
                Name = "TestContract",
                Groups = Array.Empty<ContractGroup>(),
                SupportedStandards = Array.Empty<string>(),
                Abi = new ContractAbi
                {
                    Methods =
                    [
                        new ContractMethodDescriptor
                        {
                            Name = "main",
                            Offset = 0,
                            Parameters = Array.Empty<ContractParameterDefinition>(),
                            ReturnType = ContractParameterType.Any,
                            Safe = false
                        }
                    ],
                    Events = Array.Empty<ContractEventDescriptor>()
                },
                Permissions = Array.Empty<ContractPermission>(),
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
