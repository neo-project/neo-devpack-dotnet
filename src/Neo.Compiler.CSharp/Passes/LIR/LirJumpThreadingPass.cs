using System;

namespace Neo.Compiler.LIR.Optimization;

/// <summary>
/// Removes redundant unconditional jumps and simplifies conditional jump fallthroughs in linear block order.
/// </summary>
internal sealed class LirJumpThreadingPass : ILirPass
{
    public bool Run(LirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var blocks = function.Blocks;
        var changed = false;

        for (int i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];
            var instructions = block.Instructions;
            if (instructions.Count == 0)
                continue;

            var lastIndex = instructions.Count - 1;
            var last = instructions[lastIndex];

            if (last.Op == LirOpcode.JMP && i + 1 < blocks.Count && string.Equals(last.TargetLabel, blocks[i + 1].Label, StringComparison.Ordinal))
            {
                instructions.RemoveAt(lastIndex);
                changed = true;
                lastIndex--;
            }

            if (lastIndex < 1)
                continue;

            var penultimate = instructions[lastIndex - 1];
            if (penultimate.Op == LirOpcode.JMPIF && lastIndex == instructions.Count - 1 && last.Op == LirOpcode.JMP && i + 1 < blocks.Count && string.Equals(last.TargetLabel, blocks[i + 1].Label, StringComparison.Ordinal))
            {
                instructions.RemoveAt(lastIndex);
                changed = true;
            }
        }

        return changed;
    }
}
