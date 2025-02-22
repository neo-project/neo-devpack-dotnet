// Copyright (C) 2015-2024 The Neo Project.
//
// EntryPoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Optimizer
{
    public enum EntryType
    {
        PublicMethod,
        Initialize,
        Deploy,
        PUSHA,
    }

    public static class EntryPoint
    {
        /// <summary>
        /// Gets a dictionary of method entry points based on the contract manifest and debug information.
        /// </summary>
        /// <param name="manifest">The contract manifest.</param>
        /// <param name="debugInfo">The debug information.</param>
        /// <returns>A dictionary containing method entry points. (addr -> EntryType, hasCallA)</returns>
        public static Dictionary<int, EntryType> EntryPointsByMethod(ContractManifest manifest)
        {
            Dictionary<int, EntryType> result = new();
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
            {
                if (method.Name == "_initialize")
                {
                    result.Add(method.Offset, EntryType.Initialize);
                    continue;
                }
                if (method.Name == "_deploy")
                {
                    result.Add(method.Offset, EntryType.Deploy);
                    continue;
                }
                result.Add(method.Offset, EntryType.PublicMethod);
            }
            return result;
        }

        /// <summary>
        /// Gets a dictionary of entry points based on the CALLA instruction.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <returns>A dictionary containing entry points.</returns>
        public static Dictionary<int, EntryType> EntryPointsByCallA(NefFile nef)
        {
            Dictionary<int, EntryType> result = new();
            Script script = nef.Script;
            List<(int, Instruction)> instructions = script.EnumerateInstructions().ToList();
            bool hasCallA = HasCallA(instructions);
            if (hasCallA)
                foreach ((int addr, Instruction instruction) in instructions)
                    if (instruction.OpCode == OpCode.PUSHA)
                    {
                        int target = JumpTarget.ComputeJumpTarget(addr, instruction);
                        if (target != addr && target >= 0)
                            result[target] = EntryType.PUSHA;
                    }
            return result;
        }

        /// <summary>
        /// Checks if the list of instructions contains the CALLA instruction.
        /// </summary>
        /// <param name="instructions">The list of instructions.</param>
        /// <returns>True if the CALLA instruction exists; otherwise, false.</returns>
        public static bool HasCallA(List<(int, Instruction)> instructions)
        {
            bool hasCallA = false;
            foreach ((_, Instruction instruction) in instructions)
                if (instruction.OpCode == OpCode.CALLA)
                {
                    hasCallA = true;
                    break;
                }
            return hasCallA;
        }

        /// <summary>
        /// Checks if the NEF file contains the CALLA instruction.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <returns>True if the NEF file contains the CALLA instruction; otherwise, false.</returns>
        public static bool HasCallA(NefFile nef)
        {
            Script script = nef.Script;
            return HasCallA(script.EnumerateInstructions().ToList());
        }

        /// <summary>
        /// Gets a dictionary of all entry points, including those calculated based on the CALLA instruction and methods.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <param name="manifest">The contract manifest.</param>
        /// <param name="debugInfo">The debug information.</param>
        /// <returns>A dictionary containing all entry points.</returns>
        public static Dictionary<int, EntryType> AllEntryPoints(NefFile nef, ContractManifest manifest)
            => EntryPointsByCallA(nef).Concat(EntryPointsByMethod(manifest)).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
