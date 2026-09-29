// Copyright (C) 2015-2026 The Neo Project.
//
// JumpCompresser.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static Neo.Compiler.ControlFlow.JumpTarget;
using static Neo.Compiler.ControlFlow.OpCodeTypes;

namespace Neo.Optimizer
{
    public static class JumpCompresser
    {
        /// <summary>
        /// A preparation for operations that may increase contract size.
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        /// [Strategy(Priority = int.MinValue)]  // No attribute
        public static (NefFile, ContractManifest, JObject?) UncompressJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int newAddr = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (SingleJumpInOperand(i) && i.OpCode != OpCode.CALLA)
                {// Be aware that PUSHA is a jumping instruction without _L version.
                    Instruction target = jumpSourceToTargets[i];
                    int delta = 0;  // do not care about delta in first build
                    OpCode actualOpCode = i.OpCode;
                    if (shortInstructions.Contains(i.OpCode))
                        actualOpCode = i.OpCode + 1;  // change to long version
                    IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                    // No size prefix for instruction. Skipping
                    newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(delta));
                    Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                    simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                    oldSequencePointAddressToNew.Add(a, newAddr);
                    newAddr += newInstruction.Size;

                    jumpSourceToTargets.Remove(i);
                    jumpSourceToTargets.Add(newInstruction, target);
                    jumpTargetToSources[target].Remove(i);
                    jumpTargetToSources[target].Add(newInstruction);
                    OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                        jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                    continue;
                }
                if (DoubleJumpInOperand(i))
                {
                    (Instruction t1, Instruction t2) = trySourceToTargets[i];
                    int delta = 0;  // do not care about delta in first build
                    OpCode actualOpCode = i.OpCode;
                    if (shortInstructions.Contains(i.OpCode))
                        actualOpCode = i.OpCode + 1;  // change to long version
                    IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                    int operandSizeLength = Optimizer.OperandSizePrefixTable[(int)actualOpCode];
                    // No size prefix for instruction. Skipping
                    newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(delta)).Concat(BitConverter.GetBytes(delta));
                    Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                    simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                    oldSequencePointAddressToNew.Add(a, newAddr);
                    newAddr += newInstruction.Size;

