using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Analysis;

internal static class ControlFlowAnalysis
{
    internal static Dictionary<TBlock, HashSet<TBlock>> BuildPredecessors<TBlock>(
        IEnumerable<TBlock> blocks,
        Func<TBlock, IEnumerable<TBlock>> getSuccessors)
        where TBlock : class
    {
        if (blocks is null)
            throw new ArgumentNullException(nameof(blocks));
        if (getSuccessors is null)
            throw new ArgumentNullException(nameof(getSuccessors));

        var blockList = blocks.ToArray();
        var predecessors = blockList.ToDictionary(block => block, _ => new HashSet<TBlock>());

        foreach (var block in blockList)
        {
            foreach (var successor in getSuccessors(block))
            {
                if (successor is null)
                    continue;

                if (!predecessors.TryGetValue(successor, out var predSet))
                {
                    predSet = new HashSet<TBlock>();
                    predecessors[successor] = predSet;
                }

                predSet.Add(block);
            }
        }

        return predecessors;
    }

    internal static IReadOnlyDictionary<TBlock, HashSet<TBlock>> ComputeDominators<TBlock>(
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
            return new Dictionary<TBlock, HashSet<TBlock>>();

        var predecessors = BuildPredecessors(blockList, getSuccessors);
        var dominators = new Dictionary<TBlock, HashSet<TBlock>>(blockList.Length);

        foreach (var block in blockList)
        {
            if (ReferenceEquals(block, entry))
            {
                dominators[block] = new HashSet<TBlock> { block };
            }
            else
            {
                dominators[block] = new HashSet<TBlock>(blockList);
            }
        }

        var changed = true;
        while (changed)
        {
            changed = false;

            foreach (var block in blockList)
            {
                if (ReferenceEquals(block, entry))
                    continue;

                var hasPreds = predecessors.TryGetValue(block, out var predSet) && predSet.Count > 0;
                HashSet<TBlock> newSet;
                if (!hasPreds)
                {
                    newSet = new HashSet<TBlock> { block };
                }
                else
                {
                    var enumerator = predSet!.GetEnumerator();
                    enumerator.MoveNext();
                    newSet = new HashSet<TBlock>(dominators[enumerator.Current]);
                    while (enumerator.MoveNext())
                        newSet.IntersectWith(dominators[enumerator.Current]);
                    newSet.Add(block);
                }

                var current = dominators[block];
                if (!current.SetEquals(newSet))
                {
                    current.Clear();
                    current.UnionWith(newSet);
                    changed = true;
                }
            }
        }

        return dominators;
    }
}
