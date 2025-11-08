using System;
using System.Collections.Generic;
using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.LIR.Optimization;

internal sealed class LirOptimizationPipeline
{
    private readonly IReadOnlyList<ILirPass> _passes = new ILirPass[]
    {
        new LirPeepholeAdapter(),
        new LirBranchNormalizationPass()
    };

    public void Run(LirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;
        foreach (var pass in _passes)
            changed |= pass.Run(function);

        if (changed)
        {
            // Stack effect annotation depends on the final instruction stream, so recompute afterwards.
            StackEffectAnnotator.Annotate(function);
        }
    }

    private sealed class LirPeepholeAdapter : ILirPass
    {
        public bool Run(LirFunction function) => LirPeephole.Run(function);
    }
}
