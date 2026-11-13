// Copyright (C) 2015-2026 The Neo Project.
//
// NopBufferPeephole.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using static Neo.Compiler.ControlFlow.JumpTarget;
using static Neo.Compiler.ControlFlow.OpCodeTypes;

namespace Neo.Optimizer
{
    /// <summary>
    /// Applies local peephole rewrites without moving any instruction.
    /// Bytes freed by a shorter replacement are filled with NOP and removed
    /// together with unreachable code by the final compaction pass.
    /// </summary>
    internal static class Peephole
    {
        public static NefFile Rewrite(NefFile nef, ContractManifest manifest, PeepholeRule rules)
        {
            if (rules == PeepholeRule.None) return nef;

            Script parsedScript = new(nef.Script.ToArray());
            var instructions = parsedScript.EnumerateInstructions().ToList();
            var addressByInstruction = instructions.ToDictionary(static item => item.instruction, static item => item.address);
            var (jumpTargets, tryTargets, incomingTargets) = FindAllJumpAndTrySourceToTargets(instructions);

            HashSet<int> starts = [0];
            foreach (Instruction target in jumpTargets.Values)
                starts.Add(addressByInstruction[target]);
            foreach ((Instruction target1, Instruction target2) in tryTargets.Values)
            {
                starts.Add(addressByInstruction[target1]);
                starts.Add(addressByInstruction[target2]);
            }
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                starts.Add(method.Offset);

            List<List<Instruction>> blocks = [];
            List<Instruction> current = [];
            foreach ((int address, Instruction instruction) in instructions)
            {
                if (current.Count > 0 && (starts.Contains(address) || allowedBasicBlockEnds.Contains(current[^1].OpCode)))
                {
                    blocks.Add(current);
                    current = [];
                }
                current.Add(instruction);
                if (allowedBasicBlockEnds.Contains(instruction.OpCode))
                {
                    blocks.Add(current);
                    current = [];
                }
            }

            if (current.Count > 0)
                blocks.Add(current);

            byte[] output = nef.Script.ToArray();
            foreach (List<Instruction> block in blocks)
            {
                int blockAddress = instructions.First(item => item.instruction == block[0]).address;
                RewriteBlock(block, blockAddress, output, rules);
            }

            nef.Script = output;
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            return nef;
        }

