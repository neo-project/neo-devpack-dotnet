// Copyright (C) 2015-2026 The Neo Project.
//
// SymbolicExecutor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VMInstruction = Neo.VM.Instruction;

namespace Neo.Compiler.SecurityAnalyzer.SymbolicExecution
{
    internal enum SymbolicWarningKind
    {
        UnguardedUpdate,
        UnguardedDestroy,
        VerifyStorageWrite,
        AnalysisIncomplete,
    }

    internal sealed record SymbolicWarning(SymbolicWarningKind Kind, string EntryPoint, int Address);

    internal sealed class SymbolicFindings
    {
        public bool HasUnguardedUpdate { get; private set; }
        public bool HasUnguardedDestroy { get; private set; }
        public bool HasVerifyStorageWrite { get; private set; }
        public bool AnalysisIncomplete { get; private set; }
        public List<SymbolicWarning> Warnings { get; } = new();

        public void RecordUnguardedUpdate(string entryPoint, int address, bool isDestroy)
        {
            if (isDestroy)
            {
                HasUnguardedDestroy = true;
                Warnings.Add(new SymbolicWarning(SymbolicWarningKind.UnguardedDestroy, entryPoint, address));
            }
            else
            {
                HasUnguardedUpdate = true;
                Warnings.Add(new SymbolicWarning(SymbolicWarningKind.UnguardedUpdate, entryPoint, address));
            }
        }

        public void RecordVerifyStorageWrite(string entryPoint, int address)
        {
            HasVerifyStorageWrite = true;
            Warnings.Add(new SymbolicWarning(SymbolicWarningKind.VerifyStorageWrite, entryPoint, address));
        }

        public void MarkIncomplete()
        {
            if (AnalysisIncomplete)
                return;
            AnalysisIncomplete = true;
            Warnings.Add(new SymbolicWarning(SymbolicWarningKind.AnalysisIncomplete, string.Empty, -1));
        }
    }

    internal sealed class SymbolicExecutor
    {
        public const int MaxPaths = 512;
        public const int MaxSteps = 50000;
        public const int MaxDepth = 128;

        private static readonly HashSet<OpCode> ConditionalJumps = OpCodeTypes.conditionalJump
            .Union(OpCodeTypes.conditionalJump_L)
            .ToHashSet();

        private static readonly HashSet<OpCode> UnconditionalJumps = OpCodeTypes.unconditionalJump;

        private static readonly HashSet<uint> StorageWriteSyscalls = new()
        {
            ApplicationEngine.System_Storage_Put.Hash,
            ApplicationEngine.System_Storage_Delete.Hash,
            ApplicationEngine.System_Storage_Local_Put.Hash,
            ApplicationEngine.System_Storage_Local_Delete.Hash,
        };

        private (int address, VMInstruction instruction)[] _instructions = Array.Empty<(int, VMInstruction)>();
        private Dictionary<int, int> _addressToIndex = new();
        private MethodToken[] _tokens = Array.Empty<MethodToken>();
        private int _totalPaths;
        private int _totalSteps;
        private SymbolicFindings _findings = new();

        public SymbolicFindings Analyze(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            _findings = new SymbolicFindings();
            _totalPaths = 0;
            _totalSteps = 0;

            (nef, manifest, _) = Reachability.RemoveUncoveredInstructions(nef, manifest, debugInfo as JObject);

            Script script = nef.Script;
            _instructions = script.EnumerateInstructions().ToArray();
            _addressToIndex = _instructions
                .Select((entry, index) => (entry.address, index))
                .ToDictionary(entry => entry.address, entry => entry.index);
            _tokens = nef.Tokens;

            foreach (var method in manifest.Abi.Methods)
            {
                if (!_addressToIndex.TryGetValue(method.Offset, out int startIndex))
                    continue;
                bool isVerify = string.Equals(method.Name, "verify", StringComparison.OrdinalIgnoreCase);
                ExploreEntryPoint(method.Name, startIndex, isVerify);
                if (_findings.AnalysisIncomplete)
                    break;
            }

            return _findings;
        }

