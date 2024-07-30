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
using System.Text.RegularExpressions;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;

namespace Neo.Optimizer
{
    static class Reachability
    {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);
        private static readonly Regex SequencePointRegex = new(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)", RegexOptions.Compiled);
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        [Strategy(Priority = int.MaxValue)]
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
        [Strategy(Priority = int.MaxValue - 16)]
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
                        if (jumpTargetToSources.Remove(i, out HashSet<Instruction>? sources))
                        {
                            Instruction nextInstruction = oldAddressToInstruction[a + i.Size];
                            foreach (Instruction s in sources)
                            {
                                if (jumpSourceToTargets.TryGetValue(s, out Instruction? t0) && t0 == i)
                                    jumpSourceToTargets[s] = nextInstruction;
                                if (trySourceToTargets.TryGetValue(s, out (Instruction t1, Instruction t2) t))
                                {
                                    Instruction newT1 = (t.t1 == i ? nextInstruction : t.t1);
                                    Instruction newT2 = (t.t2 == i ? nextInstruction : t.t2);
                                    trySourceToTargets[s] = (newT1, newT2);
                                }
                            }

                            jumpTargetToSources[nextInstruction] = sources;
                        }
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
    }
}
