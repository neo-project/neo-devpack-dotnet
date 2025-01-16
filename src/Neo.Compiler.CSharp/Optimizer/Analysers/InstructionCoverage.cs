// Copyright (C) 2015-2024 The Neo Project.
//
// InstructionCoverage.cs file belongs to the neo project and is free
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;

namespace Neo.Optimizer
{
    [Flags]
    public enum TryType
    {
        NONE = 1 << 0,
        TRY = 1 << 1,
        CATCH = 1 << 2,
        FINALLY = 1 << 3,
    }

    [DebuggerDisplay("{catchAddr}, {finallyAddr}, {tryStateType}, {continueAfterFinally}")]
    public struct TryState
    {
        public int catchAddr { get; init; }
        public int finallyAddr { get; init; }
        public TryType tryType { get; init; }
        public bool continueAfterFinally { get; init; }

        public TryState(int catchAddr, int finallyAddr, TryType tryStateType, bool continueAfterFinally)
        {
            this.catchAddr = catchAddr;
            this.finallyAddr = finallyAddr;
            this.tryType = tryStateType;
            this.continueAfterFinally = continueAfterFinally;
        }

        internal void Deconstruct(out int catchAddr, out int finallyAddr,
            out TryType tryStackType, out bool continueAfterFinally)
        {
            catchAddr = this.catchAddr;
            finallyAddr = this.finallyAddr;
            tryStackType = this.tryType;
            continueAfterFinally = this.continueAfterFinally;
        }
    }

    public enum BranchType
    {
        OK = 1,     // One of the branches may return without exception
        THROW = 2,  // All branches surely have exceptions, but can be catched
        ABORT = 3,  // All branches abort, and cannot be catched
        UNCOVERED = 4,
    }

    public class InstructionCoverage
    {
        Script script;
        // Starting from the address, whether the call will surely throw or surely abort, or may be OK
        public Dictionary<int, BranchType> coveredMap { get; protected set; }

        // key: starting address of basic block
        // value: addr -> instruction of all instructions in this basic block
        public Dictionary<int, Dictionary<int, Instruction>> basicBlocksInDict { get; protected set; }

        // key: starting address of basic block
        // value: starting address of the next basic block,
        //   which is reached by increased instruction pointer in normal execution
        public Dictionary<int, int> basicBlockContinuation { get; protected set; } = new();

        // key: starting address of basic block
        // value: starting address of basic blocks that is jumped to, from this basic block
        public Dictionary<int, HashSet<int>> basicBlockJump { get; protected set; } = new();
        public List<(int a, Instruction i)> addressAndInstructions { get; init; }
        public Dictionary<int, Instruction> addressToInstructions { get; init; }
        public Dictionary<Instruction, Instruction> jumpInstructionSourceToTargets { get; init; }
        public Dictionary<Instruction, (Instruction, Instruction)> tryInstructionSourceToTargets { get; init; }
        /// <summary>
        /// key: target of all kinds of Instruction that has 1 or 2 jump targets
        /// value: sources of that jump target
        /// </summary>
        public Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources { get; init; }
        public Dictionary<int, EntryType> pushaTargets { get; init; }
        public Dictionary<int, EntryType> entryPointsByMethod { get; init; }

