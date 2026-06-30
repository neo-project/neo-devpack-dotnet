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

using Neo.Compiler.ControlFlow;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.SecurityAnalyzer
{
    /// <summary>
    /// Detects public methods that perform state changes without any CheckWitness call in the
    /// method or its static helper calls. Two classes of privileged sink are tracked:
    /// <list type="bullet">
    /// <item>Persistent storage writes (<c>Storage.Put</c> / <c>Storage.Delete</c>).</item>
    /// <item>Contract-lifecycle calls (<c>ContractManagement.Update</c> / <c>Destroy</c>), which
    /// replace or remove the deployed contract and are at least as dangerous as a storage write.</item>
    /// </list>
    /// </summary>
    public static class MissingCheckWitnessAnalyzer
    {
        public class MissingCheckWitnessVulnerability
        {
            public readonly IReadOnlyList<string> vulnerableMethodNames;

            /// <summary>
            /// Public methods that reach <c>ContractManagement.Update</c> or <c>Destroy</c> without
            /// a CheckWitness call. These let any caller replace or remove the deployed contract.
            /// </summary>
            public readonly IReadOnlyList<string> unauthenticatedLifecycleMethodNames;
            public readonly JToken? debugInfo;

            public MissingCheckWitnessVulnerability(
                IReadOnlyList<string> vulnerableMethodNames,
                IReadOnlyList<string>? unauthenticatedLifecycleMethodNames = null,
                JToken? debugInfo = null)
            {
                this.vulnerableMethodNames = vulnerableMethodNames;
                this.unauthenticatedLifecycleMethodNames = unauthenticatedLifecycleMethodNames ?? Array.Empty<string>();
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                string result = "";
                if (vulnerableMethodNames.Count > 0)
                    result += $"[SECURITY] The following public methods write to storage without CheckWitness verification:{Environment.NewLine}" +
                        $"\t{string.Join(", ", vulnerableMethodNames)}{Environment.NewLine}" +
                        $"Consider adding `Runtime.CheckWitness()` before performing storage writes to prevent unauthorized access.{Environment.NewLine}";
                if (unauthenticatedLifecycleMethodNames.Count > 0)
                    result += $"[SECURITY] The following public methods call ContractManagement.Update/Destroy without CheckWitness verification:{Environment.NewLine}" +
                        $"\t{string.Join(", ", unauthenticatedLifecycleMethodNames)}{Environment.NewLine}" +
                        $"Any caller can replace or destroy the deployed contract. Add an owner/witness check (e.g. `Runtime.CheckWitness()`) before updating or destroying the contract.{Environment.NewLine}";
                if (print && result.Length > 0)
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

                int target = Neo.Compiler.ControlFlow.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target >= 0)
                    methodStartOffsets.Add(target);
            }
            int[] sortedOffsets = methodStartOffsets.OrderBy(o => o).ToArray();

            // Optional control-flow graph used only to refine the witness-present case below.
            // Building it must never change the baseline result, so any failure leaves it null.
            ContractInBasicBlocks? cfg = TryBuildCfg(nef, manifest, debugInfo);

            List<string> vulnerableMethods = new();
            List<string> unauthenticatedLifecycleMethods = new();

            foreach (ContractMethodDescriptor method in methods)
            {
                if (method.Name is "_deploy" or "_initialize")
                    continue;

                (bool hasStorageWrite, bool hasLifecycleCall, bool hasCheckWitness) = AnalyzeMethodAndStaticHelpers(
                    method.Offset,
                    nef,
                    instructions,
                    sortedOffsets,
                    methodStartOffsets);

                if (!hasCheckWitness)
                {
                    if (hasStorageWrite)
                        vulnerableMethods.Add(method.Name);
                    if (hasLifecycleCall)
                        unauthenticatedLifecycleMethods.Add(method.Name);
                    continue;
                }

                // A witness exists somewhere. The baseline conservatively treats the method as safe.
                // Refine with a control-flow-sensitive pass: only report when a storage write is
                // provably reachable on a path that no witness check dominates. The refinement is
                // sound (no false positives) and bails out (leaving the method unreported) whenever
                // the control flow cannot be modelled precisely.
                if (hasStorageWrite
                    && cfg is not null
                    && WitnessFlowAnalysis.HasUnguardedWrite(cfg, method.Offset) is true)
                    vulnerableMethods.Add(method.Name);
            }

            return new MissingCheckWitnessVulnerability(vulnerableMethods, unauthenticatedLifecycleMethods, debugInfo);
        }

        private static ContractInBasicBlocks? TryBuildCfg(NefFile nef, ContractManifest manifest, JToken? debugInfo)
        {
            try
            {
                return new ContractInBasicBlocks(nef, manifest, debugInfo);
            }
            catch
            {
                // Synthetic or malformed scripts may not form a clean CFG; refinement is skipped.
                return null;
            }
        }

        private static (bool hasStorageWrite, bool hasLifecycleCall, bool hasCheckWitness) AnalyzeMethodAndStaticHelpers(
            int methodStart,
            NefFile nef,
            (int addr, VM.Instruction instruction)[] instructions,
            int[] sortedOffsets,
            HashSet<int> methodStartOffsets)
        {
            bool hasStorageWrite = false;
            bool hasLifecycleCall = false;
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
                for (int i = 0; i < instructions.Length; i++)
                {
                    (int addr, VM.Instruction instruction) = instructions[i];
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

                        // ContractManagement.Update/Destroy invoked via the dynamic System.Contract.Call
                        // syscall, preceded by PUSHDATA1 "<method>" / PUSHDATA1 <ContractManagement hash>.
                        if (instruction.TokenU32 == ApplicationEngine.System_Contract_Call.Hash
                            && IsContractManagementLifecycleSyscall(instructions, i))
                            hasLifecycleCall = true;

                        continue;
                    }

                    // ContractManagement.Update/Destroy invoked through a method token (CALLT).
                    if (instruction.OpCode == OpCode.CALLT
                        && IsContractManagementLifecycleToken(nef, instruction.TokenU16))
                        hasLifecycleCall = true;

                    if (instruction.OpCode == OpCode.CALLA)
                    {
                        // Dynamic calls cannot be resolved through the static call graph here.
                        // Treat them as potentially reaching storage writes unless guarded.
                        hasStorageWrite = true;
                        continue;
                    }

                    if (instruction.OpCode == OpCode.CALL || instruction.OpCode == OpCode.CALL_L)
                    {
                        int target = Neo.Compiler.ControlFlow.JumpTarget.ComputeJumpTarget(addr, instruction);
                        if (methodStartOffsets.Contains(target))
                            pendingMethodStarts.Push(target);
                    }
                }
            }

            return (hasStorageWrite, hasLifecycleCall, hasCheckWitness);
        }

        private static readonly byte[] s_updateMethodBytes = System.Text.Encoding.UTF8.GetBytes("update");
        private static readonly byte[] s_destroyMethodBytes = System.Text.Encoding.UTF8.GetBytes("destroy");

        /// <summary>
        /// Returns <see langword="true"/> when the CALLT token at <paramref name="tokenId"/> targets
        /// <c>ContractManagement.update</c> or <c>destroy</c> with the <see cref="CallFlags.WriteStates"/>
        /// flag set (the only call shapes that mutate or remove contract state).
        /// </summary>
        private static bool IsContractManagementLifecycleToken(NefFile nef, ushort tokenId)
        {
            if (tokenId >= nef.Tokens.Length)
                return false;
            MethodToken token = nef.Tokens[tokenId];
            return token.Hash == NativeContract.ContractManagement.Hash
                && (token.Method == "update" || token.Method == "destroy")
                && (token.CallFlags & CallFlags.WriteStates) != 0;
        }

        /// <summary>
        /// Returns <see langword="true"/> when the System.Contract.Call syscall at
        /// <paramref name="syscallIndex"/> is preceded by the constant operand pushes that target
        /// <c>ContractManagement.update</c> or <c>destroy</c>, mirroring <see cref="UpdateAnalyzer"/>.
        /// </summary>
        private static bool IsContractManagementLifecycleSyscall(
            (int addr, VM.Instruction instruction)[] instructions, int syscallIndex)
        {
            if (syscallIndex < 2)
                return false;

            VM.Instruction methodPush = instructions[syscallIndex - 2].instruction;
            VM.Instruction hashPush = instructions[syscallIndex - 1].instruction;

            if (methodPush.OpCode != OpCode.PUSHDATA1 || hashPush.OpCode != OpCode.PUSHDATA1)
                return false;

            if (!hashPush.Operand.Span.SequenceEqual(NativeContract.ContractManagement.Hash.GetSpan()))
                return false;

            ReadOnlySpan<byte> method = methodPush.Operand.Span;
            return method.SequenceEqual(s_updateMethodBytes) || method.SequenceEqual(s_destroyMethodBytes);
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
