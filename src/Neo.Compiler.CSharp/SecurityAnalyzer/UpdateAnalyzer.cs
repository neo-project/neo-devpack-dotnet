using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.Compiler.SecurityAnalyzer
{
    public static class UpdateAnalzyer
    {
        /// <summary>
        /// This method checks whether it is possible to update the contract.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns>whether it is possible to update the contract. May be wrong.</returns>
        public static bool AnalyzeUpdate
            (NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            (nef, manifest, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, null);
            (int addr, VM.Instruction instruction)[] instructions = ((Script)nef.Script).EnumerateInstructions().ToArray();
            byte[] update = System.Text.Encoding.UTF8.GetBytes("update");
            for (int i = 0; i < instructions.Length; ++i)
            {
                VM.Instruction instruction = instructions[i].instruction;
                if (instruction.OpCode == OpCode.CALLT)
                {
                    uint tokenId = instruction.TokenU16;
                    MethodToken token = nef.Tokens[tokenId];
                    if (token.Hash == NativeContract.ContractManagement.Hash && token.Method == "update" && ((token.CallFlags | CallFlags.WriteStates) != 0))
                        return true;
                }
                if (i + 2 >= instructions.Length)
                    continue;  // Do not break or return. There can be a CALLT following.
                VM.Instruction instruction1 = instructions[i + 1].instruction;
                VM.Instruction instruction2 = instructions[i + 2].instruction;
                if (instruction.OpCode == OpCode.PUSHDATA1 && instruction.Operand.ToArray().SequenceEqual(update)
                 && instruction1.OpCode == OpCode.PUSHDATA1 && instruction1.Operand.ToArray() == NativeContract.ContractManagement.Hash
                 && instruction2.OpCode == OpCode.SYSCALL && instruction2.TokenU32 == ApplicationEngine.System_Contract_Call.Hash)
                    return true;
            }
            return false;
        }
    }
}
