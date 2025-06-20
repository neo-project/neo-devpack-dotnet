// Copyright (C) 2015-2025 The Neo Project.
//
// WriteInTryAnalyzer.cs file belongs to the neo project and is free
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
    public static class WriteInTryAnalzyer
    {
        public class WriteInTryVulnerability
        {
            // key block writes storage; value blocks in try
            public readonly Dictionary<BasicBlock, HashSet<int>> vulnerabilities;
            public JToken? DebugInfo { get; init; }
            // TODO: use debugInfo to GetWarningInfo with source codes

            public WriteInTryVulnerability(Dictionary<BasicBlock, HashSet<int>> vulnerabilities, JToken? debugInfo = null)
            {
                this.vulnerabilities = vulnerabilities;
                DebugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                StringBuilder result = new();
                if (vulnerabilities.Count <= 0) return result.ToString();

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

                foreach ((BasicBlock b, HashSet<int> tryAddr) in vulnerabilities)
                {
                    int a = b.startAddr;
                    HashSet<int> writeAddrs = new();
                    foreach (VM.Instruction i in b.instructions)
                    {
                        if (i.OpCode == VM.OpCode.SYSCALL
                        && (i.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                         || i.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash))
                            writeAddrs.Add(a);
                        a += i.Size;
                    }

                    StringBuilder additional = new();
                    additional.AppendLine($"[SEC] Writing storage in `try` block is risky - writes may not be properly reverted on exceptions");

                    // Add source location information if available
                    if (debugInfo != null)
                    {
                        foreach (int writeAddr in writeAddrs)
                        {
                            var sourceLocation = GetSourceLocation(writeAddr, debugInfo);
                            if (sourceLocation != null)
                            {
                                additional.AppendLine($"  At: {sourceLocation.FileName}:{sourceLocation.Line}:{sourceLocation.Column}");
                                if (!string.IsNullOrEmpty(sourceLocation.CodeSnippet))
                                    additional.AppendLine($"  Code: {sourceLocation.CodeSnippet}");
                            }
                            else
                            {
                                additional.AppendLine($"  At instruction address: {writeAddr}");
                            }
                        }

                        additional.AppendLine($"  Try block addresses: {{{string.Join(", ", tryAddr)}}}");
                    }
                    else
                    {
                        // Fallback to address-only format
                        additional.AppendLine($"  Try block addresses: {{{string.Join(", ", tryAddr)}}}");
                        additional.AppendLine($"  Write instruction addresses: {string.Join(", ", writeAddrs)}");
                    }

                    additional.AppendLine($"  Recommendation: Ensure storage writes are properly handled in catch/finally blocks or avoid writing in try blocks");
                    additional.AppendLine();

                    if (print)
                        Console.Write(additional.ToString());
                    result.Append(additional);
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// This method finds all Put and Delete in `try`
        /// 
        /// Writing in try is risky.
        /// In case of exception, your write should usually be reverted.
        /// But it is ambiguous whether a write in try is actually reverted.
        /// You can still have no catch, or abort or throw exception when you catch. This is safe.
        /// Or you can abort or throw in finally, though not very meaningful in practice.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns>list of addresses that write in try</returns>
        public static WriteInTryVulnerability AnalyzeWriteInTry
            (NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            HashSet<BasicBlock> allBasicBlocksWritingStorage = [];
            Dictionary<BasicBlock, HashSet<int>> result = [];
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            TryCatchFinallyCoverage tryCatchFinallyCoverage = new(contractInBasicBlocks);
            foreach (BasicBlock block in contractInBasicBlocks.sortedBasicBlocks)
                foreach (VM.Instruction i in block.instructions)
                    if (i.OpCode == VM.OpCode.SYSCALL
                    && (i.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                     || i.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash))
                        allBasicBlocksWritingStorage.Add(block);
            foreach (TryCatchFinallySingleCoverage c in tryCatchFinallyCoverage.allTry.Values)
            {
                if (c.catchBlock == null || c.catchBlock.branchType == BranchType.THROW || c.catchBlock.branchType == BranchType.ABORT)
                    continue;
                // The following is a defensive judge
                // If finally block surely throws or aborts, the try is safe even if try writes storage
                // However, if finally block surely throws or aborts, the catch block BranchType above should surely throw or abort
                // The `continue` above should be executed, and the following continue is never utilized
                // If the following continue is actually executed, there may be some problem in BranchType analysis
                if (c.finallyBlock != null && (c.finallyBlock.branchType == BranchType.THROW || c.finallyBlock.branchType == BranchType.ABORT))
                    continue;
                IEnumerable<BasicBlock> containingBasicBlocksWritingStorage = c.tryBlocks
                    .Where(allBasicBlocksWritingStorage.Contains);
                // If this try contains more internal nested trys,
                // and the internal try/catch/finally writes to storage,
                // this is unsafe.
                // But it is ok to have writes in catch and finally of this try.
                foreach (TryCatchFinallySingleCoverage nestedTry in c.nestedTrysInTry)
                    containingBasicBlocksWritingStorage = containingBasicBlocksWritingStorage
                        .Union(FindAllBasicBlocksWritingStorageInTryCatchFinally(nestedTry, [], allBasicBlocksWritingStorage));
                containingBasicBlocksWritingStorage = containingBasicBlocksWritingStorage.ToHashSet();

                foreach (BasicBlock b in containingBasicBlocksWritingStorage)
                    if (allBasicBlocksWritingStorage.Contains(b))
                    {
                        // If no catch, or surely throws or aborts from catch, this is still safe
                        // add to result
                        if (!result.TryGetValue(b, out HashSet<int>? tryAddrs))
                        {
                            tryAddrs = [];
                            result[b] = tryAddrs;
                        }
                        tryAddrs.Add(c.tryAddr);
                    }
            }
            return new(result, debugInfo);
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

        public static HashSet<BasicBlock> FindAllBasicBlocksWritingStorageInTryCatchFinally
            (TryCatchFinallySingleCoverage c, HashSet<TryCatchFinallySingleCoverage> visitedTrys, HashSet<BasicBlock> allBasicBlocksWritingStorage)
        {
            if (visitedTrys.Contains(c))
                return [];
            visitedTrys.Add(c);
            IEnumerable<BasicBlock> writesInTry = c.tryBlocks
                .Concat(c.catchBlocks)
                .Concat(c.finallyBlocks)
                .Where(allBasicBlocksWritingStorage.Contains);
            foreach (TryCatchFinallySingleCoverage nestedTry in c.nestedTrysInTry.Union(c.nestedTrysInCatch).Union(c.nestedTrysInFinally).ToHashSet())
                writesInTry = writesInTry.Concat(FindAllBasicBlocksWritingStorageInTryCatchFinally(nestedTry, visitedTrys, allBasicBlocksWritingStorage));
            return writesInTry.ToHashSet();
        }
    }
}
