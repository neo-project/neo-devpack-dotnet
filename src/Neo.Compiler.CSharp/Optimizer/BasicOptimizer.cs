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
            // Keep the algorithm linear by:
            // 1) Precomputing the "next live instruction" for each NOP
            // 2) Retargeting all branches once
            // 3) Compacting the list in a single pass

            // Collect all targeted instructions so we can safely keep terminal NOPs
            // that have no live instruction after them.
            HashSet<Instruction> targeted = new();
            foreach (Instruction instruction in instructions)
            {
                if (instruction.Target?.Instruction is Instruction target)
                    targeted.Add(target);
                if (instruction.Target2?.Instruction is Instruction target2)
                    targeted.Add(target2);
            }

            // Map NOP instructions to the next non-NOP instruction (or themselves if terminal).
            Dictionary<Instruction, Instruction> nopReplacement = new();
            Instruction? nextLive = null;
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode != OpCode.NOP)
                {
                    nextLive = instruction;
                    continue;
                }

                // If there is no live instruction after this NOP and it is targeted, keep it.
                // Otherwise, retarget it to the next live instruction when possible.
                nopReplacement[instruction] = nextLive ?? instruction;
            }

            // Retarget all branch operands that point to NOP instructions.
            foreach (Instruction instruction in instructions)
            {
                if (instruction.Target?.Instruction is Instruction target && target.OpCode == OpCode.NOP)
                    instruction.Target.Instruction = nopReplacement[target];
                if (instruction.Target2?.Instruction is Instruction target2 && target2.OpCode == OpCode.NOP)
                    instruction.Target2.Instruction = nopReplacement[target2];
            }

            // Compact in-place: remove NOPs that can be removed, but keep terminal targeted NOPs.
            int originalCount = instructions.Count;
            int write = 0;
            for (int read = 0; read < originalCount; read++)
            {
                Instruction instruction = instructions[read];
                if (instruction.OpCode != OpCode.NOP)
                {
                    instructions[write++] = instruction;
                    continue;
                }

                // Keep only if there's no replacement beyond itself and it is targeted.
                if (targeted.Contains(instruction) && nopReplacement[instruction] == instruction)
                    instructions[write++] = instruction;
            }

            if (write < originalCount)
                instructions.RemoveRange(write, originalCount - write);
        }

        public static void CompressJumps(IReadOnlyList<Instruction> instructions)
        {
            bool compressed;
            do
            {
                compressed = false;
                foreach (Instruction instruction in instructions)
                {
                    if (instruction.Target is null) continue;
                    if (instruction.OpCode >= OpCode.JMP && instruction.OpCode <= OpCode.CALL_L)
                    {
                        if ((instruction.OpCode - OpCode.JMP) % 2 == 0) continue;
                    }
                    else
                    {
                        if (instruction.OpCode != OpCode.TRY_L && instruction.OpCode != OpCode.ENDTRY_L) continue;
                    }
                    if (instruction.OpCode == OpCode.TRY_L)
                    {
                        int offset1 = instruction.Target.Instruction?.Offset - instruction.Offset ?? 0;
                        int offset2 = instruction.Target2!.Instruction?.Offset - instruction.Offset ?? 0;
                        if (offset1 >= sbyte.MinValue && offset1 <= sbyte.MaxValue && offset2 >= sbyte.MinValue && offset2 <= sbyte.MaxValue)
                        {
                            compressed = true;
                            instruction.OpCode--;
                        }
                    }
                    else
                    {
                        int offset = instruction.Target.Instruction!.Offset - instruction.Offset;
                        if (offset >= sbyte.MinValue && offset <= sbyte.MaxValue)
                        {
                            compressed = true;
                            instruction.OpCode--;
                        }
                    }
                }
                if (compressed) instructions.RebuildOffsets();
            } while (compressed);
        }
    }
}
