// Copyright (C) 2015-2026 The Neo Project.
//
// EntryPoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using VmInstruction = Neo.VM.Instruction;

namespace Neo.Compiler.ControlFlow
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
        /// Gets a dictionary of entry points referenced by PUSHA instructions.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <returns>A dictionary containing entry points.</returns>
        internal static Dictionary<int, EntryType> EntryPointsByPusha(NefFile nef)
        {
            Script script = nef.Script;
            return EntryPointsByPusha(script.EnumerateInstructions().ToList());
        }

        private static Dictionary<int, EntryType> EntryPointsByPusha(List<(int, VmInstruction)> instructions)
        {
            Dictionary<int, EntryType> result = new();
            foreach ((int addr, VmInstruction instruction) in instructions)
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
        public static bool HasCallA(List<(int, VmInstruction)> instructions)
        {
            bool hasCallA = false;
            foreach ((_, VmInstruction instruction) in instructions)
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
        /// Gets a dictionary of all method and PUSHA entry points.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <param name="manifest">The contract manifest.</param>
        /// <returns>A dictionary containing all entry points.</returns>
        public static Dictionary<int, EntryType> AllEntryPoints(NefFile nef, ContractManifest manifest)
        {
            Dictionary<int, EntryType> result = EntryPointsByPusha(nef);
            foreach ((int address, EntryType entryType) in EntryPointsByMethod(manifest))
                result[address] = entryType;
            return result;
        }
    }
}