        public InstructionCoverage(NefFile nef, ContractManifest manifest)
        {
            this.script = nef.Script;
            coveredMap = new();
            basicBlocksInDict = new();
            addressAndInstructions = script.EnumerateInstructions().ToList();
            addressToInstructions = addressAndInstructions.ToDictionary(e => e.a, e => e.i);
            (jumpInstructionSourceToTargets, tryInstructionSourceToTargets, jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(addressAndInstructions);
            pushaTargets = EntryPoint.EntryPointsByCallA(nef);
            entryPointsByMethod = EntryPoint.EntryPointsByMethod(manifest);
            ResetCoveredMap(init: true);

            // It is unsafe to go parallel, because the coveredMap value is not true/false
            //Parallel.ForEach(manifest.Abi.Methods, method =>
            //    CoverInstruction(method.Offset, script, coveredMap)
            //);
            foreach (int addr in entryPointsByMethod.Keys)
                CoverInstruction(addr);
        }

        public void ResetCoveredMap(bool init = false)
        {
            foreach ((int addr, Instruction _) in addressAndInstructions)
                if (init)
                    // This throws exception when there exists duplicate addr
                    coveredMap.Add(addr, BranchType.UNCOVERED);
                else
                    coveredMap[addr] = BranchType.UNCOVERED;
        }

        public static Stack<T> CopyStack<T>(Stack<T> stack) => new(stack.Reverse());

        public BranchType HandleThrow(int entranceAddr, int throwFromAddr, Stack<TryState> stack)
        {
            stack = CopyStack(stack);
            TryType tryStateType;
            int catchAddr; int finallyAddr;
            do
                (catchAddr, finallyAddr, tryStateType, _) = stack.Pop();
            while (tryStateType != TryType.TRY && tryStateType != TryType.CATCH && stack.Count > 0);
            if (tryStateType == TryType.TRY)  // goto CATCH or FINALLY
            {
                // try with catch: cancel throw and execute catch
                if (catchAddr != -1)
                {
                    int addr = catchAddr;
                    stack.Push(new TryState(-1, finallyAddr, TryType.CATCH, true));
                    return CoverInstruction(addr, tryStack: stack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                }
                // try without catch: execute finally but do not visit codes after finally
                else if (finallyAddr != -1)
                {
                    stack.Push(new(-1, -1, TryType.FINALLY, false));
                    if (CoverInstruction(finallyAddr, stack, jumpFromBasicBlockEntranceAddr: entranceAddr) == BranchType.ABORT)
                        // ABORT in finally
                        return BranchType.ABORT;
                    return BranchType.THROW;
                }
                throw new BadScriptException("Try without catch or finally");
            }
            // not throwed in try
            // throwed in catch with finally: execute finally,
            // and do not continue after ENDFINALLY
            if (tryStateType == TryType.CATCH)
            {
                if (finallyAddr != -1)
                {
                    stack.Push(new(-1, -1, TryType.FINALLY, false));
                    if (CoverInstruction(finallyAddr, stack, jumpFromBasicBlockEntranceAddr: entranceAddr) == BranchType.ABORT)
                        // ABORT in finally
                        return BranchType.ABORT;
                }
            }
            return BranchType.THROW;
        }

        public BranchType HandleAbort(int entranceAddr, int abortFromAddr, Stack<TryState> stack)
        {
            // See if we are in a try or catch. There may still be runtime exceptions
            (int catchAddr, int finallyAddr, TryType stackType, _) = stack.Peek();
            if (stackType == TryType.TRY && catchAddr != -1 ||
                stackType == TryType.CATCH && finallyAddr != -1)
            {
                // Visit catchAddr because there may still be exceptions at runtime
                if (HandleThrow(entranceAddr, abortFromAddr, stack) == BranchType.OK)
                    return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
            }
            return BranchType.ABORT;
        }

        /// <summary>
        /// Cover a basic block, and recursively cover all branches
        /// </summary>
        /// <param name="addr">Starting address of script. Should start at a basic block</param>
        /// <param name="tryStack">try-catch-finally stack</param>
        /// <param name="continueFromBasicBlockEntranceAddr">Specify the previous basic block entrance address, if we continue execution from the previous basic block</param>
        /// <param name="jumpFromBasicBlockEntranceAddr">Specify the entrance address of the basic block as the source of jump, if we jumped to current address from that basic block</param>
        /// <returns>Whether it is possible to return without exception</returns>
        /// <exception cref="BadScriptException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public BranchType CoverInstruction(int addr, Stack<TryState>? tryStack = null,
            int? continueFromBasicBlockEntranceAddr = null, int? jumpFromBasicBlockEntranceAddr = null)
        {
            if (continueFromBasicBlockEntranceAddr != null)
                basicBlockContinuation[(int)continueFromBasicBlockEntranceAddr] = addr;
            if (jumpFromBasicBlockEntranceAddr != null)
            {
                HashSet<int>? jumpTargets;
                if (!basicBlockJump.TryGetValue((int)jumpFromBasicBlockEntranceAddr, out jumpTargets))
                {
                    jumpTargets = new();
                    basicBlockJump[(int)jumpFromBasicBlockEntranceAddr] = jumpTargets;
                }
                jumpTargets.Add(addr);
            }
            int entranceAddr = addr;

            if (tryStack == null)
            {
                tryStack = new();
                tryStack.Push(new(-1, -1, TryType.NONE, false));
            }
            else
                tryStack = CopyStack(tryStack);

            (int catchAddr, int finallyAddr, TryType stackType, bool continueAfterFinally) = tryStack.Peek();

            while (true)
            {
                // For the analysis of basic blocks,
                // we launched new recursion when exception is catched.
                // Here we have the exception not catched
                if (!coveredMap.TryGetValue(addr, out BranchType value))
                    throw new BadScriptException($"wrong address {addr}");
                Instruction instruction = script.GetInstruction(addr);
                if (jumpTargetToSources.ContainsKey(instruction) && addr != entranceAddr)
                    // on target of jump, start a new recursion to split basic blocks
                    return coveredMap[entranceAddr] = CoverInstruction(addr, tryStack, continueFromBasicBlockEntranceAddr: entranceAddr);
                if (value != BranchType.UNCOVERED)
                {
                    if (stackType != TryType.FINALLY)
                        // We have visited the code. Skip it.
                        return coveredMap[entranceAddr] = value;
                    // if we are in finally, we may visit the codes after ENDFINALLY
                    // when previous codes did not throw
                    if (value != BranchType.OK)  // the codes in finally or the codes after ENDFINALLY will THROW or ABORT
                        return coveredMap[entranceAddr] = value;
                    tryStack.Pop();  // end current finally
                    // No THROW or ABORT in try, catch or finally
                    // visit codes after ENDFINALLY
                    if (continueAfterFinally)
                        return coveredMap[entranceAddr] = CoverInstruction(finallyAddr, tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                    // FINALLY is OK, but throwed in previous TRY (without catch) or CATCH
                    return BranchType.THROW;  // Do not set coveredMap[entranceAddr] = BranchType.THROW;
                }
                //if (instruction.OpCode != OpCode.NOP)
                {
                    coveredMap[addr] = BranchType.OK;
                    // Add a basic block starting from entranceAddr
                    if (!basicBlocksInDict.TryGetValue(entranceAddr, out Dictionary<int, Instruction>? instructions))
                    {
                        instructions = new Dictionary<int, Instruction>();
                        basicBlocksInDict.Add(entranceAddr, instructions);
                    }
                    // Add this instruction to the basic block starting from entranceAddr
                    instructions.Add(addr, instruction);
                }

                // ABORT may also THROW in execution; this has been handled in HandleAbort
                if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                    return coveredMap[entranceAddr] = HandleAbort(entranceAddr, addr, tryStack);
                if (callWithJump.Contains(instruction.OpCode))
                {
                    BranchType returnedType;
                    if (instruction.OpCode == OpCode.CALLA)
                    {
                        returnedType = BranchType.ABORT;
                        foreach (int callaTarget in pushaTargets.Keys)
                        {
                            BranchType singleCallaResult = CoverInstruction(callaTarget, tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                            if (singleCallaResult < returnedType)
                                returnedType = singleCallaResult;
                            // TODO: if a PUSHA cannot be covered, do not add it as a CALLA target
                        }
                    }
                    else
                    {
                        int callTarget = ComputeJumpTarget(addr, instruction);
                        returnedType = CoverInstruction(callTarget, tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                    }
                    if (returnedType == BranchType.OK)
                        return coveredMap[entranceAddr] = CoverInstruction(addr + instruction.Size, tryStack, continueFromBasicBlockEntranceAddr: entranceAddr);
                    if (returnedType == BranchType.ABORT)
                        return coveredMap[entranceAddr] = HandleAbort(entranceAddr, addr, tryStack);
                    if (returnedType == BranchType.THROW)
                        return coveredMap[entranceAddr] = HandleThrow(entranceAddr, addr, tryStack);
                }
                if (instruction.OpCode == OpCode.RET)
                {
                    // See if we are in a try. There may still be runtime exceptions
                    // Do not judge with current stack.Peek(),
                    // because the try can hide deep in the stack.
                    // Just throw!
                    HandleThrow(entranceAddr, addr, tryStack);
                    // We should have poped try stack; however nobody else will read it anymore.
                    // No need to handle the try stack!
                    //while (tryStack.Count > 0 && tryStack.Peek().tryType != TryType.NONE)
                    //    tryStack.Pop();
                    //if (tryStack.Count > 0 && tryStack.Peek().tryType == TryType.NONE)
                    //    tryStack.Pop();
                    return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
                }
                if (tryThrowFinally.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                    {
                        (int catchTarget, int finallyTarget) = ComputeTryTarget(addr, instruction);
                        tryStack.Push(new(catchTarget, finallyTarget, TryType.TRY, true));
                        return coveredMap[entranceAddr] = CoverInstruction(addr + instruction.Size, tryStack, continueFromBasicBlockEntranceAddr: entranceAddr);
                    }
                    if (instruction.OpCode == OpCode.THROW)
                        return coveredMap[entranceAddr] = HandleThrow(entranceAddr, addr, tryStack);
                    if (instruction.OpCode == OpCode.ENDTRY || instruction.OpCode == OpCode.ENDTRY_L)
                    {
                        if (stackType != TryType.TRY && stackType != TryType.CATCH)
                            throw new BadScriptException("No try stack on ENDTRY");

                        // Terminate the try/catch context, but
                        // visit catchAddr for current try, or finallyAddr for current catch
                        // because there may still be exceptions at runtime
                        HandleThrow(entranceAddr, addr, tryStack);

                        tryStack.Pop();  // pop the ending TRY or CATCH
                        int endPointer = ComputeJumpTarget(addr, instruction);
                        if (finallyAddr != -1)
                        {
                            tryStack.Push(new(-1, endPointer, TryType.FINALLY, true));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        return coveredMap[entranceAddr] = CoverInstruction(addr, tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                    }
                    if (instruction.OpCode == OpCode.ENDFINALLY)
                    {
                        int endPointer = finallyAddr;
                        if (stackType != TryType.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        tryStack.Pop();  // pop the ending FINALLY
                        if (continueAfterFinally)
                            return coveredMap[entranceAddr] = CoverInstruction(endPointer, tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                        // For this basic block in finally, the branch type is OK
                        // The throw is caused by previous codes
                        return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
                    }
                }
                if (unconditionalJump.Contains(instruction.OpCode))
                    //addr = ComputeJumpTarget(addr, instruction);
                    //continue;
                    // For the analysis of basic blocks, we launch a new recursion
                    return coveredMap[entranceAddr] = CoverInstruction(ComputeJumpTarget(addr, instruction), tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                {
                    BranchType noJump = CoverInstruction(addr + instruction.Size, tryStack, continueFromBasicBlockEntranceAddr: entranceAddr);
                    BranchType jump = CoverInstruction(ComputeJumpTarget(addr, instruction), tryStack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                    {
                        // See if we are in a try. There may still be runtime exceptions
                        HandleThrow(entranceAddr, addr, tryStack);
                        return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
                    }
                    if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                        return coveredMap[entranceAddr] = HandleAbort(entranceAddr, addr, tryStack);
                    if (noJump == BranchType.THROW || jump == BranchType.THROW)  // THROW, ABORT => THROW
                        return coveredMap[entranceAddr] = HandleThrow(entranceAddr, addr, tryStack);
                    throw new Exception($"Unknown {nameof(BranchType)} {noJump} {jump}");
                }

                addr += instruction.Size;
            }
        }
    }
}
