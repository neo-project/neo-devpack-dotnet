using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.HIR.Optimization;

/// <summary>
/// Propagates values through simple local stores/loads within the same block to reduce redundant temporaries.
/// </summary>
internal sealed class HirCopyPropagationPass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            var locals = new Dictionary<HirLocal, HirValue>();
            var instructions = block.Instructions.ToArray();

            foreach (var inst in instructions)
            {
                switch (inst)
                {
                    case HirStoreLocal store:
                        locals[store.Local] = store.Value;
                        break;

                    case HirLoadLocal load when locals.TryGetValue(load.Local, out var replacement):
                        HirValueRewriter.Replace(function, load, replacement);
                        block.Remove(load);
                        changed = true;
                        break;

                    default:
                        if (inst.ProducesMemoryToken || inst.ConsumesMemoryToken || inst.Effect != HirEffect.None)
                            locals.Clear();
                        break;
                }
            }
        }

        return changed;
    }
}
