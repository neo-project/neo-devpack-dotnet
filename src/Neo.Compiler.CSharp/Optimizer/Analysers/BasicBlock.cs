// Copyright (C) 2015-2024 The Neo Project.
//
// BasicBlock.cs file belongs to the neo project and is free
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
    /// <summary>
    /// A basic block is a group of assembly instructions that are surely executed together.
    /// The start of a basic block can be the target of a jump, or an entry point of execution.
    /// The end of a basic block can be a jumping instruction, an ENDFINALLY, a RET, etc.
    /// Instructions in the same basic block can be replaced with more effcient ones.
    /// </summary>
    public class BasicBlock
    {
        public readonly int startAddr;
        public List<Instruction> instructions { get; set; }  // instructions in this basic block
        public BasicBlock? prevBlock = null;  // the previous basic block (with subseqent address)
        public BasicBlock? nextBlock = null;  // the following basic block (with subseqent address)
        public HashSet<BasicBlock> jumpTargetBlocks = new();  // jump target of the last instruction of this basic block
        public HashSet<BasicBlock> jumpSourceBlocks = new();
        public BranchType branchType = BranchType.UNCOVERED;

        public BasicBlock(int startAddr, List<Instruction> instructions)
        {
            this.startAddr = startAddr;
            this.instructions = instructions;
        }

        /// <summary>
        /// Sort the instruction dictionary by address
        /// </summary>
        /// <param name="instructions">address -> <see cref="Instruction"/></param>
        public BasicBlock(Dictionary<int, Instruction> instructions)
        {
            IEnumerable<(int addr, Instruction i)> addrToInstructions = from kv in instructions orderby kv.Key ascending select (kv.Key, kv.Value);
            this.startAddr = addrToInstructions.First().addr;
            this.instructions = addrToInstructions.Select(kv => kv.i).ToList();
        }

        //public void SetNextBasicBlock(BasicBlock block) => this.nextBlock = block;
        //public void SetJumpTargetBlock1(BasicBlock block) => this.jumpTargetBlock1 = block;
        //public void SetJumpTargetBlock2(BasicBlock block) => this.jumpTargetBlock2 = block;
    }

    /// <summary>
    /// Should contain all basic blocks in a contract
    /// </summary>
    public class ContractInBasicBlocks
    {
        public static Dictionary<int, Dictionary<int, Instruction>> BasicBlocksInDict(NefFile nef, ContractManifest manifest)
            => new InstructionCoverage(nef, manifest).basicBlocksInDict;

        public Dictionary<Instruction, BasicBlock> basicBlocksByStartInstruction;
        public Dictionary<int, BasicBlock> basicBlocksByStartAddr;
        public InstructionCoverage coverage;
        public IEnumerable<(int startAddr, List<Instruction> block)> sortedListInstructions;
        public List<BasicBlock> sortedBasicBlocks;
        public ContractManifest manifest;
        public JToken? debugInfo;
        public ContractInBasicBlocks(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            this.manifest = manifest;
            this.debugInfo = debugInfo;
            coverage = new InstructionCoverage(nef, manifest);
            sortedListInstructions =
                (from kv in coverage.basicBlocksInDict
                 orderby kv.Key ascending
                 select (kv.Key,
                     // kv.Value sorted by address
                     (from singleBlockKv in kv.Value orderby singleBlockKv.Key ascending select singleBlockKv.Value).ToList()
                 ));
            sortedBasicBlocks = new();
            basicBlocksByStartInstruction = new();
            basicBlocksByStartAddr = new();
            // build all blocks without handling jumps or continuations between blocks
            foreach ((int startAddr, List<Instruction> block) in sortedListInstructions)
            {
                BasicBlock thisBlock = new(startAddr, block);
                int firstNotNopAddr = startAddr;
                foreach (Instruction i in block)
                    if (i.OpCode == OpCode.NOP)
                        firstNotNopAddr += i.Size;
                thisBlock.branchType = coverage.coveredMap[firstNotNopAddr];
                sortedBasicBlocks.Add(thisBlock);
                basicBlocksByStartInstruction.Add(block.First(), thisBlock);
                basicBlocksByStartAddr.Add(startAddr, thisBlock);
            }
            // handle jumps and continuations between blocks
            foreach ((int startAddr, List<Instruction> block) in sortedListInstructions)
            {
                if (coverage.basicBlockContinuation.TryGetValue(startAddr, out int continuationTarget))
                {
                    basicBlocksByStartAddr[startAddr].nextBlock = basicBlocksByStartAddr[continuationTarget];
                    basicBlocksByStartAddr[continuationTarget].prevBlock = basicBlocksByStartAddr[startAddr];
                }
                if (coverage.basicBlockJump.TryGetValue(startAddr, out HashSet<int>? jumpTargets))
                    foreach (int target in jumpTargets)
                    {
                        basicBlocksByStartAddr[startAddr].jumpTargetBlocks.Add(basicBlocksByStartAddr[target]);
                        basicBlocksByStartAddr[target].jumpSourceBlocks.Add(basicBlocksByStartAddr[startAddr]);
                    }
            }
        }

        public IEnumerable<Instruction> GetScriptInstructions()
        {
            // WARNING: OpCode.NOP at the start of a basic block may not be included
            // and the jumping operands may be wrong
            // Refer to InstructionCoverage coverage for jump targets
            foreach ((_, List<Instruction> basicBlock) in sortedListInstructions)
                foreach (Instruction instruction in basicBlock)
                    yield return instruction;
        }
    }
}
