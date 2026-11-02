// Copyright (C) 2015-2026 The Neo Project.
//
// ScriptGraph.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler.ControlFlow;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using static Neo.Compiler.ControlFlow.JumpTarget;
using static Neo.Compiler.ControlFlow.OpCodeTypes;

namespace Neo.Optimizer
{
    /// <summary>
    /// Mutable in-memory script representation used by the optimizer. Instructions and
    /// control-flow edges are kept stable while multiple transformations run; the script
    /// and its metadata are emitted only when all graph transformations are complete.
    /// </summary>
    internal sealed class ScriptGraph
    {
        internal sealed record Layout(
            Script Script,
            Dictionary<Instruction, int> Addresses,
            Dictionary<Instruction, Instruction> EncodedInstructions,
            Dictionary<Instruction, Instruction> JumpTargets,
            Dictionary<Instruction, (Instruction, Instruction)> TryTargets);

        internal sealed record CoverageSnapshot(
            InstructionCoverage Coverage,
            Dictionary<Instruction, int> CurrentInstructionAddresses);

        private sealed record ReplacementPlan(
            List<Instruction> Instructions,
            Dictionary<Instruction, Instruction> Replacements,
            Dictionary<Instruction, Instruction> GeneratedJumpTargets,
            Dictionary<Instruction, (Instruction, Instruction)> GeneratedTryTargets);

        private readonly Dictionary<ContractMethodDescriptor, int> originalMethodOffsets = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<int, Instruction> originalAddressToInstruction;
        private readonly Dictionary<int, Instruction> originalAddressAliases = new();
        private List<Instruction> instructions;
        private Dictionary<Instruction, int> instructionToAddress = new();
        private Dictionary<Instruction, int> instructionToIndex = new();
        private Dictionary<Instruction, Instruction> jumpSourceToTargets;
        private Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets;
        private Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources;

        public NefFile Nef { get; }
        public ContractManifest Manifest { get; }
        public JObject? DebugInfo { get; }
        public MethodToken[] Tokens { get; private set; }
        public IReadOnlyList<Instruction> Instructions => instructions;

