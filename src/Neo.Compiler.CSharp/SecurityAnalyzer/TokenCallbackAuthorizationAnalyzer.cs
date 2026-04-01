// Copyright (C) 2015-2026 The Neo Project.
//
// TokenCallbackAuthorizationAnalyzer.cs file belongs to the neo project and is free
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
    public static class TokenCallbackAuthorizationAnalyzer
    {
        public class TokenCallbackAuthorizationVulnerability
        {
            public readonly IReadOnlyList<string> vulnerableMethodNames;
            public readonly JToken? debugInfo;

            public TokenCallbackAuthorizationVulnerability(
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

                string result = $"[SECURITY] The following token payment callbacks write storage without validating Runtime.CallingScriptHash:{Environment.NewLine}" +
                    $"\t{string.Join(", ", vulnerableMethodNames)}{Environment.NewLine}" +
                    $"Validate the calling token contract hash in NEP-17/NEP-11 payment callbacks before mutating state.{Environment.NewLine}";
                if (print)
                    Console.Write(result);
                return result;
            }
        }

        public static TokenCallbackAuthorizationVulnerability AnalyzeTokenCallbacks(
            NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            (int addr, VM.Instruction instruction)[] instructions =
                ((Script)nef.Script).EnumerateInstructions().ToArray();

            ContractMethodDescriptor[] methods = manifest.Abi.Methods;
            HashSet<int> methodStartOffsets = methods.Select(m => m.Offset).ToHashSet();
            foreach ((int addr, VM.Instruction instruction) in instructions)
            {
                if (instruction.OpCode != VM.OpCode.CALL && instruction.OpCode != VM.OpCode.CALL_L)
                    continue;

                int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target >= 0)
                    methodStartOffsets.Add(target);
            }
            int[] sortedOffsets = methodStartOffsets.OrderBy(o => o).ToArray();

            var callbackMethods = methods.Where(m =>
                string.Equals(m.Name, "onNEP17Payment", StringComparison.Ordinal) ||
                string.Equals(m.Name, "onNEP11Payment", StringComparison.Ordinal));

            List<string> vulnerableMethods = new();
            foreach (ContractMethodDescriptor method in callbackMethods)
            {
                (bool hasStorageWrite, bool hasCallingScriptHashValidation) = AnalyzeMethodAndStaticHelpers(
                    method.Offset,
                    instructions,
                    sortedOffsets,
                    methodStartOffsets);

                if (hasStorageWrite && !hasCallingScriptHashValidation)
                    vulnerableMethods.Add(method.Name);
            }

            return new TokenCallbackAuthorizationVulnerability(vulnerableMethods, debugInfo);
        }

        private static (bool hasStorageWrite, bool hasCallingScriptHashValidation) AnalyzeMethodAndStaticHelpers(
            int methodStart,
            (int addr, VM.Instruction instruction)[] instructions,
            int[] sortedOffsets,
            HashSet<int> methodStartOffsets)
        {
            bool hasStorageWrite = false;
            bool hasCallingScriptHashValidation = false;

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

                    if (instruction.OpCode == VM.OpCode.SYSCALL)
                    {
                        if (instruction.TokenU32 == ApplicationEngine.System_Storage_Put.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Delete.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Local_Put.Hash
                            || instruction.TokenU32 == ApplicationEngine.System_Storage_Local_Delete.Hash)
                            hasStorageWrite = true;

                        if (instruction.TokenU32 == ApplicationEngine.System_Runtime_GetCallingScriptHash.Hash
                            && IsCallingScriptHashUsedDefensively(instructions, addr, currentEnd))
                            hasCallingScriptHashValidation = true;

                        continue;
                    }

                    if (instruction.OpCode == VM.OpCode.CALL || instruction.OpCode == VM.OpCode.CALL_L)
                    {
                        int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                        if (methodStartOffsets.Contains(target))
                            pendingMethodStarts.Push(target);
                    }
                }
            }

            return (hasStorageWrite, hasCallingScriptHashValidation);
        }

        private static bool IsCallingScriptHashUsedDefensively(
            (int addr, VM.Instruction instruction)[] instructions,
            int callingScriptHashAddr,
            int methodEnd)
        {
            int startIndex = Array.FindIndex(instructions, item => item.addr == callingScriptHashAddr);
            if (startIndex < 0)
                return false;

            int inspected = 0;
            for (int i = startIndex + 1; i < instructions.Length && instructions[i].addr < methodEnd && inspected < 6; i++)
            {
                VM.Instruction instruction = instructions[i].instruction;
                if (instruction.OpCode == VM.OpCode.NOP)
                    continue;

                inspected++;

                if (instruction.OpCode == VM.OpCode.DROP)
                    return false;

                if (instruction.OpCode is VM.OpCode.EQUAL
                    or VM.OpCode.NOTEQUAL
                    or VM.OpCode.JMPEQ
                    or VM.OpCode.JMPEQ_L
                    or VM.OpCode.JMPNE
                    or VM.OpCode.JMPNE_L
                    or VM.OpCode.JMPIF
                    or VM.OpCode.JMPIF_L
                    or VM.OpCode.JMPIFNOT
                    or VM.OpCode.JMPIFNOT_L
                    or VM.OpCode.ASSERT)
                    return true;
            }

            return false;
        }

        private static int GetMethodEnd(int methodStart, int[] sortedOffsets)
        {
            int methodIndex = Array.BinarySearch(sortedOffsets, methodStart);

            return methodIndex + 1 < sortedOffsets.Length
                ? sortedOffsets[methodIndex + 1]
                : int.MaxValue;
        }
    }
}
