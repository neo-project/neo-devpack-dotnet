using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Optimizer
{
    class BasicBlock
    {
        public static Dictionary<int, Dictionary<int, Instruction>> FindBasicBlocks(NefFile nef, ContractManifest manifest, JToken debugInfo)
            => new InstructionCoverage(nef, manifest, debugInfo).basicBlocks;

        public List<Instruction> instructions { get; set; }
        public BasicBlock? nextBlock = null;
        public BasicBlock? jumpTargetBlock1 = null;
        public BasicBlock? jumpTargetBlock2 = null;

        public BasicBlock(List<Instruction> instructions)
        {
            this.instructions = instructions;
        }

        public void SetNextBasicBlock(BasicBlock block) => this.nextBlock = block;
        public void SetJumpTargetBlock1(BasicBlock block) => this.jumpTargetBlock1 = block;
        public void SetJumpTargetBlock2(BasicBlock block) => this.jumpTargetBlock2 = block;
    }

    class BasicBlocksInContract
    {
        List<BasicBlock> basicBlocks;
        public BasicBlocksInContract(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            InstructionCoverage coverage = new InstructionCoverage(nef, manifest, debugInfo);
            Dictionary<int, Dictionary<int, Instruction>> basicBlocks = coverage.basicBlocks;
        }
    }
}
