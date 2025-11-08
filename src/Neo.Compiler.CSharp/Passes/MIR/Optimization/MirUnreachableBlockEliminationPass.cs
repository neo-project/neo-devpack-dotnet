using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirUnreachableBlockEliminationPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var visited = new HashSet<MirBlock>();
        var worklist = new Stack<MirBlock>();
        visited.Add(function.Entry);
        worklist.Push(function.Entry);

        while (worklist.Count > 0)
        {
            var block = worklist.Pop();
            foreach (var succ in MirControlFlow.GetSuccessors(block))
            {
                if (succ is null || visited.Contains(succ))
                    continue;
                visited.Add(succ);
                worklist.Push(succ);
            }

            if (block.Terminator is MirCompareBranch cmp)
            {
                EnqueueSuccessor(cmp.TrueTarget);
                EnqueueSuccessor(cmp.FalseTarget);
            }

            void EnqueueSuccessor(MirBlock? successor)
            {
                if (successor is null || visited.Contains(successor))
                    return;
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

    private static void RemoveBlock(MirFunction function, MirBlock block)
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
