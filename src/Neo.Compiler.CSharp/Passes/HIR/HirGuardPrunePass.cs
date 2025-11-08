using System;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirGuardPrunePass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            for (int i = 0; i < block.Instructions.Count; i++)
            {
                var instruction = block.Instructions[i];
                switch (instruction)
                {
                    case HirNullCheck { Policy: HirFailPolicy.Assume }:
                    case HirBoundsCheck { Policy: HirFailPolicy.Assume }:
                        block.RemoveInstructionAt(i);
                        changed = true;
                        i--;
                        break;
                }
            }
        }

        return changed;
    }
}
