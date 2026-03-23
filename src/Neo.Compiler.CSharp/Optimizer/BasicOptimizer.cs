// Copyright (C) 2015-2026 The Neo Project.
//
// BasicOptimizer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    static class BasicOptimizer
    {
        public static void RemoveNops(List<Instruction> instructions)
        {
            Dictionary<Instruction, List<JumpTarget>> incomingTargets = CollectIncomingTargets(instructions);
            List<Instruction> retained = new(instructions.Count);
            Instruction? nextRetained = null;

            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                Instruction instruction = instructions[i];

                bool keepInstruction = instruction.OpCode != OpCode.NOP
                    || nextRetained is null && incomingTargets.ContainsKey(instruction);

                if (keepInstruction)
                {
                    retained.Add(instruction);
                    nextRetained = instruction;
                    continue;
                }

                if (!incomingTargets.TryGetValue(instruction, out List<JumpTarget>? targets)) continue;
                foreach (JumpTarget target in targets)
                    target.Instruction = nextRetained;
            }

            retained.Reverse();
            instructions.Clear();
            instructions.AddRange(retained);
        }

        public static void CompressJumps(IReadOnlyList<Instruction> instructions)
        {
            if (instructions.Count == 0)
                return;

            // Basic jump compression is a fixed-point problem: compressing long-form jumps shrinks the
            // script and can make other long-form jumps compressible. We avoid rebuilding offsets after
            // each pass by computing effective offsets relative to the initial (current) layout and a
            // prefix-sum of size reductions from already-compressed instructions.

            Dictionary<Instruction, int> indexByInstruction = new();
            for (int i = 0; i < instructions.Count; i++)
                indexByInstruction[instructions[i]] = i;

            int n = instructions.Count;
            int[] baseOffsets = new int[n];
            for (int i = 0; i < n; i++)
                baseOffsets[i] = instructions[i].Offset;

            bool[] alreadyCompressed = new bool[n];
            int[] sizeReduction = new int[n];
            bool[] isLongCandidate = new bool[n];

            for (int i = 0; i < n; i++)
            {
                Instruction instruction = instructions[i];
                if (instruction.Target is null)
                    continue;

                if (instruction.OpCode >= OpCode.JMP && instruction.OpCode <= OpCode.CALL_L)
                {
                    // Long-form opcodes are the odd values (e.g. JMP_L = JMP + 1).
                    if ((instruction.OpCode - OpCode.JMP) % 2 == 0)
                        continue;

                    isLongCandidate[i] = true;
                    sizeReduction[i] = 3; // 4-byte operand -> 1-byte operand
                    continue;
                }

                if (instruction.OpCode == OpCode.ENDTRY_L)
                {
                    isLongCandidate[i] = true;
                    sizeReduction[i] = 3;
                    continue;
                }

                if (instruction.OpCode == OpCode.TRY_L)
                {
                    if (instruction.Target2 is null)
                        continue;
                    isLongCandidate[i] = true;
                    sizeReduction[i] = 6; // 2x 4-byte operand -> 2x 1-byte operand
                }
            }

            bool changed;
            bool anyCompressed = false;
            int[] prefixReduction = new int[n + 1];
            List<int> toCompress = new();

            do
            {
                changed = false;
                toCompress.Clear();

                prefixReduction[0] = 0;
                for (int i = 0; i < n; i++)
                    prefixReduction[i + 1] = prefixReduction[i] + (alreadyCompressed[i] ? sizeReduction[i] : 0);

                for (int i = 0; i < n; i++)
                {
                    if (!isLongCandidate[i] || alreadyCompressed[i])
                        continue;

                    Instruction instruction = instructions[i];
                    if (instruction.Target?.Instruction is not Instruction targetInstruction)
                        continue;
                    if (!indexByInstruction.TryGetValue(targetInstruction, out int targetIndex))
                        continue;

                    int sourceEffective = baseOffsets[i] - prefixReduction[i];
                    int targetEffective = baseOffsets[targetIndex] - prefixReduction[targetIndex];

                    // Account for the size reduction of this instruction itself, which shifts any
                    // targets that come after it. This mirrors what would happen once we flip the
                    // opcode and rebuild offsets.
                    if (i < targetIndex)
                        targetEffective -= sizeReduction[i];

                    if (instruction.OpCode == OpCode.TRY_L)
                    {
                        if (instruction.Target2?.Instruction is not Instruction target2Instruction)
                            continue;
                        if (!indexByInstruction.TryGetValue(target2Instruction, out int target2Index))
                            continue;

                        int target2Effective = baseOffsets[target2Index] - prefixReduction[target2Index];
                        if (i < target2Index)
                            target2Effective -= sizeReduction[i];

                        int delta1 = targetEffective - sourceEffective;
                        int delta2 = target2Effective - sourceEffective;
                        if (delta1 < sbyte.MinValue || delta1 > sbyte.MaxValue)
                            continue;
                        if (delta2 < sbyte.MinValue || delta2 > sbyte.MaxValue)
                            continue;

                        toCompress.Add(i);
                    }
                    else
                    {
                        int delta = targetEffective - sourceEffective;
                        if (delta < sbyte.MinValue || delta > sbyte.MaxValue)
                            continue;

                        toCompress.Add(i);
                    }
                }

                if (toCompress.Count == 0)
                    break;

                changed = true;
                anyCompressed = true;
                foreach (int index in toCompress)
                {
                    instructions[index].OpCode--;
                    alreadyCompressed[index] = true;
                }
            } while (changed);

            if (anyCompressed)
                instructions.RebuildOffsets();
        }

        private static Dictionary<Instruction, List<JumpTarget>> CollectIncomingTargets(IReadOnlyList<Instruction> instructions)
        {
            Dictionary<Instruction, List<JumpTarget>> incomingTargets = new();
            foreach (Instruction instruction in instructions)
            {
                AddIncomingTarget(instruction.Target);
                AddIncomingTarget(instruction.Target2);
            }

            return incomingTargets;

            void AddIncomingTarget(JumpTarget? target)
            {
                if (target?.Instruction is null) return;
                if (!incomingTargets.TryGetValue(target.Instruction, out List<JumpTarget>? targets))
                {
                    targets = new List<JumpTarget>();
                    incomingTargets[target.Instruction] = targets;
                }

                targets.Add(target);
            }
        }
    }
}
