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
                if (!TryFindReachableStorageWrite(entry!, out MethodConvert? writer)) continue;

                string detail = ReferenceEquals(writer, entry)
                    ? "writes to contract storage"
                    : $"reaches a contract-storage write through '{writer!.Symbol.Name}'";
                yield return new CompilationException(method.Symbol, DiagnosticId.SafeMethodStateMutation,
                    $"Method '{method.Symbol.Name}' is marked [Safe] but {detail}. " +
                    "A Safe method must not modify contract state; remove the [Safe] attribute or the state mutation.");
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
    }
}
