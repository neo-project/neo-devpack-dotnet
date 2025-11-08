using System;
using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirDeadCodeEliminationPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var analysis = MirAnalysis.Analyze(function);
        var useCounts = analysis.UseCounts;
        var worklist = new Stack<MirValue>();

        foreach (var inst in analysis.InstructionBlocks.Keys)
        {
            if (IsCandidate(inst) && useCounts.TryGetValue(inst, out var count) && count == 0)
                worklist.Push(inst);
        }

        foreach (var phi in analysis.PhiBlocks.Keys)
        {
            if (!IsCandidate(phi))
                continue;

            if (useCounts.TryGetValue(phi, out var count) && count == 0)
                worklist.Push(phi);
        }

        var changed = false;

        while (worklist.Count > 0)
        {
            var value = worklist.Pop();
            if (!useCounts.TryGetValue(value, out var remaining) || remaining != 0)
                continue;

            switch (value)
            {
                case MirInst inst when analysis.InstructionBlocks.TryGetValue(inst, out var block) && IsCandidate(inst):
                    RemoveInstruction(block, inst, useCounts, worklist);
                    changed = true;
                    break;
                case MirPhi phi when analysis.PhiBlocks.TryGetValue(phi, out var phiBlock) && IsCandidate(phi):
                    RemovePhi(phiBlock, phi, useCounts, worklist);
                    changed = true;
                    break;
            }
        }

        return changed;
    }

    private static void RemoveInstruction(MirBlock block, MirInst inst, Dictionary<MirValue, int> useCounts, Stack<MirValue> worklist)
    {
        foreach (var operand in MirAnalysis.GetOperands(inst))
            Decrement(useCounts, operand, worklist);

        block.RemoveInstruction(inst);
        useCounts[inst] = -1;
    }

    private static void RemovePhi(MirBlock block, MirPhi phi, Dictionary<MirValue, int> useCounts, Stack<MirValue> worklist)
    {
        foreach (var (_, value) in phi.Inputs)
            Decrement(useCounts, value, worklist);

        block.RemovePhi(phi);
        useCounts[phi] = -1;
    }

    private static void Decrement(Dictionary<MirValue, int> useCounts, MirValue operand, Stack<MirValue> worklist)
    {
        if (operand is null)
            return;
        if (!useCounts.TryGetValue(operand, out var count) || count <= 0)
            return;

        count--;
        useCounts[operand] = count;
        if (count == 0 && operand is MirInst inst && IsCandidate(inst))
            worklist.Push(inst);
        else if (count == 0 && operand is MirPhi phi && IsCandidate(phi))
            worklist.Push(phi);
    }

    private static bool IsCandidate(MirInst inst)
    {
        if (inst.Type == MirType.TVoid)
            return false;
        if (inst.Effect != MirEffect.None)
            return false;
        if (inst.ProducesMemoryToken || inst.ConsumesMemoryToken)
            return false;
        if (inst is MirArrayGet)
            return false;
        return true;
    }

    private static bool IsCandidate(MirPhi phi)
    {
        return phi.Type is not MirTokenType;
    }
}
