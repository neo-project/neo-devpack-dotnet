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
using System.Collections.Generic;
using System.Linq;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;

namespace Neo.Optimizer
{
    static class Reachability
    {
        [Strategy(Priority = int.MaxValue - 16)]
        public static (NefFile, ContractManifest, JObject?) RemoveUncoveredInstructions(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            InstructionCoverage oldContractCoverage = new InstructionCoverage(nef, manifest);
            Dictionary<int, BranchType> coveredMap = oldContractCoverage.coveredMap;
            List<(int, Instruction)> oldAddressAndInstructionsList = oldContractCoverage.addressAndInstructions;
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            //DumpNef.GenerateDumpNef(nef, debugInfo);
            //coveredMap.Where(kv => !kv.Value).Select(kv => (kv.Key, oldAddressToInstruction[kv.Key].OpCode)).ToList();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (coveredMap[a] != BranchType.UNCOVERED)
                {
                    simplifiedInstructionsToAddress.Add(i, currentAddress);
                    currentAddress += i.Size;
                }
            }
            // retarget all NOP targets
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (i.OpCode == OpCode.NOP && oldContractCoverage.jumpTargetToSources.ContainsKey(i))
                {
                    int currentA = a + i.Size;
                    Instruction currentI = oldAddressToInstruction[currentA];
                    while (coveredMap[currentA] == BranchType.UNCOVERED && currentI.OpCode == OpCode.NOP)
                    {
                        currentA += currentI.Size;
                        currentI = oldAddressToInstruction[currentA];
                    }
                    OptimizedScriptBuilder.RetargetJump(i, currentI,
                        oldContractCoverage.jumpInstructionSourceToTargets,
                        oldContractCoverage.tryInstructionSourceToTargets,
                        oldContractCoverage.jumpTargetToSources);
                }
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                oldContractCoverage.jumpInstructionSourceToTargets, oldContractCoverage.tryInstructionSourceToTargets,
                oldAddressToInstruction);
        }

        public static Dictionary<int, BranchType>
            FindCoveredInstructions(NefFile nef, ContractManifest manifest)
            => new InstructionCoverage(nef, manifest).coveredMap;

        /// <summary>
        /// Removes JMP and JMP_L that targets the next instruction after the JMP or JMP_L.
        /// If the JMP or JMP_L itself is a jump target,
        /// re-target to the instruction after the JMP or JMP_L
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue)]
        public static (NefFile, ContractManifest, JObject?) RemoveUnnecessaryJumps(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);

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
                        if (jumpTargetToSources[nextInstruction].Count == 0)
                            jumpTargetToSources.Remove(nextInstruction);
                        OptimizedScriptBuilder.RetargetJump(i, nextInstruction, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        continue;  // do not add this JMP into simplified instructions
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction);
        }

        /// <summary>
        /// If a JMP or JMP_L jumps to a RET, replace the JMP with RET
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue - 4)]
        public static (NefFile, ContractManifest, JObject?) ReplaceJumpWithRet(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
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
                        if (jumpTargetToSources[dstRet].Count == 0)
                            jumpTargetToSources.Remove(dstRet);
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
    }
}
