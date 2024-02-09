using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;
using static Neo.Optimizer.Optimizer;

namespace Neo.Optimizer
{
    public static class Reachability
    {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);
        private static readonly Regex SequencePointRegex = new(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)", RegexOptions.Compiled);
        private static readonly Regex DocumentRegex = new(@"\[(\d+)\](\d+)\:(\d+)\-(\d+)\:(\d+)", RegexOptions.Compiled);
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        public enum TryStack
        {
            ENTRY,
            TRY,
            CATCH,
            FINALLY,
        }

        public enum BranchType
        {
            OK,     // One of the branches may return without exception
            THROW,  // All branches surely has exceptions, but can be catched
            ABORT,  // All branches abort, and cannot be catched
        }

        [Strategy(Priority = int.MaxValue)]
        public static (NefFile, ContractManifest, JToken) RemoveUncoveredInstructions(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            Dictionary<int, bool> coveredMap = FindCoveredInstructions(nef, manifest, debugInfo);
            Script oldScript = nef.Script;
            List<(int, Instruction)> oldAddressAndInstructionsList = oldScript.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (coveredMap[a])
                {
                    simplifiedInstructionsToAddress.Add(i, currentAddress);
                    currentAddress += i.Size;
                }
                else
                    continue;
            }
            (ConcurrentDictionary<Instruction, Instruction> jumpInstructionSourceToTargets,
            ConcurrentDictionary<Instruction, (Instruction, Instruction)> tryInstructionSourceToTargets)
            = FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);

            List<byte> simplifiedScript = new();
            foreach (DictionaryEntry item in simplifiedInstructionsToAddress)
            {
                (Instruction i, int a) = ((Instruction)item.Key, (int)item.Value!);
                simplifiedScript.Add((byte)i.OpCode);
                int operandSizeLength = OperandSizePrefixTable[(int)i.OpCode];
                simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(i.Operand.Length)[0..operandSizeLength]).ToList();
                if (jumpInstructionSourceToTargets.ContainsKey(i))
                {
                    Instruction dst = jumpInstructionSourceToTargets[i];
                    int delta = (int)simplifiedInstructionsToAddress[dst]! - a;
                    if (i.OpCode == OpCode.JMP || conditionalJump.Contains(i.OpCode) || i.OpCode == OpCode.CALL || i.OpCode == OpCode.ENDTRY)
                        simplifiedScript.Add(BitConverter.GetBytes(delta)[0]);
                    if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.JMP_L || conditionalJump_L.Contains(i.OpCode) || i.OpCode == OpCode.CALL_L || i.OpCode == OpCode.ENDTRY_L)
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta)).ToList();
                    continue;
                }
                if (tryInstructionSourceToTargets.ContainsKey(i))
                {
                    (Instruction dst1, Instruction dst2) = tryInstructionSourceToTargets[i];
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
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                method.Offset = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[method.Offset]]!;
            Script newScript = new(simplifiedScript.ToArray());
            nef.Script = newScript;
            nef.Compiler = AppDomain.CurrentDomain.FriendlyName;
            nef.CheckSum = NefFile.ComputeChecksum(nef);

            Dictionary<int, (int docId, int startLine, int startCol, int endLine, int endCol)> newAddrToSequencePoint = new();
            Dictionary<int, string> newMethodStart = new();
            Dictionary<int, string> newMethodEnd = new();
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
                int methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEnd]]!;
                newMethodStart.Add(methodStart, method["id"]!.AsString());  // TODO: same format of method name as dumpnef
                newMethodEnd.Add(methodEnd, method["id"]!.AsString());
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
                    GroupCollection documentGroups = DocumentRegex.Match(sequencePointGroups[2].ToString()).Groups;
                    newAddrToSequencePoint.Add(previousSequencePoint, (
                        int.Parse(documentGroups[1].ToString()),
                        int.Parse(documentGroups[2].ToString()),
                        int.Parse(documentGroups[3].ToString()),
                        int.Parse(documentGroups[4].ToString()),
                        int.Parse(documentGroups[5].ToString())
                        ));
                }
                method["sequence-points"] = newSequencePoints;
            }
            JArray methods = (JArray)debugInfo["methods"]!;
            foreach (JToken method in methodsToRemove)
                methods.Remove(method);

            return (nef, manifest, debugInfo);
        }

        public static Dictionary<int, bool>
            FindCoveredInstructions(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            Script script = nef.Script;
            Dictionary<int, bool> coveredMap = new();
            foreach ((int addr, Instruction inst) in script.EnumerateInstructions())
                coveredMap.Add(addr, false);

            Dictionary<int, string> publicMethodStartingAddressToName = new();
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                publicMethodStartingAddressToName.Add(method.Offset, method.Name);

            Parallel.ForEach(manifest.Abi.Methods, method =>
                CoverInstruction(method.Offset, script, coveredMap)
            );
            // start from _deploy method
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                string name = method!["name"]!.AsString();  // NFTLoan.NFTLoan,RegisterRental
                name = name[(name.LastIndexOf(',') + 1)..];  // RegisterRental
                name = char.ToLower(name[0]) + name[1..];  // registerRental
                if (name == "_deploy")
                {
                    int startAddr = int.Parse(method!["range"]!.AsString().Split("-")[0]);
                    CoverInstruction(startAddr, script, coveredMap);
                }
            }
            return coveredMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="script"></param>
        /// <param name="coveredMap"></param>
        /// <returns>Whether it is possible to return without exception</returns>
        /// <exception cref="BadScriptException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static BranchType CoverInstruction(int addr, Script script, Dictionary<int, bool> coveredMap, Stack<((int returnAddr, int finallyAddr), TryStack stackType)>? stack = null, bool throwed = false)
        {
            stack ??= new();
            if (stack.Count == 0)
                stack.Push(((-1, -1), TryStack.ENTRY));
            while (stack.Count > 0)
            {
                if (!throwed)
                    goto HANDLE_NORMAL_CASE;
                HANDLE_THROW:
                throwed = true;
                TryStack stackType;
                int catchAddr; int finallyAddr;
                do
                    ((catchAddr, finallyAddr), stackType) = stack.Pop();
                while (stackType != TryStack.TRY && stack.Count > 0);
                if (stackType == TryStack.TRY)  // goto CATCH or FINALLY
                {
                    throwed = false;
                    if (catchAddr != -1)
                    {
                        addr = catchAddr;
                        stack.Push(((-1, finallyAddr), TryStack.CATCH));
                    }
                    else if (finallyAddr != -1)
                    {
                        addr = finallyAddr;
                        stack.Push(((-1, -1), TryStack.FINALLY));
                    }
                }
                if (stackType == TryStack.CATCH)  // goto FINALLY
                {
                    throwed = false;
                    if (finallyAddr != -1)
                    {
                        addr = finallyAddr;
                        stack.Push(((-1, -1), TryStack.FINALLY));
                    }
                }
                continue;
            HANDLE_NORMAL_CASE:
                if (!coveredMap.ContainsKey(addr))
                    throw new BadScriptException($"wrong address {addr}");
                if (coveredMap[addr])
                    // We have visited the code. Skip it.
                    return BranchType.OK;
                Instruction instruction = script.GetInstruction(addr);
                if (instruction.OpCode != OpCode.NOP)
                    coveredMap[addr] = true;

                // TODO: ABORTMSG may THROW instead of ABORT. Just throw new NotImplementedException for ABORTMSG?
                if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                    return BranchType.ABORT;
                if (callWithJump.Contains(instruction.OpCode))
                {
                    int callTarget = ComputeJumpTarget(addr, instruction);
                    BranchType returnedType = CoverInstruction(callTarget, script, coveredMap);
                    if (returnedType == BranchType.OK)
                    {
                        addr += instruction.Size;
                        continue;
                    }
                    if (returnedType == BranchType.ABORT)
                        return BranchType.ABORT;
                    if (returnedType == BranchType.THROW)
                        goto HANDLE_THROW;
                }
                if (instruction.OpCode == OpCode.RET)
                    return BranchType.OK;
                if (tryThrowFinally.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                        stack.Push((ComputeTryTarget(addr, instruction), TryStack.TRY));
                    if (instruction.OpCode == OpCode.THROW)
                        goto HANDLE_THROW;
                    if (instruction.OpCode == OpCode.ENDTRY)
                    {
                        ((catchAddr, finallyAddr), stackType) = stack.Peek();
                        if (stackType != TryStack.TRY && stackType != TryStack.CATCH) throw new BadScriptException("No try stack on ENDTRY");

                        // Visit catchAddr because there may still be exceptions at runtime
                        Stack<((int returnAddr, int finallyAddr), TryStack stackType)> newStack = new(stack);
                        CoverInstruction(catchAddr, script, coveredMap, stack: newStack, throwed: true);

                        stack.Pop();
                        int endPointer = addr + instruction.TokenI8;
                        if (finallyAddr != -1)
                        {
                            stack.Push(((-1, endPointer), TryStack.FINALLY));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        continue;
                    }
                    if (instruction.OpCode == OpCode.ENDTRY_L)
                    {
                        ((_, finallyAddr), stackType) = stack.Pop();
                        if (stackType != TryStack.TRY) throw new BadScriptException("No try stack on ENDTRY");
                        int endPointer = addr + instruction.TokenI32;
                        if (finallyAddr != -1)
                        {
                            stack.Push(((-1, endPointer), TryStack.FINALLY));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        continue;
                    }
                    if (instruction.OpCode == OpCode.ENDFINALLY)
                    {
                        ((_, addr), stackType) = stack.Pop();
                        if (stackType != TryStack.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        continue;
                    }
                }
                if (unconditionalJump.Contains(instruction.OpCode))
                {
                    addr = ComputeJumpTarget(addr, instruction);
                    continue;
                }
                if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                {
                    int targetAddress = ComputeJumpTarget(addr, instruction);
                    BranchType noJump = CoverInstruction(addr + instruction.Size, script, coveredMap);
                    BranchType jump = CoverInstruction(targetAddress, script, coveredMap);
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                        return BranchType.OK;
                    if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                        return BranchType.ABORT;
                    if (noJump == BranchType.THROW || jump == BranchType.THROW)  // THROW, ABORT => THROW
                        goto HANDLE_THROW;
                    throw new Exception($"Unknown {nameof(BranchType)} {noJump} {jump}");
                }

                addr += instruction.Size;
            }
            return throwed ? BranchType.THROW : BranchType.OK;
        }
    }
}
