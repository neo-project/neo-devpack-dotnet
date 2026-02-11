// Copyright (C) 2015-2026 The Neo Project.
//
// MissingCheckWitnessAnalyzer.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.SecurityAnalyzer
{
    /// <summary>
    /// Detects public methods that perform state changes (storage writes)
    /// without any CheckWitness call in the same method.
    /// </summary>
    public static class MissingCheckWitnessAnalyzer
    {
        public class MissingCheckWitnessVulnerability
        {
            public readonly List<string> vulnerableMethodNames;
            public readonly JToken? debugInfo;

            public MissingCheckWitnessVulnerability(
                List<string> vulnerableMethodNames,
                JToken? debugInfo = null)
            {
                this.vulnerableMethodNames = vulnerableMethodNames;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (vulnerableMethodNames.Count == 0)
                    return "";
                string result = $"[SEC] The following public methods write to storage without CheckWitness verification:{Environment.NewLine}" +
                    $"\t{string.Join(", ", vulnerableMethodNames)}{Environment.NewLine}" +
                    $"Consider adding `Runtime.CheckWitness()` before performing storage writes to prevent unauthorized access.{Environment.NewLine}";
                if (print)
                    Console.Write(result);
                return result;
            }
        }

        /// <summary>
        /// Analyzes the contract for public methods that write to storage
        /// without calling CheckWitness.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        public static MissingCheckWitnessVulnerability AnalyzeMissingCheckWitness(
            NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            (int addr, VM.Instruction instruction)[] instructions =
                ((Script)nef.Script).EnumerateInstructions().ToArray();

            // Build a sorted list of method offsets to determine method boundaries
            ContractMethodDescriptor[] methods = manifest.Abi.Methods;
            int[] sortedOffsets = methods.Select(m => m.Offset).OrderBy(o => o).ToArray();

            List<string> vulnerableMethods = new();

            foreach (ContractMethodDescriptor method in methods)
            {
                // Skip internal methods
                if (method.Name.StartsWith("_"))
                    continue;

                int methodStart = method.Offset;
                // Method end is the start of the next method, or end of script
                int nextMethodIndex = Array.IndexOf(sortedOffsets, methodStart) + 1;
                int methodEnd = nextMethodIndex < sortedOffsets.Length
                    ? sortedOffsets[nextMethodIndex]
                    : int.MaxValue;

                bool hasStorageWrite = false;
                bool hasCheckWitness = false;

                foreach ((int addr, VM.Instruction instruction) in instructions)
                {
                    if (addr < methodStart)
                        continue;
                    if (addr >= methodEnd)
                        break;

                    if (instruction.OpCode == OpCode.SYSCALL)
                    {
                        if (instruction.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash)
                            hasStorageWrite = true;

                        if (instruction.TokenU32 == ApplicationEngine.System_Runtime_CheckWitness.Hash)
                            hasCheckWitness = true;
                    }
                }

                if (hasStorageWrite && !hasCheckWitness)
                    vulnerableMethods.Add(method.Name);
            }

            return new MissingCheckWitnessVulnerability(vulnerableMethods, debugInfo);
        }
    }
}