                    trySourceToTargets.Remove(i);
                    trySourceToTargets.Add(newInstruction, (t1, t2));
                    jumpTargetToSources[t1].Remove(i);
                    jumpTargetToSources[t1].Add(newInstruction);
                    jumpTargetToSources[t2].Remove(i);
                    jumpTargetToSources[t2].Add(newInstruction);
                    OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                        jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                    continue;
                }
                simplifiedInstructionsToAddress.Add(i, newAddr);
                newAddr += i.Size;
            }
            // Not need to reset the delta. BuildOptimizedAssets does it.
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// Compress _L instructions to short version, if possible
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MinValue)]
        public static (NefFile, ContractManifest, JObject?) CompressJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            bool modified;
            do
            {
                modified = false;
                Script script = nef.Script;
                List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
                Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
                Dictionary<Instruction, int> oldInstructionToAddress = oldAddressAndInstructionsList.ToDictionary(e => e.i, e => e.a);
                (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                    Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                    Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                    FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
                Dictionary<int, int> oldSequencePointAddressToNew = new();

                System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
                int newAddr = 0;
                int addrDrift = 0;
                foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                {
                    if (SingleJumpInOperand(i) && i.OpCode != OpCode.CALLA)
                    {// Be aware that PUSHA is a jumping instruction without _L version.
                        Instruction target = jumpSourceToTargets[i];
                        int delta;
                        if (simplifiedInstructionsToAddress.Contains(target))
                            delta = (int)simplifiedInstructionsToAddress[target]! - newAddr;
                        else
                            delta = oldInstructionToAddress[target] - addrDrift - newAddr;
                        OpCode actualOpCode = i.OpCode;
                        if (longInstructions.Contains(i.OpCode))
                        {
                            if (delta > 0)
                                delta -= 3;
                            if (sbyte.MinValue <= delta && delta <= sbyte.MaxValue)
                            {// can use short version
                                modified = true;
                                addrDrift += 3;
                                actualOpCode = i.OpCode - 1;  // change to short version
                                IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                                // No size prefix for instruction. Skipping
                                newInstructionInBytes = newInstructionInBytes.Append(BitConverter.GetBytes(delta)[0]);
                                Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                                simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                                oldSequencePointAddressToNew.Add(a, newAddr);
                                newAddr += newInstruction.Size;

                                jumpSourceToTargets.Remove(i);
                                jumpSourceToTargets.Add(newInstruction, target);
                                jumpTargetToSources[target].Remove(i);
                                jumpTargetToSources[target].Add(newInstruction);
                                OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                        }
                    }
                    if (DoubleJumpInOperand(i))
                    {
                        (Instruction t1, Instruction t2) = trySourceToTargets[i];
                        int delta1, delta2;
                        if (simplifiedInstructionsToAddress.Contains(t1))
                            delta1 = (int)simplifiedInstructionsToAddress[t1]! - newAddr;
                        else
                            delta1 = oldInstructionToAddress[t1] - addrDrift - newAddr;
                        if (simplifiedInstructionsToAddress.Contains(t2))
                            delta2 = (int)simplifiedInstructionsToAddress[t2]! - newAddr;
                        else
                            delta2 = oldInstructionToAddress[t2] - addrDrift - newAddr;
                        OpCode actualOpCode = i.OpCode;
                        if (longInstructions.Contains(i.OpCode))
                        {
                            if (delta1 > 0)
                                delta1 -= 6;
                            if (delta2 > 0)
                                delta2 -= 6;
                            if (sbyte.MinValue <= delta1 && delta1 <= sbyte.MaxValue
                             && sbyte.MinValue <= delta2 && delta2 <= sbyte.MaxValue)
                            {// can use short version
                                modified = true;
                                addrDrift += 6;
                                actualOpCode = i.OpCode - 1;  // change to short version
                                IEnumerable<byte> newInstructionInBytes = [(byte)actualOpCode];
                                int operandSizeLength = Optimizer.OperandSizePrefixTable[(int)actualOpCode];
                                // No size prefix for instruction. Skipping
                                newInstructionInBytes = newInstructionInBytes.Concat(BitConverter.GetBytes(delta1)).Concat(BitConverter.GetBytes(delta2));
                                Instruction newInstruction = new Script(newInstructionInBytes.ToArray()).GetInstruction(0);

                                simplifiedInstructionsToAddress.Add(newInstruction, newAddr);
                                oldSequencePointAddressToNew.Add(a, newAddr);
                                newAddr += newInstruction.Size;

                                trySourceToTargets.Remove(i);
                                trySourceToTargets.Add(newInstruction, (t1, t2));
                                jumpTargetToSources[t1].Remove(i);
                                jumpTargetToSources[t1].Add(newInstruction);
                                jumpTargetToSources[t2].Remove(i);
                                jumpTargetToSources[t2].Add(newInstruction);
                                OptimizedScriptBuilder.RetargetJump(i, newInstruction,
                                    jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                                continue;
                            }
                        }
                    }
                    simplifiedInstructionsToAddress.Add(i, newAddr);
                    newAddr += i.Size;
                }
                // Not need to reset the delta. BuildOptimizedAssets does it.
                if (modified)
                    (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                        simplifiedInstructionsToAddress,
                        jumpSourceToTargets, trySourceToTargets,
                        oldAddressToInstruction,
                        oldSequencePointAddressToNew: oldSequencePointAddressToNew);
            }
            while (modified);
            return (nef, manifest, debugInfo);
        }

        /// <summary>
        /// Removes JMP and JMP_L that targets the next instruction after the JMP or JMP_L.
        /// Replace JMPIF/JMPIFNOT with DROP if it jumps to the next instruction
        /// If the removed JMP or JMP_L itself is a jump target,
        /// re-target to the instruction after the JMP or JMP_L
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        /// <summary>
        /// Conditional jumps that consume two operands from the evaluation stack
        /// (comparison-based jumps such as JMPEQ and their long forms).
        /// JMPIF / JMPIFNOT consume a single operand and are not included.
        /// </summary>
        private static readonly HashSet<OpCode> twoOperandConditionalJump = new()
        {
            OpCode.JMPEQ, OpCode.JMPNE, OpCode.JMPGT, OpCode.JMPGE, OpCode.JMPLT, OpCode.JMPLE,
            OpCode.JMPEQ_L, OpCode.JMPNE_L, OpCode.JMPGT_L, OpCode.JMPGE_L, OpCode.JMPLT_L, OpCode.JMPLE_L,
        };

        [Strategy(Priority = int.MaxValue)]
        public static (NefFile, ContractManifest, JObject?) RemoveUnnecessaryJumps(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (unconditionalJump.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (target - a == i.Size)
                    {
                        // Just jumping to the instruction after the jump itself
                        // This is unnecessary jump. The jump should be deleted.
                        // And, if this JMP is the target of other jump instructions,
                        // re-target to the next instruction after this JMP.
                        Instruction nextInstruction = oldAddressToInstruction[a + i.Size];
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[nextInstruction].Remove(i);
                        OptimizedScriptBuilder.RetargetJump(i, nextInstruction, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        continue;  // do not add this JMP into simplified instructions
                    }
                }
                if (conditionalJump.Contains(i.OpCode) || conditionalJump_L.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (target - a == i.Size)
                    {
                        // The conditional jump branches to the very next instruction, so its
                        // branch and fall-through paths coincide. Replace it with one DROP for
                        // JMPIF/JMPIFNOT and two DROPs for comparison-based jumps, preserving the
                        // operand count each conditional consumes from the evaluation stack.
                        int dropCount = twoOperandConditionalJump.Contains(i.OpCode) ? 2 : 1;
                        for (int d = 0; d < dropCount; d++)
                        {
                            Instruction drop = new Script(new byte[] { (byte)OpCode.DROP }).GetInstruction(0);
                            simplifiedInstructionsToAddress.Add(drop, currentAddress);
                            currentAddress += drop.Size;
                            if (d == 0)
                            {
                                oldSequencePointAddressToNew.Add(a, currentAddress);
                                OptimizedScriptBuilder.RetargetJump(i, drop, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                            }
                        }

                        jumpSourceToTargets.Remove(i);
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// Returns true when the opcode pushes a value whose truth is statically known:
        /// PUSHF / PUSH0 push a falsy value and PUSHT / PUSH1 push a truthy value.
        /// </summary>
        private static bool TryGetConstantPushTruth(OpCode opCode, out bool truth)
        {
            switch (opCode)
            {
                case OpCode.PUSHF:
                case OpCode.PUSH0:
                    truth = false;
                    return true;
                case OpCode.PUSHT:
                case OpCode.PUSH1:
                    truth = true;
                    return true;
                default:
                    truth = false;
                    return false;
            }
        }

        /// <summary>
        /// Folds a constant truth push immediately followed by a conditional jump.
        /// </summary>
        /// <remarks>
        /// When the pushed value is truthy, JMPIF always jumps and JMPIFNOT never jumps;
        /// when falsy the converse holds. The pair is therefore replaced by an unconditional
        /// JMP to the same target, or removed entirely when the condition never jumps.
        /// Runs after <see cref="Peephole.InitStaticToConst"/> so conditions that become
        /// constant through static-field folding are folded as well.
        /// </remarks>
        [Strategy(Priority = 1 << 4)]
        public static (NefFile, ContractManifest, JObject?) FoldConstantConditionalJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            for (int idx = 0; idx < oldAddressAndInstructionsList.Count; idx++)
            {
                (int a, Instruction i) = oldAddressAndInstructionsList[idx];
                if (idx + 1 < oldAddressAndInstructionsList.Count
                    && TryGetConstantPushTruth(i.OpCode, out bool truth))
                {
                    (int nextAddress, Instruction next) = oldAddressAndInstructionsList[idx + 1];
                    bool isJumpIfFamily = next.OpCode == OpCode.JMPIF || next.OpCode == OpCode.JMPIF_L;
                    bool isConditional = isJumpIfFamily || next.OpCode == OpCode.JMPIFNOT || next.OpCode == OpCode.JMPIFNOT_L;
                    // When another instruction jumps directly to the conditional, that path
                    // pushes its own condition value, so the pair is only foldable when the
                    // conditional has no other incoming references.
                    bool noIncomingReferences = !jumpTargetToSources.TryGetValue(next, out HashSet<Instruction>? nextSources) || nextSources.Count == 0;
                    if (isConditional && noIncomingReferences && jumpSourceToTargets.TryGetValue(next, out Instruction? target))
                    {
                        bool jumpsOnTrue = isJumpIfFamily;
                        // Degenerate self-targeting conditionals cannot be folded because the
                        // replacement would target an instruction that is itself removed.
                        bool degenerate = target == next || target == i;
                        if (!degenerate && truth == jumpsOnTrue)
                        {
                            // The condition always jumps: keep an unconditional JMP with the
                            // same operand width and target.
                            // Start with the long form. Removing the push and conditional
                            // changes the target distance, so a short replacement can cross
                            // the signed-byte boundary before the final compression pass.
                            // CompressJump will shrink it when the final distance is in range.
                            Instruction newJump = new Script(new byte[] { (byte)OpCode.JMP_L, 0, 0, 0, 0 }).GetInstruction(0);
                            jumpSourceToTargets.Remove(next);
                            jumpTargetToSources[target].Remove(next);
                            jumpSourceToTargets[newJump] = target;
                            jumpTargetToSources[target].Add(newJump);

                            // Anything that jumped to the push now goes straight to the JMP.
                            OptimizedScriptBuilder.RetargetJump(i, newJump, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);

                            simplifiedInstructionsToAddress.Add(newJump, currentAddress);
                            oldSequencePointAddressToNew.Add(a, currentAddress);
                            oldSequencePointAddressToNew.Add(nextAddress, currentAddress);
                            currentAddress += newJump.Size;
                        }
                        else if (!degenerate)
                        {
                            // The condition never jumps: remove the pair. Anything that jumped
                            // to the push or the conditional continues at the next instruction.
                            Instruction after = oldAddressToInstruction[nextAddress + next.Size];
                            jumpSourceToTargets.Remove(next);
                            jumpTargetToSources[target].Remove(next);
                            OptimizedScriptBuilder.RetargetJump(i, after, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                            OptimizedScriptBuilder.RetargetJump(next, after, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        }
                        else
                        {
                            simplifiedInstructionsToAddress.Add(i, currentAddress);
                            currentAddress += i.Size;
                            continue;
                        }
                        idx++;  // the conditional is consumed together with the push
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew: oldSequencePointAddressToNew);

            // Folding a never-taken conditional makes its fall-through body unreachable.
            // Recompute reachability after the fold so dead instructions are removed as well.
            return Reachability.RemoveUncoveredInstructions(nef, manifest, debugInfo);
        }

        /// <summary>
        /// If a JMP or JMP_L jumps to a RET, replace the JMP with RET
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue - 4)]
        public static (NefFile, ContractManifest, JObject?) ReplaceJumpWithRet(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (unconditionalJump.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (!oldAddressToInstruction.TryGetValue(target, out Instruction? dstRet))
                        throw new BadScriptException($"Bad {nameof(oldAddressToInstruction)}. No target found for {i} jumping from {a} to {target}");
                    if (dstRet.OpCode == OpCode.RET)
                    {
                        oldSequencePointAddressToNew[a] = currentAddress;
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[dstRet].Remove(i);
                        // handle the reference of the added RET
                        Instruction newRet = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);
                        // above is a workaround of new Instruction(OpCode.RET)
                        OptimizedScriptBuilder.RetargetJump(i, newRet,
                            jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        simplifiedInstructionsToAddress.Add(newRet, currentAddress);
                        currentAddress += newRet.Size;
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        /// <summary>
        /// If an unconditional jump targets an unconditional jump, re-target the first unconditional jump to its final destination
        /// If an unconditional jump targets an ENDTRY, replace the unconditional jump with ENDTRY
        /// If a conditional jump targets an unconditional jump, re-target the conditional jump to its final destination
        /// If an unconditional jump targets a conditional jump, DO NOT replace the unconditional jump with a conditional jump to its final destination. THIS IS WRONG.
        /// This should be executed very early, before <see cref="Reachability.RemoveUncoveredInstructions"/>
        /// </summary>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Manifest</param>
        /// <param name="debugInfo">Debug information</param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue - 8)]
        public static (NefFile, ContractManifest, JObject?) FoldJump(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            (nef, manifest, debugInfo) = JumpCompresser.UncompressJump(nef, manifest, debugInfo);
            bool modified;
            do
            {
                modified = false;
                Script script = nef.Script;
                List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
                Dictionary<int, Instruction> oldAddressToInstruction = oldAddressAndInstructionsList.ToDictionary(e => e.a, e => e.i);
                Dictionary<Instruction, int> oldInstructionToAddress = oldAddressAndInstructionsList.ToDictionary(e => e.i, e => e.a);
                (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                    Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                    Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                    FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
                Dictionary<int, int> oldSequencePointAddressToNew = new();

                System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
                int newAddr = 0;
                foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                {
                    if (shortInstructions.Contains(i.OpCode))
                        throw new BadScriptException($"Long version of OpCodes are required in {nameof(FoldJump)} optimization");
                    if (unconditionalJump.Contains(i.OpCode) || conditionalJump_L.Contains(i.OpCode))
                    {
                        Instruction target = jumpSourceToTargets[i];
                        // Thread the jump only when its target is itself an unconditional jump.
                        // (The second operand was a copy-paste of the same condition. Folding a
                        // jump into a conditional target is intentionally not done; see the FoldJump
                        // remarks.)
                        if (TryGetFoldedUnconditionalJumpTarget(i, target, jumpSourceToTargets, out Instruction foldedTarget))
                        {
                            modified = true;
                            // No need to change opcode. Use the old instruction without a new one.
                            // No need to reset operand. BuildOptimizedAssets does it.
                            simplifiedInstructionsToAddress.Add(i, newAddr);
                            oldSequencePointAddressToNew.Add(a, newAddr);
                            newAddr += i.Size;

                            jumpSourceToTargets[i] = foldedTarget;
                            jumpTargetToSources[target].Remove(i);
                            jumpTargetToSources[foldedTarget].Add(i);
                            continue;
                        }
                        if ((target.OpCode == OpCode.ENDTRY || target.OpCode == OpCode.ENDTRY_L)
                          && unconditionalJump.Contains(i.OpCode))
                        {// replace this JMP with ENDTRY
                            modified = true;
                            Instruction finalTarget = jumpSourceToTargets[target];
                            oldSequencePointAddressToNew.Add(a, newAddr);
                            // handle the reference of the deleted JMP
                            jumpSourceToTargets.Remove(i);
                            jumpTargetToSources[target].Remove(i);
                            // handle the reference of the added RET
                            Instruction newEndTry = new Script(new byte[] { (byte)OpCode.ENDTRY_L }.Concat(BitConverter.GetBytes(0)).ToArray()).GetInstruction(0);
                            // above is a workaround of new Instruction(OpCode.ENDTRY_L)
                            OptimizedScriptBuilder.RetargetJump(i, newEndTry,
                                jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                            jumpSourceToTargets[newEndTry] = finalTarget;
                            jumpTargetToSources[finalTarget].Remove(i);
                            jumpTargetToSources[finalTarget].Add(newEndTry);

                            simplifiedInstructionsToAddress.Add(newEndTry, newAddr);
                            newAddr += newEndTry.Size;
                            continue;
                        }
                    }
                    simplifiedInstructionsToAddress.Add(i, newAddr);
                    newAddr += i.Size;
                }

                (nef, manifest, debugInfo) = AssetBuilder.BuildOptimizedAssets(
                    nef, manifest, debugInfo,
                    simplifiedInstructionsToAddress,
                    jumpSourceToTargets, trySourceToTargets,
                    oldAddressToInstruction, oldSequencePointAddressToNew);
            }
            while (modified);
            return JumpCompresser.CompressJump(nef, manifest, debugInfo);
        }

        private static bool TryGetFoldedUnconditionalJumpTarget(
            Instruction source,
            Instruction target,
            Dictionary<Instruction, Instruction> jumpSourceToTargets,
            out Instruction finalTarget)
        {
            finalTarget = target;
            HashSet<Instruction> visited = new() { source };
            Instruction current = target;
            while (unconditionalJump.Contains(current.OpCode) &&
                   jumpSourceToTargets.TryGetValue(current, out Instruction? next))
            {
                if (!visited.Add(current) || visited.Contains(next))
                    return false;

                finalTarget = next;
                if (!unconditionalJump.Contains(next.OpCode))
                    return finalTarget != target;

                current = next;
            }

            return finalTarget != target;
        }
    }
}
