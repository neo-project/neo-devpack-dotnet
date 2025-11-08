using System;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirPhiSimplificationPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            var phis = block.Phis.ToArray();
            foreach (var phi in phis)
            {
                if (phi.Inputs.Count == 0 || phi.IsPinned)
                    continue;

                var reference = phi.Inputs[0].Value;
                var uniform = true;
                for (int i = 1; i < phi.Inputs.Count; i++)
                {
                    if (!ReferenceEquals(phi.Inputs[i].Value, reference))
                    {
                        uniform = false;
                        break;
                    }
                }

                if (!uniform)
                    continue;

                MirValueRewriter.Replace(function, phi, reference);
                block.RemovePhi(phi);
                changed = true;
            }
        }

        return changed;
    }
}
