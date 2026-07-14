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
using Neo.Compiler.ControlFlow;
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
    /// Detects public methods that perform state changes on a path not dominated by a CheckWitness
    /// call in the method or its static helper calls. Two classes of privileged sink are tracked:
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

            HashSet<int> storageWriteAddresses = new();
            HashSet<int> lifecycleCallAddresses = new();
            HashSet<int> checkWitnessAddresses = new();
            for (int i = 0; i < instructions.Length; i++)
            {
                (int addr, VM.Instruction instruction) = instructions[i];
                if (instruction.OpCode == OpCode.SYSCALL)
                {
                    if (instruction.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                        || instruction.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash
                        || instruction.TokenU32 == ApplicationEngine.System_Storage_Local_Put.Hash
                        || instruction.TokenU32 == ApplicationEngine.System_Storage_Local_Delete.Hash)
                        storageWriteAddresses.Add(addr);

                    if (instruction.TokenU32 == ApplicationEngine.System_Runtime_CheckWitness.Hash)
                        checkWitnessAddresses.Add(addr);

                    if (instruction.TokenU32 == ApplicationEngine.System_Contract_Call.Hash
                        && IsContractManagementLifecycleSyscall(instructions, i))
                        lifecycleCallAddresses.Add(addr);

                    continue;
                }

                if (instruction.OpCode == OpCode.CALLT
                    && IsContractManagementLifecycleToken(nef, instruction.TokenU16))
                    lifecycleCallAddresses.Add(addr);

                // Dynamic calls cannot be resolved through the static call graph here.
                // Treat them as potentially reaching storage writes unless guarded.
                if (instruction.OpCode == OpCode.CALLA)
                    storageWriteAddresses.Add(addr);
            }

            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            HashSet<int> normallyReturningMethods = methodStartOffsets
                .Where(offset => contractInBasicBlocks.basicBlocksByStartAddr.TryGetValue(offset, out BasicBlock? block)
                    && block.branchType == BranchType.OK)
                .ToHashSet();
            HashSet<int> witnessGuaranteedMethods = FindWitnessGuaranteedMethods(
                sortedOffsets,
                contractInBasicBlocks,
                normallyReturningMethods,
                checkWitnessAddresses);
            Dictionary<int, MethodAnalysisResult> methodResults =
                methodStartOffsets.ToDictionary(offset => offset, _ => default(MethodAnalysisResult));

            // Method summaries are monotonic. Iterating to a fixed point carries unguarded sinks
            // through nested and recursive static helper calls.
            bool changed;
            do
            {
                changed = false;
                foreach (int methodStart in sortedOffsets)
                {
                    MethodAnalysisResult result = AnalyzeMethod(
                        methodStart,
                        GetMethodEnd(methodStart, sortedOffsets),
                        contractInBasicBlocks,
                        methodResults,
                        witnessGuaranteedMethods,
                        storageWriteAddresses,
                        lifecycleCallAddresses,
                        checkWitnessAddresses);
                    MethodAnalysisResult previous = methodResults[methodStart];
                    MethodAnalysisResult combined = previous.Union(result);
                    if (!combined.Equals(previous))
                    {
                        methodResults[methodStart] = combined;
                        changed = true;
                    }
                }
            }
            while (changed);

            List<string> vulnerableMethods = new();
            List<string> unauthenticatedLifecycleMethods = new();

            foreach (ContractMethodDescriptor method in methods)
            {
                if (method.Name is "_deploy" or "_initialize")
                    continue;

                MethodAnalysisResult result = methodResults[method.Offset];
                if (result.HasUnguardedStorageWrite)
                    vulnerableMethods.Add(method.Name);
                if (result.HasUnguardedLifecycleCall)
                    unauthenticatedLifecycleMethods.Add(method.Name);
            }

            return new MissingCheckWitnessVulnerability(vulnerableMethods, unauthenticatedLifecycleMethods, debugInfo);
        }

        private static HashSet<int> FindWitnessGuaranteedMethods(
            int[] sortedOffsets,
            ContractInBasicBlocks contractInBasicBlocks,
            HashSet<int> normallyReturningMethods,
            HashSet<int> checkWitnessAddresses)
        {
            HashSet<int> witnessGuaranteedMethods = new();
            Dictionary<int, MethodAnalysisResult> noMethodResults = new();
            HashSet<int> noSinkAddresses = new();

            // Start with no trusted helpers. A method becomes trusted only after every normal
            // return path is proven to cross a direct or already-proven witness check.
            bool changed;
            do
            {
                changed = false;
                foreach (int methodStart in sortedOffsets)
                {
                    if (!normallyReturningMethods.Contains(methodStart)
                        || witnessGuaranteedMethods.Contains(methodStart))
                        continue;

                    MethodAnalysisResult result = AnalyzeMethod(
                        methodStart,
                        GetMethodEnd(methodStart, sortedOffsets),
                        contractInBasicBlocks,
                        noMethodResults,
                        witnessGuaranteedMethods,
                        noSinkAddresses,
                        noSinkAddresses,
                        checkWitnessAddresses);
                    if (!result.ReturnsWithoutWitness)
                    {
                        witnessGuaranteedMethods.Add(methodStart);
                        changed = true;
                    }
                }
            }
            while (changed);

            return witnessGuaranteedMethods;
        }

        private static MethodAnalysisResult AnalyzeMethod(
            int methodStart,
            int methodEnd,
            ContractInBasicBlocks contractInBasicBlocks,
            IReadOnlyDictionary<int, MethodAnalysisResult> methodResults,
            HashSet<int> witnessGuaranteedMethods,
            HashSet<int> storageWriteAddresses,
            HashSet<int> lifecycleCallAddresses,
            HashSet<int> checkWitnessAddresses)
        {
            if (!contractInBasicBlocks.basicBlocksByStartAddr.TryGetValue(methodStart, out BasicBlock? entryBlock))
                return default;

            bool hasUnguardedStorageWrite = false;
            bool hasUnguardedLifecycleCall = false;
            bool returnsWithoutWitness = false;
            HashSet<BasicBlock> reachableWithoutWitness = new() { entryBlock };
            Queue<BasicBlock> pendingBlocks = new(reachableWithoutWitness);

            while (pendingBlocks.Count > 0)
            {
                BasicBlock block = pendingBlocks.Dequeue();
                bool remainsUnguarded = true;
                int addr = block.startAddr;
                foreach (VM.Instruction instruction in block.instructions)
                {
                    if (storageWriteAddresses.Contains(addr))
                        hasUnguardedStorageWrite = true;
                    if (lifecycleCallAddresses.Contains(addr))
                        hasUnguardedLifecycleCall = true;

                    if (checkWitnessAddresses.Contains(addr))
                    {
                        remainsUnguarded = false;
                        break;
                    }

                    if (instruction.OpCode == OpCode.CALL || instruction.OpCode == OpCode.CALL_L)
                    {
                        int target = Neo.Compiler.ControlFlow.JumpTarget.ComputeJumpTarget(addr, instruction);
                        if (methodResults.TryGetValue(target, out MethodAnalysisResult calledMethodResult))
                        {
                            hasUnguardedStorageWrite |= calledMethodResult.HasUnguardedStorageWrite;
                            hasUnguardedLifecycleCall |= calledMethodResult.HasUnguardedLifecycleCall;
                        }

                        if (witnessGuaranteedMethods.Contains(target))
                        {
                            remainsUnguarded = false;
                            break;
                        }
                    }

                    if (instruction.OpCode == OpCode.RET)
                        returnsWithoutWitness = true;

                    addr += instruction.Size;
                }

                if (!remainsUnguarded)
                    continue;

                if (block.nextBlock != null
                    && IsInMethod(block.nextBlock, methodStart, methodEnd)
                    && reachableWithoutWitness.Add(block.nextBlock))
                    pendingBlocks.Enqueue(block.nextBlock);

                foreach (BasicBlock targetBlock in block.jumpTargetBlocks)
                    if (IsInMethod(targetBlock, methodStart, methodEnd)
                        && reachableWithoutWitness.Add(targetBlock))
                        pendingBlocks.Enqueue(targetBlock);
            }

            return new MethodAnalysisResult(
                hasUnguardedStorageWrite,
                hasUnguardedLifecycleCall,
                returnsWithoutWitness);
        }

        private static bool IsInMethod(BasicBlock block, int methodStart, int methodEnd)
            => block.startAddr >= methodStart && block.startAddr < methodEnd;

        private readonly struct MethodAnalysisResult : IEquatable<MethodAnalysisResult>
        {
            public bool HasUnguardedStorageWrite { get; }
            public bool HasUnguardedLifecycleCall { get; }
            public bool ReturnsWithoutWitness { get; }

            public MethodAnalysisResult(
                bool hasUnguardedStorageWrite,
                bool hasUnguardedLifecycleCall,
                bool returnsWithoutWitness)
            {
                HasUnguardedStorageWrite = hasUnguardedStorageWrite;
                HasUnguardedLifecycleCall = hasUnguardedLifecycleCall;
                ReturnsWithoutWitness = returnsWithoutWitness;
            }

            public MethodAnalysisResult Union(MethodAnalysisResult other)
                => new(
                    HasUnguardedStorageWrite || other.HasUnguardedStorageWrite,
                    HasUnguardedLifecycleCall || other.HasUnguardedLifecycleCall,
                    ReturnsWithoutWitness || other.ReturnsWithoutWitness);

            public bool Equals(MethodAnalysisResult other)
                => HasUnguardedStorageWrite == other.HasUnguardedStorageWrite
                    && HasUnguardedLifecycleCall == other.HasUnguardedLifecycleCall
                    && ReturnsWithoutWitness == other.ReturnsWithoutWitness;
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
