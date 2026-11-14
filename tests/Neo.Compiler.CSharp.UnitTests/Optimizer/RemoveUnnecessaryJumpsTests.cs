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
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

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
