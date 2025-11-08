using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

internal static class MirPhiUtilities
{
    internal static void PruneNonPredecessorInputs(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var predecessors = new Dictionary<MirBlock, HashSet<MirBlock>>(ReferenceEqualityComparer.Instance);
        foreach (var block in function.Blocks)
            predecessors[block] = new HashSet<MirBlock>(ReferenceEqualityComparer.Instance);

        foreach (var block in function.Blocks)
        {
            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (predecessors.TryGetValue(successor, out var set))
                    set.Add(block);
            }
        }

        foreach (var block in function.Blocks)
        {
            predecessors.TryGetValue(block, out var preds);

            for (int i = block.Phis.Count - 1; i >= 0; i--)
            {
                var phi = block.Phis[i];
                var inputs = phi.Inputs.ToArray();
                foreach (var (pred, _) in inputs)
                {
                    if (preds is null || !preds.Contains(pred))
                        phi.RemoveIncoming(pred);
                }

                if (phi.Inputs.Count == 0)
                    block.RemovePhi(phi);
            }
        }
    }
}
