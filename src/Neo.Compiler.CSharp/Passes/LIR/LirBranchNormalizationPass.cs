using System;

namespace Neo.Compiler.LIR.Optimization;

/// <summary>
/// Rewrites conditional branches so that fallthrough prefers the next block, eliminating redundant jumps.
/// </summary>
internal sealed class LirBranchNormalizationPass : ILirPass
{
    public bool Run(LirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var blocks = function.Blocks;
        var changed = false;

        for (int i = 0; i < blocks.Count; i++)
        {
            if (i + 1 >= blocks.Count)
                continue;

            var block = blocks[i];
            var instructions = block.Instructions;
            if (instructions.Count < 2)
                continue;

            var last = instructions[^1];
            if (last.Op != LirOpcode.JMP || string.IsNullOrEmpty(last.TargetLabel))
                continue;

            var conditional = instructions[^2];
            var fallthroughLabel = blocks[i + 1].Label;

            if (string.IsNullOrEmpty(fallthroughLabel))
                continue;

            if (conditional.Op == LirOpcode.JMPIF &&
                string.Equals(conditional.TargetLabel, fallthroughLabel, StringComparison.Ordinal))
            {
                // On condition true we jump to fallthrough; invert and drop explicit JMP.
                var replacement = new LirInst(LirOpcode.JMPIFNOT)
                {
                    Span = conditional.Span,
                    TargetLabel = last.TargetLabel
                };
                instructions[^2] = replacement;
                instructions.RemoveAt(instructions.Count - 1);
                changed = true;
            }
            else if (conditional.Op == LirOpcode.JMPIFNOT &&
                     string.Equals(conditional.TargetLabel, fallthroughLabel, StringComparison.Ordinal))
            {
                var replacement = new LirInst(LirOpcode.JMPIF)
                {
                    Span = conditional.Span,
                    TargetLabel = last.TargetLabel
                };
                instructions[^2] = replacement;
                instructions.RemoveAt(instructions.Count - 1);
                changed = true;
            }
        }

        return changed;
    }
}
