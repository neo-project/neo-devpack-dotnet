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
        public BasicBlock? nextBlock = null;  // the following basic block (with subseqent address)
        public BasicBlock? jumpTargetBlock1 = null;  // jump target of the last instruction of this basic block
        public BasicBlock? jumpTargetBlock2 = null;

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
            this.instructions = (from kv in instructions orderby kv.Key ascending select kv.Value).ToList();
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

        public List<BasicBlock> basicBlocks;
        public Dictionary<Instruction, BasicBlock> basicBlocksByStartInstruction;
        public ContractManifest manifest;
        public JToken? debugInfo;
        public ContractInBasicBlocks(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            this.manifest = manifest;
            this.debugInfo = debugInfo;
            InstructionCoverage coverage = new(nef, manifest);
            IEnumerable<(int startAddr, List<Instruction> block)> sortedBasicBlocks =
                from kv in coverage.basicBlocksInDict
                orderby kv.Key ascending
                select (kv.Key,
                    // kv.Value sorted by address
                    (from singleBlockKv in kv.Value orderby singleBlockKv.Key ascending select singleBlockKv.Value).ToList()
                );
            basicBlocksByStartInstruction = new();
            BasicBlock? prevBlock = null;
            // build all blocks without handling jumps between blocks
            foreach ((int startAddr, List<Instruction> block) in sortedBasicBlocks)
            {
                BasicBlock thisBlock = new(startAddr, block);
                basicBlocksByStartInstruction.Add(block.First(), thisBlock);
                if (prevBlock != null)
                {
                    OpCode prevLastOpCode = prevBlock.instructions.Last().OpCode;
                    if (!OpCodeTypes.unconditionalJump.Contains(prevLastOpCode) && prevLastOpCode != OpCode.RET)
                        prevBlock.nextBlock = thisBlock;
                }
                prevBlock = thisBlock;
            }
            // handle jumps between blocks
            foreach ((int startAddr, List<Instruction> block) in sortedBasicBlocks)
            {
                Instruction lastInstruction = block.Last();
                if (coverage.jumpInstructionSourceToTargets.TryGetValue(lastInstruction, out Instruction? target))
                    basicBlocksByStartInstruction[block.First()].jumpTargetBlock1 = basicBlocksByStartInstruction[target];
                if (coverage.tryInstructionSourceToTargets.TryGetValue(lastInstruction, out (Instruction, Instruction) targets))
                {
                    // The reachability optimizer may ask the jumping instruction to point to itself,
                    // because the original jump target may have been deleted.
                    // We do not consider a jump to self.
                    if (lastInstruction != targets.Item1)
                        basicBlocksByStartInstruction[block.First()].jumpTargetBlock1 = basicBlocksByStartInstruction[targets.Item1];
                    if (lastInstruction != targets.Item2)
                        basicBlocksByStartInstruction[block.First()].jumpTargetBlock2 = basicBlocksByStartInstruction[targets.Item2];
                }
            }
            this.basicBlocks = basicBlocksByStartInstruction.Values.ToList();
        }

        public IEnumerable<Instruction> GetScriptInstructions()
        {
            foreach (BasicBlock basicBlock in basicBlocks)
                foreach (Instruction instruction in basicBlock.instructions)
                    yield return instruction;
        }
    }
}
