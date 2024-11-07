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
    public static class WriteInTryAnalzyer
    {
        public class WriteInTryVulnerability
        {
            // key block writes storage; value blocks in try
            public readonly HashSet<BasicBlock> vulnerabilities;
            public JToken? debugInfo { get; init; }
            // TODO: use debugInfo to GetWarningInfo with source codes

            public WriteInTryVulnerability(HashSet<BasicBlock> vulnerabilities, JToken? debugInfo = null)
            {
                this.vulnerabilities = vulnerabilities;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                StringBuilder result = new();
                if (vulnerabilities.Count <= 0) return result.ToString();
                foreach (BasicBlock b in vulnerabilities)
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
                    additional.AppendLine($"[SEC] Writing storage in `try`, at instruction address:");
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
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns>list of addresses that write in try</returns>
        public static WriteInTryVulnerability AnalyzeWriteInTry
            (NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            HashSet<BasicBlock> result = [];
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            foreach (BasicBlock block in contractInBasicBlocks.sortedBasicBlocks)
            {
                if ((block.tryType | TryType.TRY) > 0)
                {
                    int a = block.startAddr;
                    foreach (Neo.VM.Instruction i in block.instructions)
                    {
                        if (i.OpCode == VM.OpCode.SYSCALL
                        && (i.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                         || i.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash))
                        {
                            // If no catch, or surely throws or aborts from catch, this is still safe
                            if (!block.catchBlocks.All(b => b.branchType == BranchType.THROW || b.branchType == BranchType.ABORT))
                            {// if (catchBlocks.Count == 0), this is a try without catch, and program does not reach here
                                result.Add(block);
                                break;
                            }
                        }
                        a += i.Size;
                    }
                }
            }
            return new(result, debugInfo);
        }
    }
}
