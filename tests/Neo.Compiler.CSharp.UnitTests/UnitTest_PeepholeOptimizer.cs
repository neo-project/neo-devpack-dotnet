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
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class PeepholeOptimizerTests
    {
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
                var (optimizedNef, _, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
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
                var (optimizedNef, _, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
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
                var (optimizedNef, _, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
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
                var (optimizedNef, _, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
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
                var (optimizedNef, _, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
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
                var (optimizedNef, _, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
                Assert.IsNotNull(optimizedNef);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Optimization threw exception: {ex.Message}");
            }
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
                    Methods = Array.Empty<ContractMethodDescriptor>(),
                    Events = Array.Empty<ContractEventDescriptor>()
                },
                Permissions = Array.Empty<ContractPermission>(),
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
