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
    }
}