        private void ExploreEntryPoint(string entryPoint, int startIndex, bool isVerify)
        {
            Stack<SymbolicState> worklist = new();
            worklist.Push(new SymbolicState(entryPoint, startIndex));

            while (worklist.Count > 0)
            {
                if (_totalPaths >= MaxPaths)
                {
                    _findings.MarkIncomplete();
                    return;
                }

                SymbolicState state = worklist.Pop();
                bool pathEnded = false;

                while (!pathEnded)
                {
                    if (_totalSteps >= MaxSteps)
                    {
                        _findings.MarkIncomplete();
                        return;
                    }
                    _totalSteps++;

                    if (state.BranchDepth > MaxDepth)
                    {
                        _findings.MarkIncomplete();
                        break;
                    }

                    if (state.InstructionIndex < 0 || state.InstructionIndex >= _instructions.Length)
                    {
                        _findings.MarkIncomplete();
                        break;
                    }

                    (int addr, VMInstruction instruction) = _instructions[state.InstructionIndex];
                    OpCode opCode = instruction.OpCode;

                    if (UnconditionalJumps.Contains(opCode) || opCode == OpCode.ENDTRY || opCode == OpCode.ENDTRY_L)
                    {
                        if (!TryJump(state, addr, instruction))
                        {
                            _findings.MarkIncomplete();
                            break;
                        }
                        continue;
                    }

                    if (ConditionalJumps.Contains(opCode))
                    {
                        HandleConditionalJump(state, addr, instruction, worklist);
                        continue;
                    }

                    switch (opCode)
                    {
                        case OpCode.CALL:
                        case OpCode.CALL_L:
                            if (!TryCall(state, addr, instruction))
                            {
                                _findings.MarkIncomplete();
                                pathEnded = true;
                            }
                            break;
                        case OpCode.CALLA:
                            _findings.MarkIncomplete();
                            pathEnded = true;
                            break;
                        case OpCode.CALLT:
                            HandleCallT(state, addr, instruction);
                            state.InstructionIndex++;
                            break;
                        case OpCode.SYSCALL:
                            HandleSyscall(state, addr, instruction, isVerify);
                            state.InstructionIndex++;
                            break;
                        case OpCode.RET:
                            if (state.TryPopReturn(out int returnIndex))
                            {
                                state.InstructionIndex = returnIndex;
                            }
                            else
                            {
                                pathEnded = true;
                            }
                            break;
                        case OpCode.THROW:
                        case OpCode.ABORT:
                        case OpCode.ABORTMSG:
                            pathEnded = true;
                            break;
                        case OpCode.ASSERT:
                            HandleAssert(state, consumeMessage: false);
                            state.InstructionIndex++;
                            break;
                        case OpCode.ASSERTMSG:
                            HandleAssert(state, consumeMessage: true);
                            state.InstructionIndex++;
                            break;
                        case OpCode.ENDFINALLY:
                            pathEnded = true;
                            break;
                        default:
                            ApplyStackEffect(state, addr, instruction);
                            state.InstructionIndex++;
                            break;
                    }
                }

                _totalPaths++;
            }
        }

        private static void HandleAssert(SymbolicState state, bool consumeMessage)
        {
            if (consumeMessage)
                state.Pop();
            SymbolicValue condition = state.Pop();
            if (condition.Kind == SymbolicValueKind.WitnessCheck)
                state.HasWitnessGuard = true;
        }

        private void HandleConditionalJump(SymbolicState state, int addr, VMInstruction instruction, Stack<SymbolicState> worklist)
        {
            OpCode opCode = instruction.OpCode;
            SymbolicValue condition;
            if (opCode is OpCode.JMPEQ or OpCode.JMPNE or OpCode.JMPGT or OpCode.JMPGE or OpCode.JMPLT or OpCode.JMPLE
                or OpCode.JMPEQ_L or OpCode.JMPNE_L or OpCode.JMPGT_L or OpCode.JMPGE_L or OpCode.JMPLT_L or OpCode.JMPLE_L)
            {
                state.Pop();
                condition = state.Pop();
            }
            else
            {
                condition = state.Pop();
            }

            if (!TryGetJumpTarget(addr, instruction, out int targetIndex))
            {
                _findings.MarkIncomplete();
                return;
            }

            SymbolicState jumpState = state.Clone();
            SymbolicState fallthroughState = state.Clone();

            bool isWitnessCondition = condition.Kind == SymbolicValueKind.WitnessCheck;
            bool jumpIfTrue = opCode is OpCode.JMPIF or OpCode.JMPIF_L;
            bool jumpIfFalse = opCode is OpCode.JMPIFNOT or OpCode.JMPIFNOT_L;

            if (isWitnessCondition)
            {
                if (jumpIfTrue)
                    jumpState.HasWitnessGuard = true;
                if (jumpIfFalse)
                    fallthroughState.HasWitnessGuard = true;
            }

            jumpState.InstructionIndex = targetIndex;
            fallthroughState.InstructionIndex = state.InstructionIndex + 1;
            jumpState.BranchDepth = state.BranchDepth + 1;
            fallthroughState.BranchDepth = state.BranchDepth + 1;

            worklist.Push(jumpState);
            state.InstructionIndex = fallthroughState.InstructionIndex;
            state.HasWitnessGuard = fallthroughState.HasWitnessGuard;
            state.BranchDepth = fallthroughState.BranchDepth;
        }

