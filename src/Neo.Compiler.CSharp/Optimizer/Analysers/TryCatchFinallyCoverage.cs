using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks.Dataflow;
using static Neo.Optimizer.JumpTarget;
using static Neo.Optimizer.OpCodeTypes;

namespace Neo.Optimizer
{
    public class TryCatchFinallySingleCoverage
    {
        public readonly ContractInBasicBlocks contractInBasicBlocks;
        public readonly int tryAddr;
        public readonly int catchAddr;
        public readonly int finallyAddr;
        public readonly BasicBlock tryBlock;
        public readonly BasicBlock? catchBlock;
        public readonly BasicBlock? finallyBlock;
        public HashSet<BasicBlock> tryBlocks { get; protected set; }
        public HashSet<BasicBlock> catchBlocks { get; protected set; }
        public HashSet<BasicBlock> finallyBlocks { get; protected set; }
        // where to go after the try-catch-finally is finished
        public HashSet<BasicBlock> endingBlocks { get; protected set; }
        public TryCatchFinallySingleCoverage(ContractInBasicBlocks contractInBasicBlocks,
            int tryAddr, int catchAddr, int finallyAddr,
            BasicBlock tryBlock, BasicBlock? catchBlock, BasicBlock? finallyBlock,
            HashSet<BasicBlock> tryBlocks, HashSet<BasicBlock> catchBlocks, HashSet<BasicBlock> finallyBlocks,
            HashSet<BasicBlock> endingBlocks)
        {
            this.contractInBasicBlocks = contractInBasicBlocks;
            this.tryAddr = tryAddr;
            this.catchAddr = catchAddr;
            this.finallyAddr = finallyAddr;
            this.tryBlock = tryBlock;
            this.catchBlock = catchBlock;
            this.finallyBlock = finallyBlock;
            this.tryBlocks = tryBlocks;
            this.catchBlocks = catchBlocks;
            this.finallyBlocks = finallyBlocks;
            this.endingBlocks = endingBlocks;
        }

        public TryCatchFinallySingleCoverage(ContractInBasicBlocks contractInBasicBlocks,
            int tryAddr, int catchAddr, int finallyAddr,
            BasicBlock tryBlock, BasicBlock? catchBlock, BasicBlock? finallyBlock) :
            this(contractInBasicBlocks, tryAddr, catchAddr, finallyAddr, tryBlock, catchBlock, finallyBlock,
                [], [], [], [])
        { }
    }

    public class TryCatchFinallyCoverage
    {
        public ContractInBasicBlocks contractInBasicBlocks { get; protected set; }
        // key: start of try block. prevBlock.instructions.Last() is TRY
        public Dictionary<BasicBlock, TryCatchFinallySingleCoverage> allTry { get; protected set; }

