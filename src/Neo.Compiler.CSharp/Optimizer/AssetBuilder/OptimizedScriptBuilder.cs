// Copyright (C) 2015-2025 The Neo Project.
//
// OptimizedScriptBuilder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Neo.Optimizer.OpCodeTypes;
using static Neo.Optimizer.Optimizer;

namespace Neo.Optimizer
{
    static class OptimizedScriptBuilder
    {
        private static readonly IReadOnlyDictionary<OpCode, (OpCode LongOp, int SizeDelta)> ShortToLongJumpMap =
            new Dictionary<OpCode, (OpCode, int)>
            {
                { OpCode.JMP, (OpCode.JMP_L, 3) },
                { OpCode.JMPIF, (OpCode.JMPIF_L, 3) },
                { OpCode.JMPIFNOT, (OpCode.JMPIFNOT_L, 3) },
                { OpCode.JMPEQ, (OpCode.JMPEQ_L, 3) },
                { OpCode.JMPNE, (OpCode.JMPNE_L, 3) },
                { OpCode.JMPGT, (OpCode.JMPGT_L, 3) },
                { OpCode.JMPGE, (OpCode.JMPGE_L, 3) },
                { OpCode.JMPLT, (OpCode.JMPLT_L, 3) },
                { OpCode.JMPLE, (OpCode.JMPLE_L, 3) },
                { OpCode.CALL, (OpCode.CALL_L, 3) },
                { OpCode.ENDTRY, (OpCode.ENDTRY_L, 3) },
            };

