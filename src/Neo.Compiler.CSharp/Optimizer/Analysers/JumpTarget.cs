using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static Neo.Optimizer.OpCodeTypes;
using static Neo.VM.OpCode;

namespace Neo.Optimizer
{
    static class JumpTarget
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

        public static (Dictionary<Instruction, Instruction>,
            Dictionary<Instruction, (Instruction, Instruction)>,
            Dictionary<Instruction, HashSet<Instruction>>)
            FindAllJumpAndTrySourceToTargets(NefFile nef)
        {
            Script script = nef.Script;
            return FindAllJumpAndTrySourceToTargets(script);
        }
        public static (Dictionary<Instruction, Instruction>,
            Dictionary<Instruction, (Instruction, Instruction)>,
            Dictionary<Instruction, HashSet<Instruction>>)
            FindAllJumpAndTrySourceToTargets(Script script) => FindAllJumpAndTrySourceToTargets(script.EnumerateInstructions().ToList());
        public static (
            Dictionary<Instruction, Instruction>,  // jump source to target
            Dictionary<Instruction, (Instruction, Instruction)>,  // try source to targets
            Dictionary<Instruction, HashSet<Instruction>>  // target to source
            )
            FindAllJumpAndTrySourceToTargets(List<Instruction> instructionsList)
        {
            int addr = 0;
            List<(int, Instruction)> addressAndInstructionsList = new();
            foreach (Instruction i in instructionsList)
            {
                addressAndInstructionsList.Add((addr, i));
                addr += i.Size;
            }
            return FindAllJumpAndTrySourceToTargets(addressAndInstructionsList);
        }
        public static (
            Dictionary<Instruction, Instruction>,  // jump source to target
            Dictionary<Instruction, (Instruction, Instruction)>,  // try source to targets
            Dictionary<Instruction, HashSet<Instruction>>  // target to source
            )
            FindAllJumpAndTrySourceToTargets(List<(int, Instruction)> addressAndInstructionsList)
        {
            Dictionary<int, Instruction> addressToInstruction = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
                addressToInstruction.Add(a, i);
            Dictionary<Instruction, Instruction> jumpSourceToTargets = new();
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets = new();
            Dictionary<Instruction, HashSet<Instruction>> targetToSources = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
            {
                if (SingleJumpInOperand(i))
                {
                    int targetAddr = ComputeJumpTarget(a, i);
                    Instruction target = addressToInstruction[targetAddr];
                    jumpSourceToTargets.TryAdd(i, target);
                    if (!targetToSources.TryGetValue(target, out HashSet<Instruction>? sources))
                    {
                        sources = new();
                        targetToSources.Add(target, sources);
                    }
                    sources.Add(i);
                }
                if (i.OpCode == TRY || i.OpCode == TRY_L)
                {
                    (int a1, int a2) = i.OpCode == TRY ?
                        (a + i.TokenI8, a + i.TokenI8_1) :
                        (a + i.TokenI32, a + i.TokenI32_1);
                    (Instruction t1, Instruction t2) = (addressToInstruction[a1], addressToInstruction[a2]);
                    trySourceToTargets.TryAdd(i, (t1, t2));
                    if (!targetToSources.TryGetValue(t1, out HashSet<Instruction>? sources1))
                    {
                        sources1 = new();
                        targetToSources.Add(t1, sources1);
                    }
                    sources1.Add(i);
                    if (!targetToSources.TryGetValue(t1, out HashSet<Instruction>? sources2))
                    {
                        sources2 = new();
                        targetToSources.Add(t2, sources2);
                    }
                    sources2.Add(i);
                }
            }
            return (jumpSourceToTargets, trySourceToTargets, targetToSources);
        }
    }
}
