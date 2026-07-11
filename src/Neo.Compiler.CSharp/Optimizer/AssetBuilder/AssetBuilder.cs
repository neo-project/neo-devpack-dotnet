// Copyright (C) 2015-2026 The Neo Project.
//
// AssetBuilder.cs file belongs to the neo project and is free
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
using System.Collections.Specialized;

namespace Neo.Optimizer
{
    public static class AssetBuilder
    {
        /// <summary>
        /// Make sure all the Instruction objects are of the same reference.
        /// That means you should get the Instructions from the same initial source.
        /// Do not script.EnumerateInstructions for many times.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <param name="simplifiedInstructionsToAddress">new Instruction => int address</param>
        /// <param name="jumpSourceToTargets">All jumping instructions source => target</param>
        /// <param name="trySourceToTargets">All try instructions source => target</param>
        /// <param name="oldAddressToInstruction">old int address => Instruction</param>
        /// <param name="oldSequencePointAddressToNew">old int address => new int address</param>
        /// <returns></returns>
        public static (NefFile, ContractManifest, JObject?) BuildOptimizedAssets(
            NefFile nef, ContractManifest manifest, JObject? debugInfo,
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress,
            Dictionary<Instruction, Instruction> jumpSourceToTargets,
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
            Dictionary<int, Instruction> oldAddressToInstruction,
            Dictionary<int, int>? oldSequencePointAddressToNew = null)
        {
            nef.Script = OptimizedScriptBuilder.BuildScriptWithJumpTargets(
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction);
            //nef.Compiler = AppDomain.CurrentDomain.FriendlyName;
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                if (oldAddressToInstruction.TryGetValue(method.Offset, out Instruction? i)
                 && simplifiedInstructionsToAddress.Contains(i))
                    method.Offset = (int)simplifiedInstructionsToAddress[i]!;
                else if (oldSequencePointAddressToNew is not null && oldSequencePointAddressToNew.TryGetValue(method.Offset, out int newOffset))
                    method.Offset = newOffset;
                else if (TryResolveDeletedMethodOffset(method.Offset, simplifiedInstructionsToAddress, oldAddressToInstruction, out newOffset))
                    method.Offset = newOffset;
            debugInfo = DebugInfoBuilder.ModifyDebugInfo(
                debugInfo, simplifiedInstructionsToAddress, oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
            if (debugInfo is not null)
                debugInfo["hash"] = nef.Script.Span.ToScriptHash().ToString();
            return (nef, manifest, debugInfo);
        }

        private static bool TryResolveDeletedMethodOffset(
            int oldOffset,
            OrderedDictionary simplifiedInstructionsToAddress,
            Dictionary<int, Instruction> oldAddressToInstruction,
            out int newOffset)
        {
            for (int currentOffset = oldOffset;
                oldAddressToInstruction.TryGetValue(currentOffset, out Instruction? instruction);
                currentOffset += instruction.Size)
            {
                if (simplifiedInstructionsToAddress.Contains(instruction))
                {
                    newOffset = (int)simplifiedInstructionsToAddress[instruction]!;
                    return true;
                }
            }

            newOffset = oldOffset;
            return false;
        }
    }
}
