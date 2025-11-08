using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirPhiSimplificationPass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            var phis = block.Phis.ToArray();
            foreach (var phi in phis)
            {
                if (phi.Inputs.Count == 0)
                    continue;

                var referenceValue = phi.Inputs[0].Value;
                var uniform = true;

                for (int i = 1; i < phi.Inputs.Count; i++)
                {
                    if (!ReferenceEquals(phi.Inputs[i].Value, referenceValue))
                    {
                        uniform = false;
                        break;
                    }
                }

                if (!uniform)
                    continue;

                // All inputs the same (or phi has a single input)
                HirValueRewriter.Replace(function, phi, referenceValue);
                block.RemovePhi(phi);
                changed = true;
            }
        }

        return changed;
    }
}
