// Copyright (C) 2015-2026 The Neo Project.
//
// JumpTarget.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Optimizer;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static Neo.Compiler.ControlFlow.OpCodeTypes;
using static Neo.VM.OpCode;
using VmInstruction = Neo.VM.Instruction;

namespace Neo.Compiler.ControlFlow
{
    static class JumpTarget
    {
        public static bool SingleJumpInOperand(VmInstruction instruction) => SingleJumpInOperand(instruction.OpCode);
        public static bool SingleJumpInOperand(OpCode opcode)
        {
            if (conditionalJump.Contains(opcode)) return true;
            if (conditionalJump_L.Contains(opcode)) return true;
            if (unconditionalJump.Contains(opcode)) return true;
            if (callWithJump.Contains(opcode)) return true;
            if (opcode == ENDTRY || opcode == ENDTRY_L || opcode == PUSHA) return true;
            return false;
        }

        public static bool DoubleJumpInOperand(VmInstruction instruction) => DoubleJumpInOperand(instruction.OpCode);
        public static bool DoubleJumpInOperand(OpCode opcode) => (opcode == TRY || opcode == TRY_L);

        public static int ComputeJumpTarget(int addr, VmInstruction instruction)
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

        public static (int catchTarget, int finallyTarget) ComputeTryTarget(int addr, VmInstruction instruction)
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

        public static (Dictionary<VmInstruction, VmInstruction>,
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)>,
            Dictionary<VmInstruction, HashSet<VmInstruction>>)
            FindAllJumpAndTrySourceToTargets(NefFile nef, bool includePUSHA = true)
        {
            Script script = nef.Script;
            return FindAllJumpAndTrySourceToTargets(script, includePUSHA);
        }
        public static (Dictionary<VmInstruction, VmInstruction>,
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)>,
            Dictionary<VmInstruction, HashSet<VmInstruction>>)
            FindAllJumpAndTrySourceToTargets(Script script, bool includePUSHA = true) => FindAllJumpAndTrySourceToTargets(script.EnumerateInstructions().ToList(), includePUSHA);
        public static (
            Dictionary<VmInstruction, VmInstruction>,  // jump source to target
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)>,  // try source to targets
            Dictionary<VmInstruction, HashSet<VmInstruction>>  // target to source
            )
            FindAllJumpAndTrySourceToTargets(List<VmInstruction> instructionsList, bool includePUSHA = true)
        {
            int addr = 0;
            List<(int, VmInstruction)> addressAndInstructionsList = new();
            foreach (VmInstruction i in instructionsList)
            {
                addressAndInstructionsList.Add((addr, i));
                addr += i.Size;
            }
            return FindAllJumpAndTrySourceToTargets(addressAndInstructionsList, includePUSHA);
        }
        public static (
            Dictionary<VmInstruction, VmInstruction>,  // jump source to target
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)>,  // try source to targets
            Dictionary<VmInstruction, HashSet<VmInstruction>>  // all jump and try targets to sources
            )
            FindAllJumpAndTrySourceToTargets(List<(int, VmInstruction)> addressAndInstructionsList, bool includePUSHA = true)
        {
            Dictionary<int, VmInstruction> addressToInstruction = new();
            foreach ((int a, VmInstruction i) in addressAndInstructionsList)
                addressToInstruction.Add(a, i);
            Dictionary<VmInstruction, VmInstruction> jumpSourceToTargets = new();
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)> trySourceToTargets = new();
            Dictionary<VmInstruction, HashSet<VmInstruction>> targetToSources = new();
            foreach ((int a, VmInstruction i) in addressAndInstructionsList)
            {
                if ((SingleJumpInOperand(i) && i.OpCode != CALLA) || (includePUSHA && i.OpCode == PUSHA))
                {
                    int targetAddr = ComputeJumpTarget(a, i);
                    VmInstruction target = GetTargetInstruction(addressToInstruction, targetAddr, a, i);
                    jumpSourceToTargets[i] = target;
                    if (!targetToSources.TryGetValue(target, out HashSet<VmInstruction>? sources))
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
                    (VmInstruction t1, VmInstruction t2) = (
                        GetTargetInstruction(addressToInstruction, a1, a, i),
                        GetTargetInstruction(addressToInstruction, a2, a, i));
                    trySourceToTargets.TryAdd(i, (t1, t2));
                    if (!targetToSources.TryGetValue(t1, out HashSet<VmInstruction>? sources1))
                    {
                        sources1 = new();
                        targetToSources.Add(t1, sources1);
                    }
                    sources1.Add(i);
                    if (!targetToSources.TryGetValue(t2, out HashSet<VmInstruction>? sources2))
                    {
                        sources2 = new();
                        targetToSources.Add(t2, sources2);
                    }
                    sources2.Add(i);
                }
            }
            return (jumpSourceToTargets, trySourceToTargets, targetToSources);
        }

        private static VmInstruction GetTargetInstruction(
            Dictionary<int, VmInstruction> addressToInstruction,
            int targetAddr,
            int sourceAddr,
            VmInstruction sourceInstruction)
        {
            if (!addressToInstruction.TryGetValue(targetAddr, out VmInstruction? target))
                throw new BadScriptException($"{sourceInstruction.OpCode} at address {sourceAddr} targets invalid address {targetAddr}");
            return target;
        }
    }
}
