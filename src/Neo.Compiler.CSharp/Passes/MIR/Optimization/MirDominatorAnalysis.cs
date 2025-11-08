using System;
using System.Collections.Generic;
using Neo.Compiler.Analysis;

namespace Neo.Compiler.MIR.Optimization;

internal static class MirDominatorAnalysis
{
    internal static IReadOnlyDictionary<MirBlock, HashSet<MirBlock>> Compute(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        return ControlFlowAnalysis.ComputeDominators(function.Blocks, function.Entry, GetSuccessors);
    }

    internal static Dictionary<MirBlock, HashSet<MirBlock>> BuildPredecessors(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        return ControlFlowAnalysis.BuildPredecessors(function.Blocks, GetSuccessors);
    }

    internal static bool Dominates(MirBlock dominator, MirBlock block, IReadOnlyDictionary<MirBlock, HashSet<MirBlock>> dominators)
    {
        if (dominator is null)
            throw new ArgumentNullException(nameof(dominator));
        if (block is null)
            throw new ArgumentNullException(nameof(block));
        if (dominators is null)
            throw new ArgumentNullException(nameof(dominators));

        return dominators.TryGetValue(block, out var set) && set.Contains(dominator);
    }

    private static IEnumerable<MirBlock> GetSuccessors(MirBlock block)
    {
        if (block is null)
            yield break;

        foreach (var successor in MirControlFlow.GetSuccessors(block))
            yield return successor;
    }
}
