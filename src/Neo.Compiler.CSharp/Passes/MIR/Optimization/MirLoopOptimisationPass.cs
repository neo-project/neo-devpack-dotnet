using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Neo.Compiler.Analysis;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Performs a lightweight loop optimisation pass focused on loop-invariant code motion.
/// </summary>
internal sealed class MirLoopOptimisationPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var hasExceptionalControl = function.Blocks.Any(block =>
            block.Instructions.Any(inst => inst is MirTry or MirCatch or MirFinally) ||
            block.Terminator is MirLeave or MirEndFinally);

        if (hasExceptionalControl)
            return false;

        var dominators = MirDominatorAnalysis.Compute(function);
        var predecessors = MirDominatorAnalysis.BuildPredecessors(function);
        var usage = MirAnalysis.Analyze(function);

        var loops = LoopAnalysis.FindNaturalLoops(function.Blocks, function.Entry, GetSuccessors);
        var changed = false;

        foreach (var loop in loops)
        {
            if (NormaliseLoop(function, loop, predecessors))
                changed = true;

            if (TryHoistInvariantInstructions(loop, predecessors, usage))
                changed = true;
        }

        return changed;
    }

    private bool NormaliseLoop(
        MirFunction function,
        LoopAnalysis.LoopInfo<MirBlock> loop,
        Dictionary<MirBlock, HashSet<MirBlock>> predecessors)
    {
        if (!predecessors.TryGetValue(loop.Header, out var headerPreds))
            return false;

        var loopReachable = ComputeReachableWithinLoop(loop.Header, loop.Blocks);
        var outsidePreds = headerPreds.Where(pred => !loopReachable.Contains(pred)).ToList();
        if (outsidePreds.Count == 0)
            return false;

        var phiValues = new List<(MirPhi Phi, MirValue Value)>();
        MirBlock? canonicalOutside = null;
        var needsPreheader = outsidePreds.Count switch
        {
            0 => false,
            1 when outsidePreds[0].Terminator is MirBranch { Target: var branchTarget } && ReferenceEquals(branchTarget, loop.Header)
                => false,
            1 => true,
            _ => true
        };
        if (outsidePreds.Count == 1)
            canonicalOutside = outsidePreds[0];

        foreach (var phi in loop.Header.Phis)
        {
            var incoming = phi.Inputs.Where(input => outsidePreds.Contains(input.Block)).ToArray();
            if (incoming.Length == 0)
                continue;

            var referenceValue = incoming[0].Value;
            var uniform = true;
            for (int i = 1; i < incoming.Length; i++)
            {
                if (!ReferenceEquals(referenceValue, incoming[i].Value))
                {
                    uniform = false;
                    break;
                }
            }

            if (!uniform)
                return false;

            phiValues.Add((phi, referenceValue));

            if (canonicalOutside is null || incoming.Any(input => !ReferenceEquals(input.Block, canonicalOutside)))
                needsPreheader = true;
        }

        if (!needsPreheader)
            return false;

        var preheader = CreatePreheaderBlock(function, loop.Header);
        if (!predecessors.ContainsKey(preheader))
            predecessors[preheader] = new HashSet<MirBlock>();

        foreach (var pred in outsidePreds)
        {
            RedirectSuccessor(pred, loop.Header, preheader);
            headerPreds.Remove(pred);
            predecessors[preheader].Add(pred);
        }

        headerPreds.Add(preheader);

        foreach (var (phi, value) in phiValues)
        {
            foreach (var pred in outsidePreds)
                phi.RemoveIncoming(pred);
            phi.AddIncoming(preheader, value);
        }

        return true;
    }

    private bool TryHoistInvariantInstructions(
        LoopAnalysis.LoopInfo<MirBlock> loop,
        Dictionary<MirBlock, HashSet<MirBlock>> predecessors,
        MirUsageGraph usage)
    {
        var preheader = FindPreheader(loop, predecessors);
        if (preheader is null)
            return false;

        var changed = false;
        foreach (var block in loop.Blocks)
        {
            var instructions = block.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var inst = instructions[i];
                if (!IsHoistable(inst))
                    continue;

                if (!OperandsAreLoopInvariant(inst, loop.Blocks, usage))
                    continue;

                instructions.RemoveAt(i);
                preheader.Instructions.Insert(preheader.Instructions.Count, inst);
                i--;
                changed = true;
            }
        }

        return changed;
    }

    private static MirBlock? FindPreheader(LoopAnalysis.LoopInfo<MirBlock> loop, Dictionary<MirBlock, HashSet<MirBlock>> predecessors)
    {
        if (!predecessors.TryGetValue(loop.Header, out var preds))
            return null;

        MirBlock? preheader = null;
        foreach (var pred in preds)
        {
            if (loop.Blocks.Contains(pred))
                continue;

            if (preheader is not null)
                return null; // Require unique preheader.

            preheader = pred;
        }

        if (preheader is null)
            return null;

        if (preheader.Terminator is MirBranch { Target: var target } && ReferenceEquals(target, loop.Header))
            return preheader;

        return null;
    }

    private static bool OperandsAreLoopInvariant(MirInst inst, HashSet<MirBlock> loopBlocks, MirUsageGraph usage)
    {
        foreach (var operand in MirAnalysis.GetOperands(inst))
        {
            var defBlock = GetDefiningBlock(operand, usage);
            if (defBlock is not null && loopBlocks.Contains(defBlock))
                return false;
        }

        return true;
    }

    private static MirBlock? GetDefiningBlock(MirValue value, MirUsageGraph usage)
    {
        if (value is MirInst inst && usage.InstructionBlocks.TryGetValue(inst, out var block))
            return block;
        if (value is MirPhi phi && usage.PhiBlocks.TryGetValue(phi, out var phiBlock))
            return phiBlock;
        return null;
    }

    private static bool IsHoistable(MirInst inst)
    {
        if (inst is null)
            return false;

        if (inst.Effect != MirEffect.None)
            return false;

        if (inst.ProducesMemoryToken || inst.ConsumesMemoryToken)
            return false;

        return inst switch
        {
            MirGuardNull => false,
            MirGuardBounds => false,
            MirCall => false,
            MirPointerCall => false,
            MirSyscall => false,
            _ => true
        };
    }

    private static IEnumerable<MirBlock> GetSuccessors(MirBlock block)
    {
        return MirControlFlow.GetSuccessors(block);
    }

    private static HashSet<MirBlock> ComputeReachableWithinLoop(MirBlock header, HashSet<MirBlock> loopBlocks)
    {
        var reachable = new HashSet<MirBlock>();
        if (header is null || loopBlocks is null || loopBlocks.Count == 0)
            return reachable;

        var stack = new Stack<MirBlock>();
        stack.Push(header);

        while (stack.Count > 0)
        {
            var block = stack.Pop();
            if (!loopBlocks.Contains(block))
                continue;
            if (!reachable.Add(block))
                continue;

            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (loopBlocks.Contains(successor))
                    stack.Push(successor);
            }
        }

        return reachable;
    }

    private static MirBlock CreatePreheaderBlock(MirFunction function, MirBlock header)
    {
        var baseLabel = header.Label + ".preheader";
        var label = baseLabel;
        var counter = 0;
        while (function.Blocks.Any(b => string.Equals(b.Label, label, StringComparison.Ordinal)))
            label = $"{baseLabel}_{++counter}";

        var block = function.CreateBlock(label);
        var headerIndex = function.Blocks.IndexOf(header);
        function.Blocks.Remove(block);
        function.Blocks.Insert(headerIndex, block);
        block.Terminator = new MirBranch(header);
        return block;
    }

    private static void RedirectSuccessor(MirBlock block, MirBlock oldTarget, MirBlock newTarget)
    {
        switch (block.Terminator)
        {
            case MirBranch branch when ReferenceEquals(branch.Target, oldTarget):
                block.Terminator = new MirBranch(newTarget) { Span = branch.Span };
                break;

            case MirCondBranch cond:
                var trueTarget = cond.TrueTarget;
                var falseTarget = cond.FalseTarget;
                var updated = false;
                if (ReferenceEquals(trueTarget, oldTarget))
                {
                    trueTarget = newTarget;
                    updated = true;
                }

                if (ReferenceEquals(falseTarget, oldTarget))
                {
                    falseTarget = newTarget;
                    updated = true;
                }

                if (updated)
                    block.Terminator = new MirCondBranch(cond.Condition, trueTarget, falseTarget) { Span = cond.Span };
                break;

            case MirSwitch @switch:
                var cases = new List<(BigInteger Case, MirBlock Target)>(@switch.Cases.Count);
                var changed = false;
                foreach (var (caseValue, target) in @switch.Cases)
                {
                    if (ReferenceEquals(target, oldTarget))
                    {
                        cases.Add((caseValue, newTarget));
                        changed = true;
                    }
                    else
                    {
                        cases.Add((caseValue, target));
                    }
                }

                var defaultTarget = ReferenceEquals(@switch.DefaultTarget, oldTarget) ? newTarget : @switch.DefaultTarget;
                if (changed || !ReferenceEquals(defaultTarget, @switch.DefaultTarget))
                    block.Terminator = new MirSwitch(@switch.Key, cases, defaultTarget) { Span = @switch.Span };
                break;
        }
    }
}
