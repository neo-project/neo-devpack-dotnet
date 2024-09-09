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
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <param name="simplifiedInstructionsToAddress">new Instruction => int address</param>
        /// <param name="jumpSourceToTargets"></param>
        /// <param name="trySourceToTargets"></param>
        /// <param name="oldAddressToInstruction"></param>
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
                method.Offset = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[method.Offset]]!;
            debugInfo = DebugInfoBuilder.ModifyDebugInfo(
                debugInfo, simplifiedInstructionsToAddress, oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
            return (nef, manifest, debugInfo);
        }
    }
}
