using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler
{
    static class Optimizer
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
                        int offset1 = (instruction.Target.Instruction?.Offset - instruction.Offset) ?? 0;
                        int offset2 = (instruction.Target2!.Instruction?.Offset - instruction.Offset) ?? 0;
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
