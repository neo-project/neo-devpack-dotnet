using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Analysis;

internal static class LoopAnalysis
{
    internal sealed record LoopInfo<TBlock>(TBlock Header, TBlock Latch, HashSet<TBlock> Blocks) where TBlock : class;

    internal static IReadOnlyList<LoopInfo<TBlock>> FindNaturalLoops<TBlock>(
        IEnumerable<TBlock> blocks,
        TBlock entry,
        Func<TBlock, IEnumerable<TBlock>> getSuccessors)
        where TBlock : class
    {
        if (blocks is null)
            throw new ArgumentNullException(nameof(blocks));
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));
        if (getSuccessors is null)
            throw new ArgumentNullException(nameof(getSuccessors));

        var blockList = blocks.ToArray();
        if (blockList.Length == 0)
            return Array.Empty<LoopInfo<TBlock>>();

        var dominators = ControlFlowAnalysis.ComputeDominators(blockList, entry, getSuccessors);
        var predecessors = ControlFlowAnalysis.BuildPredecessors(blockList, getSuccessors);
        var loops = new List<LoopInfo<TBlock>>();
        var seen = new HashSet<(TBlock Header, TBlock Latch)>();

        foreach (var block in blockList)
        {
            foreach (var successor in getSuccessors(block))
            {
                if (successor is null)
                    continue;

                if (!dominators.TryGetValue(block, out var blockDominators) || !blockDominators.Contains(successor))
                    continue;

                if (!seen.Add((successor, block)))
                    continue;

                var loopBlocks = CollectNaturalLoop(successor, block, predecessors);
                loops.Add(new LoopInfo<TBlock>(successor, block, loopBlocks));
            }
        }

        return loops;
    }

    private static HashSet<TBlock> CollectNaturalLoop<TBlock>(
        TBlock header,
        TBlock latch,
        Dictionary<TBlock, HashSet<TBlock>> predecessors)
        where TBlock : class
    {
        var loopBlocks = new HashSet<TBlock> { header };
        var stack = new Stack<TBlock>();
        stack.Push(latch);

        while (stack.Count > 0)
        {
            var block = stack.Pop();
            if (!loopBlocks.Add(block))
                continue;

            if (predecessors.TryGetValue(block, out var preds))
            {
                foreach (var pred in preds)
                    stack.Push(pred);
            }
        }

        return loopBlocks;
    }
}
