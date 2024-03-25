using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Optimizer
{
    public class BasicBlock
    {
        public List<Instruction> instructions { get; set; }
        public BasicBlock? nextBlock = null;
        public BasicBlock? jumpTargetBlock1 = null;
        public BasicBlock? jumpTargetBlock2 = null;

        public BasicBlock(List<Instruction> instructions)
        {
            this.instructions = instructions;
        }

        public BasicBlock(Dictionary<int, Instruction> instructions)
        {
            this.instructions = (from kv in instructions orderby kv.Key ascending select kv.Value).ToList();
        }

        //public void SetNextBasicBlock(BasicBlock block) => this.nextBlock = block;
        //public void SetJumpTargetBlock1(BasicBlock block) => this.jumpTargetBlock1 = block;
        //public void SetJumpTargetBlock2(BasicBlock block) => this.jumpTargetBlock2 = block;
    }

    public class ContractInBasicBlocks
    {
        public static Dictionary<int, Dictionary<int, Instruction>> BasicBlocksInDict(NefFile nef, ContractManifest manifest, JToken debugInfo)
            => new InstructionCoverage(nef, manifest, debugInfo).basicBlocksInDict;

        public List<BasicBlock> basicBlocks;
        public Dictionary<Instruction, BasicBlock> basicBlocksByStartingInstruction;
        public ContractManifest manifest;
        public JToken debugInfo;
        public ContractInBasicBlocks(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            this.manifest = manifest;
            this.debugInfo = debugInfo;
            InstructionCoverage coverage = new(nef, manifest, debugInfo);
            IEnumerable<(int startingAddr, List<Instruction> block)> sortedBasicBlocks =
                from kv in coverage.basicBlocksInDict
                orderby kv.Key ascending
                select (kv.Key,
                    // kv.Value sorted by address
                    (from singleBlockKv in kv.Value orderby singleBlockKv.Key ascending select singleBlockKv.Value).ToList()
                );
            basicBlocksByStartingInstruction = new();
            BasicBlock? prevBlock = null;
            foreach ((int startingAddr, List<Instruction> block) in sortedBasicBlocks)
            {
                BasicBlock thisBlock = new(block);
                basicBlocksByStartingInstruction.Add(block.First(), thisBlock);
                if (prevBlock != null)
                    prevBlock.nextBlock = thisBlock;
                prevBlock = thisBlock;
            }
            foreach ((int startingAddr, List<Instruction> block) in sortedBasicBlocks)
            {
                Instruction lastInstruction = block.Last();
                if (coverage.jumpInstructionSourceToTargets.TryGetValue(lastInstruction, out Instruction? target))
                    basicBlocksByStartingInstruction[block.First()].jumpTargetBlock1 = basicBlocksByStartingInstruction[target];
                if (coverage.tryInstructionSourceToTargets.TryGetValue(lastInstruction, out (Instruction, Instruction) targets))
                {
                    if (lastInstruction != targets.Item1)
                        basicBlocksByStartingInstruction[block.First()].jumpTargetBlock1 = basicBlocksByStartingInstruction[targets.Item1];
                    if (lastInstruction != targets.Item2)
                        basicBlocksByStartingInstruction[block.First()].jumpTargetBlock2 = basicBlocksByStartingInstruction[targets.Item2];
                }
            }
            this.basicBlocks = basicBlocksByStartingInstruction.Values.ToList();
        }

        public IEnumerable<Instruction> EnumerateInstructions()
        {
            foreach (BasicBlock basicBlock in basicBlocks)
                foreach (Instruction instruction in basicBlock.instructions)
                    yield return instruction;
        }
    }
}
