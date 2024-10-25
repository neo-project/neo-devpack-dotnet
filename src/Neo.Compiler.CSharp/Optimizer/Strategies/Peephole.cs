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

namespace Neo.Optimizer
{
    public static class Peephole
    {
        public static HashSet<OpCode> RemoveDupDropOpCodes = new() { OpCode.REVERSEITEMS, OpCode.CLEARITEMS, OpCode.DUP, OpCode.DROP, OpCode.ABORTMSG };

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
            Dictionary<int, Instruction> oldAddressToInstruction = oldContractCoverage.addressToInstructions;
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
        /// Delete unnecessary STSFLD in _initialize method
        /// and replace LDSFLD with PUSH const.
        /// This may increase contract size.
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = 1 << 5)]
        public static (NefFile, ContractManifest, JObject?) InitStaticToConst(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            (nef, manifest, debugInfo) = JumpCompresser.UncompressJump(nef, manifest, debugInfo);

            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, Instruction> oldAddressToInstruction = oldContractCoverage.addressToInstructions;
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            int[] inits = oldContractCoverage.entryPointsByMethod
                .Where(kv => kv.Value == EntryType.Initialize)
                .Select(kv => kv.Key).ToArray();
            if (inits.Length > 1)
                throw new BadScriptException($"{inits.Length} _initialize methods in contract {manifest.Name}");
            if (inits.Length == 0)  // no _initialize method; do nothing
                return (nef, manifest, debugInfo);
            HashSet<BasicBlock> initBlocks = contractInBasicBlocks.BlocksCoveredFromAddr(inits[0], true);
            HashSet<int> initAddrs = ContractInBasicBlocks.AddrCoveredByBlocks(initBlocks);

            // key: static field index
            // value: instruction that sets const value for the field
            // If we decide not to handle a field, store RET in value
            // We remove PUSH and STSFLD in _initialize if the field is written with const only once and never read in _initialize.
            // We replace LDSFLD with PUSH in an ordinary method if the field is written with const only once in _initialize
            // and never written in the method.
            Dictionary<byte, Instruction> writtenConstInInit = [];
            // all instructions before STSFLD that push const
            HashSet<Instruction> pushConstStaticInInit = [];
            HashSet<byte> readInInit = [];

            foreach (BasicBlock currentBlock in initBlocks)
            {
                Instruction? prevPushConst = null;
                foreach (Instruction i in currentBlock.instructions)
                {
                    if (OpCodeTypes.storeStaticFields.Contains(i.OpCode))
                    {
                        byte staticFieldIndex = OpCodeTypes.SlotIndex(i);
                        if (prevPushConst == null)
                            // No const pushed from prev instruction. Give up.
                            writtenConstInInit[staticFieldIndex] = Instruction.RET;
                        else if (!writtenConstInInit.TryAdd(staticFieldIndex, prevPushConst))
                            // multiple STSFLD to the same index
                            // do not handle this staticfield.
                            writtenConstInInit[staticFieldIndex] = Instruction.RET;
                        else
                            pushConstStaticInInit.Add(prevPushConst);
                        prevPushConst = null;
                    }
                    if (OpCodeTypes.loadStaticFields.Contains(i.OpCode))
                        readInInit.Add(OpCodeTypes.SlotIndex(i));
                    if (OpCodeTypes.pushConst.Contains(i.OpCode))
                        prevPushConst = i;
                    else
                        prevPushConst = null;
                }
            }

            // do not replace these LDSFLD with PUSH
            // because they are written in ordinary methods
            HashSet<int> doNotReplaceLdsfld = [];
            HashSet<byte> doNotDeleteStsfld = [];
            foreach ((int entry, EntryType _) in oldContractCoverage.entryPointsByMethod
                .Where(kv => kv.Value != EntryType.Initialize))
            {
                HashSet<BasicBlock> blocks = contractInBasicBlocks.BlocksCoveredFromAddr(entry, true);
                HashSet<int> addrs = ContractInBasicBlocks.AddrCoveredByBlocks(blocks);
                HashSet<byte> stsfldInOrdinaryMethod = [];
                Dictionary<byte, HashSet<int>> ldsfldInOrdinaryMethod = [];
                foreach (int a in addrs)
                {
                    Instruction i = oldAddressToInstruction[a];
                    if (OpCodeTypes.storeStaticFields.Contains(i.OpCode))
                        stsfldInOrdinaryMethod.Add(OpCodeTypes.SlotIndex(i));
                    if (OpCodeTypes.loadStaticFields.Contains(i.OpCode))
                    {
                        byte staticFieldIndex = OpCodeTypes.SlotIndex(i);
                        if (!ldsfldInOrdinaryMethod.TryGetValue(staticFieldIndex, out HashSet<int>? ldAddrs))
                        {
                            ldAddrs = new HashSet<int>();
                            ldsfldInOrdinaryMethod.Add(staticFieldIndex, ldAddrs);
                        }
                        ldAddrs.Add(a);
                    }
                }
                foreach (byte staticFieldIndex in stsfldInOrdinaryMethod)
                    if (ldsfldInOrdinaryMethod.TryGetValue(staticFieldIndex, out HashSet<int>? ldAddrs))
                        doNotReplaceLdsfld = doNotReplaceLdsfld.Union(ldAddrs).ToHashSet();
                doNotDeleteStsfld = doNotDeleteStsfld.Union(stsfldInOrdinaryMethod).ToHashSet();
            }

            // Start building new script
            int currentAddr = 0;
            Instruction? oldI = null;
            for (int a = 0; a < nef.Script.Length; a += oldI.Size)
            {
                if (!oldAddressToInstruction.TryGetValue(a, out oldI))
                    oldI = Instruction.RET;
                if (!initAddrs.Contains(a) && OpCodeTypes.loadStaticFields.Contains(oldI.OpCode)
                 && !doNotReplaceLdsfld.Contains(a))
                {
                    byte staticFieldIndex = OpCodeTypes.SlotIndex(oldI);
                    if (writtenConstInInit.TryGetValue(staticFieldIndex, out Instruction? push))
                        if (OpCodeTypes.pushConst.Contains(push.OpCode))
                        {// copy a push and add it
                            IEnumerable<byte> pushInBytes = [(byte)push.OpCode];
                            pushInBytes = pushInBytes.Concat(BitConverter.GetBytes(push.Operand.Length)[0..Optimizer.OperandSizePrefixTable[(int)push.OpCode]]).Concat(push.Operand.ToArray());
                            Instruction copiedPush = new Script(pushInBytes.ToArray()).GetInstruction(0);
                            simplifiedInstructionsToAddress.Add(copiedPush, currentAddr);
                            oldSequencePointAddressToNew[a] = currentAddr;
                            currentAddr += copiedPush.Size;
                            if (JumpTarget.SingleJumpInOperand(copiedPush))
                            {
                                jumpSourceToTargets[copiedPush] = jumpSourceToTargets[push];
                                jumpTargetToSources[jumpSourceToTargets[copiedPush]].Add(copiedPush);
                            }
                            OptimizedScriptBuilder.RetargetJump(oldI, copiedPush,
                                jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                            continue;
                        }
                }
                if (initAddrs.Contains(a) && pushConstStaticInInit.Contains(oldI))
                {
                    Instruction push = oldI;
                    Instruction store = oldAddressToInstruction[a + push.Size];
                    byte staticFieldIndex = OpCodeTypes.SlotIndex(store);
                    if (!readInInit.Contains(staticFieldIndex))
                    {
                        if (doNotDeleteStsfld.Contains(staticFieldIndex))
                        {// keep this PUSH and STSFLD
                            simplifiedInstructionsToAddress.Add(push, currentAddr);
                            currentAddr += push.Size;
                            continue;
                        }
                        // delete PUSH+STSFLD instruction; just re-target jump
                        if (!oldAddressToInstruction.TryGetValue(a + push.Size + store.Size, out Instruction? nextI))
                        {// maybe this STSFLD is last instruction of the script; should RET after it
                            nextI = Instruction.RET;
                            simplifiedInstructionsToAddress.Add(Instruction.RET, currentAddr);
                            currentAddr += Instruction.RET.Size;
                            oldSequencePointAddressToNew[a] = currentAddr;
                            oldSequencePointAddressToNew[a + push.Size] = currentAddr;
                        }
                        OptimizedScriptBuilder.RetargetJump(push, nextI,
                            jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        a += push.Size;
                        oldI = store;
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(oldI, currentAddr);
                currentAddr += oldI.Size;
            }

            (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew);
            return JumpCompresser.CompressJump(nef, manifest, debugInfo);
        }

        /// <summary>
        /// Delete unnecessary _initialize method
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = 1 << 5)]
        public static (NefFile, ContractManifest, JObject?) RemoveInitialize(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, Instruction> oldAddressToInstruction = oldContractCoverage.addressToInstructions;
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            int[] inits = oldContractCoverage.entryPointsByMethod
                .Where(kv => kv.Value == EntryType.Initialize)
                .Select(kv => kv.Key).ToArray();
            if (inits.Length > 1)
                throw new BadScriptException($"{inits.Length} _initialize methods in contract {manifest.Name}");
            if (inits.Length == 0)  // no _initialize method; do nothing
                return (nef, manifest, debugInfo);
            int init = inits[0];

            if (!oldAddressToInstruction.TryGetValue(init, out Instruction? initsslot)
             || initsslot.OpCode != OpCode.INITSSLOT)
                return (nef, manifest, debugInfo);
            if (jumpTargetToSources.TryGetValue(initsslot, out HashSet<Instruction>? sources)
             && sources.Count > 0)
                return (nef, manifest, debugInfo);

            foreach (Instruction i in oldAddressToInstruction.Values)
                if (OpCodeTypes.loadStaticFields.Contains(i.OpCode))
                    return (nef, manifest, debugInfo);

            int currentAddr = 0;
            Instruction? oldI = null;
            for (int a = 0; a < nef.Script.Length; a += oldI.Size)
            {
                if (a == init)
                {
                    if (!oldAddressToInstruction.TryGetValue(a + initsslot.Size, out Instruction? ret)
                     || ret.OpCode == OpCode.RET)
                    {
                        List<ContractMethodDescriptor> methods = manifest.Abi.Methods.ToList();
                        methods.RemoveAll(m => m.Offset == init);
                        manifest.Abi.Methods = methods.ToArray();
                        // skip!
                        a += initsslot.Size;
                        oldI = Instruction.RET;
                        continue;
                    }
                }
                if (!oldAddressToInstruction.TryGetValue(a, out oldI))
                    oldI = Instruction.RET;
                simplifiedInstructionsToAddress.Add(oldI, currentAddr);
                currentAddr += oldI.Size;
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction);
        }
    }
}
