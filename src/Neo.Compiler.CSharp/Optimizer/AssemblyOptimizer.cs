using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using static Neo.Optimizer.OpCodeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Neo.Optimizer
{
    public class AssemblyOptimizer
    {
        public static int[] OperandSizePrefixTable = new int[256];
        public static int[] OperandSizeTable = new int[256];
        static AssemblyOptimizer()
        {
            foreach (FieldInfo field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OperandSizeAttribute? attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                if (attribute == null) continue;
                int index = (int)(OpCode)field.GetValue(null)!;
                OperandSizePrefixTable[index] = attribute.SizePrefix;
                OperandSizeTable[index] = attribute.Size;
            }
        }

        public static (NefFile, ContractManifest, string, JToken) RemoveUncoveredInstructions(
    Dictionary<int, bool> coveredMap, NefFile nef, ContractManifest oldManifest, JToken oldDebugInfo)
        {
            Script oldScript = nef.Script;
            List<(int, Instruction)> oldAddressAndInstructionsList = oldScript.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            Dictionary<Instruction, int> simplifiedInstructionsToAddress = new();
            Dictionary<Instruction, Instruction> jumpInstructionSourceToTargets = new();
            Dictionary<Instruction, (Instruction, Instruction)> tryInstructionSourceToTargets = new();
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
                if (i.OpCode == OpCode.JMP || conditionalJump.Contains(i.OpCode) || i.OpCode == OpCode.CALL || i.OpCode == OpCode.ENDTRY)
                    jumpInstructionSourceToTargets.Add(i, oldAddressToInstruction[a + i.TokenI8]);
                if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.JMP_L || conditionalJump_L.Contains(i.OpCode) || i.OpCode == OpCode.CALL_L || i.OpCode == OpCode.ENDTRY_L)
                    jumpInstructionSourceToTargets.Add(i, oldAddressToInstruction[a + i.TokenI32]);
                if (i.OpCode == OpCode.TRY)
                    tryInstructionSourceToTargets.Add(i, (oldAddressToInstruction[a + i.TokenI8], oldAddressToInstruction[a + i.TokenI8_1]));
                if (i.OpCode == OpCode.TRY_L)
                    tryInstructionSourceToTargets.Add(i, (oldAddressToInstruction[a + i.TokenI32], oldAddressToInstruction[a + i.TokenI32_1]));
            }
            List<byte> simplifiedScript = new();
            foreach ((Instruction i, int a) in simplifiedInstructionsToAddress)
            {
                simplifiedScript.Add((byte)i.OpCode);
                int operandSizeLength = OperandSizePrefixTable[(int)i.OpCode];
                simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(i.Operand.Length)[0..operandSizeLength]).ToList();
                if (jumpInstructionSourceToTargets.ContainsKey(i))
                {
                    Instruction dst = jumpInstructionSourceToTargets[i];
                    int delta = simplifiedInstructionsToAddress[dst] - a;
                    if (i.OpCode == OpCode.JMP || conditionalJump.Contains(i.OpCode) || i.OpCode == OpCode.CALL || i.OpCode == OpCode.ENDTRY)
                        simplifiedScript.Add(BitConverter.GetBytes(delta)[0]);
                    if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.JMP_L || conditionalJump_L.Contains(i.OpCode) || i.OpCode == OpCode.CALL_L || i.OpCode == OpCode.ENDTRY_L)
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta)).ToList();
                    continue;
                }
                if (tryInstructionSourceToTargets.ContainsKey(i))
                {
                    (Instruction dst1, Instruction dst2) = tryInstructionSourceToTargets[i];
                    (int delta1, int delta2) = (simplifiedInstructionsToAddress[dst1] - a, simplifiedInstructionsToAddress[dst2] - a);
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
            foreach (ContractMethodDescriptor method in oldManifest.Abi.Methods)
                method.Offset = simplifiedInstructionsToAddress[oldAddressToInstruction[method.Offset]];
            Script newScript = new Script(simplifiedScript.ToArray());
            nef.Script = newScript;
            nef.Compiler = System.AppDomain.CurrentDomain.FriendlyName;
            nef.CheckSum = NefFile.ComputeChecksum(nef);

            Dictionary<int, (int docId, int startLine, int startCol, int endLine, int endCol)> newAddrToSequencePoint = new();
            Dictionary<int, string> newMethodStart = new();
            Dictionary<int, string> newMethodEnd = new();
            HashSet<JToken> methodsToRemove = new();
            foreach (JToken? method in (JArray)oldDebugInfo["methods"]!)
            {
                Regex rangeRegex = new Regex(@"(\d+)\-(\d+)");
                GroupCollection rangeGroups = rangeRegex.Match(method!["range"]!.AsString()).Groups;
                (int oldMethodStart, int oldMethodEnd) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                if (!simplifiedInstructionsToAddress.ContainsKey(oldAddressToInstruction[oldMethodStart]))
                {
                    methodsToRemove.Add(method);
                    continue;
                }
                int methodStart = simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodStart]];
                int methodEnd = simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEnd]];
                newMethodStart.Add(methodStart, method["id"]!.AsString());  // TODO: same format of method name as dumpnef
                newMethodEnd.Add(methodEnd, method["id"]!.AsString());
                method["range"] = $"{methodStart}-{methodEnd}";

                Regex sequencePointRegex = new Regex(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)");
                int previousSequencePoint = methodStart;
                JArray newSequencePoints = new();
                foreach (JToken? sequencePoint in (JArray)method!["sequence-points"]!)
                {
                    GroupCollection sequencePointGroups = sequencePointRegex.Match(sequencePoint!.AsString()).Groups;
                    int startingInstructionAddress = int.Parse(sequencePointGroups[1].ToString());
                    Instruction oldInstruction = oldAddressToInstruction[startingInstructionAddress];
                    if (simplifiedInstructionsToAddress.ContainsKey(oldInstruction))
                    {
                        startingInstructionAddress = simplifiedInstructionsToAddress[oldInstruction];
                        newSequencePoints.Add(new JString($"{startingInstructionAddress}{sequencePointGroups[2]}"));
                        previousSequencePoint = startingInstructionAddress;
                    }
                    else
                        newSequencePoints.Add(new JString($"{previousSequencePoint}{sequencePointGroups[2]}"));
                    Regex documentRegex = new Regex(@"\[(\d+)\](\d+)\:(\d+)\-(\d+)\:(\d+)");
                    GroupCollection documentGroups = documentRegex.Match(sequencePointGroups[2].ToString()).Groups;
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

            Dictionary<string, string[]> docPathToContent = new();
            string dumpnef = "";
            foreach ((int a, Instruction i) in newScript.EnumerateInstructions(/*print: true*/).ToList())
            {
                if (newMethodStart.ContainsKey(a))
                    dumpnef += $"# Method Start {newMethodStart[a]}\n";
                if (newMethodEnd.ContainsKey(a))
                    dumpnef += $"# Method End {newMethodEnd[a]}\n";
                if (newAddrToSequencePoint.ContainsKey(a))
                {
                    var docInfo = newAddrToSequencePoint[a];
                    string docPath = oldDebugInfo["documents"]![docInfo.docId]!.AsString();
                    if (oldDebugInfo["document-root"] != null)
                        docPath = Path.Combine(oldDebugInfo["document-root"]!.AsString(), docPath);
                    if (!docPathToContent.ContainsKey(docPath))
                        docPathToContent.Add(docPath, File.ReadAllLines(docPath).ToArray());
                    if (docInfo.startLine == docInfo.endLine)
                        dumpnef += $"# Code {Path.GetFileName(docPath)} line {docInfo.startLine}: \"{docPathToContent[docPath][docInfo.startLine - 1][(docInfo.startCol - 1)..(docInfo.endCol - 1)]}\"\n";
                    else
                        for (int lineIndex = docInfo.startLine; lineIndex <= docInfo.endLine; lineIndex++)
                        {
                            string src;
                            if (lineIndex == docInfo.startLine)
                                src = docPathToContent[docPath][lineIndex - 1][(docInfo.startCol - 1)..].Trim();
                            else if (lineIndex == docInfo.endLine)
                                src = docPathToContent[docPath][lineIndex - 1][..(docInfo.endCol - 1)].Trim();
                            else
                                src = docPathToContent[docPath][lineIndex - 1].Trim();
                            dumpnef += $"# Code {Path.GetFileName(docPath)} line {docInfo.startLine}: \"{src}\"\n";
                        }
                }
                if (a < newScript.Length)
                    dumpnef += $"{DumpNef.WriteInstruction(a, newScript.GetInstruction(a), newScript.GetInstructionAddressPadding(), nef.Tokens)}\n";
            }

            return (nef, oldManifest, dumpnef, oldDebugInfo);
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

            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                CoverInstruction(method.Offset, script, coveredMap);
            // start from _deploy method
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                string name = method!["name"]!.AsString();  // NFTLoan.NFTLoan,RegisterRental
                name = name.Substring(name.LastIndexOf(',') + 1);  // RegisterRental
                name = char.ToLower(name[0]) + name.Substring(1);  // registerRental
                if (name == "_deploy")
                {
                    int startAddr = int.Parse(method!["range"]!.AsString().Split("-")[0]);
                    CoverInstruction(startAddr, script, coveredMap);
                }
            }
            return coveredMap;
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="script"></param>
        /// <param name="coveredMap"></param>
        /// <returns>Whether it is possible to return without exception</returns>
        /// <exception cref="BadScriptException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static BranchType CoverInstruction(int addr, Script script, Dictionary<int, bool> coveredMap, Stack<(int returnAddr, int finallyAddr, TryStack stackType)>? stack = null, bool throwed = false)
        {
            if (stack == null)
                stack = new();
            if (stack.Count == 0)
                stack.Push((-1, -1, TryStack.ENTRY));
            while (stack.Count > 0)
            {
                if (!throwed)
                    goto HANDLE_NORMAL_CASE;
                HANDLE_THROW:
                throwed = true;
                TryStack stackType;
                int catchAddr; int finallyAddr;
                do
                    (catchAddr, finallyAddr, stackType) = stack.Pop();
                while (stackType != TryStack.TRY && stack.Count > 0);
                if (stackType == TryStack.TRY)  // goto CATCH or FINALLY
                {
                    throwed = false;
                    if (catchAddr != -1)
                    {
                        addr = catchAddr;
                        stack.Push((-1, finallyAddr, TryStack.CATCH));
                    }
                    else if (finallyAddr != -1)
                    {
                        addr = finallyAddr;
                        stack.Push((-1, -1, TryStack.FINALLY));
                    }
                }
                if (stackType == TryStack.CATCH)  // goto FINALLY
                {
                    throwed = false;
                    if (finallyAddr != -1)
                    {
                        addr = finallyAddr;
                        stack.Push((-1, -1, TryStack.FINALLY));
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
                    int callTarget = -1;
                    if (instruction.OpCode == OpCode.CALL)
                        callTarget = addr + instruction.TokenI8;
                    if (instruction.OpCode == OpCode.CALL_L)
                        callTarget = addr + instruction.TokenI32;
                    if (instruction.OpCode == OpCode.CALLA)
                        throw new NotImplementedException("CALLA is dynamic; not supported");
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
                    if (instruction.OpCode == OpCode.TRY)
                        stack.Push((
                            instruction.TokenI8 == 0 ? -1 : addr + instruction.TokenI8,
                            instruction.TokenI8_1 == 0 ? -1 : addr + instruction.TokenI8_1,
                            TryStack.TRY));
                    if (instruction.OpCode == OpCode.TRY_L)
                        stack.Push((
                            instruction.TokenI32 == 0 ? -1 : addr + instruction.TokenI32,
                            instruction.TokenI32_1 == 0 ? -1 : addr + instruction.TokenI32_1,
                            TryStack.TRY));
                    if (instruction.OpCode == OpCode.THROW)
                        goto HANDLE_THROW;
                    if (instruction.OpCode == OpCode.ENDTRY)
                    {
                        (catchAddr, finallyAddr, stackType) = stack.Peek();
                        if (stackType != TryStack.TRY && stackType != TryStack.CATCH) throw new BadScriptException("No try stack on ENDTRY");

                        // Visit catchAddr because there may still be exceptions at runtime
                        Stack<(int returnAddr, int finallyAddr, TryStack stackType)> newStack = new(stack);
                        CoverInstruction(catchAddr, script, coveredMap, stack: newStack, throwed: true);

                        stack.Pop();
                        int endPointer = addr + instruction.TokenI8;
                        if (finallyAddr != -1)
                        {
                            stack.Push((-1, endPointer, TryStack.FINALLY));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        continue;
                    }
                    if (instruction.OpCode == OpCode.ENDTRY_L)
                    {
                        (_, finallyAddr, stackType) = stack.Pop();
                        if (stackType != TryStack.TRY) throw new BadScriptException("No try stack on ENDTRY");
                        int endPointer = addr + instruction.TokenI32;
                        if (finallyAddr != -1)
                        {
                            stack.Push((-1, endPointer, TryStack.FINALLY));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        continue;
                    }
                    if (instruction.OpCode == OpCode.ENDFINALLY)
                    {
                        (_, addr, stackType) = stack.Pop();
                        if (stackType != TryStack.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        continue;
                    }
                }
                if (unconditionalJump.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.JMP)
                        addr += instruction.TokenI8;
                    if (instruction.OpCode == OpCode.JMP_L)
                        addr += instruction.TokenI32;
                    continue;
                }
                if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                {
                    int jumpAddress = conditionalJump.Contains(instruction.OpCode) ?
                        addr + instruction.TokenI8 : addr + instruction.TokenI32;
                    BranchType noJump = CoverInstruction(addr + instruction.Size, script, coveredMap);
                    BranchType jump = CoverInstruction(jumpAddress, script, coveredMap);
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
