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

namespace Neo.Optimizer
{
    public static class Peephole
    {
        public static HashSet<OpCode> RemoveDupDropOpCodes = new() { OpCode.REVERSEITEMS, OpCode.CLEARITEMS, OpCode.DUP, OpCode.DROP, OpCode.ABORTMSG };
        public static HashSet<OpCode> JmpWithNotOpCodes = new() { OpCode.JMPIF, OpCode.JMPIFNOT, OpCode.JMPIF_L, OpCode.JMPIFNOT_L };

        /// <summary>
        /// DUP SOMEOP DROP
        /// delete DUP and DROP when they are meaningless, optimizing to SOMPOP only
        /// This is mainly used for simple assignments like `a=1`, which is compiled to
        /// PUSH1 DUP STLOC:$index_of_a DROP
        /// This is correct compilation, because the expression `a=1` has return value 1
        /// The return value of assignment expression is used in continuous assignments like `a=b=1`
        /// But at runtime we just need PUSH1 STLOC:$index_of_a
        /// TODO in the future: use symbolic VM to judge multiple instructions between DUP and DROP
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = 1 << 10)]
        public static (NefFile, ContractManifest, JObject?) RemoveDupDrop(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldContractCoverage.addressAndInstructions)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach (List<Instruction> basicBlock in contractInBasicBlocks.sortedListInstructions.Select(i => i.block))
            {
                for (int index = 0; index < basicBlock.Count; index++)
                {
                    if (index + 2 < basicBlock.Count
                     && basicBlock[index].OpCode == OpCode.DUP
                     && basicBlock[index + 2].OpCode == OpCode.DROP)
                    {
                        Instruction currentDup = basicBlock[index];
                        Instruction nextInstruction = basicBlock[index + 1];
                        OpCode opAfterDup = nextInstruction.OpCode;
                        if (OpCodeTypes.storeArguments.Contains(opAfterDup)
                         || OpCodeTypes.storeStaticFields.Contains(opAfterDup)
                         || OpCodeTypes.storeLocalVariables.Contains(opAfterDup)
                         || RemoveDupDropOpCodes.Contains(opAfterDup))
                        {
                            // Include only the instruction between DUP and DROP
                            simplifiedInstructionsToAddress.Add(nextInstruction, currentAddress);
                            currentAddress += nextInstruction.Size;
                            index += 2;

                            // If the old DUP is target of jump, re-target to the next instruction
                            OptimizedScriptBuilder.RetargetJump(currentDup, nextInstruction,
                                jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                            continue;
                        }
                    }
                    simplifiedInstructionsToAddress.Add(basicBlock[index], currentAddress);
                    currentAddress += basicBlock[index].Size;
                }
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction);
        }

        /// <summary>
        /// PUSHNULL EQUAL -> ISNULL
        /// PUSHNULL NOTEQUAL -> ISNULL NOT
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = 1 << 9)]
        public static (NefFile, ContractManifest, JObject?) UseIsNull(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldContractCoverage.addressAndInstructions)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            Dictionary<int, int> oldSequencePointAddressToNew = new();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int oldStartAddr, List<Instruction> basicBlock) in contractInBasicBlocks.sortedListInstructions)
            {
                int oldAddr = oldStartAddr;
                for (int index = 0; index < basicBlock.Count; index++)
                {
                    if (index + 1 < basicBlock.Count)
                    {
                        Instruction current = basicBlock[index];
                        Instruction next = basicBlock[index + 1];
                        if (current.OpCode == OpCode.PUSHNULL)
                        {
                            if (next.OpCode == OpCode.EQUAL)
                            {
                                Instruction isNull = new Script(new byte[] { (byte)OpCode.ISNULL }).GetInstruction(0);
                                simplifiedInstructionsToAddress.Add(isNull, currentAddress);
                                oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                                oldAddr += current.Size;
                                oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                                oldAddr += next.Size;
                                currentAddress += isNull.Size;
                                index += 1;
                                OptimizedScriptBuilder.RetargetJump(current, isNull,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                            if (next.OpCode == OpCode.NOTEQUAL)
                            {
                                Script script = new Script(new byte[] { (byte)OpCode.ISNULL, (byte)OpCode.NOT });
                                Instruction isNull = script.GetInstruction(0);
                                simplifiedInstructionsToAddress.Add(isNull, currentAddress);
                                oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                                oldAddr += current.Size;
                                currentAddress += isNull.Size;
                                Instruction not_ = script.GetInstruction(isNull.Size);
                                simplifiedInstructionsToAddress.Add(not_, currentAddress);
                                oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                                oldAddr += next.Size;
                                currentAddress += not_.Size;
                                index += 1;
                                OptimizedScriptBuilder.RetargetJump(current, isNull,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                        }
                    }
                    simplifiedInstructionsToAddress.Add(basicBlock[index], currentAddress);
                    currentAddress += basicBlock[index].Size;
                }
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// NOT JMPIFNOT -> JMPIF
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = (1 << 9) - 1)]
        public static (NefFile, ContractManifest, JObject?) FoldNotInJmp(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldContractCoverage.addressAndInstructions)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            Dictionary<int, int> oldSequencePointAddressToNew = new();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int oldStartAddr, List<Instruction> basicBlock) in contractInBasicBlocks.sortedListInstructions)
            {
                int oldAddr = oldStartAddr;
                for (int index = 0; index < basicBlock.Count; index++)
                {
                    if (index + 1 < basicBlock.Count)
                    {
                        Instruction current = basicBlock[index];
                        Instruction next = basicBlock[index + 1];
                        if (current.OpCode == OpCode.NOT)
                        {
                            if (JmpWithNotOpCodes.Contains(next.OpCode))
                            {
                                OpCode newJmpOpCode = next.OpCode switch
                                {
                                    OpCode.JMPIF => OpCode.JMPIFNOT,
                                    OpCode.JMPIFNOT => OpCode.JMPIF,
                                    OpCode.JMPIF_L => OpCode.JMPIFNOT_L,
                                    OpCode.JMPIFNOT_L => OpCode.JMPIF_L,
                                    _ => throw new BadScriptException($"Bad definition in optimizer: {next.OpCode} in {nameof(JmpWithNotOpCodes)}. This is a bug from author.")
                                };
                                IEnumerable<byte> newJmpInBytes = [(byte)newJmpOpCode];
                                int operandSize = Optimizer.OperandSizeTable[(int)newJmpOpCode];
                                newJmpInBytes = newJmpInBytes.Concat(Enumerable.Repeat<byte>(0x00, operandSize));
                                // No need to handle offset. OptimizedScriptBuilder.RetargetJump handles it.
                                Instruction newJmp = new Script(newJmpInBytes.ToArray()).GetInstruction(0);
                                simplifiedInstructionsToAddress.Add(newJmp, currentAddress);
                                oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                                oldAddr += current.Size;
                                oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                                oldAddr += next.Size;
                                currentAddress += newJmp.Size;
                                index += 1;

                                jumpSourceToTargets[newJmp] = jumpSourceToTargets[next];
                                jumpTargetToSources[jumpSourceToTargets[newJmp]].Add(newJmp);
                                OptimizedScriptBuilder.RetargetJump(current, newJmp,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                        }
                    }
                    simplifiedInstructionsToAddress.Add(basicBlock[index], currentAddress);
                    currentAddress += basicBlock[index].Size;
                }
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }
    }
}
