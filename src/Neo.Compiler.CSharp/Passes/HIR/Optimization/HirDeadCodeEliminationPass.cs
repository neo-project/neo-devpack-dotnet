using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirDeadCodeEliminationPass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var analysis = HirAnalysis.Analyze(function);
        var useCounts = analysis.UseCounts;
        var worklist = new Stack<HirValue>();

        foreach (var inst in analysis.InstructionBlocks.Keys)
        {
            if (IsRemovable(inst) && useCounts.TryGetValue(inst, out var count) && count == 0)
                worklist.Push(inst);
        }

        foreach (var phi in analysis.PhiBlocks.Keys)
        {
            if (phi.IsLocalPhi)
                continue;
            if (useCounts.TryGetValue(phi, out var count) && count == 0)
                worklist.Push(phi);
        }

        var modified = false;

        while (worklist.Count > 0)
        {
            var value = worklist.Pop();

            if (!useCounts.TryGetValue(value, out var uses) || uses != 0)
                continue;

            switch (value)
            {
                case HirPhi phi when analysis.PhiBlocks.TryGetValue(phi, out var phiBlock):
                    RemovePhi(phiBlock, phi, useCounts, worklist);
                    modified = true;
                    break;

                case HirInst inst when analysis.InstructionBlocks.TryGetValue(inst, out var block) && IsRemovable(inst):
                    RemoveInstruction(block, inst, useCounts, worklist);
                    modified = true;
                    break;
            }
        }

        return modified;
    }

    private static void RemoveInstruction(HirBlock block, HirInst inst, Dictionary<HirValue, int> useCounts, Stack<HirValue> worklist)
    {
        foreach (var operand in HirAnalysis.GetOperands(inst))
            Decrement(useCounts, operand, worklist);

        block.Remove(inst);
        useCounts[inst] = -1; // mark as removed
    }

    private static void RemovePhi(HirBlock block, HirPhi phi, Dictionary<HirValue, int> useCounts, Stack<HirValue> worklist)
    {
        foreach (var incoming in phi.Inputs)
            Decrement(useCounts, incoming.Value, worklist);

        block.RemovePhi(phi);
        useCounts[phi] = -1;
    }

    private static void Decrement(Dictionary<HirValue, int> useCounts, HirValue operand, Stack<HirValue> worklist)
    {
        if (operand is null)
            return;

        if (!useCounts.TryGetValue(operand, out var count))
            return;

        if (count <= 0)
            return;

        count--;
        useCounts[operand] = count;

        if (count == 0 && IsCandidate(operand))
            worklist.Push(operand);
    }

    private static bool IsCandidate(HirValue value) => value switch
    {
        HirPhi phi => !phi.IsLocalPhi,
        HirInst inst => IsRemovable(inst),
        _ => false
    };

    private static bool IsRemovable(HirInst inst)
    {
        if (inst is null)
            return false;

        if (inst.Type == HirType.VoidType)
            return false;

        if (inst.Effect != HirEffect.None)
            return false;

        if (inst.ProducesMemoryToken || inst.ConsumesMemoryToken)
            return false;

        return true;
    }
}
