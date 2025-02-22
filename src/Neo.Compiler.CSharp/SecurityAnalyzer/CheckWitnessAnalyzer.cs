// Copyright (C) 2015-2024 The Neo Project.
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
                string result = $"[SEC] The returned values of CheckWitness at the following instruction addresses are DROPped:{Environment.NewLine}" +
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
            for (int i = 0; i < instructions.Length; ++i)
            {
                VM.Instruction instruction = instructions[i].instruction;
                if (instruction.OpCode == OpCode.SYSCALL && instruction.TokenU32 == ApplicationEngine.System_Runtime_CheckWitness.Hash)
                {
                    if (i + 1 >= instructions.Length)
                        return new CheckWitnessVulnerability(result);
                    if (instructions[i + 1].instruction.OpCode == OpCode.DROP)
                        result.Add(i);
                }
            }
            return new CheckWitnessVulnerability(result);
        }
    }
}
