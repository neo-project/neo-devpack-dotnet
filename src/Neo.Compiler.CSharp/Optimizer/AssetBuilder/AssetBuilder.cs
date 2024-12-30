using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;

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
                else  // old start of method was deleted
                    method.Offset = oldSequencePointAddressToNew![method.Offset];
            debugInfo = DebugInfoBuilder.ModifyDebugInfo(
                debugInfo, simplifiedInstructionsToAddress, oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
            return (nef, manifest, debugInfo);
        }
    }
}
