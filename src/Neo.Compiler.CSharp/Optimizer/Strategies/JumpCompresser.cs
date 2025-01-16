// Copyright (C) 2015-2024 The Neo Project.
//
// Reachability.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;

namespace Neo.Optimizer
{
    public static class JumpCompresser
    {
        /// <summary>
        /// A preparation for operations that may increase contract size.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        /// [Strategy(Priority = int.MinValue)]  // No attribute
        public static (NefFile, ContractManifest, JObject?) UncompressJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int newAddr = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (SingleJumpInOperand(i) && i.OpCode != OpCode.CALLA)
                {// Be aware that PUSHA is a jumping instruction without _L version.
                    Instruction target = jumpSourceToTargets[i];
                    int delta = 0;  // do not care about delta in first build
                    OpCode actualOpCode = i.OpCode;
                    if (shortInstructions.Contains(i.OpCode))
                        actualOpCode = i.OpCode + 1;  // change to long version
                    IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                    // No size prefix for instruction. Skipping
                    newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(delta));
                    Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                    simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                    oldSequencePointAddressToNew.Add(a, newAddr);
                    newAddr += newInstruction.Size;

                    jumpSourceToTargets.Remove(i);
                    jumpSourceToTargets.Add(newInstruction, target);
                    jumpTargetToSources[target].Remove(i);
                    jumpTargetToSources[target].Add(newInstruction);
                    OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                        jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                    continue;
                }
                if (DoubleJumpInOperand(i))
                {
                    (Instruction t1, Instruction t2) = trySourceToTargets[i];
                    int delta = 0;  // do not care about delta in first build
                    OpCode actualOpCode = i.OpCode;
                    if (shortInstructions.Contains(i.OpCode))
                        actualOpCode = i.OpCode + 1;  // change to long version
                    IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                    int operandSizeLength = Optimizer.OperandSizePrefixTable[(int)actualOpCode];
                    // No size prefix for instruction. Skipping
                    newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(delta)).Concat(BitConverter.GetBytes(delta));
                    Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                    simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                    oldSequencePointAddressToNew.Add(a, newAddr);
                    newAddr += newInstruction.Size;

