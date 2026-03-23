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