        private bool TryJump(SymbolicState state, int addr, VMInstruction instruction)
        {
            if (!TryGetJumpTarget(addr, instruction, out int targetIndex))
                return false;
            state.InstructionIndex = targetIndex;
            return true;
        }

        private bool TryGetJumpTarget(int addr, VMInstruction instruction, out int targetIndex)
        {
            targetIndex = -1;
            int targetAddr;
            try
            {
                targetAddr = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
            }
            catch
            {
                return false;
            }
            return _addressToIndex.TryGetValue(targetAddr, out targetIndex);
        }

        private bool TryCall(SymbolicState state, int addr, VMInstruction instruction)
        {
            if (!TryGetJumpTarget(addr, instruction, out int targetIndex))
                return false;
            int returnIndex = state.InstructionIndex + 1;
            state.PushReturn(returnIndex);
            state.InstructionIndex = targetIndex;
            if (state.CallStack.Count > MaxDepth)
                return false;
            return true;
        }

        private void HandleCallT(SymbolicState state, int addr, VMInstruction instruction)
        {
            if (instruction.TokenU16 >= _tokens.Length)
                return;
            MethodToken token = _tokens[instruction.TokenU16];
            if (token.Hash == NativeContract.ContractManagement.Hash)
            {
                bool isUpdate = token.Method == "update";
                bool isDestroy = token.Method == "destroy";
                if ((isUpdate || isDestroy) && (token.CallFlags & CallFlags.WriteStates) != 0)
                {
                    if (!state.HasWitnessGuard)
                        _findings.RecordUnguardedUpdate(state.EntryPoint, addr, isDestroy);
                }
            }
            PopArgs(state, token.ParametersCount);
            if (token.HasReturnValue)
                state.Push(SymbolicValue.Unknown);
        }

