using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.SecurityAnalyzer
{
    public static class WriteInTryAnalzyer
    {
        public class WriteInTryVulnerability
        {
            // key block writes storage; value blocks in try
            public readonly Dictionary<BasicBlock, HashSet<int>> vulnerabilities;
            public JToken? debugInfo { get; init; }
            // TODO: use debugInfo to GetWarningInfo with source codes

            public WriteInTryVulnerability(Dictionary<BasicBlock, HashSet<int>> vulnerabilities, JToken? debugInfo = null)
            {
                this.vulnerabilities = vulnerabilities;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                StringBuilder result = new();
                if (vulnerabilities.Count <= 0) return result.ToString();
                foreach ((BasicBlock b, HashSet<int> tryAddr) in vulnerabilities)
                {
                    int a = b.startAddr;
                    HashSet<int> writeAddrs = new();
                    foreach (Neo.VM.Instruction i in b.instructions)
                    {
                        if (i.OpCode == VM.OpCode.SYSCALL
                        && (i.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                         || i.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash))
                            writeAddrs.Add(a);
                        a += i.Size;
                    }
                    StringBuilder additional = new();
                    additional.AppendLine($"[SEC] Writing storage in `try` (address {tryAddr}), at instruction address:");
                    additional.AppendLine($"\t{string.Join(", ", writeAddrs)}");
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
        /// You can still abort or throw exception when you catch an exception from try. This is safe.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns>list of addresses that write in try</returns>
        public static WriteInTryVulnerability AnalyzeWriteInTry
            (NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            HashSet<BasicBlock> blockWriteStorage = [];
            Dictionary<BasicBlock, HashSet<int>> result = [];
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            TryCatchFinallyCoverage tryCatchFinallyCoverage = new(contractInBasicBlocks);
            foreach (BasicBlock block in contractInBasicBlocks.sortedBasicBlocks)
                foreach (Neo.VM.Instruction i in block.instructions)
                    if (i.OpCode == VM.OpCode.SYSCALL
                    && (i.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                     || i.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash))
                        blockWriteStorage.Add(block);
            foreach (TryCatchFinallySingleCoverage c in tryCatchFinallyCoverage.allTry.Values)
                foreach (BasicBlock b in c.tryBlocks)
                    if (blockWriteStorage.Contains(b))
                    {
                        // If no catch, or surely throws or aborts from catch, this is still safe
                        if (c.catchBlock == null || c.catchBlock.branchType == BranchType.THROW || c.catchBlock.branchType == BranchType.ABORT)
                            continue;
                        // add to result
                        if (!result.TryGetValue(b, out HashSet<int>? tryAddrs))
                        {
                            tryAddrs = new();
                            result[b] = tryAddrs;
                        }
                        tryAddrs.Add(c.tryAddr);
                    }
            return new(result, debugInfo);
        }
    }
}
