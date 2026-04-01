// Copyright (C) 2015-2026 The Neo Project.
//
// UnboundedOperationAnalyzer.cs file belongs to the neo project and is free
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
    /// Detects potential Gas DoS patterns by identifying backward jumps
    /// that could indicate unbounded loops.
    /// </summary>
    public static class UnboundedOperationAnalyzer
    {
        public class UnboundedOperationVulnerability
        {
            public readonly IReadOnlyList<int> backwardJumpAddresses;
            public readonly IReadOnlyList<int> recursiveCallAddresses;
            public readonly JToken? debugInfo;

            public UnboundedOperationVulnerability(
                IReadOnlyList<int> backwardJumpAddresses,
                IReadOnlyList<int> recursiveCallAddresses,
                JToken? debugInfo = null)
            {
                this.backwardJumpAddresses = backwardJumpAddresses;
                this.recursiveCallAddresses = recursiveCallAddresses;
                this.debugInfo = debugInfo;
            }

            public UnboundedOperationVulnerability(
                IReadOnlyList<int> backwardJumpAddresses,
                JToken? debugInfo = null)
                : this(backwardJumpAddresses, Array.Empty<int>(), debugInfo)
            {
            }

            public string GetWarningInfo(bool print = false)
            {
                if (backwardJumpAddresses.Count == 0 && recursiveCallAddresses.Count == 0)
                    return "";

                string result = "[SECURITY] Potential unbounded operations detected";
                if (backwardJumpAddresses.Count > 0)
                {
                    result += $"{Environment.NewLine}Backward jumps at instruction addresses:{Environment.NewLine}" +
                        $"\t{string.Join(", ", backwardJumpAddresses)}";
                }
                if (recursiveCallAddresses.Count > 0)
                {
                    result += $"{Environment.NewLine}Direct recursive calls at instruction addresses:{Environment.NewLine}" +
                        $"\t{string.Join(", ", recursiveCallAddresses)}";
                }
                result += $"{Environment.NewLine}Unbounded loops or recursion can lead to excessive GAS consumption (DoS). Consider adding iteration limits or recursion guards.{Environment.NewLine}";
                if (print)
                    Console.Write(result);
                return result;
            }
        }

        /// <summary>
        /// Analyzes the contract for backward jumps that may indicate unbounded loops.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        public static UnboundedOperationVulnerability AnalyzeUnboundedOperations(
            NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            (int addr, VM.Instruction instruction)[] instructions =
                ((Script)nef.Script).EnumerateInstructions().ToArray();

            List<int> backwardJumps = new();
            List<int> recursiveCalls = new();

            HashSet<int> methodStartOffsets = manifest.Abi.Methods.Select(m => m.Offset).ToHashSet();
            foreach ((int addr, VM.Instruction instruction) in instructions)
            {
                if (instruction.OpCode != OpCode.CALL && instruction.OpCode != OpCode.CALL_L)
                    continue;

                int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target >= 0)
                    methodStartOffsets.Add(target);
            }
            int[] sortedOffsets = methodStartOffsets.OrderBy(o => o).ToArray();

            foreach ((int addr, VM.Instruction instruction) in instructions)
            {
                if (!instruction.OpCode.IsJumpInstruction())
                    continue;

                int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target < addr)
                    backwardJumps.Add(addr);
            }

            foreach (int methodStart in sortedOffsets)
            {
                int methodEnd = GetMethodEnd(methodStart, sortedOffsets);
                foreach ((int addr, VM.Instruction instruction) in instructions)
                {
                    if (addr < methodStart)
                        continue;
                    if (addr >= methodEnd)
                        break;

                    if (instruction.OpCode != OpCode.CALL && instruction.OpCode != OpCode.CALL_L)
                        continue;

                    int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                    if (target == methodStart)
                        recursiveCalls.Add(addr);
                }
            }

            return new UnboundedOperationVulnerability(backwardJumps, recursiveCalls, debugInfo);
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
