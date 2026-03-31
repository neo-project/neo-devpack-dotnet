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
    /// without any CheckWitness call in the method or its static helper calls.
    /// </summary>
    public static class MissingCheckWitnessAnalyzer
    {
        public class MissingCheckWitnessVulnerability
        {
            public readonly IReadOnlyList<string> vulnerableMethodNames;
            public readonly JToken? debugInfo;

            public MissingCheckWitnessVulnerability(
                IReadOnlyList<string> vulnerableMethodNames,
                JToken? debugInfo = null)
            {
                this.vulnerableMethodNames = vulnerableMethodNames;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (vulnerableMethodNames.Count == 0)
                    return "";
                string result = $"[SECURITY] The following public methods write to storage without CheckWitness verification:{Environment.NewLine}" +
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

            // Build a sorted list of method offsets to determine method boundaries.
            // Include static call targets to cover private helper methods not present in ABI.
            ContractMethodDescriptor[] methods = manifest.Abi.Methods;
            HashSet<int> methodStartOffsets = methods.Select(m => m.Offset).ToHashSet();
            foreach ((int addr, VM.Instruction instruction) in instructions)
            {
                if (instruction.OpCode != OpCode.CALL && instruction.OpCode != OpCode.CALL_L)
                    continue;

                int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target >= 0)
                    methodStartOffsets.Add(target);
            }
            int[] sortedOffsets = methodStartOffsets.OrderBy(o => o).ToArray();

            List<string> vulnerableMethods = new();

            foreach (ContractMethodDescriptor method in methods)
            {
                if (method.Name is "_deploy" or "_initialize")
                    continue;

                (bool hasStorageWrite, bool hasCheckWitness) = AnalyzeMethodAndStaticHelpers(
                    method.Offset,
                    instructions,
                    sortedOffsets,
                    methodStartOffsets);

                if (hasStorageWrite && !hasCheckWitness)
                    vulnerableMethods.Add(method.Name);
            }

            return new MissingCheckWitnessVulnerability(vulnerableMethods, debugInfo);
        }

        private static (bool hasStorageWrite, bool hasCheckWitness) AnalyzeMethodAndStaticHelpers(
            int methodStart,
            (int addr, VM.Instruction instruction)[] instructions,
            int[] sortedOffsets,
            HashSet<int> methodStartOffsets)
        {
            bool hasStorageWrite = false;
            bool hasCheckWitness = false;

            Stack<int> pendingMethodStarts = new();
            HashSet<int> visitedMethodStarts = new();
            pendingMethodStarts.Push(methodStart);

            while (pendingMethodStarts.Count > 0)
            {
                int currentStart = pendingMethodStarts.Pop();
                if (!visitedMethodStarts.Add(currentStart))
                    continue;

                int currentEnd = GetMethodEnd(currentStart, sortedOffsets);
                foreach ((int addr, VM.Instruction instruction) in instructions)
                {
                    if (addr < currentStart)
                        continue;
                    if (addr >= currentEnd)
                        break;

                    if (instruction.OpCode == OpCode.SYSCALL)
                    {
                        if (instruction.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Local_Put.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Local_Delete.Hash)
                            hasStorageWrite = true;

                        if (instruction.TokenU32 == ApplicationEngine.System_Runtime_CheckWitness.Hash)
                            hasCheckWitness = true;

                        continue;
                    }

                    if (instruction.OpCode == OpCode.CALL || instruction.OpCode == OpCode.CALL_L)
                    {
                        int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                        if (methodStartOffsets.Contains(target))
                            pendingMethodStarts.Push(target);
                    }
                }
            }

            return (hasStorageWrite, hasCheckWitness);
        }

        private static int GetMethodEnd(int methodStart, int[] sortedOffsets)
        {
            int methodIndex = Array.BinarySearch(sortedOffsets, methodStart);
            if (methodIndex < 0)
                methodIndex = ~methodIndex;

            return methodIndex + 1 < sortedOffsets.Length
                ? sortedOffsets[methodIndex + 1]
                : int.MaxValue;
        }
    }
}
