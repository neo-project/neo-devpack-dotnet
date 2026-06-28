// Copyright (C) 2015-2026 The Neo Project.
//
// WitnessFlowAnalysis.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler.ControlFlow;
using Neo.SmartContract;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.SecurityAnalyzer
{
    /// <summary>
    /// Control-flow-sensitive refinement for <see cref="MissingCheckWitnessAnalyzer"/>.
    /// It answers a single question for a method that already contains at least one
    /// <c>Runtime.CheckWitness</c>: is there a storage write reachable on a path that no witness
    /// check dominates?
    /// </summary>
    /// <remarks>
    /// The analysis is intentionally <b>sound (no false positives)</b>: a missing-witness warning is
    /// a trust signal, so it only reports when the control flow is fully modelled and a write is
    /// provably unguarded. It returns <see langword="null"/> (bail out, leave the method unreported)
    /// whenever it meets anything it cannot model precisely - inter-procedural calls, dynamic calls,
    /// exception handlers, or a witness result it cannot trace to an assert / conditional branch.
    /// A "guard" is the truth outcome of a <c>CheckWitness</c> consumed by <c>ASSERT</c> or a
    /// conditional jump; <c>guarded</c> is computed as a forward must-dataflow (a point is guarded
    /// only when every path that reaches it passes such a guard).
    /// </remarks>
    internal static class WitnessFlowAnalysis
    {
        private static readonly uint[] StorageWriteSyscalls =
        {
            ApplicationEngine.System_Storage_Put.Hash,
            ApplicationEngine.System_Storage_Delete.Hash,
            ApplicationEngine.System_Storage_Local_Put.Hash,
            ApplicationEngine.System_Storage_Local_Delete.Hash,
        };

        private sealed class BlockInfo
        {
            // Index (into the block's instruction list) of an ASSERT that consumes a positive
            // CheckWitness result; everything in the block at or after this index is guarded. -1 = none.
            public int AssertGuardIndex = -1;
            // The successor block entered only when a CheckWitness returned true (edge guard). null = none.
            public BasicBlock? WitnessTrueSuccessor;
            public bool HasStorageWrite;
        }

        /// <summary>
        /// Returns <see langword="true"/> when a storage write is provably reachable unguarded,
        /// <see langword="false"/> when every write is guarded, and <see langword="null"/> when the
        /// method cannot be analysed precisely and must be left to the caller's baseline behaviour.
        /// </summary>
        public static bool? HasUnguardedWrite(ContractInBasicBlocks cfg, int methodOffset)
        {
            if (!cfg.basicBlocksByStartAddr.TryGetValue(methodOffset, out BasicBlock? entry))
                return null;

            HashSet<BasicBlock> blocks;
            try
            {
                // includeCall:false keeps the method's own blocks (including post-call continuations)
                // without descending into callees. Inter-procedural witnesses are out of scope.
                blocks = cfg.BlocksCoveredFromAddr(methodOffset, includeCall: false);
            }
            catch
            {
                return null;
            }

            var info = new Dictionary<BasicBlock, BlockInfo>();
            foreach (BasicBlock b in blocks)
            {
                BlockInfo? bi = Classify(b, blocks);
                if (bi is null)
                    return null; // unmodellable construct -> bail
                info[b] = bi;
            }

            // Forward must-dataflow: guarded[b] = on every path from entry to b, a witness guard passed.
            var guarded = new Dictionary<BasicBlock, bool>();
            foreach (BasicBlock b in blocks)
                guarded[b] = !ReferenceEquals(b, entry); // entry starts unguarded; others start ⊤
            guarded[entry] = false;

            // Precompute predecessors within the method block set.
            var preds = new Dictionary<BasicBlock, List<BasicBlock>>();
            foreach (BasicBlock b in blocks)
                preds[b] = new List<BasicBlock>();
            foreach (BasicBlock b in blocks)
                foreach (BasicBlock s in Successors(b, blocks))
                    preds[s].Add(b);

            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (BasicBlock b in blocks)
                {
                    if (ReferenceEquals(b, entry))
                        continue;
                    if (preds[b].Count == 0)
                        continue; // unreachable (other than entry); leave as ⊤
                    bool acc = true;
                    foreach (BasicBlock p in preds[b])
                        acc &= EdgeGuardValue(p, b, info[p], guarded[p]);
                    if (acc != guarded[b])
                    {
                        guarded[b] = acc;
                        changed = true;
                    }
                }
            }

            // Report when any block contains a storage write reached while unguarded.
            foreach (BasicBlock b in blocks)
            {
                if (!info[b].HasStorageWrite)
                    continue;
                if (WriteReachedUnguarded(b, guarded[b], info[b].AssertGuardIndex))
                    return true;
            }
            return false;
        }

        /// <summary>Guarded state propagated from <paramref name="pred"/> along the edge to <paramref name="succ"/>.</summary>
        private static bool EdgeGuardValue(BasicBlock pred, BasicBlock succ, BlockInfo predInfo, bool predGuardedIn)
        {
            if (predInfo.WitnessTrueSuccessor is not null && ReferenceEquals(predInfo.WitnessTrueSuccessor, succ))
                return true;
            // The guard from an in-block ASSERT (if any) makes every outgoing edge guarded.
            return predGuardedIn || predInfo.AssertGuardIndex >= 0;
        }

        private static IEnumerable<BasicBlock> Successors(BasicBlock b, HashSet<BasicBlock> blocks)
        {
            if (b.nextBlock is not null && blocks.Contains(b.nextBlock))
                yield return b.nextBlock;
            foreach (BasicBlock t in b.jumpTargetBlocks)
                if (blocks.Contains(t))
                    yield return t;
        }

        private static bool WriteReachedUnguarded(BasicBlock b, bool guardedIn, int assertGuardIndex)
        {
            bool guarded = guardedIn;
            var ins = b.instructions;
            for (int i = 0; i < ins.Count; i++)
            {
                if (assertGuardIndex >= 0 && i > assertGuardIndex)
                    guarded = true;
                var instr = ins[i];
                if (instr.OpCode == OpCode.SYSCALL && instr.Operand is { Length: 4 }
                    && StorageWriteSyscalls.Contains(instr.TokenU32) && !guarded)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Builds per-block guard/write facts, or returns <see langword="null"/> when the block
        /// contains a construct the analysis refuses to model (so the whole method bails out).
        /// </summary>
        private static BlockInfo? Classify(BasicBlock b, HashSet<BasicBlock> blocks)
        {
            var bi = new BlockInfo();
            var ins = b.instructions;
            int witnessIndex = -1;

            for (int i = 0; i < ins.Count; i++)
            {
                var instr = ins[i];
                OpCode op = instr.OpCode;

                if (op == OpCode.CALL || op == OpCode.CALL_L || op == OpCode.CALLA)
                    return null; // inter-procedural / dynamic - out of scope
                if (OpCodeTypes.tryThrowFinally.Contains(op))
                    return null; // exception control flow - out of scope

                if (op == OpCode.SYSCALL && instr.Operand is { Length: 4 })
                {
                    if (StorageWriteSyscalls.Contains(instr.TokenU32))
                        bi.HasStorageWrite = true;
                    else if (instr.TokenU32 == ApplicationEngine.System_Runtime_CheckWitness.Hash)
                    {
                        if (witnessIndex >= 0)
                            return null; // more than one witness in a block - keep it simple, bail
                        witnessIndex = i;
                    }
                }
            }

            if (witnessIndex < 0)
                return bi; // no witness in this block

            // Trace the witness result: skip NOP, fold NOT (polarity), then require ASSERT or the
            // block's terminating conditional jump as the consumer.
            bool polarity = true;
            int j = witnessIndex + 1;
            while (j < ins.Count)
            {
                OpCode op = ins[j].OpCode;
                if (op == OpCode.NOP) { j++; continue; }
                if (op == OpCode.NOT) { polarity = !polarity; j++; continue; }
                break;
            }
            if (j >= ins.Count)
                return null; // witness result unused within block - cannot classify

            OpCode consumer = ins[j].OpCode;

            if (consumer == OpCode.ASSERT || consumer == OpCode.ASSERTMSG)
            {
                if (!polarity)
                    return null; // asserting !witness is not a guard we model
                bi.AssertGuardIndex = j;
                return bi;
            }

            // Otherwise the consumer must be the block's terminating conditional jump.
            if (j != ins.Count - 1 || !IsConditionalJump(consumer))
                return null;
            if (b.jumpTargetBlocks.Count != 1)
                return null;

            bool jumpOnWitnessTrue = consumer switch
            {
                OpCode.JMPIF or OpCode.JMPIF_L => polarity,        // jumps when tested value is true
                OpCode.JMPIFNOT or OpCode.JMPIFNOT_L => !polarity, // jumps when tested value is false
                _ => false,
            };

            BasicBlock? jumpTarget = b.jumpTargetBlocks.First();
            BasicBlock? witnessTrue = jumpOnWitnessTrue ? jumpTarget : b.nextBlock;
            if (witnessTrue is null || !blocks.Contains(witnessTrue))
                return null; // witness-true edge leaves the analysed region - cannot model
            bi.WitnessTrueSuccessor = witnessTrue;
            return bi;
        }

        private static bool IsConditionalJump(OpCode op)
            => OpCodeTypes.conditionalJump.Contains(op) || OpCodeTypes.conditionalJump_L.Contains(op);
    }
}
