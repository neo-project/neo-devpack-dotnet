using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirUnreachableBlockEliminationPass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var visited = new HashSet<HirBlock>();
        var worklist = new Stack<HirBlock>();
        worklist.Push(function.Entry);
        visited.Add(function.Entry);

        while (worklist.Count > 0)
        {
            var block = worklist.Pop();
            foreach (var successor in HirControlFlow.GetSuccessors(block))
            {
                if (successor is null || visited.Contains(successor))
                    continue;
                visited.Add(successor);
                worklist.Push(successor);
            }
        }

        var removed = false;

        foreach (var block in function.Blocks.ToArray())
        {
            if (visited.Contains(block))
                continue;

            RemoveBlock(function, block);
            removed = true;
        }

        return removed;
    }

    private static void RemoveBlock(HirFunction function, HirBlock block)
    {
        foreach (var other in function.Blocks)
        {
            foreach (var phi in other.Phis.ToArray())
            {
                phi.RemoveIncoming(block);
            }
        }

        function.RemoveBlock(block);
    }
}