        /// <summary>
        /// Build script with instruction dictionary and jump
        /// </summary>
        /// <param name="simplifiedInstructionsToAddress">new Instruction => int address</param>
        /// <param name="jumpSourceToTargets">All jumping instructions source => target</param>
        /// <param name="trySourceToTargets">All try instructions source => target</param>
        /// <param name="oldAddressToInstruction">Used for convenient debugging only</param>
        /// <returns></returns>
        /// <exception cref="BadScriptException"></exception>
        public static Script BuildScriptWithJumpTargets(
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress,
            Dictionary<Instruction, Instruction> jumpSourceToTargets,
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
            Dictionary<int, Instruction>? oldAddressToInstruction = null)
        {
            List<byte> simplifiedScript = new();
            int instructionIndex = 0;
            foreach (DictionaryEntry item in simplifiedInstructionsToAddress)
            {
                (Instruction i, int a) = ((Instruction)item.Key, (int)item.Value!);
                bool operandEmitted = false;
                OpCode opcodeToEmit = i.OpCode;
                bool upgradedToLong = false;
                int delta = 0;
                jumpSourceToTargets.TryGetValue(i, out Instruction? maybeTarget);
                bool hasJumpTarget = maybeTarget is not null;
                if (hasJumpTarget)
                {
                    Instruction dst = maybeTarget!;
                    if (simplifiedInstructionsToAddress.Contains(dst))
                    {
                        delta = (int)simplifiedInstructionsToAddress[dst]! - a;
                    }
                    else if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.ENDTRY || i.OpCode == OpCode.ENDTRY_L)
                    {
                        delta = 0;  // TODO: decide a good target
                    }
                    else
                    {
                        if (oldAddressToInstruction != null)
                            foreach ((int oldAddress, Instruction oldInstruction) in oldAddressToInstruction)
                                if (oldInstruction == i)
                                    throw new BadScriptException($"Target instruction of {i.OpCode} at old address {oldAddress} is deleted");
                        throw new BadScriptException($"Target instruction of {i.OpCode} at new address {a} is deleted");
                    }

                    if (ShortToLongJumpMap.TryGetValue(opcodeToEmit, out (OpCode LongOp, int SizeDelta) map) && (delta < sbyte.MinValue || delta > sbyte.MaxValue))
                    {
                        opcodeToEmit = map.LongOp;
                        upgradedToLong = true;
                        AdjustInstructionAddresses(simplifiedInstructionsToAddress, instructionIndex + 1, map.SizeDelta);
                        if (simplifiedInstructionsToAddress.Contains(dst))
                            delta = (int)simplifiedInstructionsToAddress[dst]! - a;
                    }
                }

                simplifiedScript.Add((byte)opcodeToEmit);
                int operandSizeLength = OperandSizePrefixTable[(int)opcodeToEmit];
                simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(i.Operand.Length)[0..operandSizeLength]).ToList();
                if (hasJumpTarget)
                {
                    if (opcodeToEmit == OpCode.JMP || conditionalJump.Contains(opcodeToEmit) || opcodeToEmit == OpCode.CALL || opcodeToEmit == OpCode.ENDTRY)
                    {
                        simplifiedScript.Add(BitConverter.GetBytes(delta)[0]);
                        operandEmitted = true;
                    }
                    else if (opcodeToEmit == OpCode.PUSHA || opcodeToEmit == OpCode.JMP_L || conditionalJump_L.Contains(opcodeToEmit) || opcodeToEmit == OpCode.CALL_L || opcodeToEmit == OpCode.ENDTRY_L)
                    {
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta)).ToList();
                        operandEmitted = true;
                    }
                    else if (upgradedToLong)
                    {
                        throw new NotImplementedException($"Long form emission missing handler for {opcodeToEmit}.");
                    }
                }
                if (trySourceToTargets.TryGetValue(i, out (Instruction dst1, Instruction dst2) dsts))
                {
                    (Instruction dst1, Instruction dst2) = (dsts.dst1, dsts.dst2);
                    (int delta1, int delta2) = ((int)simplifiedInstructionsToAddress[dst1]! - a, (int)simplifiedInstructionsToAddress[dst2]! - a);
                    if (i.OpCode == OpCode.TRY)
                    {
                        simplifiedScript.Add(BitConverter.GetBytes(delta1)[0]);
                        simplifiedScript.Add(BitConverter.GetBytes(delta2)[0]);
                    }
                    if (i.OpCode == OpCode.TRY_L)
                    {
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta1)).ToList();
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta2)).ToList();
                    }
                    operandEmitted = true;
                }
                if (!operandEmitted && i.Operand.Length != 0)
                    simplifiedScript = simplifiedScript.Concat(i.Operand.ToArray()).ToList();
                instructionIndex++;
            }
            Script script = new(simplifiedScript.ToArray());
            return script;
        }

        private static void AdjustInstructionAddresses(System.Collections.Specialized.OrderedDictionary instructionsToAddress, int startIndex, int sizeDelta)
        {
            if (sizeDelta == 0) return;
            for (int idx = startIndex; idx < instructionsToAddress.Count; idx++)
            {
                int current = (int)instructionsToAddress[idx]!;
                instructionsToAddress[idx] = current + sizeDelta;
            }
        }

        /// <summary>
        /// Typically used when you delete the oldTarget from script
        /// and the newTarget is the first following instruction undeleted in script
        /// </summary>
        /// <param name="oldTarget">target instruction in the old script</param>
        /// <param name="newTarget">target instruction in the new script</param>
        /// <param name="jumpSourceToTargets">All jumping instructions source => target</param>
        /// <param name="trySourceToTargets">All try instructions source => target</param>
        /// <param name="jumpTargetToSources">All target instructions that are targets of jumps and trys</param>
        public static void RetargetJump(Instruction oldTarget, Instruction newTarget,
            Dictionary<Instruction, Instruction> jumpSourceToTargets,
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
            Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources)
        {
            if (jumpTargetToSources.Remove(oldTarget, out HashSet<Instruction>? sources) && sources.Count > 0)
            {
                foreach (Instruction s in sources)
                {
                    if (jumpSourceToTargets.TryGetValue(s, out Instruction? t0) && t0 == oldTarget)
                        jumpSourceToTargets[s] = newTarget;
                    if (trySourceToTargets.TryGetValue(s, out (Instruction t1, Instruction t2) t))
                    {
                        Instruction newT1 = (t.t1 == oldTarget ? newTarget : t.t1);
                        Instruction newT2 = (t.t2 == oldTarget ? newTarget : t.t2);
                        trySourceToTargets[s] = (newT1, newT2);
                    }
                }
                if (jumpTargetToSources.TryGetValue(newTarget, out HashSet<Instruction>? newTargetSources))
                    sources = newTargetSources.Union(sources).ToHashSet();
                jumpTargetToSources[newTarget] = sources;
            }
        }
    }
}
