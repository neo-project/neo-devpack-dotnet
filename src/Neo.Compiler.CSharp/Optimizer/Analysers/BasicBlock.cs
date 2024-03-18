using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Optimizer
{
    static class BasicBlock
    {
        public static Dictionary<int, Dictionary<int, Instruction>> FindBasicBlocks(NefFile nef, ContractManifest manifest, JToken debugInfo)
            => new InstructionCoverage(nef, manifest, debugInfo).basicBlocks;
    }
}