        private void HandleSyscall(SymbolicState state, int addr, VMInstruction instruction, bool isVerify)
        {
            uint sysCall = instruction.TokenU32;
            if (sysCall == ApplicationEngine.System_Runtime_CheckWitness.Hash)
            {
                SymbolicValue argument = state.Pop();
                state.Push(SymbolicValue.WitnessCheck(argument));
                return;
            }

            if (StorageWriteSyscalls.Contains(sysCall))
            {
                if (isVerify)
                    _findings.RecordVerifyStorageWrite(state.EntryPoint, addr);
                PopArgs(state, 3);
                return;
            }

            if (sysCall == ApplicationEngine.System_Contract_Call.Hash)
            {
                SymbolicValue? hashValue = PeekStack(state, 0);
                SymbolicValue? methodValue = PeekStack(state, 1);
                if (hashValue != null && methodValue != null
                    && hashValue.TryGetUInt160(out var hash)
                    && methodValue.TryGetString(out string method)
                    && hash == NativeContract.ContractManagement.Hash
                    && (string.Equals(method, "update", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(method, "destroy", StringComparison.OrdinalIgnoreCase)))
                {
                    if (!state.HasWitnessGuard)
                        _findings.RecordUnguardedUpdate(state.EntryPoint, addr, string.Equals(method, "destroy", StringComparison.OrdinalIgnoreCase));
                }
                PopArgs(state, 4);
                state.Push(SymbolicValue.Unknown);
                return;
            }

            // Default: conservatively drop one argument if present to avoid unbounded stack growth.
            if (state.Stack.Count > 0)
                state.Pop();
        }

        private static void ApplyStackEffect(SymbolicState state, int addr, VMInstruction instruction)
        {
            OpCode opCode = instruction.OpCode;
            if (OpCodeTypes.pushConst.Contains(opCode))
            {
                state.Push(CreateConstant(addr, instruction));
                return;
            }

            switch (opCode)
            {
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                    state.Push(SymbolicValue.FromByteString(instruction.Operand.Span.ToArray()));
                    break;
                case OpCode.DUP:
                    state.Push(state.Peek());
                    break;
                case OpCode.OVER:
                    if (state.Stack.Count >= 2)
                        state.Push(state.Stack[^2]);
                    else
                        state.Push(SymbolicValue.Unknown);
                    break;
                case OpCode.SWAP:
                    if (state.Stack.Count >= 2)
                    {
                        int last = state.Stack.Count - 1;
                        (state.Stack[last], state.Stack[last - 1]) = (state.Stack[last - 1], state.Stack[last]);
                    }
                    break;
                case OpCode.DROP:
                    state.Pop();
                    break;
                case OpCode.NIP:
                    if (state.Stack.Count >= 2)
                        state.Stack.RemoveAt(state.Stack.Count - 2);
                    break;
                case OpCode.ROT:
                    if (state.Stack.Count >= 3)
                    {
                        int last = state.Stack.Count - 1;
                        SymbolicValue a = state.Stack[last - 2];
                        state.Stack[last - 2] = state.Stack[last - 1];
                        state.Stack[last - 1] = state.Stack[last];
                        state.Stack[last] = a;
                    }
                    break;
                case OpCode.PICK:
                case OpCode.ROLL:
                    state.Pop();
                    state.Push(SymbolicValue.Unknown);
                    break;
                case OpCode.NOT:
                    state.Pop();
                    state.Push(SymbolicValue.Unknown);
                    break;
                case OpCode.PUSHNULL:
                    state.Push(SymbolicValue.Unknown);
                    break;
                case OpCode.PUSHT:
                    state.Push(SymbolicValue.FromBoolean(true));
                    break;
                case OpCode.PUSHF:
                    state.Push(SymbolicValue.FromBoolean(false));
                    break;
                case OpCode.PUSHM1:
                    state.Push(SymbolicValue.FromInteger(-1));
                    break;
                case OpCode.PUSH0:
                    state.Push(SymbolicValue.FromInteger(0));
                    break;
                case OpCode.PUSH1:
                case OpCode.PUSH2:
                case OpCode.PUSH3:
                case OpCode.PUSH4:
                case OpCode.PUSH5:
                case OpCode.PUSH6:
                case OpCode.PUSH7:
                case OpCode.PUSH8:
                case OpCode.PUSH9:
                case OpCode.PUSH10:
                case OpCode.PUSH11:
                case OpCode.PUSH12:
                case OpCode.PUSH13:
                case OpCode.PUSH14:
                case OpCode.PUSH15:
                case OpCode.PUSH16:
                    state.Push(SymbolicValue.FromInteger((int)opCode - (int)OpCode.PUSH0));
                    break;
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    state.Push(SymbolicValue.FromInteger(new BigInteger(instruction.Operand.Span)));
                    break;
                case OpCode.PUSHA:
                    try
                    {
                        int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                        state.Push(SymbolicValue.FromInteger(target));
                    }
                    catch
                    {
                        state.Push(SymbolicValue.Unknown);
                    }
                    break;
                default:
                    break;
            }
        }

        private static SymbolicValue CreateConstant(int addr, VMInstruction instruction)
        {
            OpCode opCode = instruction.OpCode;
            if (opCode == OpCode.PUSHT)
                return SymbolicValue.FromBoolean(true);
            if (opCode == OpCode.PUSHF)
                return SymbolicValue.FromBoolean(false);
            if (opCode == OpCode.PUSHM1)
                return SymbolicValue.FromInteger(-1);
            if (opCode >= OpCode.PUSH0 && opCode <= OpCode.PUSH16)
                return SymbolicValue.FromInteger((int)opCode - (int)OpCode.PUSH0);
            if (opCode == OpCode.PUSHNULL)
                return SymbolicValue.Unknown;
            if (opCode == OpCode.PUSHA)
            {
                try
                {
                    int target = Neo.Optimizer.JumpTarget.ComputeJumpTarget(addr, instruction);
                    return SymbolicValue.FromInteger(target);
                }
                catch
                {
                    return SymbolicValue.Unknown;
                }
            }
            if (opCode == OpCode.PUSHINT8
                || opCode == OpCode.PUSHINT16
                || opCode == OpCode.PUSHINT32
                || opCode == OpCode.PUSHINT64
                || opCode == OpCode.PUSHINT128
                || opCode == OpCode.PUSHINT256)
                return SymbolicValue.FromInteger(new BigInteger(instruction.Operand.Span));

            if (opCode == OpCode.PUSHDATA1 || opCode == OpCode.PUSHDATA2 || opCode == OpCode.PUSHDATA4)
                return SymbolicValue.FromByteString(instruction.Operand.Span.ToArray());

            return SymbolicValue.Unknown;
        }

        private static void PopArgs(SymbolicState state, int count)
        {
            for (int i = 0; i < count; i++)
                state.Pop();
        }

        private static SymbolicValue? PeekStack(SymbolicState state, int indexFromTop)
        {
            int index = state.Stack.Count - 1 - indexFromTop;
            if (index < 0 || index >= state.Stack.Count)
                return null;
            return state.Stack[index];
        }
    }
}
