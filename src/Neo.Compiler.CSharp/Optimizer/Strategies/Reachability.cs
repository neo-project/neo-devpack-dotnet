using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;
using static Neo.Optimizer.Optimizer;

namespace Neo.Optimizer
{
    static class Reachability
    {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);
        private static readonly Regex SequencePointRegex = new(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)", RegexOptions.Compiled);
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        [Strategy(Priority = int.MaxValue)]
        public static (NefFile, ContractManifest, JObject) RemoveUncoveredInstructions(NefFile nef, ContractManifest manifest, JObject debugInfo)
        {
            Dictionary<int, BranchType> coveredMap = FindCoveredInstructions(nef, manifest, debugInfo);
            Script oldScript = nef.Script;
            List<(int, Instruction)> oldAddressAndInstructionsList = oldScript.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            //DumpNef.GenerateDumpNef(nef, debugInfo);
            //coveredMap.Where(kv => !kv.Value).Select(kv => (kv.Key, oldAddressToInstruction[kv.Key].OpCode)).ToList();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (coveredMap[a] != BranchType.UNCOVERED)
                {
                    simplifiedInstructionsToAddress.Add(i, currentAddress);
                    currentAddress += i.Size;
                }
                else
                    continue;
            }
            (Dictionary<Instruction, Instruction> jumpInstructionSourceToTargets,
            Dictionary<Instruction, (Instruction, Instruction)> tryInstructionSourceToTargets, _)
            = FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);

            List<byte> simplifiedScript = new();
            foreach (DictionaryEntry item in simplifiedInstructionsToAddress)
            {
                (Instruction i, int a) = ((Instruction)item.Key, (int)item.Value!);
                simplifiedScript.Add((byte)i.OpCode);
                int operandSizeLength = OperandSizePrefixTable[(int)i.OpCode];
                simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(i.Operand.Length)[0..operandSizeLength]).ToList();
                if (jumpInstructionSourceToTargets.TryGetValue(i, out Instruction? dst))
                {
                    int delta;
                    if (simplifiedInstructionsToAddress.Contains(dst))  // target instruction not deleted
                        delta = (int)simplifiedInstructionsToAddress[dst]! - a;
                    else if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.ENDTRY || i.OpCode == OpCode.ENDTRY_L)
                        delta = 0;  // TODO: decide a good target
                    else
                    {
                        foreach ((int oldAddress, Instruction oldInstruction) in oldAddressAndInstructionsList)
                            if (oldInstruction == i)
                                throw new BadScriptException($"Target instruction of {i.OpCode} at old address {oldAddress} is deleted");
                        throw new BadScriptException($"Target instruction of {i.OpCode} at new address {a} is deleted");
                    }
                    if (i.OpCode == OpCode.JMP || conditionalJump.Contains(i.OpCode) || i.OpCode == OpCode.CALL || i.OpCode == OpCode.ENDTRY)
                        simplifiedScript.Add(BitConverter.GetBytes(delta)[0]);
                    if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.JMP_L || conditionalJump_L.Contains(i.OpCode) || i.OpCode == OpCode.CALL_L || i.OpCode == OpCode.ENDTRY_L)
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta)).ToList();
                    continue;
                }
                if (tryInstructionSourceToTargets.TryGetValue(i, out (Instruction dst1, Instruction dst2) dsts))
                {
                    (Instruction dst1, Instruction dst2) = (dsts.dst1, dsts.dst2);
                    (int delta1, int delta2) = ((int)simplifiedInstructionsToAddress[dst1]! - a, (int)simplifiedInstructionsToAddress[dst2]! - a);
                    if (i.OpCode == OpCode.TRY)
                    {
                        simplifiedScript.Add(BitConverter.GetBytes(delta1)[0]);
                        simplifiedScript.Add(BitConverter.GetBytes(delta2)[0]);
                    }
                    if (i.OpCode == OpCode.TRY_L)
                    {
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta1)).ToList();
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta2)).ToList();
                    }
                    continue;
                }
                if (i.Operand.Length != 0)
                    simplifiedScript = simplifiedScript.Concat(i.Operand.ToArray()).ToList();
            }

            //Dictionary<int, (int docId, int startLine, int startCol, int endLine, int endCol)> newAddrToSequencePoint = new();
            HashSet<JToken> methodsToRemove = new();
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                GroupCollection rangeGroups = RangeRegex.Match(method!["range"]!.AsString()).Groups;
                (int oldMethodStart, int oldMethodEnd) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                if (!simplifiedInstructionsToAddress.Contains(oldAddressToInstruction[oldMethodStart]))
                {
                    methodsToRemove.Add(method);
                    continue;
                }
                int methodStart = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodStart]]!;
                // The instruction at the end of the method may have been deleted.
                // We need to find the last instruction that is not deleted.
                //int methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEnd]]!;
                int oldMethodEndNotDeleted = oldAddressToInstruction.Where(kv =>
                kv.Key >= oldMethodStart && kv.Key <= oldMethodEnd &&
                simplifiedInstructionsToAddress.Contains(kv.Value)
                ).Max(kv => kv.Key);
                int methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEndNotDeleted]]!;
                method["range"] = $"{methodStart}-{methodEnd}";

                int previousSequencePoint = methodStart;
                JArray newSequencePoints = new();
                foreach (JToken? sequencePoint in (JArray)method!["sequence-points"]!)
                {
                    GroupCollection sequencePointGroups = SequencePointRegex.Match(sequencePoint!.AsString()).Groups;
                    int startingInstructionAddress = int.Parse(sequencePointGroups[1].ToString());
                    Instruction oldInstruction = oldAddressToInstruction[startingInstructionAddress];
                    if (simplifiedInstructionsToAddress.Contains(oldInstruction))
                    {
                        startingInstructionAddress = (int)simplifiedInstructionsToAddress[oldInstruction]!;
                        newSequencePoints.Add(new JString($"{startingInstructionAddress}{sequencePointGroups[2]}"));
                        previousSequencePoint = startingInstructionAddress;
                    }
                    else
                        newSequencePoints.Add(new JString($"{previousSequencePoint}{sequencePointGroups[2]}"));
                }
                method["sequence-points"] = newSequencePoints;
            }
            JArray methods = (JArray)debugInfo["methods"]!;
            foreach (JToken method in methodsToRemove)
                methods.Remove(method);

            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                method.Offset = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[method.Offset]]!;
            Script newScript = new(simplifiedScript.ToArray());
            nef.Script = newScript;
            //nef.Compiler = AppDomain.CurrentDomain.FriendlyName;
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            return (nef, manifest, debugInfo);
        }

        public static Dictionary<int, BranchType>
            FindCoveredInstructions(NefFile nef, ContractManifest manifest, JToken debugInfo)
            => new InstructionCoverage(nef, manifest, debugInfo).coveredMap;
    }
}
