using Neo.Json;
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
        public static Dictionary<int, EntryType> EntryPointsByMethod(ContractManifest manifest, JToken debugInfo)
        {
            Dictionary<int, EntryType> result = new();
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
            {
                if (method.Name == "_initialize")
                    result.Add(method.Offset, EntryType.Initialize);
                else
                    result.Add(method.Offset, EntryType.PublicMethod);
            }
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                string name = method!["name"]!.AsString();  // NFTLoan.NFTLoan,RegisterRental
                name = name[(name.LastIndexOf(',') + 1)..];  // RegisterRental
                name = char.ToLower(name[0]) + name[1..];  // registerRental
                if (name == "_deploy")
                {
                    int startAddr = int.Parse(method!["range"]!.AsString().Split("-")[0]);
                    result[startAddr] = EntryType.Initialize;  // set instead of add; _deploy may be in the manifest
                }
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
                            result.Add(addr, EntryType.PUSHA);
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
        public static Dictionary<int, EntryType> AllEntryPoints(NefFile nef, ContractManifest manifest, JToken debugInfo)
            => EntryPointsByCallA(nef).Concat(EntryPointsByMethod(manifest, debugInfo)).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