        public TryCatchFinallyCoverage(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
            : this(new ContractInBasicBlocks(nef, manifest, debugInfo)) { }

        public TryCatchFinallyCoverage(ContractInBasicBlocks contractInBasicBlocks)
        {
            this.contractInBasicBlocks = contractInBasicBlocks;
            allTry = new();
            foreach (BasicBlock b in contractInBasicBlocks.sortedBasicBlocks)
            {
                Instruction lastI = b.instructions.Last();
                if (lastI.OpCode == OpCode.TRY || lastI.OpCode == OpCode.TRY_L)
                {
                    (int catchAddr, int finallyAddr) = JumpTarget.ComputeTryTarget(b.lastAddr, lastI);
                    BasicBlock? catchBlock = catchAddr < 0 ? null : contractInBasicBlocks.basicBlocksByStartAddr[catchAddr];
                    BasicBlock? finallyBlock = finallyAddr < 0 ? null : contractInBasicBlocks.basicBlocksByStartAddr[finallyAddr];
                    allTry.Add(b.nextBlock!, new TryCatchFinallySingleCoverage(contractInBasicBlocks, b.lastAddr, catchAddr, finallyAddr, b, catchBlock, finallyBlock));
                }
            }
            foreach (BasicBlock b in allTry.Keys)
            {
                Stack<(BasicBlock tryBlock, BasicBlock? endFinallyBlock, TryType tryType, bool continueAfterFinally)> tryStack = new();
                tryStack.Push((b, null, TryType.TRY, true));
                CoverSingleTry(b, tryStack);
            }
        }

        public BranchType CoverSingleTry(BasicBlock currentBlock,
            Stack<(BasicBlock tryBlock, BasicBlock? endFinallyBlock, TryType tryType, bool continueAfterFinally)> tryStack)
        {
            if (tryStack.Count <= 0)
                return BranchType.OK;
            tryStack = InstructionCoverage.CopyStack(tryStack);
            (BasicBlock tryBlock, BasicBlock? endFinallyBlock, TryType tryType, bool continueAfterFinally) = tryStack.Peek();
            HashSet<BasicBlock> handledBlocks = tryType switch
            {
                TryType.TRY => allTry[tryBlock].tryBlocks,
                TryType.CATCH => allTry[tryBlock].catchBlocks,
                TryType.FINALLY => allTry[tryBlock].finallyBlocks,
                _ => throw new ArgumentException($"Invalid {nameof(tryType)} {tryType}"),
            };
            while (true)
            {
                if (handledBlocks.Contains(currentBlock))
                    return currentBlock.branchType;
                handledBlocks.Add(currentBlock);
                Instruction instruction = currentBlock.instructions.Last();
                if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                    return BranchType.ABORT;
                if (callWithJump.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.CALLA)
                        foreach (int callaTarget in contractInBasicBlocks.coverage.pushaTargets.Keys)
                        {
                            BasicBlock callABlock = contractInBasicBlocks.basicBlocksByStartAddr[callaTarget];
                            CoverSingleTry(callABlock, tryStack);
                            // TODO: if a PUSHA cannot be covered, do not add it as a CALLA target
                        }
                    else
                    {
                        int callTarget = ComputeJumpTarget(currentBlock.lastAddr, instruction);
                        BasicBlock calledBlock = contractInBasicBlocks.basicBlocksByStartAddr[callTarget];
                        CoverSingleTry(calledBlock, tryStack);
                    }
                    if (currentBlock.branchType == BranchType.OK)
                        if (currentBlock.nextBlock != null)
                            // nextBlock can still be null, when we call a method in try that ABORTs
                            return CoverSingleTry(currentBlock.nextBlock!, tryStack);
                    return currentBlock.branchType;
                }
                if (instruction.OpCode == OpCode.RET)
                    return currentBlock.branchType;
                if (tryThrowFinally.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                    {
                        tryStack.Push((currentBlock.nextBlock!, null, TryType.TRY, true));
                        CoverSingleTry(currentBlock.nextBlock!, tryStack);
                    }
                    if (instruction.OpCode == OpCode.THROW)
                    {
                        BasicBlock prevTryBlock;
                        TryType tryStateType;
                        do
                            (prevTryBlock, _, tryStateType, _) = tryStack.Pop();
                        while (tryStateType != TryType.TRY && tryStateType != TryType.CATCH && tryStack.Count > 0);
                        if (tryStateType == TryType.TRY)
                        {
                            if (allTry[prevTryBlock].catchBlock != null)
                            {
                                tryStack.Push((prevTryBlock, null, TryType.CATCH, true));
                                return CoverSingleTry(allTry[prevTryBlock].catchBlock!, tryStack);
                            }
                            else if (allTry[prevTryBlock].finallyBlock != null)
                            {
                                tryStack.Push((prevTryBlock, null, TryType.FINALLY, false));
                                if (CoverSingleTry(allTry[prevTryBlock].finallyBlock!, tryStack) == BranchType.ABORT)
                                    return BranchType.ABORT;
                                return BranchType.THROW;
                            }
                        }
                        if (tryType == TryType.CATCH && allTry[tryBlock].finallyBlock != null)
                        {
                            tryStack.Push((prevTryBlock, null, TryType.FINALLY, false));
                            if (CoverSingleTry(allTry[prevTryBlock].finallyBlock!, tryStack) == BranchType.ABORT)
                                return BranchType.ABORT;
                        }
                        return BranchType.THROW;
                    }
                    if (instruction.OpCode == OpCode.ENDTRY || instruction.OpCode == OpCode.ENDTRY_L)
                    {
                        if (tryType != TryType.TRY && tryType != TryType.CATCH)
                            throw new BadScriptException("No try stack on ENDTRY");
                        tryStack.Pop();  // pop the ending TRY or CATCH
                        if (tryType == TryType.TRY && allTry[tryBlock].catchBlock != null)
                        {
                            tryStack.Push((tryBlock, null, TryType.CATCH, true));
                            CoverSingleTry(allTry[tryBlock].catchBlock!, tryStack);
                            tryStack.Pop();  // Pop the CATCH
                        }
                        int endPointer = ComputeJumpTarget(currentBlock.lastAddr, instruction);
                        endFinallyBlock = contractInBasicBlocks.basicBlocksByStartAddr[endPointer];
                        BasicBlock nextBlock;
                        if (allTry[tryBlock].finallyBlock != null)
                        {
                            tryStack.Push(new(tryBlock, endFinallyBlock, TryType.FINALLY, true));
                            nextBlock = allTry[tryBlock].finallyBlock!;
                        }
                        else
                        {
                            allTry[tryBlock].endingBlocks.Add(endFinallyBlock);
                            nextBlock = endFinallyBlock;
                        }
                        return CoverSingleTry(nextBlock, tryStack);
                    }
                    if (instruction.OpCode == OpCode.ENDFINALLY)
                    {
                        if (tryType != TryType.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        tryStack.Pop();  // pop the ending FINALLY
                        if (continueAfterFinally)
                        {
                            allTry[tryBlock].endingBlocks.Add(endFinallyBlock!);
                            return CoverSingleTry(endFinallyBlock!, tryStack);
                        }
                        // For this basic block in finally, the branch type is OK
                        // The throw is caused by previous codes
                        return BranchType.OK;
                    }
                }
                if (unconditionalJump.Contains(instruction.OpCode))
                {
                    int target = ComputeJumpTarget(currentBlock.lastAddr, instruction);
                    BasicBlock targetBlock = contractInBasicBlocks.basicBlocksByStartAddr[target];
                    return CoverSingleTry(targetBlock, tryStack);
                }
                if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                {
                    BranchType noJump = CoverSingleTry(currentBlock.nextBlock!, tryStack);
                    int target = ComputeJumpTarget(currentBlock.lastAddr, instruction);
                    BasicBlock targetBlock = contractInBasicBlocks.basicBlocksByStartAddr[target];
                    BranchType jump = CoverSingleTry(targetBlock, tryStack);
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                        return BranchType.OK;
                    if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                        return BranchType.ABORT;
                    if (noJump == BranchType.THROW || jump == BranchType.THROW)  // THROW, ABORT => THROW
                        return BranchType.THROW;
                }
                currentBlock = currentBlock.nextBlock!;
            }
        }
    }
}