        private ScriptGraph(NefFile nef, ContractManifest manifest, JObject? debugInfo,
            List<Instruction> parsedInstructions, Dictionary<int, Instruction> originalAddressToInstruction)
        {
            Nef = nef;
            Manifest = manifest;
            DebugInfo = debugInfo;
            Tokens = nef.Tokens;
            instructions = parsedInstructions;
            this.originalAddressToInstruction = originalAddressToInstruction;
            foreach ((int address, Instruction instruction) in originalAddressToInstruction)
                originalAddressAliases[address] = instruction;
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                originalMethodOffsets[method] = method.Offset;

            (jumpSourceToTargets, trySourceToTargets, jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(instructions);
            RebuildAddresses();
        }

        public static ScriptGraph Parse(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            List<Instruction> parsedInstructions = new Script(nef.Script.ToArray()).EnumerateInstructions()
                .Select(static item => item.instruction)
                .ToList();
            Dictionary<int, Instruction> addressToInstruction = new();
            int address = 0;
            foreach (Instruction instruction in parsedInstructions)
            {
                addressToInstruction.Add(address, instruction);
                address += instruction.Size;
            }
            return new ScriptGraph(nef, manifest, debugInfo, parsedInstructions, addressToInstruction);
        }

        public Instruction? GetNextLiveInstruction(Instruction instruction)
        {
            int index = instructionToIndex[instruction];
            return index + 1 < instructions.Count ? instructions[index + 1] : null;
        }

        public List<List<Instruction>> BuildStructuralBlocks()
        {
            if (instructions.Count == 0) return [];

            HashSet<int> starts = [0];
            foreach (Instruction target in jumpSourceToTargets.Values)
                starts.Add(instructionToAddress[target]);
            foreach ((Instruction t1, Instruction t2) in trySourceToTargets.Values)
            {
                starts.Add(instructionToAddress[t1]);
                starts.Add(instructionToAddress[t2]);
            }
            foreach (int originalOffset in originalMethodOffsets.Values)
            {
                Instruction entry = ResolveOriginalAddress(originalOffset);
                if (instructionToAddress.TryGetValue(entry, out int currentAddress))
                    starts.Add(currentAddress);
            }

            List<List<Instruction>> blocks = [];
            List<Instruction> current = [];
            foreach (Instruction instruction in instructions)
            {
                int address = instructionToAddress[instruction];
                if (current.Count > 0)
                {
                    Instruction previous = current[^1];
                    if (starts.Contains(address) || allowedBasicBlockEnds.Contains(previous.OpCode))
                    {
                        blocks.Add(current);
                        current = [];
                    }
                }
                current.Add(instruction);
                if (allowedBasicBlockEnds.Contains(instruction.OpCode))
                {
                    blocks.Add(current);
                    current = [];
                }
            }
            if (current.Count > 0) blocks.Add(current);
            return blocks;
        }

        public void ApplyConstantConditionalJumps()
        {
            bool changed;
            do
            {
                changed = false;
                List<Instruction> next = [];
                Dictionary<Instruction, Instruction> replacements = new();
                Dictionary<Instruction, Instruction> generatedJumpTargets = new();
                foreach (List<Instruction> block in BuildStructuralBlocks())
                {
                    for (int index = 0; index < block.Count; index++)
                    {
                        Instruction current = block[index];
                        if (index + 1 >= block.Count || !TryGetConstantPushTruth(current.OpCode, out bool truth))
                        {
                            next.Add(current);
                            continue;
                        }

                        Instruction jump = block[index + 1];
                        bool isJumpIf = jump.OpCode is OpCode.JMPIF or OpCode.JMPIF_L;
                        bool isConditional = isJumpIf || jump.OpCode is OpCode.JMPIFNOT or OpCode.JMPIFNOT_L;
                        bool hasIncoming = jumpTargetToSources.TryGetValue(jump, out var sources) && sources.Count > 0;
                        if (!isConditional || hasIncoming || !jumpSourceToTargets.TryGetValue(jump, out var target) || target == jump || target == current)
                        {
                            next.Add(current);
                            continue;
                        }

                        bool alwaysJumps = truth == isJumpIf;
                        if (alwaysJumps)
                        {
                            Instruction replacement = CreateJumpInstruction(OpCode.JMP_L);
                            generatedJumpTargets[replacement] = target;
                            next.Add(replacement);
                            replacements[current] = replacement;
                            replacements[jump] = replacement;
                        }
                        else
                        {
                            int currentIndex = instructionToIndex[current];
                            int jumpIndex = instructionToIndex[jump];
                            if (jumpIndex + 1 >= instructions.Count)
                            {
                                next.Add(current);
                                continue;
                            }
                            Instruction after = instructions[jumpIndex + 1];
                            replacements[current] = after;
                            replacements[jump] = after;
                        }

                        index++;
                        changed = true;
                    }
                }

                if (changed) Apply(new ReplacementPlan(next, replacements, generatedJumpTargets, []));
            }
            while (changed);
        }

        public void NormalizeJumps(bool removeJumpsToNextInstruction = true, bool replaceJumpsToReturn = true)
        {
            if (removeJumpsToNextInstruction) RemoveJumpsToNextInstruction();
            if (replaceJumpsToReturn) ReplaceJumpToReturn();

            bool changed;
            do
            {
                changed = false;
                var resolvedTargets = ResolveUnconditionalJumpTargets();
                List<Instruction> next = [];
                Dictionary<Instruction, Instruction> replacements = new();
                Dictionary<Instruction, Instruction> generatedJumpTargets = new();
                foreach (Instruction instruction in instructions)
                {
                    if (!jumpSourceToTargets.TryGetValue(instruction, out Instruction? target))
                    {
                        next.Add(instruction);
                        continue;
                    }

                    Instruction resolved = resolvedTargets[target];
                    if (unconditionalJump.Contains(instruction.OpCode))
                    {
                        int sourceAddress = instructionToAddress[instruction];
                        int targetAddress = instructionToAddress[resolved];
                        if (removeJumpsToNextInstruction && targetAddress == sourceAddress + instruction.Size)
                        {
                            Instruction after = GetNextLiveInstruction(instruction)!;
                            replacements[instruction] = after;
                            changed = true;
                            continue;
                        }

                        if (replaceJumpsToReturn && resolved.OpCode == OpCode.RET)
                        {
                            Instruction ret = CreateInstruction(OpCode.RET);
                            next.Add(ret);
                            replacements[instruction] = ret;
                            changed = true;
                            continue;
                        }

                        if (resolved.OpCode is OpCode.ENDTRY or OpCode.ENDTRY_L
                            && jumpSourceToTargets.TryGetValue(resolved, out Instruction? endTryTarget))
                        {
                            Instruction endTry = CreateJumpInstruction(OpCode.ENDTRY_L);
                            generatedJumpTargets[endTry] = endTryTarget;
                            next.Add(endTry);
                            replacements[instruction] = endTry;
                            changed = true;
                            continue;
                        }
                    }

                    // PUSHA materializes its target as a Pointer. Threading it through a
                    // jump changes the value observed by EQUAL/NOTEQUAL.
                    if (instruction.OpCode != OpCode.PUSHA && resolved != target)
                    {
                        Instruction replacement = CreateJumpInstruction(instruction.OpCode);
                        next.Add(replacement);
                        replacements[instruction] = replacement;
                        generatedJumpTargets[replacement] = resolved;
                        changed = true;
                        continue;
                    }

                    next.Add(instruction);
                }

                if (changed) Apply(new ReplacementPlan(next, replacements, generatedJumpTargets, []));
            }
            while (changed);
        }

        public void ReplaceJumpToReturn()
        {
            List<Instruction> next = [];
            Dictionary<Instruction, Instruction> replacements = new();
            foreach (Instruction instruction in instructions)
            {
                if (unconditionalJump.Contains(instruction.OpCode)
                    && jumpSourceToTargets.TryGetValue(instruction, out Instruction? target)
                    && target.OpCode == OpCode.RET)
                {
                    Instruction ret = CreateInstruction(OpCode.RET);
                    next.Add(ret);
                    replacements[instruction] = ret;
                }
                else
                {
                    next.Add(instruction);
                }
            }
            if (replacements.Count > 0) Apply(new ReplacementPlan(next, replacements, [], []));
        }

        public void CollapseAdjacentReturns()
        {
            List<Instruction> next = [];
            Dictionary<Instruction, Instruction> replacements = new();
            HashSet<Instruction> methodEntries = GetMethodEntryInstructions();
            Instruction? previousReturn = null;
            foreach (Instruction instruction in instructions)
            {
                if (instruction.OpCode == OpCode.RET)
                {
                    if (previousReturn is not null && !methodEntries.Contains(instruction))
                    {
                        replacements[instruction] = previousReturn;
                        continue;
                    }
                    previousReturn = instruction;
                }
                else
                {
                    previousReturn = null;
                }
                next.Add(instruction);
            }

            if (replacements.Count > 0) Apply(new ReplacementPlan(next, replacements, [], []));
        }

        private HashSet<Instruction> GetMethodEntryInstructions()
        {
            HashSet<Instruction> entries = [];
            foreach ((ContractMethodDescriptor method, int offset) in originalMethodOffsets)
            {
                if (Manifest.Abi.Methods.Any(active => ReferenceEquals(active, method)))
                    entries.Add(ResolveOriginalAddress(offset));
            }

            foreach ((Instruction source, Instruction target) in jumpSourceToTargets)
            {
                if (source.OpCode == OpCode.PUSHA || callWithJump.Contains(source.OpCode))
                    entries.Add(target);
            }
            return entries;
        }

        public void RemoveJumpsToNextInstruction()
        {
            List<Instruction> next = [];
            Dictionary<Instruction, Instruction> replacements = new();
            foreach (Instruction instruction in instructions)
            {
                if (unconditionalJump.Contains(instruction.OpCode)
                    && jumpSourceToTargets.TryGetValue(instruction, out Instruction? target)
                    && instructionToAddress[target] == instructionToAddress[instruction] + instruction.Size)
                {
                    Instruction after = GetNextLiveInstruction(instruction)!;
                    replacements[instruction] = after;
                }
                else
                {
                    next.Add(instruction);
                }
            }
            if (replacements.Count > 0) Apply(new ReplacementPlan(next, replacements, [], []));
        }

        private Dictionary<Instruction, Instruction> ResolveUnconditionalJumpTargets()
        {
            Dictionary<Instruction, Instruction> resolved = new();
            foreach (var instruction in instructions)
            {
                if (!jumpSourceToTargets.TryGetValue(instruction, out Instruction? target)) continue;

                HashSet<Instruction> visited = [instruction];
                Instruction current = target;
                while (unconditionalJump.Contains(current.OpCode)
                    && !visited.Contains(current)
                    && jumpSourceToTargets.TryGetValue(current, out Instruction? next))
                {
                    visited.Add(current);
                    if (visited.Contains(next)) break;
                    current = next;
                }
                resolved[target] = current;
            }
            return resolved;
        }

        public void ApplyStaticFieldConstants(CoverageSnapshot snapshot)
        {
            var coverage = snapshot.Coverage;
            var currentAddresses = snapshot.CurrentInstructionAddresses.ToDictionary(static pair => pair.Value, static pair => pair.Key);
            int[] inits = coverage.entryPointsByMethod
                .Where(static pair => pair.Value == EntryType.Initialize)
                .Select(static pair => pair.Key)
                .ToArray();
            if (inits.Length > 1)
                throw new BadScriptException($"{inits.Length} _initialize methods in contract {Manifest.Name}");
            if (inits.Length == 0) return;

            var initAddresses = GetReachableAddresses(coverage, inits[0]);
            Dictionary<byte, Instruction> writtenConstInInit = new();
            HashSet<byte> readInInit = new();
            foreach (int blockStart in initAddresses.Where(coverage.basicBlocksInDict.ContainsKey).Order())
            {
                Instruction? previousPush = null;
                foreach ((int address, Instruction _) in coverage.basicBlocksInDict[blockStart].OrderBy(static x => x.Key))
                {
                    if (!initAddresses.Contains(address) || !currentAddresses.TryGetValue(address, out var instruction))
                    {
                        continue;
                    }
                    if (storeStaticFields.Contains(instruction.OpCode))
                    {
                        byte slot = SlotIndex(instruction);
                        if (previousPush is null || !writtenConstInInit.TryAdd(slot, previousPush))
                            writtenConstInInit[slot] = Instruction.RET;
                        previousPush = null;
                    }
                    if (loadStaticFields.Contains(instruction.OpCode))
                        readInInit.Add(SlotIndex(instruction));
                    previousPush = pushConst.Contains(instruction.OpCode) ? instruction : null;
                }
            }

            HashSet<Instruction> removableInitInstructions = writtenConstInInit.Values
                .Where(static instruction => pushConst.Contains(instruction.OpCode))
                .ToHashSet();

            HashSet<int> doNotReplaceLdsfld = [];
            HashSet<byte> doNotDeleteStsfld = [];
            foreach ((int entry, EntryType _) in coverage.entryPointsByMethod.Where(static pair => pair.Value != EntryType.Initialize))
            {
                HashSet<int> methodAddresses = GetReachableAddresses(coverage, entry);
                HashSet<byte> stores = [];
                Dictionary<byte, HashSet<int>> loads = [];
                foreach (int address in methodAddresses)
                {
                    if (!currentAddresses.TryGetValue(address, out Instruction? instruction)) continue;
                    if (storeStaticFields.Contains(instruction.OpCode)) stores.Add(SlotIndex(instruction));
                    if (loadStaticFields.Contains(instruction.OpCode))
                    {
                        byte slot = SlotIndex(instruction);
                        if (!loads.TryGetValue(slot, out HashSet<int>? addresses))
                        {
                            addresses = [];
                            loads.Add(slot, addresses);
                        }
                        addresses.Add(address);
                    }
                }
                foreach (byte slot in stores)
                {
                    if (loads.TryGetValue(slot, out HashSet<int>? addresses))
                        doNotReplaceLdsfld.UnionWith(addresses);
                    doNotDeleteStsfld.UnionWith(stores);
                }
            }

            List<Instruction> next = [];
            Dictionary<Instruction, Instruction> replacements = new();
            Dictionary<Instruction, Instruction> generatedJumpTargets = new();
            for (int index = 0; index < instructions.Count; index++)
            {
                Instruction instruction = instructions[index];
                int address = snapshot.CurrentInstructionAddresses[instruction];

                if (!initAddresses.Contains(address) && loadStaticFields.Contains(instruction.OpCode) && !doNotReplaceLdsfld.Contains(address))
                {
                    byte slot = SlotIndex(instruction);
                    if (writtenConstInInit.TryGetValue(slot, out Instruction? push) && pushConst.Contains(push.OpCode))
                    {
                        Instruction copiedPush = CloneRegular(push);
                        next.Add(copiedPush);
                        replacements[instruction] = copiedPush;
                        if (SingleJumpInOperand(copiedPush) && jumpSourceToTargets.TryGetValue(push, out Instruction? target))
                            generatedJumpTargets[copiedPush] = target;
                        continue;
                    }
                }

                if (initAddresses.Contains(address) && removableInitInstructions.Contains(instruction) && index + 1 < instructions.Count)
                {
                    Instruction store = instructions[index + 1];
                    if (storeStaticFields.Contains(store.OpCode))
                    {
                        byte slot = SlotIndex(store);
                        if (!writtenConstInInit.TryGetValue(slot, out var configuredPush) || configuredPush != instruction || readInInit.Contains(slot))
                        {
                            next.Add(instruction);
                            continue;
                        }
                        if (doNotDeleteStsfld.Contains(slot))
                        {
                            next.Add(instruction);
                            continue;
                        }

                        var after = index + 2 < instructions.Count ? instructions[index + 2] : CreateInstruction(OpCode.RET);
                        if (index + 2 >= instructions.Count) next.Add(after);
                        replacements[instruction] = after;
                        replacements[store] = after;
                        index++;
                        continue;
                    }
                }
                next.Add(instruction);
            }

            if (replacements.Count > 0)
                Apply(new ReplacementPlan(next, replacements, generatedJumpTargets, []));
        }

        public void RemoveUnusedInitialize()
        {
            ContractMethodDescriptor? initialize = Manifest.Abi.Methods
                .SingleOrDefault(static method => method.Name == "_initialize");
            if (initialize is null || !originalMethodOffsets.TryGetValue(initialize, out int initializeOffset)) return;

            Instruction initSlot = ResolveOriginalAddress(initializeOffset);
            if (initSlot.OpCode != OpCode.INITSSLOT || jumpTargetToSources.ContainsKey(initSlot)) return;
            if (instructions.Any(static inst => loadStaticFields.Contains(inst.OpCode) || storeStaticFields.Contains(inst.OpCode))) return;

            Instruction? after = GetNextLiveInstruction(initSlot);
            if (after is not null && after.OpCode != OpCode.RET) return;

            Manifest.Abi.Methods = Manifest.Abi.Methods.Where(static method => method.Name != "_initialize").ToArray();

            var next = instructions.Where(instruction => instruction != initSlot).ToList();
            Dictionary<Instruction, Instruction> replacements = new() { [initSlot] = after ?? Instruction.RET };
            Apply(new ReplacementPlan(next, replacements, [], []));
        }

        public void RemoveUnreachable()
        {
            List<Instruction> next = [];
            Dictionary<Instruction, Instruction> replacements = new();
            Dictionary<Instruction, Instruction> generatedJumpTargets = new();
            Layout layout = SolveLayout();
            AssignManifestOffsets(layout);

            Script temporaryScript = layout.Script;
            NefFile temporaryNef = CopyNef(temporaryScript);
            InstructionCoverage coverage = new(temporaryNef, Manifest);
            var keep = new bool[instructions.Count];
            for (int index = 0; index < instructions.Count; index++)
            {
                Instruction instruction = instructions[index];
                Instruction encoded = layout.EncodedInstructions[instruction];
                int address = layout.Addresses[encoded];
                bool reachable = coverage.coveredMap[address] != BranchType.UNCOVERED;
                if (instruction.OpCode != OpCode.NOP)
                {
                    // Keep a terminal RET even when it is unreachable from an ABI entry.
                    // Dead code immediately before it is still removed and retargeted here.
                    keep[index] = reachable || (instruction.OpCode == OpCode.RET && !HasFollowingInstruction(index));
                    continue;
                }

                bool targeted = jumpTargetToSources.ContainsKey(instruction);
                keep[index] = targeted && !HasFollowingInstruction(index);
            }

            Instruction? following = null;
            for (int index = instructions.Count - 1; index >= 0; index--)
            {
                Instruction instruction = instructions[index];
                if (keep[index])
                {
                    next.Add(instruction);
                    following = instruction;
                }
                else if (following is not null)
                {
                    replacements[instruction] = following;
                }
            }
            next.Reverse();

            foreach (Instruction instruction in instructions)
            {
                if (!keep[instructionToIndex[instruction]] && !replacements.ContainsKey(instruction))
                {
                    Instruction target = next.LastOrDefault() ?? CreateInstruction(OpCode.RET);
                    replacements[instruction] = target;
                }
            }

            if (next.Count != instructions.Count) Apply(new ReplacementPlan(next, replacements, generatedJumpTargets, []));
        }

        public CoverageSnapshot BuildCoverage()
        {
            Layout layout = SolveLayout();
            AssignManifestOffsets(layout);
            InstructionCoverage coverage = new(CopyNef(layout.Script), Manifest);
            Dictionary<Instruction, int> currentAddresses = new();
            foreach ((Instruction current, Instruction encoded) in layout.EncodedInstructions)
                currentAddresses[current] = layout.Addresses[encoded];
            return new CoverageSnapshot(coverage, currentAddresses);
        }

        private bool HasFollowingInstruction(int index)
        {
            for (int next = index + 1; next < instructions.Count; next++)
            {
                if (instructions[next].OpCode != OpCode.NOP) return true;
            }
            return false;
        }

        public void RemoveUnusedMethodTokens()
        {
            if (Tokens.Length == 0) return;

            bool[] needed = new bool[Tokens.Length];
            foreach (Instruction instruction in instructions)
            {
                if (instruction.OpCode != OpCode.CALLT) continue;
                if (instruction.TokenU16 >= Tokens.Length)
                    throw new BadScriptException($"CALLT token index {instruction.TokenU16} is outside the token table");
                needed[instruction.TokenU16] = true;
            }

            int[] remap = new int[needed.Length];
            for (int index = 0, newIndex = 0; index < needed.Length; index++)
            {
                remap[index] = needed[index] ? newIndex++ : -1;
            }

            Tokens = Tokens.Where((_, index) => needed[index]).ToArray();
            if (Tokens.Length == needed.Length) return;

            List<Instruction> next = [];
            Dictionary<Instruction, Instruction> replacements = new();
            foreach (Instruction instruction in instructions)
            {
                if (instruction.OpCode == OpCode.CALLT && remap[instruction.TokenU16] != instruction.TokenU16)
                {
                    Instruction call = CreateInstruction(OpCode.CALLT, BitConverter.GetBytes((ushort)remap[instruction.TokenU16]));
                    next.Add(call);
                    replacements[instruction] = call;
                }
                else
                {
                    next.Add(instruction);
                }
            }
            if (replacements.Count > 0)
            {
                Apply(new ReplacementPlan(next, replacements, [], []));
            }
        }

        private HashSet<int> GetReachableAddresses(InstructionCoverage coverage, int entry)
        {
            HashSet<int> result = [];
            if (!coverage.basicBlocksInDict.ContainsKey(entry)) return result;

            Queue<int> queue = new();
            queue.Enqueue(entry);
            result.Add(entry);
            while (queue.Count > 0)
            {
                int block = queue.Dequeue();
                if (coverage.basicBlockContinuation.TryGetValue(block, out int continuation) && result.Add(continuation))
                    queue.Enqueue(continuation);
                if (coverage.basicBlockJump.TryGetValue(block, out HashSet<int>? targets))
                {
                    foreach (int target in targets)
                    {
                        if (result.Add(target)) queue.Enqueue(target);
                    }
                }
            }
            foreach (int block in result.ToArray())
            {
                if (coverage.basicBlocksInDict.TryGetValue(block, out Dictionary<int, Instruction>? instructionsInBlock))
                    result.UnionWith(instructionsInBlock.Keys);
            }
            return result;
        }

        private void Apply(ReplacementPlan plan)
        {
            var live = plan.Instructions.ToHashSet();
            var replacements = plan.Replacements;
            Instruction Resolve(Instruction instruction)
            {
                HashSet<Instruction> visited = [];
                Instruction current = instruction;
                while (!live.Contains(current) && replacements.TryGetValue(current, out var replacement) && visited.Add(current))
                {
                    current = replacement;
                }
                return current;
            }

            Dictionary<Instruction, Instruction> jumpTargets = new();
            Dictionary<Instruction, (Instruction, Instruction)> tryTargets = new();
            foreach (Instruction instruction in plan.Instructions)
            {
                if (plan.GeneratedJumpTargets.TryGetValue(instruction, out Instruction? generatedTarget))
                    jumpTargets[instruction] = Resolve(generatedTarget);
                else if (jumpSourceToTargets.TryGetValue(instruction, out Instruction? target))
                    jumpTargets[instruction] = Resolve(target);

                if (plan.GeneratedTryTargets.TryGetValue(instruction, out (Instruction, Instruction) generatedTries))
                    tryTargets[instruction] = (Resolve(generatedTries.Item1), Resolve(generatedTries.Item2));
                else if (trySourceToTargets.TryGetValue(instruction, out (Instruction, Instruction) targets))
                    tryTargets[instruction] = (Resolve(targets.Item1), Resolve(targets.Item2));
            }

            instructions = plan.Instructions;
            jumpSourceToTargets = jumpTargets;
            trySourceToTargets = tryTargets;
            jumpTargetToSources = new();
            foreach ((Instruction source, Instruction target) in jumpSourceToTargets)
            {
                if (!jumpTargetToSources.TryGetValue(target, out HashSet<Instruction>? sources))
                {
                    sources = [];
                    jumpTargetToSources.Add(target, sources);
                }
                sources.Add(source);
            }
            foreach ((Instruction source, (Instruction target1, Instruction target2)) in trySourceToTargets)
            {
                AddTarget(target1, source);
                AddTarget(target2, source);
            }

            RebuildAddresses();
            foreach (int originalAddress in originalAddressAliases.Keys.ToArray())
            {
                Instruction current = originalAddressAliases[originalAddress];
                originalAddressAliases[originalAddress] = Resolve(current);
            }

            void AddTarget(Instruction target, Instruction source)
            {
                if (!jumpTargetToSources.TryGetValue(target, out HashSet<Instruction>? sources))
                {
                    sources = [];
                    jumpTargetToSources.Add(target, sources);
                }
                sources.Add(source);
            }
        }

        private void RebuildAddresses()
        {
            instructionToAddress = new();
            instructionToIndex = new();
            int address = 0;
            for (int index = 0; index < instructions.Count; index++)
            {
                Instruction instruction = instructions[index];
                instructionToAddress.Add(instruction, address);
                instructionToIndex.Add(instruction, index);
                address += instruction.Size;
            }
        }

        private void AssignManifestOffsets(Layout layout)
        {
            foreach (ContractMethodDescriptor method in Manifest.Abi.Methods)
            {
                if (!originalMethodOffsets.TryGetValue(method, out int originalOffset)) continue;

                Instruction anchor = ResolveOriginalAddress(originalOffset);
                if (layout.EncodedInstructions.TryGetValue(anchor, out Instruction? encoded)
                    && layout.Addresses.TryGetValue(encoded, out int address))
                {
                    method.Offset = address;
                }
            }
        }

        private Instruction ResolveOriginalAddress(int originalAddress)
        {
            if (originalAddressAliases.TryGetValue(originalAddress, out Instruction? alias)) return alias;
            if (originalAddressToInstruction.TryGetValue(originalAddress, out Instruction? exact)) return exact;

            int nextAddress = originalAddressToInstruction.Keys
                .Where(address => address >= originalAddress)
                .DefaultIfEmpty(originalAddress)
                .Min();
            return originalAddressAliases.TryGetValue(nextAddress, out Instruction? next)
                ? next
                : instructions.FirstOrDefault() ?? Instruction.RET;
        }

        public (NefFile, ContractManifest, JObject?) Emit(bool compressJumps = true)
        {
            Layout layout = SolveLayout(compressJumps);
            OrderedDictionary instructionsToAddress = new();
            foreach ((Instruction instruction, int address) in layout.Addresses)
                instructionsToAddress[instruction] = address;

            Script script = OptimizedScriptBuilder.BuildScriptWithJumpTargets(instructionsToAddress,
                layout.JumpTargets, layout.TryTargets, originalAddressToInstruction);

            Nef.Script = script;
            Nef.Tokens = Tokens;
            Nef.CheckSum = NefFile.ComputeChecksum(Nef);
            foreach (ContractMethodDescriptor method in Manifest.Abi.Methods)
            {
                if (!originalMethodOffsets.TryGetValue(method, out int originalOffset)) continue;

                Instruction current = ResolveOriginalAddress(originalOffset);
                if (layout.EncodedInstructions.TryGetValue(current, out Instruction? encoded)
                    && layout.Addresses.TryGetValue(encoded, out int newOffset))
                {
                    method.Offset = newOffset;
                }
            }

            Dictionary<int, int> originalSequencePoints = new();
            foreach (int originalAddress in originalAddressToInstruction.Keys)
            {
                Instruction current = ResolveOriginalAddress(originalAddress);
                if (layout.EncodedInstructions.TryGetValue(current, out Instruction? encoded)
                    && layout.Addresses.TryGetValue(encoded, out int newAddress))
                {
                    originalSequencePoints[originalAddress] = newAddress;
                }
            }
            CorrectDebugMethodEndMappings(originalSequencePoints, layout);
            ClampMappingsToActiveAbiMethodBoundaries(originalSequencePoints, layout);

            JObject? optimizedDebugInfo = DebugInfoBuilder.ModifyDebugInfo(DebugInfo, instructionsToAddress,
                originalAddressToInstruction, originalSequencePoints);
            if (optimizedDebugInfo is not null)
                optimizedDebugInfo["hash"] = Nef.Script.Span.ToScriptHash().ToString();
            return (Nef, Manifest, optimizedDebugInfo);
        }

        private void CorrectDebugMethodEndMappings(Dictionary<int, int> mappings, Layout layout)
        {
            if (DebugInfo?["methods"] is not JArray debugMethods) return;

            List<(int Start, int End, int CurrentStart, int CurrentEnd, bool IsAbi, string? AbiName)> methods = [];
            foreach (JObject method in debugMethods.OfType<JObject>())
            {
                string[] range = method["range"]?.AsString().Split('-') ?? [];
                if (range.Length != 2 || !int.TryParse(range[0], out int start) || !int.TryParse(range[1], out int end)
                    || !mappings.TryGetValue(start, out int currentStart) || !mappings.TryGetValue(end, out int currentEnd))
                {
                    continue;
                }

                JObject? abi = method["abi"] as JObject;
                string? abiName = abi?["name"]?.AsString();
                string? debugName = method["name"]?.AsString();
                bool isAbi = abiName is not null;
                bool isRemovedInitialize = debugName?.EndsWith(",_initialize", StringComparison.Ordinal) == true
                    && !Manifest.Abi.Methods.Any(static method => method.Name == "_initialize");
                bool survives = isAbi
                    ? Manifest.Abi.Methods.Any(method => method.Name == abiName)
                    : currentStart != currentEnd && !isRemovedInitialize;
                if (survives) methods.Add((start, end, currentStart, currentEnd, isAbi, abiName));
            }

            methods.Sort(static (left, right) => left.CurrentStart.CompareTo(right.CurrentStart));
            int[] finalAddresses = layout.Addresses.Values.Distinct().Order().ToArray();
            for (int index = 0; index < methods.Count; index++)
            {
                var method = methods[index];
                int nextStart = methods.Skip(index + 1)
                    .Select(static candidate => candidate.CurrentStart)
                    .FirstOrDefault(address => address > method.CurrentStart);
                int finalEnd;
                if (nextStart > method.CurrentStart)
                {
                    finalEnd = finalAddresses.LastOrDefault(address => address < nextStart, method.CurrentStart);
                }
                else
                {
                    finalEnd = IsTerminalAddress(method.CurrentEnd, layout, method.CurrentStart)
                        ? method.CurrentEnd
                        : finalAddresses.LastOrDefault(method.CurrentStart);
                }

                mappings[method.End] = finalEnd;
                foreach (int oldAddress in mappings.Keys.Where(address => address >= method.Start && address <= method.End).ToArray())
                {
                    if (mappings[oldAddress] > finalEnd) mappings[oldAddress] = finalEnd;
                }
            }
        }

        private static bool IsTerminalAddress(int address, Layout layout, int methodStart)
        {
            if (address < methodStart || address >= layout.Script.Length) return false;
            return allowedBasicBlockEnds.Contains(layout.Script.GetInstruction(address).OpCode);
        }

        private void ClampMappingsToActiveAbiMethodBoundaries(Dictionary<int, int> mappings, Layout layout)
        {
            (ContractMethodDescriptor Method, int OldStart)[] methods = originalMethodOffsets
                .Where(pair => Manifest.Abi.Methods.Contains(pair.Key))
                .Select(static pair => (pair.Key, pair.Value))
                .OrderBy(static pair => pair.Value)
                .ToArray();
            int[] finalAddresses = layout.Addresses.Values.Distinct().Order().ToArray();
            for (int index = 0; index + 1 < methods.Length; index++)
            {
                var (_, oldStart) = methods[index];
                var (next, nextOldStart) = methods[index + 1];
                int nextStart = next.Offset;
                int previousInstruction = finalAddresses.LastOrDefault(address => address < nextStart, -1);
                if (previousInstruction < 0) continue;

                int? oldMethodEnd = mappings.Keys
                    .Where(address => address >= oldStart && address < nextOldStart)
                    .Select(static address => (int?)address)
                    .Max();
                if (oldMethodEnd is not null) mappings[oldMethodEnd.Value] = previousInstruction;

                foreach (int oldAddress in mappings.Keys.Where(address => address >= oldStart && address < nextOldStart).ToArray())
                {
                    if (mappings[oldAddress] >= nextStart) mappings[oldAddress] = previousInstruction;
                }
            }
        }

        private Layout SolveLayout(bool compressJumps = true)
        {
            int count = instructions.Count;
            var offsets = new int[count];
            var sizes = new int[count];
            var isCandidate = new bool[count];
            var useShort = new bool[count];
            var shortOpcodes = new OpCode[count];
            var longOpcodes = new OpCode[count];
            for (int index = 0; index < count; index++)
            {
                var instruction = instructions[index];
                OpCode opcode = instruction.OpCode;
                if (singleShortJumpOpcodes.Contains(opcode) || singleLongJumpOpcodes.Contains(opcode))
                {
                    isCandidate[index] = true;
                    shortOpcodes[index] = singleShortJumpOpcodes.Contains(opcode) ? opcode : opcode - 1;
                    longOpcodes[index] = shortOpcodes[index] + 1;
                    useShort[index] = !compressJumps && singleShortJumpOpcodes.Contains(opcode);
                    sizes[index] = InstructionSize(compressJumps ? longOpcodes[index] : opcode);
                }
                else if (opcode is OpCode.TRY or OpCode.TRY_L)
                {
                    isCandidate[index] = true;
                    shortOpcodes[index] = OpCode.TRY;
                    longOpcodes[index] = OpCode.TRY_L;
                    useShort[index] = !compressJumps && opcode == OpCode.TRY;
                    sizes[index] = InstructionSize(compressJumps ? OpCode.TRY_L : opcode);
                }
                else
                {
                    sizes[index] = instruction.Size;
                }
            }

            bool changed;
            do
            {
                changed = false;
                int offset = 0;
                for (int index = 0; compressJumps && index < count; index++)
                {
                    offsets[index] = offset;
                    offset += sizes[index];
                }

                for (int index = 0; compressJumps && index < count; index++)
                {
                    if (!isCandidate[index] || useShort[index]) continue;

                    int reduction = sizes[index] - InstructionSize(shortOpcodes[index]);
                    Instruction source = instructions[index];
                    bool fits;
                    if (trySourceToTargets.TryGetValue(source, out (Instruction, Instruction) targets))
                    {
                        fits = Fits(index, targets.Item1, reduction) && Fits(index, targets.Item2, reduction);
                    }
                    else if (jumpSourceToTargets.TryGetValue(source, out Instruction? target))
                    {
                        fits = Fits(index, target, reduction);
                    }
                    else
                    {
                        fits = false;
                    }

                    if (!fits) continue;
                    useShort[index] = true;
                    sizes[index] = InstructionSize(shortOpcodes[index]);
                    changed = true;
                }
            }
            while (changed);

            int address = 0;
            for (int index = 0; index < count; index++)
            {
                offsets[index] = address;
                address += sizes[index];
            }

            List<Instruction> encoded = new(count);
            Dictionary<Instruction, Instruction> currentToEncoded = new();
            for (int index = 0; index < count; index++)
            {
                Instruction current = instructions[index];
                Instruction output;
                if (isCandidate[index])
                {
                    OpCode opcode = useShort[index] ? shortOpcodes[index] : longOpcodes[index];
                    output = CreateJumpInstruction(opcode);
                }
                else
                {
                    output = current;
                }
                encoded.Add(output);
                currentToEncoded[current] = output;
            }

            Dictionary<Instruction, int> encodedAddresses = new();
            for (int index = 0; index < encoded.Count; index++)
                encodedAddresses[encoded[index]] = offsets[index];

            Dictionary<Instruction, Instruction> encodedJumpTargets = new();
            foreach ((Instruction source, Instruction target) in jumpSourceToTargets)
                encodedJumpTargets[currentToEncoded[source]] = currentToEncoded[target];

            Dictionary<Instruction, (Instruction, Instruction)> encodedTryTargets = new();
            foreach ((Instruction source, (Instruction target1, Instruction target2)) in trySourceToTargets)
                encodedTryTargets[currentToEncoded[source]] = (currentToEncoded[target1], currentToEncoded[target2]);

            OrderedDictionary instructionsToAddress = new();
            foreach (Instruction instruction in encoded)
                instructionsToAddress[instruction] = encodedAddresses[instruction];
            Script script = OptimizedScriptBuilder.BuildScriptWithJumpTargets(
                instructionsToAddress, encodedJumpTargets, encodedTryTargets, originalAddressToInstruction);
            return new Layout(script, encodedAddresses, currentToEncoded, encodedJumpTargets, encodedTryTargets);

            bool Fits(int sourceIndex, Instruction target, int selfReduction)
            {
                int targetOffset = offsets[instructionToIndex[target]];
                if (sourceIndex < instructionToIndex[target]) targetOffset -= selfReduction;
                int delta = targetOffset - offsets[sourceIndex];
                return delta >= sbyte.MinValue && delta <= sbyte.MaxValue;
            }
        }

        private static int InstructionSize(OpCode opcode)
        {
            return 1 + Optimizer.OperandSizeTable[(int)opcode];
        }

        private NefFile CopyNef(Script script)
        {
            return new NefFile
            {
                Compiler = Nef.Compiler,
                Source = Nef.Source,
                Tokens = Tokens,
                Script = script,
            };
        }

        public static Instruction CreateInstruction(OpCode opcode, byte[]? operand = null)
        {
            operand ??= [];
            int prefixSize = Optimizer.OperandSizePrefixTable[(int)opcode];
            List<byte> bytes = [(byte)opcode];
            int length = operand.Length;
            for (int index = 0; index < prefixSize; index++)
            {
                bytes.Add((byte)length);
                length >>= 8;
            }
            bytes.AddRange(operand);
            return new Script(bytes.ToArray()).GetInstruction(0);
        }

        public static Instruction CreateJumpInstruction(OpCode opcode)
        {
            int operandSize = Optimizer.OperandSizeTable[(int)opcode];
            if (opcode is OpCode.TRY or OpCode.TRY_L) operandSize *= 2;
            List<byte> bytes = [(byte)opcode];
            bytes.AddRange(new byte[operandSize]);
            return new Script(bytes.ToArray()).GetInstruction(0);
        }

        private static Instruction CloneRegular(Instruction instruction)
        {
            return CreateInstruction(instruction.OpCode, instruction.Operand.ToArray());
        }

        private static bool IsIntegerPush(Instruction instruction, int value)
        {
            if (instruction.OpCode is OpCode.PUSH0 or OpCode.PUSH1)
                return value == (instruction.OpCode == OpCode.PUSH1 ? 1 : 0);
            if (!pushInt.Contains(instruction.OpCode)) return false;
            return new System.Numerics.BigInteger(instruction.Operand.Span) == value;
        }

        private static bool TryGetConstantPushTruth(OpCode opcode, out bool truth)
        {
            switch (opcode)
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

        private static readonly HashSet<OpCode> singleShortJumpOpcodes =
        [
            OpCode.JMP, OpCode.CALL, OpCode.ENDTRY,  .. conditionalJump,
        ];

        private static readonly HashSet<OpCode> singleLongJumpOpcodes =
        [
            OpCode.JMP_L, OpCode.CALL_L, OpCode.ENDTRY_L, .. conditionalJump_L,
        ];
    }

    [Flags]
    internal enum PeepholeRule
    {
        None = 0,
        FoldNotInEqual = 1 << 0,
        RemoveDupDrop = 1 << 1,
        UseIncDec = 1 << 2,
        UseIsNull = 1 << 3,
        UseNz = 1 << 4,
        FoldNotInJmp = 1 << 5,
        All = FoldNotInEqual | RemoveDupDrop | UseIncDec | UseIsNull | UseNz | FoldNotInJmp,
    }
}
