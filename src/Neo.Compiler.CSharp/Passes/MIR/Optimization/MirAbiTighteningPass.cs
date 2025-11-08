using System;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Performs small ABI-oriented clean-ups: ensure aggregate return packs are materialised immediately prior to the return.
/// </summary>
internal sealed class MirAbiTighteningPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var usage = MirAnalysis.Analyze(function);
        var changed = false;

        foreach (var block in function.Blocks)
        {
            if (block.Terminator is not MirReturn ret || ret.Value is not MirStructPack pack)
                continue;

            if (!usage.InstructionBlocks.TryGetValue(pack, out var packBlock))
                continue;

            if (!usage.UseCounts.TryGetValue(pack, out var uses) || uses != 1)
                continue;

            if (ReferenceEquals(packBlock, block))
                continue;

            packBlock.RemoveInstruction(pack);
            block.Instructions.Insert(block.Instructions.Count, pack);
            changed = true;
        }

        return changed;
    }
}
