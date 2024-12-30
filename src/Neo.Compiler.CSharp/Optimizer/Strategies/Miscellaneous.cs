using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Optimizer
{
    public static class Miscellaneous
    {
        /// <summary>
        /// If any method token in nef is not utilized by CALLT, remove the method token.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MinValue)]
        public static (NefFile, ContractManifest, JObject?) RemoveMethodToken(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            if (nef.Tokens.Length == 0)
                return (nef, manifest, debugInfo);
            List<bool> oldTokenNeeded = Enumerable.Repeat(false, nef.Tokens.Length).ToList();
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            foreach ((_, Instruction i) in oldAddressAndInstructionsList)
                if (i.OpCode == OpCode.CALLT)
                    // Possibly i.TokenU16 >= result.Count
                    // In this case the CALLT is invalid. We just let it throw exceptions.
                    oldTokenNeeded[i.TokenU16] = true;
            Dictionary<int, int> oldTokenIdToNew = new();
            List<MethodToken> newTokens = new();
            for (int i = 0; i < oldTokenNeeded.Count; ++i)
            {
                if (oldTokenNeeded[i])
                    newTokens.Add(nef.Tokens[i]);
                oldTokenIdToNew.Add(i, newTokens.Count - 1);
            }
            nef.Tokens = newTokens.ToArray();
            if (newTokens.Count == 0 || newTokens.Count == oldTokenNeeded.Count)
            {// all tokens deleted, or no token deleted
                nef.CheckSum = NefFile.ComputeChecksum(nef);
                return (nef, manifest, debugInfo);
            }
            // else: some operand of CALLT should be changed
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                JumpTarget.FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int newAddr = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (i.OpCode == OpCode.CALLT && oldTokenIdToNew[i.TokenU16] != i.TokenU16)
                {
                    IEnumerable<byte> newInstructionInBytes = [(byte)OpCode.CALLT];
                    newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(oldTokenIdToNew[i.TokenU16])[0..2]);
                    Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);
                    simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                    oldSequencePointAddressToNew.Add(a, newAddr);
                    newAddr += i.Size;
                    OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                        jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                    continue;
                }
                simplifiedInstructionsToAddress.Add(i, newAddr);
                newAddr += i.Size;
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }
    }
}
