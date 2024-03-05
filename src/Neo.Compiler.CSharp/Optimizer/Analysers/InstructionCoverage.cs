using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;

namespace Neo.Optimizer
{
    public enum TryStackType
    {
        ENTRY,
        TRY,
        CATCH,
        FINALLY,
    }

    public enum BranchType
    {
        OK,     // One of the branches may return without exception
        THROW,  // All branches surely have exceptions, but can be catched
        ABORT,  // All branches abort, and cannot be catched
        UNCOVERED,
    }

    public class InstructionCoverage
    {
        Script script;
        // Starting from the address, whether the call will surely throw or surely abort, or may be OK
        public Dictionary<int, BranchType> coveredMap { get; protected set; }
        public Dictionary<int, Dictionary<int, Instruction>> basicBlocks { get; protected set; }
        public List<(int a, Instruction i)> addressAndInstructions { get; init; }
        public Dictionary<int, HashSet<int>> jumpTargetToSources { get; init; }
        public InstructionCoverage(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            this.script = nef.Script;
            coveredMap = new();
            basicBlocks = new();
            addressAndInstructions = script.EnumerateInstructions().ToList();
            (_, _, jumpTargetToSources) = FindAllJumpAndTrySourceToTargets(addressAndInstructions);
            foreach ((int addr, Instruction _) in addressAndInstructions)
                coveredMap.Add(addr, BranchType.UNCOVERED);

            // It is unsafe to go parallel, because the coveredMap value is not true/false
            //Parallel.ForEach(manifest.Abi.Methods, method =>
            //    CoverInstruction(method.Offset, script, coveredMap)
            //);
            foreach ((int addr, _) in EntryPoint.EntryPointsByMethod(manifest, debugInfo))
                CoverInstruction(addr);
        }

        public static Stack<((int returnAddr, int finallyAddr), TryStackType stackType)> CopyStack
            (Stack<((int returnAddr, int finallyAddr), TryStackType stackType)> stack) => new(stack.Reverse());

        public BranchType HandleThrow(int entranceAddr, int addr, Stack<((int catchAddr, int finallyAddr), TryStackType stackType)> stack)
        {
            stack = CopyStack(stack);
            TryStackType stackType;
            int catchAddr; int finallyAddr;
            do
                ((catchAddr, finallyAddr), stackType) = stack.Pop();
            while (stackType != TryStackType.TRY && stackType != TryStackType.CATCH && stack.Count > 0);
            if (stackType == TryStackType.TRY)  // goto CATCH or FINALLY
            {
                // try with catch: cancel throw and execute catch
                if (catchAddr != -1)
                {
                    addr = catchAddr;
                    stack.Push(((-1, finallyAddr), TryStackType.CATCH));
                    coveredMap[entranceAddr] = CoverInstruction(addr, stack: stack, throwed: false);
                    return coveredMap[entranceAddr];
                }
                // try without catch: execute finally but keep throwing
                else if (finallyAddr != -1)
                {
                    coveredMap[addr] = BranchType.THROW;
                    addr = finallyAddr;
                    stack.Push(((-1, -1), TryStackType.FINALLY));
                    coveredMap[entranceAddr] = CoverInstruction(addr, stack: stack, throwed: true);
                    return coveredMap[entranceAddr];
                }
            }
            // throwed in catch with finally: execute finally but keep throwing
            if (stackType == TryStackType.CATCH)
            {
                if (finallyAddr != -1)
                {
                    addr = finallyAddr;
                    stack.Push(((-1, -1), TryStackType.FINALLY));
                }
                return CoverInstruction(addr, stack: stack, throwed: true);
            }
            // not in try and not in catch
            coveredMap[entranceAddr] = BranchType.THROW;
            return BranchType.THROW;
        }

        public BranchType HandleAbort(int entranceAddr, int addr, Stack<((int returnAddr, int finallyAddr), TryStackType stackType)> stack)
        {
            // See if we are in a try or catch. There may still be runtime exceptions
            ((int catchAddr, int finallyAddr), TryStackType stackType) = stack.Peek();
            if (stackType == TryStackType.TRY && catchAddr != -1 ||
                stackType == TryStackType.CATCH && finallyAddr != -1)
            {
                // Visit catchAddr because there may still be exceptions at runtime
                if (HandleThrow(entranceAddr, addr, stack) == BranchType.OK)
                {
                    coveredMap[entranceAddr] = BranchType.OK;
                    return BranchType.OK;
                }
            }
            coveredMap[entranceAddr] = BranchType.ABORT;
            return coveredMap[entranceAddr];
        }

