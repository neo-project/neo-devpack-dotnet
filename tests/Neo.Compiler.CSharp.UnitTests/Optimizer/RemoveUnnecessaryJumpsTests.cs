// Copyright (C) 2015-2026 The Neo Project.
//
// RemoveUnnecessaryJumpsTests.cs file belongs to the neo project and is free
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
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ControlFlowJumpTarget = Neo.Compiler.ControlFlow.JumpTarget;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    /// <summary>
    /// Conditional jumps that target the next instruction are deliberately preserved
    /// (they convert their operands and may fault, so replacing them with DROPs would
    /// change VM behavior); the tests document that decision instead of asserting a
    /// simplification.
    /// </summary>
    [TestClass]
    public class RemoveUnnecessaryJumpsTests
    {
        [TestMethod]
        public void ShortJmpToNext_IsRemoved()
        {
            // [0] JMP +2 -> [2] PUSH1; [2] PUSH1; [3] RET
            var nef = CreateNefFile(new byte[] { (byte)OpCode.JMP, 0x02, (byte)OpCode.PUSH1, (byte)OpCode.RET });

            (nef, _, _) = Strategies.RemoveUnnecessaryJumps(nef, CreateManifest(), null);
            CollectionAssert.AreEqual(new[] { OpCode.PUSH1, OpCode.RET }, GetOpCodes(nef),
                "short JMP to its own next instruction must be removed");
        }

        [TestMethod]
        public void LongJmpToNext_IsRemoved()
        {
            // [0] JMP_L +5 -> [5] PUSH1; [5] PUSH1; [6] RET
            var nef = CreateNefFile(new byte[]
            {
                (byte)OpCode.JMP_L, 0x05, 0x00, 0x00, 0x00,
                (byte)OpCode.PUSH1, (byte)OpCode.RET
            });

            (nef, _, _) = Strategies.RemoveUnnecessaryJumps(nef, CreateManifest(), null);
            CollectionAssert.AreEqual(new[] { OpCode.PUSH1, OpCode.RET }, GetOpCodes(nef),
                "long JMP to its own next instruction must be removed");
        }

        [TestMethod]
        public void IncomingJumpToRemovedJmp_IsRetargeted()
        {
            // Layout:
            //   [0] JMP +4 -> [4] PUSH1   (outer JMP; its own next instruction is the inner JMP at [2])
            //   [2] JMP +2 -> [4] PUSH1   (inner JMP targets its own next instruction)
            //   [4] PUSH1; [5] RET
            var nef = CreateNefFile(new byte[]
            {
                (byte)OpCode.JMP, 0x04,
                (byte)OpCode.JMP, 0x02,
                (byte)OpCode.PUSH1, (byte)OpCode.RET
            });

            (nef, _, _) = Strategies.RemoveUnnecessaryJumps(nef, CreateManifest(), null);

            // Inner JMP is removed; outer JMP is retargeted to PUSH1.
            CollectionAssert.AreEqual(new[] { OpCode.JMP, OpCode.PUSH1, OpCode.RET }, GetOpCodes(nef),
                "inner redundant JMP removed, outer JMP retargeted");

            var instructions = GetInstructions(nef);
            var jmpIdx = instructions.FindIndex(i => i.OpCode == OpCode.JMP);
            var pushIdx = instructions.FindIndex(i => i.OpCode == OpCode.PUSH1);
            int jmpAddress = instructions.Take(jmpIdx).Sum(i => i.Size);
            int pushAddress = instructions.Take(pushIdx).Sum(i => i.Size);
            // Jump operands are byte offsets relative to the start of the instruction.
            Assert.AreEqual(pushAddress - jmpAddress, instructions[jmpIdx].TokenI8,
                "outer jump must be retargeted to PUSH1 after the inner JMP is removed");
        }

        [TestMethod]
        public void RemovingCatchEntryJump_RetargetsHandlerAndUpdatesMetadata()
        {
            var nef = CreateNefFile(
            [
                (byte)OpCode.TRY, 5, 0,      // Catch at 5
                (byte)OpCode.PUSH1,
                (byte)OpCode.THROW,
                (byte)OpCode.JMP, 2,        // Redundant catch entry
                (byte)OpCode.DROP,
                (byte)OpCode.PUSH2,
                (byte)OpCode.ENDTRY, 2,     // Resume at 11
                (byte)OpCode.RET,
                (byte)OpCode.PUSH3,        // Second ABI method at 12
                (byte)OpCode.RET
            ]);
            var manifest = CreateManifest();
            manifest.Abi.Methods =
            [
                new ContractMethodDescriptor { Name = "handle", Offset = 0, Parameters = [], ReturnType = ContractParameterType.Integer },
                new ContractMethodDescriptor { Name = "getValue", Offset = 12, Parameters = [], ReturnType = ContractParameterType.Integer }
            ];
            var debug = (JObject)JToken.Parse("""
                {
                    "methods": [
                        {
                            "range": "0-11",
                            "sequence-points": ["5[0]10:1-10:9", "7[0]11:1-11:9"]
                        },
                        {
                            "range": "12-13",
                            "abi": {"offset": 12},
                            "sequence-points": ["12[0]15:1-15:9"]
                        }
                    ]
                }
                """)!;

            var (optimized, optimizedManifest, optimizedDebug) = Strategies.RemoveUnnecessaryJumps(nef, manifest, debug);

            var script = new Script(optimized.Script);
            var (catchOffset, _) = ControlFlowJumpTarget.ComputeTryTarget(0, script.GetInstruction(0));
            Assert.AreEqual(OpCode.DROP, script.GetInstruction(catchOffset).OpCode);
            Assert.AreEqual(10, optimizedManifest.Abi.Methods[1].Offset);
            Assert.AreEqual(OpCode.PUSH3, script.GetInstruction(optimizedManifest.Abi.Methods[1].Offset).OpCode);

            var methods = (JArray)optimizedDebug!["methods"]!;
            Assert.AreEqual("0-9", methods[0]!["range"]!.AsString());
            Assert.AreEqual("5[0]10:1-10:9", methods[0]!["sequence-points"]![0]!.AsString());
            Assert.AreEqual("5[0]11:1-11:9", methods[0]!["sequence-points"]![1]!.AsString());
            Assert.AreEqual("10-11", methods[1]!["range"]!.AsString());
            Assert.AreEqual(10, (int)methods[1]!["abi"]!["offset"]!.AsNumber());
            Assert.AreEqual("10[0]15:1-15:9", methods[1]!["sequence-points"]![0]!.AsString());

            using ExecutionEngine engine = new();
            engine.LoadScript(script);
            Assert.AreEqual(VMState.HALT, engine.Execute());
            Assert.AreEqual(new BigInteger(2), engine.ResultStack.Peek().GetInteger());
        }

        [TestMethod]
        public void ConditionalToNext_IsPreserved_ByDesign()
        {
            foreach (var opCode in new[]
            {
                OpCode.JMPIF, OpCode.JMPIFNOT,
                OpCode.JMPEQ, OpCode.JMPNE, OpCode.JMPGT, OpCode.JMPGE, OpCode.JMPLT, OpCode.JMPLE
            })
            {
                var (nef, _, _) = Strategies.RemoveUnnecessaryJumps(
                    CreateNefFile(new byte[] { (byte)OpCode.PUSH0, (byte)OpCode.PUSH0, (byte)opCode, 0x02, (byte)OpCode.PUSH1, (byte)OpCode.RET }),
                    CreateManifest(), null);
                CollectionAssert.Contains(GetOpCodes(nef), opCode, $"conditional {opCode} targeting the next instruction is intentionally preserved");
            }
        }

        [TestMethod]
        public void FoldJump_ThroughIntermediate_ProducesJmpClearedByCleanup()
        {
            // Layout:
            //   [0] JMP +4   -> [4] inner JMP
            //   [2] PUSH1
            //   [3] RET
            //   [4] JMP_L -2 -> [2] PUSH1   (inner JMP targets its own previous instruction)
            //   [9] PUSH2
            //   [10] RET
            // FoldJump threads the outer JMP through the inner JMP onto PUSH1 at [2],
            // which is now the outer JMP's own next instruction; the follow-up
            // RemoveUnnecessaryJumps pass clears it, collapsing the two leading instructions.
            var nef = CreateNefFile(new byte[]
            {
                (byte)OpCode.JMP, 0x04,
                (byte)OpCode.PUSH1, (byte)OpCode.RET,
                (byte)OpCode.JMP_L, 0xFE, 0xFF, 0xFF, 0xFF,
                (byte)OpCode.PUSH2, (byte)OpCode.RET
            });

            (nef, _, _) = Strategies.NormalizeJumps(nef, CreateManifest(), null);
            CollectionAssert.AreEqual(
                new[] { OpCode.PUSH1, OpCode.RET, OpCode.JMP, OpCode.PUSH2, OpCode.RET },
                GetOpCodes(nef),
                "outer jump folded onto its own next instruction must be cleared; compressed inner JMP retained");
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

        private static ContractManifest CreateManifest()
        {
            return new ContractManifest
            {
                Name = "test",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods = [],
                    Events = []
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            };
        }

        private static OpCode[] GetOpCodes(NefFile nef)
        {
            return GetInstructions(nef).Select(i => i.OpCode).ToArray();
        }

        private static List<Neo.VM.Instruction> GetInstructions(NefFile nef)
        {
            var result = new List<Neo.VM.Instruction>();
            var script = new Script(nef.Script);
            for (int ip = 0; ip < script.Length;)
            {
                Neo.VM.Instruction instruction = script.GetInstruction(ip);
                result.Add(instruction);
                ip += instruction.Size;
            }
            return result;
        }
    }
}
