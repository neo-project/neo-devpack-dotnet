// Copyright (C) 2015-2025 The Neo Project.
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
using System.Linq;

namespace Neo.Compiler.Optimizer
{
    static class BasicOptimizer
    {
        public static void RemoveNops(List<Instruction> instructions)
        {
            for (int i = 0; i < instructions.Count;)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode == OpCode.NOP)
                {
                    instructions.RemoveAt(i);
                    foreach (Instruction other in instructions)
                    {
                        if (other.Target?.Instruction == instruction)
                            other.Target.Instruction = instructions[i];
                        if (other.Target2?.Instruction == instruction)
                            other.Target2.Instruction = instructions[i];
                    }
                }
                else
                {
                    i++;
                }
            }
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

        public static void RemoveRedundantConversions(List<Instruction> instructions)
        {
            for (int i = 1; i < instructions.Count; i++)
            {
                var previous = instructions[i - 1];
                var current = instructions[i];
                if (current.OpCode != OpCode.CONVERT || previous.OpCode != OpCode.CONVERT)
                    continue;

                if (!SameOperand(previous, current))
                    continue;

                ReplaceInstruction(instructions, current, previous);
                instructions.RemoveAt(i);
                i--;
            }
        }

        private static bool SameOperand(Instruction first, Instruction second)
        {
            if (first.Operand is null || second.Operand is null)
                return first.Operand == second.Operand;

            return first.Operand.SequenceEqual(second.Operand);
        }

        private static void ReplaceInstruction(IEnumerable<Instruction> instructions, Instruction removed, Instruction replacement)
        {
            foreach (var other in instructions)
            {
                if (other.Target?.Instruction == removed)
                    other.Target.Instruction = replacement;
                if (other.Target2?.Instruction == removed)
                    other.Target2.Instruction = replacement;
            }
        }
    }
}
