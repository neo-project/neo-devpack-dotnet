// Copyright (C) 2015-2025 The Neo Project.
//
// ReEntrancyAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
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
            public JToken? DebugInfo { get; init; }
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
                DebugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (vulnerabilityPairs.Count <= 0) return "";

                // Parse debug info if available
                NeoDebugInfo? debugInfo = null;
                if (DebugInfo != null)
                {
                    try
                    {
                        if (DebugInfo is JObject jObj)
                            debugInfo = NeoDebugInfo.FromDebugInfoJson(jObj);
                    }
                    catch
                    {
                        // Fallback to address-only warnings if debug info parsing fails
                    }
                }

                StringBuilder result = new();
                foreach ((BasicBlock callBlock, HashSet<BasicBlock> writeBlocks) in vulnerabilityPairs)
                {
                    StringBuilder additional = new();
                    additional.AppendLine($"[SEC] Potential Re-entrancy vulnerability detected");
                    additional.AppendLine($"  Issue: Contract calls external code before writing to storage, allowing potential re-entrancy attacks");

                    // Add source location information for contract calls
                    additional.AppendLine($"  External contract calls:");
                    foreach (int callAddr in callOtherContractInstructions[callBlock])
                    {
                        if (debugInfo != null)
                        {
                            var sourceLocation = GetSourceLocation(callAddr, debugInfo);
                            if (sourceLocation != null)
                            {
                                additional.AppendLine($"    At: {sourceLocation.FileName}:{sourceLocation.Line}:{sourceLocation.Column}");
                                if (!string.IsNullOrEmpty(sourceLocation.CodeSnippet))
                                    additional.AppendLine($"    Code: {sourceLocation.CodeSnippet}");
                            }
                            else
                            {
                                additional.AppendLine($"    At instruction address: {callAddr}");
                            }
                        }
                        else
                        {
                            additional.AppendLine($"    At instruction address: {callAddr}");
                        }
                    }

                    // Add source location information for storage writes
                    additional.AppendLine($"  Storage writes that occur after external calls:");
                    foreach (BasicBlock writeBlock in writeBlocks)
                    {
                        foreach (int writeAddr in writeStorageInstructions[writeBlock])
                        {
                            if (debugInfo != null)
                            {
                                var sourceLocation = GetSourceLocation(writeAddr, debugInfo);
                                if (sourceLocation != null)
                                {
                                    additional.AppendLine($"    At: {sourceLocation.FileName}:{sourceLocation.Line}:{sourceLocation.Column}");
                                    if (!string.IsNullOrEmpty(sourceLocation.CodeSnippet))
                                        additional.AppendLine($"    Code: {sourceLocation.CodeSnippet}");
                                }
                                else
                                {
                                    additional.AppendLine($"    At instruction address: {writeAddr}");
                                }
                            }
                            else
                            {
                                additional.AppendLine($"    At instruction address: {writeAddr}");
                            }
                        }
                    }

                    additional.AppendLine($"  Recommendation: Perform all storage writes before making external contract calls, or use reentrancy guards");
                    additional.AppendLine();

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
                foreach (VM.Instruction instruction in b.instructions)
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
            return new(vulnerabilityPairs, callOtherContractInstructions, writeStorageInstructions, debugInfo);
        }

        /// <summary>
        /// Represents source code location information for diagnostic messages
        /// </summary>
        private class SourceLocation
        {
            public string FileName { get; set; } = string.Empty;
            public int Line { get; set; }
            public int Column { get; set; }
            public string? CodeSnippet { get; set; }
        }

        /// <summary>
        /// Gets source code location information for an instruction address
        /// </summary>
        /// <param name="instructionAddress">The instruction address to look up</param>
        /// <param name="debugInfo">Debug information containing source mappings</param>
        /// <returns>Source location information if found, null otherwise</returns>
        private static SourceLocation? GetSourceLocation(int instructionAddress, NeoDebugInfo debugInfo)
        {
            // Find the sequence point that covers this instruction address
            foreach (var method in debugInfo.Methods)
            {
                if (instructionAddress >= method.Range.Start && instructionAddress <= method.Range.End)
                {
                    // Find the closest sequence point at or before this address
                    var sequencePoint = method.SequencePoints
                        .Where(sp => sp.Address <= instructionAddress)
                        .OrderByDescending(sp => sp.Address)
                        .FirstOrDefault();

                    if (sequencePoint.Document >= 0 && sequencePoint.Document < debugInfo.Documents.Count)
                    {
                        var fileName = debugInfo.Documents[sequencePoint.Document];
                        return new SourceLocation
                        {
                            FileName = System.IO.Path.GetFileName(fileName),
                            Line = sequencePoint.Start.Line,
                            Column = sequencePoint.Start.Column,
                            CodeSnippet = null // Could be enhanced to read actual source code
                        };
                    }
                }
            }
            return null;
        }
    }
}
