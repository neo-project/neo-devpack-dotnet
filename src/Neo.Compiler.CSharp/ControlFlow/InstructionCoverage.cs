// Copyright (C) 2015-2026 The Neo Project.
//
// InstructionCoverage.cs file belongs to the neo project and is free
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Neo.Compiler.ControlFlow.JumpTarget;
using static Neo.Compiler.ControlFlow.OpCodeTypes;
using VmInstruction = Neo.VM.Instruction;

namespace Neo.Compiler.ControlFlow
{
    [Flags]
    public enum TryType
    {
        NONE = 1 << 0,
        TRY = 1 << 1,
        CATCH = 1 << 2,
        FINALLY = 1 << 3,
    }

    [DebuggerDisplay("{catchAddr}, {finallyAddr}, {tryType}, {continueAfterFinally}")]
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
        THROW = 2,  // All branches surely have exceptions, but can be caught
        ABORT = 3,  // All branches abort, and cannot be caught
        UNCOVERED = 4,
    }

    public class InstructionCoverage
    {
        internal const int MaxControlFlowAnalysisDepth = 1024;

        Script script;
        // Starting from the address, whether the call will surely throw or surely abort, or may be OK
        public Dictionary<int, BranchType> coveredMap { get; protected set; }

        // key: starting address of basic block
        // value: addr -> instruction of all instructions in this basic block
        public Dictionary<int, Dictionary<int, VmInstruction>> basicBlocksInDict { get; protected set; }

        // key: starting address of basic block
        // value: starting address of the next basic block,
        //   which is reached by increased instruction pointer in normal execution
        public Dictionary<int, int> basicBlockContinuation { get; protected set; } = new();

        // key: starting address of basic block
        // value: starting address of basic blocks that is jumped to, from this basic block
        public Dictionary<int, HashSet<int>> basicBlockJump { get; protected set; } = new();
        public List<(int a, VmInstruction i)> addressAndInstructions { get; init; }
        public Dictionary<int, VmInstruction> addressToInstructions { get; init; }
        public Dictionary<VmInstruction, VmInstruction> jumpInstructionSourceToTargets { get; init; }
        public Dictionary<VmInstruction, (VmInstruction, VmInstruction)> tryInstructionSourceToTargets { get; init; }
        /// <summary>
        /// key: target of all kinds of instruction that has 1 or 2 jump targets
        /// value: sources of that jump target
        /// </summary>
        public Dictionary<VmInstruction, HashSet<VmInstruction>> jumpTargetToSources { get; init; }
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
            pushaTargets = EntryPoint.EntryPointsByPusha(nef);
            entryPointsByMethod = EntryPoint.EntryPointsByMethod(manifest);
            ResetCoveredMap(init: true);

            // It is unsafe to go parallel, because the coveredMap value is not true/false
            //Parallel.ForEach(manifest.Abi.Methods, method =>
            //    CoverInstruction(method.Offset, script, coveredMap)
            //);
            foreach (int addr in entryPointsByMethod.Keys)
                CoverInstruction(addr);
            foreach (int addr in pushaTargets.Keys)
                CoverInstruction(addr);
        }

        public void ResetCoveredMap(bool init = false)
        {
            foreach ((int addr, VmInstruction _) in addressAndInstructions)
                if (init)
                    // This throws exception when there exists duplicate addr
                    coveredMap.Add(addr, BranchType.UNCOVERED);
                else
                    coveredMap[addr] = BranchType.UNCOVERED;
        }

        public static Stack<T> CopyStack<T>(Stack<T> stack) => new(stack.Reverse());

        public BranchType HandleThrow(int entranceAddr, int throwFromAddr, Stack<TryState> stack, int analysisDepth = 0)
        {
            Frame frame = new()
            {
                EntranceAddr = entranceAddr,
                TryStack = stack,
                AnalysisDepth = analysisDepth,
                PendingExceptionCompletion = ExceptionCompletion.ReturnThrowUnassigned,
            };
            Stack<Frame> frames = new();
            frames.Push(frame);
            if (!BeginException(frame, frames, ExceptionCompletion.ReturnThrowUnassigned))
                return BranchType.THROW;
            return RunFrames(frames);
        }

        /// <summary>
        /// ABORT and ABORTMSG terminate execution without being caught.
        /// Exception handlers are still covered conservatively for other faults in the protected region.
        /// </summary>
        public BranchType HandleAbort(int entranceAddr, int abortFromAddr, Stack<TryState> stack, int analysisDepth = 0)
        {
            Frame frame = new()
            {
                EntranceAddr = entranceAddr,
                TryStack = stack,
                AnalysisDepth = analysisDepth,
                PendingExceptionCompletion = ExceptionCompletion.ReturnAbortUnassigned,
            };
            Stack<Frame> frames = new();
            frames.Push(frame);
            if (!BeginException(frame, frames, ExceptionCompletion.ReturnAbortUnassigned))
                return BranchType.ABORT;
            return RunFrames(frames);
        }

        private sealed class Frame
        {
            public int Addr;
            public Stack<TryState>? TryStackParam;
            public int? ContinueFrom;
            public int? JumpFrom;
            public int AnalysisDepth;

            public int EntranceAddr;
            public Stack<TryState> TryStack = null!;
            public readonly List<int> TailChainEntrances = new();
            public PendingKind Pending;
            public int PendingInstrAddr;
            public int PendingInstrSize;
            public IEnumerator<int>? PendingCallaTargets;
            public BranchType PendingCallaBest;
            public BranchType PendingBranchResult;
            public ExceptionResult PendingExceptionResult;
            public ExceptionCompletion PendingExceptionCompletion;
        }

        private enum PendingKind
        {
            None,
            Call,
            Calla,
            ConditionalNoJump,
            ConditionalJump,
            Exception,
        }

        private enum ExceptionResult
        {
            Catch,
            Finally,
        }

        private enum ExceptionCompletion
        {
            ReturnThrow,
            ReturnAbort,
            ReturnOk,
            ContinueEndTry,
            ReturnThrowUnassigned,
            ReturnAbortUnassigned,
        }

        private static Stack<TryState> CreateFreshTryStack()
        {
            Stack<TryState> stack = new();
            stack.Push(new TryState(-1, -1, TryType.NONE, false));
            return stack;
        }

        private BranchType Finalize(Frame frame, BranchType result)
        {
            foreach (int e in frame.TailChainEntrances)
                coveredMap[e] = result;
            return result;
        }

        private BranchType ReturnWithAssign(Frame frame, int entrance, BranchType result)
        {
            frame.TailChainEntrances.Add(entrance);
            return Finalize(frame, result);
        }

        private bool BeginException(Frame frame, Stack<Frame> frames, ExceptionCompletion completion)
        {
            Stack<TryState> stack = CopyStack(frame.TryStack);
            TryType tryStateType;
            int catchAddr;
            int finallyAddr;
            do
                (catchAddr, finallyAddr, tryStateType, _) = stack.Pop();
            while (tryStateType != TryType.TRY && tryStateType != TryType.CATCH && stack.Count > 0);

            int handlerAddr;
            ExceptionResult exceptionResult;
            if (tryStateType == TryType.TRY && catchAddr != -1)
            {
                handlerAddr = catchAddr;
                stack.Push(new TryState(-1, finallyAddr, TryType.CATCH, true));
                exceptionResult = ExceptionResult.Catch;
            }
            else if ((tryStateType == TryType.TRY || tryStateType == TryType.CATCH) && finallyAddr != -1)
            {
                handlerAddr = finallyAddr;
                stack.Push(new TryState(-1, -1, TryType.FINALLY, false));
                exceptionResult = ExceptionResult.Finally;
            }
            else
            {
                if (tryStateType == TryType.TRY)
                    throw new BadScriptException("Try without catch or finally");
                return false;
            }

            frame.Pending = PendingKind.Exception;
            frame.PendingExceptionResult = exceptionResult;
            frame.PendingExceptionCompletion = completion;
            frames.Push(new Frame
            {
                Addr = handlerAddr,
                TryStackParam = stack,
                JumpFrom = frame.EntranceAddr,
                AnalysisDepth = frame.AnalysisDepth + 1,
            });
            return true;
        }

        private BranchType? CompleteException(Frame frame, BranchType throwResult)
        {
            switch (frame.PendingExceptionCompletion)
            {
                case ExceptionCompletion.ReturnThrow:
                    return ReturnWithAssign(frame, frame.EntranceAddr, throwResult);
                case ExceptionCompletion.ReturnAbort:
                    return ReturnWithAssign(frame, frame.EntranceAddr, BranchType.ABORT);
                case ExceptionCompletion.ReturnOk:
                    return Finalize(frame, BranchType.OK);
                case ExceptionCompletion.ContinueEndTry:
                    ContinueAfterEndTry(frame);
                    return null;
                case ExceptionCompletion.ReturnThrowUnassigned:
                    return throwResult;
                case ExceptionCompletion.ReturnAbortUnassigned:
                    return BranchType.ABORT;
                default:
                    throw new InvalidOperationException($"Unknown {nameof(ExceptionCompletion)} {frame.PendingExceptionCompletion}");
            }
        }

        private void ContinueAfterEndTry(Frame frame)
        {
            Stack<TryState> tryStack = frame.TryStack;
            (_, int finallyAddr, TryType stackType, _) = tryStack.Peek();
            if (stackType != TryType.TRY && stackType != TryType.CATCH)
                throw new BadScriptException("No try stack on ENDTRY");

            tryStack.Pop();
            int endPointer = ComputeJumpTarget(frame.PendingInstrAddr, script.GetInstruction(frame.PendingInstrAddr));
            int nextAddr;
            if (finallyAddr != -1)
            {
                tryStack.Push(new(-1, endPointer, TryType.FINALLY, true));
                nextAddr = finallyAddr;
            }
            else
                nextAddr = endPointer;
            frame.TailChainEntrances.Add(frame.EntranceAddr);
            frame.Addr = nextAddr;
            frame.TryStackParam = tryStack;
            frame.ContinueFrom = null;
            frame.JumpFrom = frame.EntranceAddr;
            frame.AnalysisDepth += 1;
        }

        /// <summary>
        /// Cover a basic block and iteratively cover all branches.
        /// </summary>
        /// <param name="addr">Starting address of script. Should start at a basic block</param>
        /// <param name="tryStack">try-catch-finally stack</param>
        /// <param name="continueFromBasicBlockEntranceAddr">Specify the previous basic block entrance address, if we continue execution from the previous basic block</param>
        /// <param name="jumpFromBasicBlockEntranceAddr">Specify the entrance address of the basic block as the source of jump, if we jumped to current address from that basic block</param>
        /// <returns>Whether it is possible to return without exception</returns>
        /// <exception cref="BadScriptException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public BranchType CoverInstruction(int addr, Stack<TryState>? tryStack = null,
            int? continueFromBasicBlockEntranceAddr = null, int? jumpFromBasicBlockEntranceAddr = null,
            int analysisDepth = 0)
        {
            Stack<Frame> frames = new();
            frames.Push(new Frame
            {
                Addr = addr,
                TryStackParam = tryStack,
                ContinueFrom = continueFromBasicBlockEntranceAddr,
                JumpFrom = jumpFromBasicBlockEntranceAddr,
                AnalysisDepth = analysisDepth,
            });

            return RunFrames(frames);
        }

        private BranchType RunFrames(Stack<Frame> frames)
        {
            BranchType? childResult = null;
            while (true)
            {
                Frame frame = frames.Peek();
                BranchType? result = RunFrame(frame, frames, childResult);
                childResult = null;
                if (result == null)
                    // frame pushed a child frame and is waiting for its result
                    continue;
                frames.Pop();
                if (frames.Count == 0)
                    return result.Value;
                childResult = result.Value;
            }
        }

        private BranchType? RunFrame(Frame frame, Stack<Frame> frames, BranchType? childResult)
        {
            if (frame.Pending != PendingKind.None)
            {
                PendingKind pending = frame.Pending;
                frame.Pending = PendingKind.None;
                BranchType result = childResult!.Value;

                if (pending == PendingKind.Exception)
                {
                    BranchType throwResult = frame.PendingExceptionResult == ExceptionResult.Catch
                        ? result
                        : result == BranchType.ABORT ? BranchType.ABORT : BranchType.THROW;
                    BranchType? completed = CompleteException(frame, throwResult);
                    if (completed != null)
                        return completed;
                }
                else if (pending == PendingKind.ConditionalNoJump)
                {
                    frame.PendingBranchResult = result;
                    frame.Pending = PendingKind.ConditionalJump;
                    frames.Push(new Frame
                    {
                        Addr = ComputeJumpTarget(frame.PendingInstrAddr, script.GetInstruction(frame.PendingInstrAddr)),
                        TryStackParam = frame.TryStack,
                        JumpFrom = frame.EntranceAddr,
                        AnalysisDepth = frame.AnalysisDepth + 1,
                    });
                    return null;
                }
                else if (pending == PendingKind.ConditionalJump)
                {
                    BranchType noJump = frame.PendingBranchResult;
                    BranchType jump = result;
                    ExceptionCompletion completion;
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                        completion = ExceptionCompletion.ReturnOk;
                    else if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                        completion = ExceptionCompletion.ReturnAbort;
                    else if (noJump == BranchType.THROW || jump == BranchType.THROW)
                        completion = ExceptionCompletion.ReturnThrow;
                    else
                        throw new Exception($"Unknown {nameof(BranchType)} {noJump} {jump}");

                    frame.PendingExceptionCompletion = completion;
                    if (BeginException(frame, frames, completion))
                        return null;
                    return CompleteException(frame, BranchType.THROW);
                }
                else
                {
                    if (pending == PendingKind.Calla)
                    {
                        if (result < frame.PendingCallaBest)
                            frame.PendingCallaBest = result;
                        if (frame.PendingCallaTargets!.MoveNext())
                        {
                            // TODO: if a PUSHA cannot be covered, do not add it as a CALLA target
                            frames.Push(new Frame
                            {
                                Addr = frame.PendingCallaTargets.Current,
                                TryStackParam = null,
                                JumpFrom = frame.EntranceAddr,
                                AnalysisDepth = frame.AnalysisDepth + 1,
                            });
                            frame.Pending = PendingKind.Calla;
                            return null;
                        }
                        result = frame.PendingCallaBest;
                    }

                    int instrAddr = frame.PendingInstrAddr;
                    int instrSize = frame.PendingInstrSize;
                    if (result == BranchType.OK)
                    {
                        frame.TailChainEntrances.Add(frame.EntranceAddr);
                        frame.Addr = instrAddr + instrSize;
                        frame.TryStackParam = frame.TryStack;
                        frame.ContinueFrom = frame.EntranceAddr;
                        frame.JumpFrom = null;
                        frame.AnalysisDepth += 1;
                    }
                    else
                    {
                        ExceptionCompletion completion = result == BranchType.ABORT
                            ? ExceptionCompletion.ReturnAbort
                            : ExceptionCompletion.ReturnThrow;
                        frame.PendingExceptionCompletion = completion;
                        if (BeginException(frame, frames, completion))
                            return null;
                        return CompleteException(frame, BranchType.THROW);
                    }
                }
            }

            while (true)
            {
                if (frame.AnalysisDepth > MaxControlFlowAnalysisDepth)
                    throw new BadScriptException($"Control flow analysis depth exceeds {MaxControlFlowAnalysisDepth}");
                if (frame.ContinueFrom != null)
                    basicBlockContinuation[(int)frame.ContinueFrom] = frame.Addr;
                if (frame.JumpFrom != null)
                {
                    if (!basicBlockJump.TryGetValue((int)frame.JumpFrom, out HashSet<int>? jumpTargets))
                    {
                        jumpTargets = new();
                        basicBlockJump[(int)frame.JumpFrom] = jumpTargets;
                    }
                    jumpTargets.Add(frame.Addr);
                }
                int entranceAddr = frame.Addr;
                frame.EntranceAddr = entranceAddr;

                Stack<TryState> tryStack = frame.TryStackParam == null
                    ? CreateFreshTryStack()
                    : CopyStack(frame.TryStackParam);
                frame.TryStack = tryStack;

                (int catchAddr, int finallyAddr, TryType stackType, bool continueAfterFinally) = tryStack.Peek();

                int addr = entranceAddr;
                bool startedNewBlock = false;

                while (true)
                {
                    // For the analysis of basic blocks,
                    // we launched a new iteration when exception is catched.
                    // Here we have the exception not catched
                    if (!coveredMap.TryGetValue(addr, out BranchType value))
                        throw new BadScriptException($"wrong address {addr}");
                    VmInstruction instruction = script.GetInstruction(addr);
                    if (jumpTargetToSources.ContainsKey(instruction) && addr != entranceAddr)
                    {
                        // on target of jump, start a new iteration to split basic blocks
                        frame.TailChainEntrances.Add(entranceAddr);
                        frame.Addr = addr;
                        frame.TryStackParam = tryStack;
                        frame.ContinueFrom = entranceAddr;
                        frame.JumpFrom = null;
                        frame.AnalysisDepth += 1;
                        startedNewBlock = true;
                        break;
                    }
                    if (value != BranchType.UNCOVERED)
                    {
                        if (stackType != TryType.FINALLY)
                            // We have visited the code. Skip it.
                            return ReturnWithAssign(frame, entranceAddr, value);
                        // if we are in finally, we may visit the codes after ENDFINALLY
                        // when previous codes did not throw
                        if (value != BranchType.OK)  // the codes in finally or the codes after ENDFINALLY will THROW or ABORT
                            return ReturnWithAssign(frame, entranceAddr, value);
                        tryStack.Pop();  // end current finally
                        // No THROW or ABORT in try, catch or finally
                        // visit codes after ENDFINALLY
                        if (continueAfterFinally)
                        {
                            frame.TailChainEntrances.Add(entranceAddr);
                            frame.Addr = finallyAddr;
                            frame.TryStackParam = tryStack;
                            frame.ContinueFrom = null;
                            frame.JumpFrom = entranceAddr;
                            frame.AnalysisDepth += 1;
                            startedNewBlock = true;
                            break;
                        }
                        // FINALLY is OK, but throwed in previous TRY (without catch) or CATCH
                        return Finalize(frame, value);  // Do not set coveredMap[entranceAddr] = BranchType.THROW;
                    }
                    //if (instruction.OpCode != OpCode.NOP)
                    {
                        coveredMap[addr] = BranchType.OK;
                        // Add a basic block starting from entranceAddr
                        if (!basicBlocksInDict.TryGetValue(entranceAddr, out Dictionary<int, VmInstruction>? instructions))
                        {
                            instructions = new Dictionary<int, VmInstruction>();
                            basicBlocksInDict.Add(entranceAddr, instructions);
                        }
                        // Add this instruction to the basic block starting from entranceAddr
                        instructions.Add(addr, instruction);
                    }

                    // ABORT and ABORTMSG terminate execution and cannot be caught.
                    if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                    {
                        frame.PendingExceptionCompletion = ExceptionCompletion.ReturnAbort;
                        if (BeginException(frame, frames, ExceptionCompletion.ReturnAbort))
                            return null;
                        return CompleteException(frame, BranchType.THROW);
                    }
                    if (callWithJump.Contains(instruction.OpCode))
                    {
                        frame.PendingInstrAddr = addr;
                        frame.PendingInstrSize = instruction.Size;
                        frame.TryStack = tryStack;
                        if (instruction.OpCode == OpCode.CALLA)
                        {
                            IEnumerator<int> targets = pushaTargets.Keys.GetEnumerator();
                            if (!targets.MoveNext())
                            {
                                frame.PendingExceptionCompletion = ExceptionCompletion.ReturnAbort;
                                if (BeginException(frame, frames, ExceptionCompletion.ReturnAbort))
                                    return null;
                                return CompleteException(frame, BranchType.THROW);
                            }
                            frame.PendingCallaTargets = targets;
                            frame.PendingCallaBest = BranchType.ABORT;
                            frame.Pending = PendingKind.Calla;
                            // Use `tryStack: null` to avoid using current try stack in a deeper call stack
                            frames.Push(new Frame
                            {
                                Addr = targets.Current,
                                TryStackParam = null,
                                JumpFrom = entranceAddr,
                                AnalysisDepth = frame.AnalysisDepth + 1,
                            });
                            return null;
                        }
                        else
                        {
                            int callTarget = ComputeJumpTarget(addr, instruction);
                            frame.Pending = PendingKind.Call;
                            // Use `tryStack: null` to avoid using current try stack in a deeper call stack
                            frames.Push(new Frame
                            {
                                Addr = callTarget,
                                TryStackParam = null,
                                JumpFrom = entranceAddr,
                                AnalysisDepth = frame.AnalysisDepth + 1,
                            });
                            return null;
                        }
                    }
                    if (instruction.OpCode == OpCode.RET)
                    {
                        frame.PendingExceptionCompletion = ExceptionCompletion.ReturnOk;
                        if (BeginException(frame, frames, ExceptionCompletion.ReturnOk))
                            return null;
                        return CompleteException(frame, BranchType.THROW);
                    }
                    if (tryThrowFinally.Contains(instruction.OpCode))
                    {
                        if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                        {
                            (int catchTarget, int finallyTarget) = ComputeTryTarget(addr, instruction);
                            tryStack.Push(new(catchTarget, finallyTarget, TryType.TRY, true));
                            frame.TailChainEntrances.Add(entranceAddr);
                            frame.Addr = addr + instruction.Size;
                            frame.TryStackParam = tryStack;
                            frame.ContinueFrom = entranceAddr;
                            frame.JumpFrom = null;
                            frame.AnalysisDepth += 1;
                            startedNewBlock = true;
                            break;
                        }
                        if (instruction.OpCode == OpCode.THROW)
                        {
                            frame.PendingExceptionCompletion = ExceptionCompletion.ReturnThrow;
                            if (BeginException(frame, frames, ExceptionCompletion.ReturnThrow))
                                return null;
                            return CompleteException(frame, BranchType.THROW);
                        }
                        if (instruction.OpCode == OpCode.ENDTRY || instruction.OpCode == OpCode.ENDTRY_L)
                        {
                            if (stackType != TryType.TRY && stackType != TryType.CATCH)
                                throw new BadScriptException("No try stack on ENDTRY");

                            frame.PendingInstrAddr = addr;
                            frame.PendingExceptionCompletion = ExceptionCompletion.ContinueEndTry;
                            if (BeginException(frame, frames, ExceptionCompletion.ContinueEndTry))
                                return null;
                            return CompleteException(frame, BranchType.THROW);
                        }
                        if (instruction.OpCode == OpCode.ENDFINALLY)
                        {
                            int endPointer = finallyAddr;
                            if (stackType != TryType.FINALLY)
                                throw new BadScriptException("No finally stack on ENDFINALLY");
                            tryStack.Pop();  // pop the ending FINALLY
                            if (continueAfterFinally)
                            {
                                frame.TailChainEntrances.Add(entranceAddr);
                                frame.Addr = endPointer;
                                frame.TryStackParam = tryStack;
                                frame.ContinueFrom = null;
                                frame.JumpFrom = entranceAddr;
                                frame.AnalysisDepth += 1;
                                startedNewBlock = true;
                                break;
                            }
                            // For this basic block in finally, the branch type is OK
                            // The throw is caused by previous codes
                            return Finalize(frame, BranchType.OK);  // No need to set coveredMap[entranceAddr] because it's OK when covered
                        }
                    }
                    if (unconditionalJump.Contains(instruction.OpCode))
                    {
                        // For the analysis of basic blocks, we launch a new iteration
                        frame.TailChainEntrances.Add(entranceAddr);
                        frame.Addr = ComputeJumpTarget(addr, instruction);
                        frame.TryStackParam = tryStack;
                        frame.ContinueFrom = null;
                        frame.JumpFrom = entranceAddr;
                        frame.AnalysisDepth += 1;
                        startedNewBlock = true;
                        break;
                    }
                    if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                    {
                        frame.PendingInstrAddr = addr;
                        frame.PendingInstrSize = instruction.Size;
                        frame.Pending = PendingKind.ConditionalNoJump;
                        frames.Push(new Frame
                        {
                            Addr = addr + instruction.Size,
                            TryStackParam = tryStack,
                            ContinueFrom = entranceAddr,
                            AnalysisDepth = frame.AnalysisDepth + 1,
                        });
                        return null;
                    }

                    addr += instruction.Size;
                }

                if (!startedNewBlock)
                    throw new InvalidOperationException("Unreachable: inner loop exited without scheduling next iteration or returning");
            }
        }
    }
}
