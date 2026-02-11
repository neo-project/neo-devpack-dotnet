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
            public readonly List<int> backwardJumpAddresses;
            public readonly JToken? debugInfo;

            public UnboundedOperationVulnerability(
                List<int> backwardJumpAddresses,
                JToken? debugInfo = null)
            {
                this.backwardJumpAddresses = backwardJumpAddresses;
                this.debugInfo = debugInfo;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (backwardJumpAddresses.Count == 0)
                    return "";
                string result = $"[SEC] Potential unbounded operations (backward jumps) detected at instruction addresses:{Environment.NewLine}" +
                    $"\t{string.Join(", ", backwardJumpAddresses)}{Environment.NewLine}" +
                    $"Unbounded loops can lead to excessive GAS consumption (DoS). Consider adding iteration limits.{Environment.NewLine}";
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

            foreach ((int addr, VM.Instruction instruction) in instructions)
            {
                if (!IsJumpInstruction(instruction.OpCode))
                    continue;

                int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                if (target < addr)
                    backwardJumps.Add(addr);
            }

            return new UnboundedOperationVulnerability(backwardJumps, debugInfo);
        }

        private static bool IsJumpInstruction(OpCode opCode)
        {
            return opCode == OpCode.JMP
                || opCode == OpCode.JMP_L
                || opCode == OpCode.JMPIF
                || opCode == OpCode.JMPIF_L
                || opCode == OpCode.JMPIFNOT
                || opCode == OpCode.JMPIFNOT_L
                || opCode == OpCode.JMPEQ
                || opCode == OpCode.JMPEQ_L
                || opCode == OpCode.JMPNE
                || opCode == OpCode.JMPNE_L
                || opCode == OpCode.JMPGT
                || opCode == OpCode.JMPGT_L
                || opCode == OpCode.JMPGE
                || opCode == OpCode.JMPGE_L
                || opCode == OpCode.JMPLT
                || opCode == OpCode.JMPLT_L
                || opCode == OpCode.JMPLE
                || opCode == OpCode.JMPLE_L;
        }
    }
}