        private static void RewriteBlock(List<Instruction> block, int startAddress, byte[] output, PeepholeRule rules)
        {
            int[] addresses = new int[block.Count];
            var opcodes = block.Select(static instruction => instruction.OpCode).ToArray();
            int address = startAddress;
            for (int index = 0; index < block.Count; index++)
            {
                addresses[index] = address;
                address += block[index].Size;
            }

            for (int index = 0; index < block.Count; index++)
            {
                Instruction current = block[index];
                void SetOpcode(int targetIndex, OpCode opcode)
                {
                    ReplaceOpcode(output, addresses[targetIndex], opcode);
                    opcodes[targetIndex] = opcode;
                }

                // PUSH0|PUSHINT(0), NUMEQUAL, NOT -> NZ, NOP, NOP
                if (index + 2 < block.Count && rules.HasFlag(PeepholeRule.FoldNotInEqual)
                    && IsIntegerPush(current, 0) && opcodes[index + 1] == OpCode.NUMEQUAL && opcodes[index + 2] == OpCode.NOT)
                {
                    SetOpcode(index, OpCode.NZ);
                    NopRemainder(output, addresses[index], current.Size - 1);
                    SetOpcode(index + 1, OpCode.NOP);
                    SetOpcode(index + 2, OpCode.NOP);
                    index += 2;
                    continue;
                }

                // PUSHNULL, NOTEQUAL, NOT -> ISNULL, NOP, NOP
                if (index + 2 < block.Count && rules.HasFlag(PeepholeRule.FoldNotInEqual)
                    && opcodes[index] == OpCode.PUSHNULL && opcodes[index + 1] == OpCode.NOTEQUAL && opcodes[index + 2] == OpCode.NOT)
                {
                    SetOpcode(index, OpCode.ISNULL);
                    SetOpcode(index + 1, OpCode.NOP);
                    SetOpcode(index + 2, OpCode.NOP);
                    index += 2;
                    continue;
                }

                // DUP, <side-effect/store>, DROP -> NOP, <side-effect/store>, NOP
                if (index + 2 < block.Count && rules.HasFlag(PeepholeRule.RemoveDupDrop)
                 && opcodes[index] == OpCode.DUP && IsDupDropMiddle(opcodes[index + 1]) && opcodes[index + 2] == OpCode.DROP)
                {
                    SetOpcode(index, OpCode.NOP);
                    SetOpcode(index + 2, OpCode.NOP);
                    index += 2;
                    continue;
                }

                // PUSH1|PUSHINT(1), ADD -> INC, NOP
                // PUSH1|PUSHINT(1), SUB -> DEC, NOP
                if (index + 1 < block.Count && rules.HasFlag(PeepholeRule.UseIncDec)
                    && IsIntegerPush(current, 1) && opcodes[index + 1] is OpCode.ADD or OpCode.SUB)
                {
                    SetOpcode(index, opcodes[index + 1] == OpCode.ADD ? OpCode.INC : OpCode.DEC);
                    NopRemainder(output, addresses[index], current.Size - 1);
                    SetOpcode(index + 1, OpCode.NOP);
                    index++;
                    continue;
                }

                // PUSHNULL, EQUAL    -> ISNULL, NOP
                // PUSHNULL, NOTEQUAL -> ISNULL, NOT
                if (index + 1 < block.Count && rules.HasFlag(PeepholeRule.UseIsNull)
                    && opcodes[index] == OpCode.PUSHNULL && opcodes[index + 1] is OpCode.EQUAL or OpCode.NOTEQUAL)
                {
                    SetOpcode(index, OpCode.ISNULL);
                    SetOpcode(index + 1, opcodes[index + 1] == OpCode.NOTEQUAL ? OpCode.NOT : OpCode.NOP);
                    continue;
                }

                // PUSH0|PUSHINT(0), NUMEQUAL    -> NZ, NOT
                // PUSH0|PUSHINT(0), NUMNOTEQUAL -> NZ, NOP
                if (index + 1 < block.Count && rules.HasFlag(PeepholeRule.UseNz)
                    && IsIntegerPush(current, 0) && opcodes[index + 1] is OpCode.NUMEQUAL or OpCode.NUMNOTEQUAL)
                {
                    OpCode comparison = opcodes[index + 1];
                    SetOpcode(index, OpCode.NZ);
                    NopRemainder(output, addresses[index], current.Size - 1);
                    SetOpcode(index + 1, comparison == OpCode.NUMEQUAL ? OpCode.NOT : OpCode.NOP);
                    continue;
                }

                // EQUAL, NOT      -> NOTEQUAL, NOP
                // NOTEQUAL, NOT   -> EQUAL, NOP
                // NUMEQUAL, NOT   -> NUMNOTEQUAL, NOP
                // NUMNOTEQUAL, NOT -> NUMEQUAL, NOP
                if (index + 1 < block.Count && rules.HasFlag(PeepholeRule.FoldNotInEqual)
                    && opcodes[index + 1] == OpCode.NOT && opcodes[index] is OpCode.EQUAL or OpCode.NOTEQUAL or OpCode.NUMEQUAL or OpCode.NUMNOTEQUAL)
                {
                    SetOpcode(index, opcodes[index] switch
                    {
                        OpCode.EQUAL => OpCode.NOTEQUAL,
                        OpCode.NOTEQUAL => OpCode.EQUAL,
                        OpCode.NUMEQUAL => OpCode.NUMNOTEQUAL,
                        OpCode.NUMNOTEQUAL => OpCode.NUMEQUAL,
                        _ => throw new BadScriptException($"Unexpected equality opcode {opcodes[index]}"),
                    });
                    SetOpcode(index + 1, OpCode.NOP);
                    index++;
                    continue;
                }

                // NOT, JMPIF      -> NOP, JMPIFNOT
                // NOT, JMPIFNOT   -> NOP, JMPIF
                // NOT, JMPIF_L    -> NOP, JMPIFNOT_L
                // NOT, JMPIFNOT_L -> NOP, JMPIF_L
                if (index + 1 < block.Count && rules.HasFlag(PeepholeRule.FoldNotInJmp)
                    && opcodes[index] == OpCode.NOT && opcodes[index + 1] is OpCode.JMPIF or OpCode.JMPIFNOT or OpCode.JMPIF_L or OpCode.JMPIFNOT_L)
                {
                    SetOpcode(index, OpCode.NOP);
                    SetOpcode(index + 1, opcodes[index + 1] switch
                    {
                        OpCode.JMPIF => OpCode.JMPIFNOT,
                        OpCode.JMPIFNOT => OpCode.JMPIF,
                        OpCode.JMPIF_L => OpCode.JMPIFNOT_L,
                        OpCode.JMPIFNOT_L => OpCode.JMPIF_L,
                        _ => throw new BadScriptException($"Unexpected conditional jump {opcodes[index + 1]}"),
                    });
                    index++;
                }
            }
        }

        private static void ReplaceOpcode(byte[] script, int address, OpCode opcode)
        {
            script[address] = (byte)opcode;
        }

        /// <summary>
        /// Fills bytes freed by a shorter replacement with NOP. The final
        /// compaction pass removes these placeholders and rebuilds jump offsets.
        /// </summary>
        private static void NopRemainder(byte[] script, int address, int count)
        {
            for (int index = 1; index <= count; index++)
                script[address + index] = (byte)OpCode.NOP;
        }

        private static bool IsIntegerPush(Instruction instruction, int value)
        {
            if (instruction.OpCode is OpCode.PUSH0 or OpCode.PUSH1)
                return value == (instruction.OpCode == OpCode.PUSH1 ? 1 : 0);
            return pushInt.Contains(instruction.OpCode) && new BigInteger(instruction.Operand.Span) == value;
        }

        private static bool IsDupDropMiddle(OpCode opcode)
        {
            return storeArguments.Contains(opcode)
                || storeStaticFields.Contains(opcode)
                || storeLocalVariables.Contains(opcode)
                || opcode is OpCode.REVERSEITEMS or OpCode.CLEARITEMS or OpCode.DUP or OpCode.DROP or OpCode.ABORTMSG;
        }
    }
}
