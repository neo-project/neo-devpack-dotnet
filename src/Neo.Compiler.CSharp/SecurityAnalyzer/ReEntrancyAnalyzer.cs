using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.Compiler.SecurityAnalyzer
{
    /// <summary>
    /// Re-entrancy can happen when you call untrusted external code
    /// before writing to your own system.
    /// </summary>
    public static class ReEntrancyAnalyzer
    {
        public class ReEntrancyVulnerabilityPair
        {
            // key block calls another contract; value blocks write storage
            public readonly Dictionary<BasicBlock, HashSet<BasicBlock>> vulnerabilityPairs;
            // values are instruction addresses
            // where this basic block calls contract or writes storage
            public readonly Dictionary<BasicBlock, HashSet<int>> callOtherContractInstructions;
            public readonly Dictionary<BasicBlock, HashSet<int>> writeStorageInstructions;
            public JToken? debugInfo { get; init; }
            // TODO: use debugInfo to GetWarningInfo with source codes

            public ReEntrancyVulnerabilityPair(
                Dictionary<BasicBlock, HashSet<BasicBlock>> vulnerabilityPairs,
                Dictionary<BasicBlock, HashSet<int>> callOtherContractInstructions,
                Dictionary<BasicBlock, HashSet<int>> writeStorageInstructions,
                JToken? debugInfo = null)
            {
                this.vulnerabilityPairs = vulnerabilityPairs
                    .Where(v => v.Value.Count > 0).ToDictionary();
                this.callOtherContractInstructions = callOtherContractInstructions;
                this.writeStorageInstructions = writeStorageInstructions;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (vulnerabilityPairs.Count <= 0) return "";
                StringBuilder result = new();
                foreach ((BasicBlock callBlock, HashSet<BasicBlock> writeBlocks) in vulnerabilityPairs)
                {
                    StringBuilder additional = new();
                    additional.AppendLine($"[SEC] Potential Re-entrancy: Calling contracts at instruction address: " +
                        $"{string.Join(", ", callOtherContractInstructions[callBlock])} before writing storage at");
                    foreach (BasicBlock writeBlock in writeBlocks)
                        additional.AppendLine($"\t{string.Join(", ", writeStorageInstructions[writeBlock])}");
                    if (print)
                        Console.Write(additional.ToString());
                    result.Append(additional);
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// This method finds all cases where your contract A first call another contract B,
        /// and then you write to your storage (storage of A).
        /// 
        /// This DOES NOT prevent cross-contract re-entrancy, where
        /// your contract A calls an untrusted contract B,
        /// then B calls another contract C, changing the storage of C,
        /// finally you call C.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        public static ReEntrancyVulnerabilityPair AnalyzeSingleContractReEntrancy
            (NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            List<BasicBlock> basicBlocks = contractInBasicBlocks.sortedBasicBlocks;
            // key block calls another contract; value blocks write storage
            Dictionary<BasicBlock, HashSet<BasicBlock>> vulnerabilityPairs =
                basicBlocks.ToDictionary(b => b, b => new HashSet<BasicBlock>());
            // Whether each basic block may call other contract or write to storage
            Dictionary<BasicBlock, HashSet<int>> callOtherContractInstructions =
                basicBlocks.ToDictionary(b => b, b => new HashSet<int>());
            Dictionary<BasicBlock, HashSet<int>> writeStorageInstructions =
                basicBlocks.ToDictionary(b => b, b => new HashSet<int>());
            // Detect basic blocks that call and write in itself
            foreach (BasicBlock b in basicBlocks)
            {
                int addr = b.startAddr;
                foreach (Neo.VM.Instruction instruction in b.instructions)
                {
                    if (instruction.OpCode == VM.OpCode.SYSCALL)
                    {
                        if (instruction.TokenU32 == ApplicationEngine.System_Contract_Call.Hash)
                            callOtherContractInstructions[b].Add(addr);
                        if (instruction.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash)
                        {
                            writeStorageInstructions[b].Add(addr);
                            if (callOtherContractInstructions[b].Count > 0)
                                vulnerabilityPairs[b].Add(b);
                        }
                    }
                    addr += instruction.Size;
                }
            }

            // For each basic block that writes to storage,
            // find upstream blocks that call another contract and may go into this block
            foreach (BasicBlock block in basicBlocks.Where(b => writeStorageInstructions[b].Count > 0))
            {
                HashSet<BasicBlock> visited = new();
                Queue<BasicBlock> q = new(block.jumpSourceBlocks);
                if (block.prevBlock != null)
                    q.Enqueue(block.prevBlock);
                while (q.Count > 0)
                {
                    BasicBlock current = q.Dequeue();
                    visited.Add(current);
                    if (callOtherContractInstructions[current].Count > 0)
                        vulnerabilityPairs[current].Add(block);
                    foreach (BasicBlock referringBlock in current.jumpSourceBlocks)
                        if (!visited.Contains(referringBlock))
                            q.Enqueue(referringBlock);
                    if (current.prevBlock != null && !visited.Contains(current.prevBlock))
                        q.Enqueue(current.prevBlock);
                }
            }
            return new(vulnerabilityPairs, callOtherContractInstructions, writeStorageInstructions);
        }
    }
}
