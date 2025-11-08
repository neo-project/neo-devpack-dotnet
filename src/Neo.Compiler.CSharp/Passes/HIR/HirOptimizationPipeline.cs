using System;
namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirOptimizationPipeline
{
    private static readonly IHirPass[] s_passes =
    {
        new HirConstantFolderPass(),
        new HirPhiSimplificationPass(),
        new HirGuardPrunePass(),
        new HirGuardHoistingPass(),
        new HirCopyPropagationPass(),
        new HirDeadCodeEliminationPass(),
        new HirBranchSimplificationPass(),
        new HirUnreachableBlockEliminationPass()
    };

    public void Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changedAny = false;
        bool changedIteration;

        int iteration = 0;
        do
        {
            changedIteration = false;
            foreach (var pass in s_passes)
            {
                if (pass.Run(function))
                {
                    changedIteration = true;
                    changedAny = true;
                }
            }
            iteration++;
        } while (changedIteration);

        if (changedAny)
        {
            // Re-run simple structural verification when the IR was updated.
            var verifier = new HirVerifier();
            var errors = verifier.Verify(function);
            if (errors.Count > 0)
                throw new InvalidOperationException($"HIR verification failed after optimisation:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
        }
    }
}