        /// <summary>
        /// Cover a basic block, and recursively cover all branches
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="script"></param>
        /// <param name="coveredMap"></param>
        /// <returns>Whether it is possible to return without exception</returns>
        /// <exception cref="BadScriptException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public BranchType CoverInstruction(int addr,
            Stack<((int returnAddr, int finallyAddr), TryStackType stackType)>? stack = null,
            bool throwed = false)
        {
            int entranceAddr = addr;
            if (stack == null)
            {
                stack = new();
                stack.Push(((-1, -1), TryStackType.ENTRY));
            }
            else
                stack = CopyStack(stack);
            if (throwed)
            {
                ((int catchAddr, int finallyAddr), TryStackType stackType) = stack.Peek();
                if (stackType != TryStackType.FINALLY)
                {
                    coveredMap[entranceAddr] = BranchType.THROW;
                    return BranchType.THROW;
                }
            }
            while (true)
            {
                // For the analysis of basic blocks,
                // we launched new recursion when exception is catched.
                // Here we have the exception not catched
                if (!coveredMap.ContainsKey(addr))
                    throw new BadScriptException($"wrong address {addr}");
                if (coveredMap[addr] != BranchType.UNCOVERED)
                    // We have visited the code. Skip it.
                    return coveredMap[addr];
                if (jumpTargetToSources.ContainsKey(addr) && addr != entranceAddr)
                    // on target of jump, start a new recursion to split basic blocks
                    return CoverInstruction(addr, stack, throwed);
                Instruction instruction = script.GetInstruction(addr);
                if (instruction.OpCode != OpCode.NOP)
                {
                    coveredMap[addr] = BranchType.OK;
                    // Add a basic block starting from entranceAddr
                    if (!basicBlocks.TryGetValue(entranceAddr, out Dictionary<int, Instruction>? instructions))
                    {
                        instructions = new Dictionary<int, Instruction>();
                        basicBlocks.Add(entranceAddr, instructions);
                    }
                    // Add this instruction to the basic block starting from entranceAddr
                    instructions.Add(addr, instruction);
                }

                // TODO: ABORTMSG may THROW instead of ABORT. Just throw new NotImplementedException for ABORTMSG?
                if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                    return HandleAbort(entranceAddr, addr, stack);
                if (callWithJump.Contains(instruction.OpCode))
                {
                    int callTarget = ComputeJumpTarget(addr, instruction);
                    BranchType returnedType = CoverInstruction(callTarget);
                    if (returnedType == BranchType.OK)
                        return CoverInstruction(addr + instruction.Size, stack);
                    if (returnedType == BranchType.ABORT)
                        return HandleAbort(entranceAddr, addr, stack);
                    if (returnedType == BranchType.THROW)
                        return HandleThrow(entranceAddr, addr, stack);
                }
                if (instruction.OpCode == OpCode.RET)
                {
                    // See if we are in a try. There may still be runtime exceptions
                    HandleThrow(entranceAddr, addr, stack);
                    coveredMap[entranceAddr] = BranchType.OK;
                    return coveredMap[entranceAddr];
                }
                if (tryThrowFinally.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                    {
                        stack.Push((ComputeTryTarget(addr, instruction), TryStackType.TRY));
                        return CoverInstruction(addr + instruction.Size, stack);
                    }
                    if (instruction.OpCode == OpCode.THROW)
                        return HandleThrow(entranceAddr, addr, stack);
                    if (instruction.OpCode == OpCode.ENDTRY || instruction.OpCode == OpCode.ENDTRY_L)
                    {
                        ((int catchAddr, int finallyAddr), TryStackType stackType) = stack.Peek();
                        if (stackType != TryStackType.TRY && stackType != TryStackType.CATCH)
                            throw new BadScriptException("No try stack on ENDTRY");

                        // Visit catchAddr and finallyAddr because there may still be exceptions at runtime
                        HandleThrow(entranceAddr, addr, stack);
                        coveredMap[entranceAddr] = BranchType.OK;

                        stack.Pop();
                        int endPointer = ComputeJumpTarget(addr, instruction);
                        if (finallyAddr != -1)
                        {
                            stack.Push(((-1, endPointer), TryStackType.FINALLY));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        return CoverInstruction(addr, stack, throwed);
                    }
                    if (instruction.OpCode == OpCode.ENDFINALLY)
                    {
                        ((int catchAddr, int finallyAddr), TryStackType stackType) = stack.Pop();
                        if (stackType != TryStackType.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        if (throwed)
                        {
                            // For this basic block in finally, the branch type is OK
                            coveredMap[entranceAddr] = BranchType.OK;
                            // The throw is caused by previous codes
                            return BranchType.THROW;
                        }
                        return CoverInstruction(addr + instruction.Size, stack, false);
                    }
                }
                if (unconditionalJump.Contains(instruction.OpCode))
                    //addr = ComputeJumpTarget(addr, instruction);
                    //continue;
                    // For the analysis of basic blocks, we launch a new recursion
                    return CoverInstruction(ComputeJumpTarget(addr, instruction), stack, throwed);
                if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                {
                    BranchType noJump = CoverInstruction(addr + instruction.Size, stack);
                    BranchType jump = CoverInstruction(ComputeJumpTarget(addr, instruction), stack);
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                    {
                        // See if we are in a try. There may still be runtime exceptions
                        HandleThrow(entranceAddr, addr, stack);
                        coveredMap[entranceAddr] = BranchType.OK;
                        return coveredMap[entranceAddr];
                    }
                    if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                        return HandleAbort(entranceAddr, addr, stack);
                    if (noJump == BranchType.THROW || jump == BranchType.THROW)  // THROW, ABORT => THROW
                        return HandleThrow(entranceAddr, addr, stack);
                    throw new Exception($"Unknown {nameof(BranchType)} {noJump} {jump}");
                }

                addr += instruction.Size;
            }
        }
    }
}
