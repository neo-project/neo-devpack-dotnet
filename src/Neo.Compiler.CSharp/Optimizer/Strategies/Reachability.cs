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
            UNCOVERED,
        }

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
            (ConcurrentDictionary<Instruction, Instruction> jumpInstructionSourceToTargets,
            ConcurrentDictionary<Instruction, (Instruction, Instruction)> tryInstructionSourceToTargets, _)
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
                    else if (i.OpCode == OpCode.PUSHA)
                        delta = 0;  // TODO: decide a good target
                    else
                        throw new BadScriptException($"Target instruction of {i.OpCode} at address {a} is deleted");
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
            nef.Compiler = AppDomain.CurrentDomain.FriendlyName;
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            return (nef, manifest, debugInfo);
        }

        public static Dictionary<int, BranchType>
            FindCoveredInstructions(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            Script script = nef.Script;
            Dictionary<int, BranchType> coveredMap = new();
            foreach ((int addr, Instruction _) in script.EnumerateInstructions())
                coveredMap.Add(addr, BranchType.UNCOVERED);

            Dictionary<int, string> publicMethodStartingAddressToName = new();
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                publicMethodStartingAddressToName.Add(method.Offset, method.Name);

            Parallel.ForEach(manifest.Abi.Methods, method =>
                CoverInstruction(method.Offset, script, coveredMap)
            );
            //foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
            //    CoverInstruction(method.Offset, script, coveredMap);
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
        public static BranchType CoverInstruction(int addr, Script script, Dictionary<int, BranchType> coveredMap, Stack<((int returnAddr, int finallyAddr), TryStack stackType)>? stack = null, bool throwed = false)
        {
            int entranceAddr = addr;
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
                while (stackType != TryStack.TRY && stackType != TryStack.CATCH && stack.Count > 0);
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
                if (coveredMap[addr] != BranchType.UNCOVERED)
                    // We have visited the code. Skip it.
                    return coveredMap[addr];
                Instruction instruction = script.GetInstruction(addr);
                if (instruction.OpCode != OpCode.NOP)
                    coveredMap[addr] = BranchType.OK;

                // TODO: ABORTMSG may THROW instead of ABORT. Just throw new NotImplementedException for ABORTMSG?
                if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                {
                    // See if we are in a try. There may still be runtime exceptions
                    ((catchAddr, finallyAddr), stackType) = stack.Peek();
                    if (stackType == TryStack.TRY && catchAddr != -1)
                    {
                        // Visit catchAddr because there may still be exceptions at runtime
                        coveredMap[entranceAddr] = CoverInstruction(catchAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                        return coveredMap[entranceAddr];
                    }
                    if (stackType == TryStack.CATCH && finallyAddr != -1)
                    {
                        // Visit finallyAddr because there may still be exceptions at runtime
                        coveredMap[entranceAddr] = CoverInstruction(finallyAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                        return coveredMap[entranceAddr];
                    }
                    coveredMap[entranceAddr] = BranchType.ABORT;
                    return coveredMap[entranceAddr];
                }
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
                    {
                        // See if we are in a try. There may still be runtime exceptions
                        ((catchAddr, finallyAddr), stackType) = stack.Peek();
                        if (stackType == TryStack.TRY && catchAddr != -1)
                        {
                            // Visit catchAddr because there may still be exceptions at runtime
                            coveredMap[entranceAddr] = CoverInstruction(catchAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                            return coveredMap[entranceAddr];
                        }
                        if (stackType == TryStack.CATCH && finallyAddr != -1)
                        {
                            // Visit finallyAddr because there may still be exceptions at runtime
                            coveredMap[entranceAddr] = CoverInstruction(finallyAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                            return coveredMap[entranceAddr];
                        }
                        coveredMap[entranceAddr] = BranchType.ABORT;
                        return coveredMap[entranceAddr];
                    }
                    if (returnedType == BranchType.THROW)
                        goto HANDLE_THROW;
                }
                if (instruction.OpCode == OpCode.RET)
                {
                    // See if we are in a try. There may still be runtime exceptions
                    ((catchAddr, finallyAddr), stackType) = stack.Peek();
                    if (stackType == TryStack.TRY && catchAddr != -1)
                        // Visit catchAddr because there may still be exceptions at runtime
                        CoverInstruction(catchAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                    if (stackType == TryStack.CATCH && finallyAddr != -1)
                        // Visit finallyAddr because there may still be exceptions at runtime
                        CoverInstruction(finallyAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                    coveredMap[entranceAddr] = BranchType.OK;
                    return coveredMap[entranceAddr];
                }
                if (tryThrowFinally.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                        stack.Push((ComputeTryTarget(addr, instruction), TryStack.TRY));
                    if (instruction.OpCode == OpCode.THROW)
                        goto HANDLE_THROW;
                    if (instruction.OpCode == OpCode.ENDTRY)
                    {
                        ((catchAddr, finallyAddr), stackType) = stack.Peek();
                        if (stackType != TryStack.TRY && stackType != TryStack.CATCH)
                            throw new BadScriptException("No try stack on ENDTRY");

                        if (stackType == TryStack.TRY && catchAddr != -1)
                            // Visit catchAddr because there may still be exceptions at runtime
                            CoverInstruction(catchAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                        if (stackType == TryStack.CATCH && finallyAddr != -1)
                            // Visit finallyAddr because there may still be exceptions at runtime
                            CoverInstruction(finallyAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);

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
                        ((_, _), stackType) = stack.Pop();
                        if (stackType != TryStack.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        addr += instruction.Size;
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
                    BranchType noJump = CoverInstruction(addr + instruction.Size, script, coveredMap, stack: new(stack.Reverse()));
                    BranchType jump = CoverInstruction(targetAddress, script, coveredMap, stack: new(stack.Reverse()));
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                    {
                        // See if we are in a try. There may still be runtime exceptions
                        ((catchAddr, finallyAddr), stackType) = stack.Peek();
                        if (stackType == TryStack.TRY && catchAddr != -1)
                            // Visit catchAddr because there may still be exceptions at runtime
                            CoverInstruction(catchAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                        if (stackType == TryStack.CATCH && finallyAddr != -1)
                            // Visit finallyAddr because there may still be exceptions at runtime
                            CoverInstruction(finallyAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                        coveredMap[entranceAddr] = BranchType.OK;
                        return coveredMap[entranceAddr];
                    }
                    if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                    {
                        // See if we are in a try. There may still be runtime exceptions
                        ((catchAddr, finallyAddr), stackType) = stack.Peek();
                        if (stackType == TryStack.TRY && catchAddr != -1)
                        {
                            // Visit catchAddr because there may still be exceptions at runtime
                            coveredMap[entranceAddr] = CoverInstruction(catchAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                            return coveredMap[entranceAddr];
                        }
                        if (stackType == TryStack.CATCH && finallyAddr != -1)
                        {
                            // Visit finallyAddr because there may still be exceptions at runtime
                            coveredMap[entranceAddr] = CoverInstruction(finallyAddr, script, coveredMap, stack: new(stack.Reverse()), throwed: true);
                            return coveredMap[entranceAddr];
                        }
                        coveredMap[entranceAddr] = BranchType.ABORT;
                        return coveredMap[entranceAddr];
                    }
                    if (noJump == BranchType.THROW || jump == BranchType.THROW)  // THROW, ABORT => THROW
                        goto HANDLE_THROW;
                    throw new Exception($"Unknown {nameof(BranchType)} {noJump} {jump}");
                }

                addr += instruction.Size;
            }
            coveredMap[entranceAddr] = throwed ? BranchType.THROW : BranchType.OK;
            return coveredMap[entranceAddr];
        }
    }
}
