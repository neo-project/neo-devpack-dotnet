// Copyright (C) 2015-2026 The Neo Project.
//
// CompilationContext.SafeMethodAnalysis.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler.ABI;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler
{
    public partial class CompilationContext
    {
        // Interop services that mutate persistent contract storage. A method that can reach any
        // of these is not read-only and therefore cannot honestly carry the [Safe] ABI flag.
        private static readonly uint[] StorageWriteSyscalls =
        {
            ApplicationEngine.System_Storage_Put.Hash,
            ApplicationEngine.System_Storage_Delete.Hash,
            ApplicationEngine.System_Storage_Local_Put.Hash,
            ApplicationEngine.System_Storage_Local_Delete.Hash,
        };

        /// <summary>
        /// Verifies that every exported method marked <c>[Safe]</c> is free of contract-state
        /// mutation. The manifest's <c>safe</c> flag tells wallets, explorers and dApps that a
        /// method is read-only and can be invoked without a signature prompt, so a Safe method
        /// that writes storage advertises a false on-chain guarantee — the direct analog of a
        /// Solidity <c>view</c> function performing an <c>SSTORE</c>, which solc rejects at
        /// compile time.
        /// </summary>
        /// <remarks>
        /// Detection is sound (no false positives) over the statically-resolvable intra-contract
        /// call graph: a violation is reported when a Safe method, or any method transitively
        /// reachable from it through direct calls, emits a <c>System.Storage.Put</c> /
        /// <c>System.Storage.Delete</c> syscall. Writes reachable only through virtual dispatch,
        /// dynamic (CALLA) invocations, or another contract (a cross-contract call carrying the
        /// <c>WriteStates</c> flag) are out of scope here — they require interprocedural /
        /// cross-contract dataflow and may be added later. <c>Runtime.Notify</c> is intentionally
        /// not treated as a mutation: emitting an event does not change contract state.
        /// </remarks>
        private IEnumerable<CompilationException> GetSafeMethodViolations()
        {
            foreach (AbiMethod method in _methodsExported)
            {
                if (!method.Safe) continue;
                if (!_methodsConverted.TryGetValue(method.Symbol, out MethodConvert? entry)) continue;

                if (TryFindReachableStorageWrite(entry!, out MethodConvert? writer))
                {
                    string detail = ReferenceEquals(writer, entry)
                        ? "writes to contract storage"
                        : $"reaches a contract-storage write through '{writer!.Symbol.Name}'";
                    yield return new CompilationException(method.Symbol, DiagnosticId.SafeMethodStateMutation,
                        $"Method '{method.Symbol.Name}' is marked [Safe] but {detail}. " +
                        "A Safe method must not modify contract state; remove the [Safe] attribute or the state mutation.");
                }

                if (TryFindReachableWriteCapableCall(entry!, out MethodConvert? caller))
                {
                    string detail = ReferenceEquals(caller, entry)
                        ? "calls another contract with state-writing call flags"
                        : $"reaches a state-writing external contract call through '{caller!.Symbol.Name}'";
                    yield return new CompilationException(method.Symbol, DiagnosticId.SafeMethodWriteCapableCall,
                        $"Method '{method.Symbol.Name}' is marked [Safe] but {detail}. " +
                        "The [Safe] flag tells wallets the method is read-only and can be invoked without a " +
                        "signature prompt, but a Contract.Call carrying CallFlags.WriteStates can mutate state. " +
                        "Use read-only call flags (e.g. CallFlags.ReadOnly / CallFlags.None) or remove the [Safe] attribute.");
                }
            }
        }

        /// <summary>
        /// Performs a depth-first walk of the intra-contract call graph starting at
        /// <paramref name="entry"/> and reports the first method found that directly emits a
        /// storage-write syscall (the entry method itself is checked first).
        /// </summary>
        private static bool TryFindReachableStorageWrite(MethodConvert entry, out MethodConvert? writer)
        {
            HashSet<MethodConvert> visited = new() { entry };
            Stack<MethodConvert> pending = new();
            pending.Push(entry);
            while (pending.Count > 0)
            {
                MethodConvert current = pending.Pop();
                if (WritesStorageDirectly(current))
                {
                    writer = current;
                    return true;
                }
                foreach (MethodConvert callee in current.Callees)
                    if (visited.Add(callee))
                        pending.Push(callee);
            }
            writer = null;
            return false;
        }

        /// <summary>
        /// Returns <see langword="true"/> when the method's own emitted instructions contain a
        /// storage-write syscall (which also covers writes pulled in by inline expansion).
        /// </summary>
        private static bool WritesStorageDirectly(MethodConvert method)
        {
            foreach (Instruction instruction in method.Instructions)
            {
                if (instruction.OpCode != OpCode.SYSCALL || instruction.Operand is not { Length: 4 })
                    continue;
                uint token = BitConverter.ToUInt32(instruction.Operand, 0);
                if (Array.IndexOf(StorageWriteSyscalls, token) >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Mirrors <see cref="TryFindReachableStorageWrite"/> but looks for a write-capable
        /// <c>System.Contract.Call</c>: a method marked <c>[Safe]</c> must not be able to mutate
        /// state through another contract either, so a Contract.Call carrying
        /// <see cref="CallFlags.WriteStates"/> is treated as a state mutation.
        /// </summary>
        private static bool TryFindReachableWriteCapableCall(MethodConvert entry, out MethodConvert? caller)
        {
            HashSet<MethodConvert> visited = new() { entry };
            Stack<MethodConvert> pending = new();
            pending.Push(entry);
            while (pending.Count > 0)
            {
                MethodConvert current = pending.Pop();
                if (MakesWriteCapableExternalCall(current))
                {
                    caller = current;
                    return true;
                }
                foreach (MethodConvert callee in current.Callees)
                    if (visited.Add(callee))
                        pending.Push(callee);
            }
            caller = null;
            return false;
        }

        /// <summary>
        /// Returns <see langword="true"/> only when the method provably emits a
        /// <c>System.Contract.Call</c> whose <c>CallFlags</c> argument is a compile-time constant
        /// that includes <see cref="CallFlags.WriteStates"/>.
        /// </summary>
        /// <remarks>
        /// Detection is deliberately sound (no false positives): because <c>[Safe]</c> violations
        /// fail the build, the analysis only reports when the operand layout is unambiguous. At the
        /// syscall the eval stack (top→down) is <c>scriptHash, method, callFlags, args</c>. We only
        /// conclude when the script-hash and method operands are each produced by a single
        /// value-push instruction, which proves the instruction three positions before the syscall
        /// produces the <c>callFlags</c> argument; that operand must then be a constant integer
        /// carrying the WriteStates bit. Any computed flag, non-trivial hash/method expression, or
        /// unrecognized push shape is left unflagged (a tolerated false negative).
        /// </remarks>
        private static bool MakesWriteCapableExternalCall(MethodConvert method)
        {
            IReadOnlyList<Instruction> instructions = method.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode != OpCode.SYSCALL || instruction.Operand is not { Length: 4 })
                    continue;
                if (BitConverter.ToUInt32(instruction.Operand, 0) != ApplicationEngine.System_Contract_Call.Hash)
                    continue;

                if (i < 3)
                    continue;
                if (!IsSingleValuePush(instructions[i - 1].OpCode)) // scriptHash
                    continue;
                if (!IsSingleValuePush(instructions[i - 2].OpCode)) // method
                    continue;
                if (!TryGetPushedInteger(instructions[i - 3], out BigInteger flags)) // callFlags
                    continue;

                if ((flags & (int)CallFlags.WriteStates) != 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// True for opcodes that push exactly one item onto the stack and pop nothing, so that the
        /// instruction can be skipped over when locating an argument by stack position.
        /// </summary>
        private static bool IsSingleValuePush(OpCode opCode)
        {
            if (opCode >= OpCode.PUSH0 && opCode <= OpCode.PUSH16)
                return true;
            switch (opCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                case OpCode.PUSHM1:
                case OpCode.PUSHT:
                case OpCode.PUSHF:
                case OpCode.PUSHA:
                case OpCode.PUSHNULL:
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                case OpCode.LDARG0:
                case OpCode.LDARG1:
                case OpCode.LDARG2:
                case OpCode.LDARG3:
                case OpCode.LDARG4:
                case OpCode.LDARG5:
                case OpCode.LDARG6:
                case OpCode.LDARG:
                case OpCode.LDLOC0:
                case OpCode.LDLOC1:
                case OpCode.LDLOC2:
                case OpCode.LDLOC3:
                case OpCode.LDLOC4:
                case OpCode.LDLOC5:
                case OpCode.LDLOC6:
                case OpCode.LDLOC:
                case OpCode.LDSFLD0:
                case OpCode.LDSFLD1:
                case OpCode.LDSFLD2:
                case OpCode.LDSFLD3:
                case OpCode.LDSFLD4:
                case OpCode.LDSFLD5:
                case OpCode.LDSFLD6:
                case OpCode.LDSFLD:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Decodes a compile-time integer constant from a push instruction, covering the opcodes the
        /// compiler emits for a <see cref="CallFlags"/> literal. Returns <see langword="false"/> for
        /// any non-constant or non-integer push so the caller can bail out without a false positive.
        /// </summary>
        private static bool TryGetPushedInteger(Instruction instruction, out BigInteger value)
        {
            OpCode opCode = instruction.OpCode;
            if (opCode >= OpCode.PUSH0 && opCode <= OpCode.PUSH16)
            {
                value = opCode - OpCode.PUSH0;
                return true;
            }
            if (opCode == OpCode.PUSHM1)
            {
                value = BigInteger.MinusOne;
                return true;
            }
            switch (opCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    if (instruction.Operand is { Length: > 0 } operand)
                    {
                        value = new BigInteger(operand);
                        return true;
                    }
                    break;
            }
            value = BigInteger.Zero;
            return false;
        }
    }
}
