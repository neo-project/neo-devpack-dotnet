// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_OptimizerAutomation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using OptimizerClass = Neo.Optimizer.Optimizer;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class UnitTest_OptimizerAutomation : DebugAndTestBase<Contract_Array>
    {
        [TestMethod]
        public void Test_RemoveMethodTokenRemapsRemainingCallTokens()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x01, 0x00,
                (byte)OpCode.RET
            ];
            MethodToken[] tokens =
            [
                CreateMethodToken("unused"),
                CreateMethodToken("used")
            ];
            var nef = CreateNefFile(script, tokens);

            var (optimizedNef, _, _) = Strategies.RemoveMethodTokens(nef, CreateEmptyManifest(), null);

            Assert.AreEqual(1, optimizedNef.Tokens.Length);
            Assert.AreEqual("used", optimizedNef.Tokens[0].Method);
            var instruction = new Script(optimizedNef.Script.ToArray()).GetInstruction(0);
            Assert.AreEqual(OpCode.CALLT, instruction.OpCode);
            Assert.AreEqual(0, instruction.TokenU16);
        }

        [TestMethod]
        public void Test_FoldJump_DoesNotHangOnSelfJump()
        {
            var nef = CreateNefFile([(byte)OpCode.JMP, 0x00], Array.Empty<MethodToken>());
            Task optimizeTask = Task.Run(() => OptimizerClass.Optimize(
                nef,
                CreateEmptyManifest(),
                null,
                CompilationOptions.OptimizationType.Experimental));

            Assert.IsTrue(optimizeTask.Wait(TimeSpan.FromSeconds(2)), "Optimizer should converge on a self-referential jump");
        }

        [TestMethod]
        public void Test_FoldJump_DoesNotHangOnJumpCycle()
        {
            var nef = CreateNefFile([(byte)OpCode.JMP, 0x02, (byte)OpCode.JMP, 0xfe], Array.Empty<MethodToken>());
            Task optimizeTask = Task.Run(() => OptimizerClass.Optimize(
                nef,
                CreateEmptyManifest(),
                null,
                CompilationOptions.OptimizationType.Experimental));

            Assert.IsTrue(optimizeTask.Wait(TimeSpan.FromSeconds(2)), "Optimizer should converge on a cyclic jump chain");
        }

        [TestMethod]
        public void Test_FoldJump_RetargetsJumpChainToFinalTarget()
        {
            var nef = CreateNefFile([(byte)OpCode.JMP, 0x02, (byte)OpCode.JMP, 0x02, (byte)OpCode.JMP, 0x02, (byte)OpCode.RET], Array.Empty<MethodToken>());

            var (optimizedNef, _, _) = Strategies.NormalizeJumps(
                nef,
                CreateEmptyManifest(),
                null);

            var instructions = new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().ToArray();
            Assert.AreEqual(OpCode.RET, instructions[0].instruction.OpCode);
        }

        [TestMethod]
        public void Test_FoldJump_ReplacesJumpToEndTryWithEndTry()
        {
            var nef = CreateNefFile([(byte)OpCode.JMP, 0x02, (byte)OpCode.ENDTRY, 0x02, (byte)OpCode.RET], Array.Empty<MethodToken>());

            var (optimizedNef, _, _) = Strategies.NormalizeJumps(
                nef,
                CreateEmptyManifest(),
                null);

            var instructions = new Script(optimizedNef.Script.ToArray()).EnumerateInstructions().ToArray();
            Assert.AreEqual(OpCode.ENDTRY, instructions[0].instruction.OpCode);
            Assert.AreEqual(2, instructions[0].instruction.TokenI8);
        }

        [TestMethod]
        public void Test_BuildOptimizedAssets_RetargetsDeletedMethodOffsetWithoutSequencePointRemap()
        {
            var nef = CreateNefFile([(byte)OpCode.NOP, (byte)OpCode.RET], Array.Empty<MethodToken>());
            var manifest = CreateManifestWithMethodOffset(0);
            var instructions = new Script(nef.Script.ToArray()).EnumerateInstructions().ToArray();
            Neo.VM.Instruction nop = instructions[0].instruction;
            Neo.VM.Instruction ret = instructions[1].instruction;
            OrderedDictionary simplifiedInstructionsToAddress = new()
            {
                { ret, 0 }
            };

            var (_, optimizedManifest, _) = AssetBuilder.BuildOptimizedAssets(
                nef,
                manifest,
                null,
                simplifiedInstructionsToAddress,
                new Dictionary<Neo.VM.Instruction, Neo.VM.Instruction>(),
                new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
                new Dictionary<int, Neo.VM.Instruction>
                {
                    [0] = nop,
                    [nop.Size] = ret
                });

            Assert.AreEqual(0, optimizedManifest.Abi.Methods[0].Offset);
        }

        [TestMethod]
        public void Test_AutomatedOptimization()
        {
            var (nef, manifest, _) = OptimizerClass.Optimize(
                NefFile, Manifest, null, CompilationOptions.OptimizationType.Experimental);

            Assert.IsNotNull(nef);
            Assert.IsNotNull(manifest);
            Assert.IsNotNull(manifest.Extra);
            Assert.IsTrue(manifest.Extra.ContainsProperty("nef"));
            Assert.IsTrue((manifest.Extra["nef"] as JObject)?.ContainsProperty("optimization") == true);
        }

        [TestMethod]
        public void Test_OptimizationWithoutExperimental()
        {
            var originalNef = NefFile;
            var originalManifest = Manifest;

            var (nef, manifest, _) = OptimizerClass.Optimize(
                originalNef, originalManifest, null, CompilationOptions.OptimizationType.None);

            Assert.AreSame(originalNef, nef);
            Assert.AreSame(originalManifest, manifest);
        }

        [TestMethod]
        public void Test_OptimizationTypeFlags()
        {
            var (resultNef, _, _) = OptimizerClass.Optimize(
                NefFile, Manifest, null, CompilationOptions.OptimizationType.Basic);

            Assert.AreEqual(NefFile.Script.Length, resultNef.Script.Length);
        }

        private static MethodToken CreateMethodToken(string method)
        {
            return new MethodToken
            {
                Hash = NativeContract.NEO.Hash,
                Method = method,
                ParametersCount = 0,
                HasReturnValue = false,
                CallFlags = CallFlags.ReadOnly
            };
        }

        private static NefFile CreateNefFile(byte[] script, MethodToken[] tokens)
        {
            return new NefFile
            {
                Compiler = "test",
                Source = "test.cs",
                Tokens = tokens,
                Script = script
            };
        }

        private static ContractManifest CreateEmptyManifest()
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

        private static ContractManifest CreateManifestWithMethodOffset(int offset)
        {
            var manifest = CreateEmptyManifest();
            manifest.Abi.Methods =
            [
                new ContractMethodDescriptor
                {
                    Name = "main",
                    Offset = offset,
                    Parameters = Array.Empty<ContractParameterDefinition>(),
                    ReturnType = ContractParameterType.Void,
                    Safe = false
                }
            ];
            return manifest;
        }
    }
}
