using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Neo.Optimizer.OpCodeTypes;
using static Neo.VM.OpCode;

namespace Neo.Optimizer
{
    public static class JumpTarget
    {
        public static bool SingleJumpInOperand(Instruction instruction) => SingleJumpInOperand(instruction.OpCode);
        public static bool SingleJumpInOperand(OpCode opcode)
        {
            if (conditionalJump.Contains(opcode)) return true;
            if (conditionalJump_L.Contains(opcode)) return true;
            if (unconditionalJump.Contains(opcode)) return true;
            if (callWithJump.Contains(opcode)) return true;
            if (opcode == ENDTRY || opcode == ENDTRY_L || opcode == PUSHA) return true;
            return false;
        }

        public static bool DoubleJumpInOperand(Instruction instruction) => DoubleJumpInOperand(instruction.OpCode);
        public static bool DoubleJumpInOperand(OpCode opcode) => (opcode == TRY || opcode == TRY_L);

        public static int ComputeJumpTarget(int addr, Instruction instruction)
        {
            if (conditionalJump.Contains(instruction.OpCode))
                return addr + instruction.TokenI8;
            if (conditionalJump_L.Contains(instruction.OpCode))
                return addr + instruction.TokenI32;

            return instruction.OpCode switch
            {
                JMP or CALL or ENDTRY => addr + instruction.TokenI8,
                PUSHA or JMP_L or CALL_L or ENDTRY_L => addr + instruction.TokenI32,
                CALLA => throw new NotImplementedException("CALLA is dynamic; not supported"),
                _ => throw new NotImplementedException($"Unknown instruction {instruction.OpCode}"),
            };
        }

        public static (int catchTarget, int finallyTarget) ComputeTryTarget(int addr, Instruction instruction)
        {
            return instruction.OpCode switch
            {
                TRY =>
                    (instruction.TokenI8 == 0 ? -1 : addr + instruction.TokenI8,
                        instruction.TokenI8_1 == 0 ? -1 : addr + instruction.TokenI8_1),
                TRY_L =>
                    (instruction.TokenI32 == 0 ? -1 : addr + instruction.TokenI32,
                        instruction.TokenI32_1 == 0 ? -1 : addr + instruction.TokenI32_1),
                _ => throw new NotImplementedException($"Unknown instruction {instruction.OpCode}"),
            };
        }

        public static (ConcurrentDictionary<Instruction, Instruction>,
            ConcurrentDictionary<Instruction, (Instruction, Instruction)>)
            FindAllJumpAndTrySourceToTargets(NefFile nef)
        {
            Script script = nef.Script;
            return FindAllJumpAndTrySourceToTargets(script);
        }
        public static (ConcurrentDictionary<Instruction, Instruction>,
            ConcurrentDictionary<Instruction, (Instruction, Instruction)>)
            FindAllJumpAndTrySourceToTargets(Script script) => FindAllJumpAndTrySourceToTargets(script.EnumerateInstructions().ToList());
        public static (ConcurrentDictionary<Instruction, Instruction>,
            ConcurrentDictionary<Instruction, (Instruction, Instruction)>)
            FindAllJumpAndTrySourceToTargets(List<(int, Instruction)> addressAndInstructionsList)
        {
            Dictionary<int, Instruction> addressToInstruction = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
                addressToInstruction.Add(a, i);
            ConcurrentDictionary<Instruction, Instruction> jumpSourceToTargets = new();
            ConcurrentDictionary<Instruction, (Instruction, Instruction)> trySourceToTargets = new();
            Parallel.ForEach(addressAndInstructionsList, item =>
            {
                (int a, Instruction i) = (item.Item1, item.Item2);
                if (SingleJumpInOperand(i))
                    jumpSourceToTargets.TryAdd(i, addressToInstruction[ComputeJumpTarget(a, i)]);
                if (i.OpCode == TRY)
                    trySourceToTargets.TryAdd(i, (addressToInstruction[a + i.TokenI8], addressToInstruction[a + i.TokenI8_1]));
                if (i.OpCode == TRY_L)
                    trySourceToTargets.TryAdd(i, (addressToInstruction[a + i.TokenI32], addressToInstruction[a + i.TokenI32_1]));
            });
            return (jumpSourceToTargets, trySourceToTargets);
        }
    }
}
