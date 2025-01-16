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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.Optimizer
{
    /// <summary>
    /// A basic block is a group of assembly instructions that are surely executed together.
    /// The start of a basic block can be the target of a jump, or an entry point of execution.
    /// The end of a basic block can be a jumping instruction, an ENDFINALLY, a RET, etc.
    /// Instructions in the same basic block can be replaced with more effcient ones.
    /// </summary>
    [DebuggerDisplay("BasicBlock addr={startAddr}")]
    public class BasicBlock
    {
        public readonly int startAddr;
        private int lastAddrCache = -1;
        public int lastAddr
        {
            get
            {
                if (lastAddrCache >= 0)
                    return lastAddrCache;
                lastAddrCache = startAddr;
                foreach (Instruction i in instructions[..^1])
                    lastAddrCache += i.Size;
                return lastAddrCache;
            }
        }
        public int nextAddr { get => lastAddr + instructions.Last().Size; }
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

        public int FindFirstOpCode(OpCode opCode, ReadOnlyMemory<byte>? operand = null)
        {
            int addr = this.startAddr;
            foreach (Instruction i in this.instructions)
                if (i.OpCode == opCode && (operand == null || operand.Equals(i.Operand)))
                    return addr;
                else
                    addr += i.Size;
            return -1;
        }
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
                thisBlock.branchType = coverage.coveredMap[startAddr];
                sortedBasicBlocks.Add(thisBlock);
                basicBlocksByStartInstruction.Add(block.First(), thisBlock);
                basicBlocksByStartAddr.Add(startAddr, thisBlock);
            }
            // handle jumps and continuations between blocks
            foreach (BasicBlock block in sortedBasicBlocks)
            {
                int startAddr = block.startAddr;
                if (coverage.basicBlockContinuation.TryGetValue(startAddr, out int continuationTarget))
                {
                    block.nextBlock = basicBlocksByStartAddr[continuationTarget];
                    basicBlocksByStartAddr[continuationTarget].prevBlock = block;
                }
                if (coverage.basicBlockJump.TryGetValue(startAddr, out HashSet<int>? jumpTargets))
                    foreach (int target in jumpTargets)
                    {
                        block.jumpTargetBlocks.Add(basicBlocksByStartAddr[target]);
                        basicBlocksByStartAddr[target].jumpSourceBlocks.Add(block);
                    }
            }
        }

        /// <summary>
        /// Get the tree of basic blocks covered from the entryAddr.
        /// </summary>
        /// <param name="entryAddr">Entry address of a basic block</param>
        /// <param name="includeCall">If true, calls to other basic blocks are included</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public HashSet<BasicBlock> BlocksCoveredFromAddr(int entryAddr, bool includeCall = true)
        {
            if (!basicBlocksByStartAddr.TryGetValue(entryAddr, out BasicBlock? entryBlock))
                throw new ArgumentException($"{nameof(entryAddr)} must be starting address of a basic block");
            BasicBlock currentBlock = entryBlock;
            HashSet<BasicBlock> returned = new() { currentBlock };
            Queue<BasicBlock> queue = new Queue<BasicBlock>(returned);
            while (queue.Count > 0)
            {
                currentBlock = queue.Dequeue();
                if (currentBlock.nextBlock != null && returned.Add(currentBlock.nextBlock))
                    queue.Enqueue(currentBlock.nextBlock);
                if (includeCall || !OpCodeTypes.callWithJump.Contains(currentBlock.instructions.Last().OpCode))
                    foreach (BasicBlock target in currentBlock.jumpTargetBlocks)
                        if (returned.Add(target))
                            queue.Enqueue(target);
            }
            return returned;
        }

        /// <summary>
        /// Get the set of addresses covered by the input set of BasicBlocks
        /// </summary>
        /// <param name="blocks">Basic blocks that composes the whole contract</param>
        /// <returns></returns>
        public static HashSet<int> AddrCoveredByBlocks(IEnumerable<BasicBlock> blocks)
        {
            HashSet<int> result = new();
            foreach (BasicBlock currentBlock in blocks)
            {
                int addr = currentBlock.startAddr;
                foreach (Instruction i in currentBlock.instructions)
                {
                    result.Add(addr);
                    addr += i.Size;
                }
            }
            return result;
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
