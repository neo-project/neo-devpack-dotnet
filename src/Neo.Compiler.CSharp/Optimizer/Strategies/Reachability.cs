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
    static class Reachability
    {
        [Strategy(Priority = int.MaxValue - 16)]
        public static (NefFile, ContractManifest, JObject?) RemoveUncoveredInstructions(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            InstructionCoverage oldContractCoverage = new InstructionCoverage(nef, manifest);
            Dictionary<int, BranchType> coveredMap = oldContractCoverage.coveredMap;
            List<(int, Instruction)> oldAddressAndInstructionsList = oldContractCoverage.addressAndInstructions;
            Dictionary<int, Instruction> oldAddressToInstruction = oldContractCoverage.addressToInstructions;
            //DumpNef.GenerateDumpNef(nef, debugInfo);
            //coveredMap.Where(kv => !kv.Value).Select(kv => (kv.Key, oldAddressToInstruction[kv.Key].OpCode)).ToList();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (coveredMap[a] != BranchType.UNCOVERED && i.OpCode != OpCode.NOP)
                {
                    simplifiedInstructionsToAddress.Add(i, currentAddress);
                    currentAddress += i.Size;
                }
            }
            // retarget all NOP jump targets
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
        /// RET RET -> RET
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        /// <exception cref="BadScriptException"></exception>
        [Strategy(Priority = int.MaxValue - 16)]
        public static (NefFile, ContractManifest, JObject?) RemoveMultiRet(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
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
            int currentAddr = 0;
            Instruction? oldI = null;
            for (int a = 0; a < script.Length;)
            {
                if (!oldAddressToInstruction.TryGetValue(a, out oldI))
                    oldI = Instruction.RET;
                simplifiedInstructionsToAddress.Add(oldI, currentAddr);
                a += oldI.Size;
                if (oldI.OpCode == OpCode.RET)
                    // Ignore all the following RET in old script; re-target jump
                    for (; a < script.Length
                        && oldAddressToInstruction.TryGetValue(a, out Instruction? currentI)
                        && currentI.OpCode == OpCode.RET;
                        a += currentI.Size)
                    {
                        oldSequencePointAddressToNew.Add(a, currentAddr);
                        OptimizedScriptBuilder.RetargetJump(currentI, oldI,
                            jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                    }
                currentAddr += oldI.Size;
            }

            (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(
                nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew);
            return (nef, manifest, debugInfo);
        }
    }
}