                    trySourceToTargets.Remove(i);
                    trySourceToTargets.Add(newInstruction, (t1, t2));
                    jumpTargetToSources[t1].Remove(i);
                    jumpTargetToSources[t1].Add(newInstruction);
                    jumpTargetToSources[t2].Remove(i);
                    jumpTargetToSources[t2].Add(newInstruction);
                    OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                        jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                    continue;
                }
                simplifiedInstructionsToAddress.Add(i, newAddr);
                newAddr += i.Size;
            }
            // Not need to reset the delta. BuildOptimizedAssets does it.
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// Compress _L instructions to short version, if possible
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MinValue)]
        public static (NefFile, ContractManifest, JObject?) CompressJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            bool modified;
            do
            {
                modified = false;
                Script script = nef.Script;
                List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
                Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
                Dictionary<Instruction, int> oldInstructionToAddress = oldAddressAndInstructionsList.ToDictionary(e => e.i, e => e.a);
                (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                    Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                    Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                    FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
                Dictionary<int, int> oldSequencePointAddressToNew = new();

                System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
                int newAddr = 0;
                int addrDrift = 0;
                foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                {
                    if (SingleJumpInOperand(i) && i.OpCode != OpCode.CALLA)
                    {// Be aware that PUSHA is a jumping instruction without _L version.
                        Instruction target = jumpSourceToTargets[i];
                        int delta;
                        if (simplifiedInstructionsToAddress.Contains(target))
                            delta = (int)simplifiedInstructionsToAddress[target]! - newAddr;
                        else
                            delta = oldInstructionToAddress[target] - addrDrift - newAddr;
                        OpCode actualOpCode = i.OpCode;
                        if (longInstructions.Contains(i.OpCode))
                        {
                            if (delta > 0)
                                delta -= 3;
                            if (sbyte.MinValue <= delta && delta <= sbyte.MaxValue)
                            {// can use short version
                                modified = true;
                                addrDrift += 3;
                                actualOpCode = i.OpCode - 1;  // change to short version
                                IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                                // No size prefix for instruction. Skipping
                                newInstructionInBytes = newInstructionInBytes.Append(BitConverter.GetBytes(delta)[0]);
                                Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                                simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                                oldSequencePointAddressToNew.Add(a, newAddr);
                                newAddr += newInstruction.Size;

                                jumpSourceToTargets.Remove(i);
                                jumpSourceToTargets.Add(newInstruction, target);
                                jumpTargetToSources[target].Remove(i);
                                jumpTargetToSources[target].Add(newInstruction);
                                OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                        }
                    }
                    if (DoubleJumpInOperand(i))
                    {
                        (Instruction t1, Instruction t2) = trySourceToTargets[i];
                        int delta1, delta2;
                        if (simplifiedInstructionsToAddress.Contains(t1))
                            delta1 = (int)simplifiedInstructionsToAddress[t1]! - newAddr;
                        else
                            delta1 = oldInstructionToAddress[t1] - addrDrift - newAddr;
                        if (simplifiedInstructionsToAddress.Contains(t2))
                            delta2 = (int)simplifiedInstructionsToAddress[t2]! - newAddr;
                        else
                            delta2 = oldInstructionToAddress[t2] - addrDrift - newAddr;
                        OpCode actualOpCode = i.OpCode;
                        if (longInstructions.Contains(i.OpCode))
                        {
                            if (delta1 > 0)
                                delta1 -= 6;
                            if (delta2 > 0)
                                delta2 -= 6;
                            if (sbyte.MinValue <= delta1 && delta1 <= sbyte.MaxValue
                             && sbyte.MinValue <= delta2 && delta2 <= sbyte.MaxValue)
                            {// can use short version
                                modified = true;
                                addrDrift += 6;
                                actualOpCode = i.OpCode - 1;  // change to short version
                                IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                                int operandSizeLength = Optimizer.OperandSizePrefixTable[(int)actualOpCode];
                                // No size prefix for instruction. Skipping
                                newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(delta1)).Concat(BitConverter.GetBytes(delta2));
                                Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                                simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                                oldSequencePointAddressToNew.Add(a, newAddr);
                                newAddr += newInstruction.Size;

                                trySourceToTargets.Remove(i);
                                trySourceToTargets.Add(newInstruction, (t1, t2));
                                jumpTargetToSources[t1].Remove(i);
                                jumpTargetToSources[t1].Add(newInstruction);
                                jumpTargetToSources[t2].Remove(i);
                                jumpTargetToSources[t2].Add(newInstruction);
                                OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                        }
                    }
                    simplifiedInstructionsToAddress.Add(i, newAddr);
                    newAddr += i.Size;
                }
                // Not need to reset the delta. BuildOptimizedAssets does it.
                if (modified)
                    (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                        simplifiedInstructionsToAddress,
                        jumpSourceToTargets, trySourceToTargets,
                        oldAddressToInstruction,
                        oldSequencePointAddressToNew: oldSequencePointAddressToNew);
            }
            while (modified);
            return (nef, manifest, debugInfo);
        }

        /// <summary>
        /// Removes JMP and JMP_L that targets the next instruction after the JMP or JMP_L.
        /// Replace JMPIF/JMPIFNOT with DROP if it jumps to the next instruction
        /// If the removed JMP or JMP_L itself is a jump target,
        /// re-target to the instruction after the JMP or JMP_L
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue)]
        public static (NefFile, ContractManifest, JObject?) RemoveUnnecessaryJumps(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (unconditionalJump.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (target - a == i.Size)
                    {
                        // Just jumping to the instruction after the jump itself
                        // This is unnecessary jump. The jump should be deleted.
                        // And, if this JMP is the target of other jump instructions,
                        // re-target to the next instruction after this JMP.
                        Instruction nextInstruction = oldAddressToInstruction[a + i.Size];
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[nextInstruction].Remove(i);
                        OptimizedScriptBuilder.RetargetJump(i, nextInstruction, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        continue;  // do not add this JMP into simplified instructions
                    }
                }
                if (i.OpCode == OpCode.JMPIF || i.OpCode == OpCode.JMPIFNOT
                 || i.OpCode == OpCode.JMPIF_L || i.OpCode == OpCode.JMPIFNOT_L)
                {
                    int target = ComputeJumpTarget(a, i);
                    if (target - a == i.Size)
                    {
                        Instruction newDrop = new Script(new byte[] { (byte)OpCode.DROP }).GetInstruction(0);
                        simplifiedInstructionsToAddress.Add(newDrop, currentAddress);
                        oldSequencePointAddressToNew.Add(a, currentAddress);
                        currentAddress += newDrop.Size;

                        Instruction nextInstruction = oldAddressToInstruction[a + i.Size];
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[nextInstruction].Remove(i);
                        OptimizedScriptBuilder.RetargetJump(i, newDrop, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// If a JMP or JMP_L jumps to a RET, replace the JMP with RET
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue - 4)]
        public static (NefFile, ContractManifest, JObject?) ReplaceJumpWithRet(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (unconditionalJump.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (!oldAddressToInstruction.TryGetValue(target, out Instruction? dstRet))
                        throw new BadScriptException($"Bad {nameof(oldAddressToInstruction)}. No target found for {i} jumping from {a} to {target}");
                    if (dstRet.OpCode == OpCode.RET)
                    {
                        oldSequencePointAddressToNew[a] = currentAddress;
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[dstRet].Remove(i);
                        // handle the reference of the added RET
                        Instruction newRet = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);
                        // above is a workaround of new Instruction(OpCode.RET)
                        OptimizedScriptBuilder.RetargetJump(i, newRet,
                            jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        simplifiedInstructionsToAddress.Add(newRet, currentAddress);
                        currentAddress += newRet.Size;
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// If an unconditional jump targets an unconditional jump, re-target the first unconditional jump to its final destination
        /// If a conditional jump targets an unconditional jump, re-target the conditional jump to its final destination
        /// If an unconditional jump targets a conditional jump, DO NOT replace the unconditional jump with a conditional jump to its final destination. THIS IS WRONG.
        /// This should be executed very early, before <see cref="Reachability.RemoveUncoveredInstructions"/>
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue - 8)]
        public static (NefFile, ContractManifest, JObject?) FoldJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            (nef, manifest, debugInfo) = JumpCompresser.UncompressJump(nef, manifest, debugInfo);
            bool modified;
            do
            {
                modified = false;
                Script script = nef.Script;
                List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
                Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
                Dictionary<Instruction, int> oldInstructionToAddress = oldAddressAndInstructionsList.ToDictionary(e => e.i, e => e.a);
                (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                    Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                    Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                    FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
                Dictionary<int, int> oldSequencePointAddressToNew = new();

                System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
                int newAddr = 0;
                foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                {
                    if (shortInstructions.Contains(i.OpCode))
                        throw new BadScriptException($"Long version of OpCodes are required in {nameof(FoldJump)} optimization");
                    if (unconditionalJump.Contains(i.OpCode) || conditionalJump_L.Contains(i.OpCode))
                    {
                        Instruction target = jumpSourceToTargets[i];
                        if (unconditionalJump.Contains(target.OpCode) || unconditionalJump.Contains(target.OpCode))
                        {
                            modified = true;
                            Instruction finalTarget = jumpSourceToTargets[target];
                            // No need to change opcode. Use the old instruction without a new one.
                            // No need to reset operand. BuildOptimizedAssets does it.
                            simplifiedInstructionsToAddress.Add(i, newAddr);
                            oldSequencePointAddressToNew.Add(a, newAddr);
                            newAddr += i.Size;

                            jumpSourceToTargets[i] = finalTarget;
                            jumpTargetToSources[target].Remove(i);
                            jumpTargetToSources[finalTarget].Add(i);
                            continue;
                        }
                    }
                    simplifiedInstructionsToAddress.Add(i, newAddr);
                    newAddr += i.Size;
                }

                (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(
                    nef, manifest, debugInfo,
                    simplifiedInstructionsToAddress,
                    jumpSourceToTargets, trySourceToTargets,
                    oldAddressToInstruction, oldSequencePointAddressToNew);
            }
            while (modified);
            return JumpCompresser.CompressJump(nef, manifest, debugInfo);
        }
    }
}
