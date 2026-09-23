// Copyright (C) 2015-2026 The Neo Project.
//
// CheckWitnessAnalyzer.cs file belongs to the neo project and is free
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
    /// Always Assert(CheckWitness(someone))
    /// Do not just CheckWitness(someone)
    /// </summary>
    public static class CheckWitnessAnalyzer
    {
        public class CheckWitnessVulnerability
        {
            public readonly List<int> droppedCheckWitnessResults;
            public readonly JToken? debugInfo;
            public CheckWitnessVulnerability(
                List<int> droppedCheckWitnessResults,
                JToken? debugInfo = null)
            {
                this.droppedCheckWitnessResults = droppedCheckWitnessResults;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (droppedCheckWitnessResults.Count == 0)
                    return "";
                string result = $"[SECURITY] The returned values of CheckWitness at the following instruction addresses are DROPped:{Environment.NewLine}" +
                    $"\t{string.Join(", ", droppedCheckWitnessResults)}{Environment.NewLine}" +
                    $"You should typically `Assert(CheckWitness({nameof(UInt160)} someone))`{Environment.NewLine}" +
                    $"instead of just `CheckWitness({nameof(UInt160)} someone)`{Environment.NewLine}";
                if (print)
                    Console.Write(result);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        public static CheckWitnessVulnerability AnalyzeCheckWitness
            (NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            (int addr, VM.Instruction instruction)[] instructions = ((Script)nef.Script).EnumerateInstructions().ToArray();
            List<int> result = [];
            HashSet<int> methodStartOffsets = manifest.Abi.Methods.Select(m => m.Offset).ToHashSet();
            foreach ((int addr, VM.Instruction instruction) in instructions)
            {
                if (instruction.OpCode is not (OpCode.CALL or OpCode.CALL_L))
                    continue;

                int target = Neo.Compiler.ControlFlow.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target >= 0)
                    methodStartOffsets.Add(target);
            }
            int[] sortedMethodStarts = methodStartOffsets.OrderBy(offset => offset).ToArray();

            for (int i = 0; i < instructions.Length; ++i)
            {
                VM.Instruction instruction = instructions[i].instruction;
                if (instruction.OpCode == OpCode.SYSCALL && instruction.TokenU32 == ApplicationEngine.System_Runtime_CheckWitness.Hash)
                {
                    if (IsDroppedCheckWitnessResult(instructions, i, GetMethodEnd(instructions[i].addr, sortedMethodStarts)))
                        result.Add(instructions[i].addr);
                }
            }
            return new CheckWitnessVulnerability(result);
        }

        private static bool IsDroppedCheckWitnessResult(
            (int addr, VM.Instruction instruction)[] instructions,
            int checkWitnessIndex,
            int methodEnd)
        {
            int i = checkWitnessIndex + 1;
            while (i < instructions.Length && instructions[i].addr < methodEnd && instructions[i].instruction.OpCode == OpCode.NOP)
                i++;

            if (i >= instructions.Length || instructions[i].addr >= methodEnd)
                return false;

            if (instructions[i].instruction.OpCode == OpCode.DROP)
                return true;

            if (!TryGetLocalStoreSlot(instructions[i].instruction, out byte slot))
                return false;

            for (i++; i < instructions.Length && instructions[i].addr < methodEnd; i++)
            {
                OpCode opcode = instructions[i].instruction.OpCode;
                if (opcode == OpCode.NOP)
                    continue;

                if (TryGetLocalLoadSlot(instructions[i].instruction, out byte loadedSlot))
                {
                    if (loadedSlot != slot)
                        return false;

                    for (i++; i < instructions.Length && instructions[i].addr < methodEnd; i++)
                    {
                        if (instructions[i].instruction.OpCode == OpCode.NOP)
                            continue;
                        return instructions[i].instruction.OpCode == OpCode.DROP;
                    }
                    return false;
                }

                // The result was overwritten or discarded by returning before it could
                // influence control flow. Both cases leave the witness decision unused.
                if (TryGetLocalStoreSlot(instructions[i].instruction, out byte storedSlot) && storedSlot == slot)
                    return true;
                if (opcode == OpCode.RET)
                    return true;
            }

            return false;
        }

        private static int GetMethodEnd(int instructionAddress, int[] sortedMethodStarts)
        {
            if (sortedMethodStarts.Length == 0)
                return int.MaxValue;

            int methodIndex = Array.BinarySearch(sortedMethodStarts, instructionAddress);
            if (methodIndex < 0)
                methodIndex = ~methodIndex - 1;

            return methodIndex + 1 < sortedMethodStarts.Length
                ? sortedMethodStarts[methodIndex + 1]
                : int.MaxValue;
        }

        private static bool TryGetLocalStoreSlot(VM.Instruction instruction, out byte slot)
        {
            switch (instruction.OpCode)
            {
                case OpCode.STLOC0:
                case OpCode.STLOC1:
                case OpCode.STLOC2:
                case OpCode.STLOC3:
                case OpCode.STLOC4:
                case OpCode.STLOC5:
                case OpCode.STLOC6:
                    slot = (byte)(instruction.OpCode - OpCode.STLOC0);
                    return true;
                case OpCode.STLOC:
                    slot = instruction.TokenU8;
                    return true;
                default:
                    slot = 0;
                    return false;
            }
        }

        private static bool TryGetLocalLoadSlot(VM.Instruction instruction, out byte slot)
        {
            switch (instruction.OpCode)
            {
                case OpCode.LDLOC0:
                case OpCode.LDLOC1:
                case OpCode.LDLOC2:
                case OpCode.LDLOC3:
                case OpCode.LDLOC4:
                case OpCode.LDLOC5:
                case OpCode.LDLOC6:
                    slot = (byte)(instruction.OpCode - OpCode.LDLOC0);
                    return true;
                case OpCode.LDLOC:
                    slot = instruction.TokenU8;
                    return true;
                default:
                    slot = 0;
                    return false;
            }
        }
    }
}
