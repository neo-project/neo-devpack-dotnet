using System;
using Neo.Compiler.LIR;

namespace Neo.Compiler.LIR.Backend;

/// <summary>
/// Performs small local rewrites on stack LIR to eliminate redundant shuffle sequences and constant patterns. This
/// starter pass only captures a handful of canonical cleanups and is expected to grow alongside instruction coverage.
/// </summary>
internal static class LirPeephole
{
    internal static bool Run(LirFunction function)
    {
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        var changed = false;

        foreach (var block in function.Blocks)
        {
            var insts = block.Instructions;
            var i = 0;

            while (i + 1 < insts.Count)
            {
                var current = insts[i];
                var next = insts[i + 1];

                if (current.Op == LirOpcode.DUP && next.Op == LirOpcode.DROP)
                {
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                if (current.Op == LirOpcode.SWAP && next.Op == LirOpcode.SWAP)
                {
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                if (current.Op == LirOpcode.OVER && next.Op == LirOpcode.DROP)
                {
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                if (current.Op == LirOpcode.PUSH0 && next.Op == LirOpcode.ADD)
                {
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                if (current.Op == LirOpcode.PUSH0 && next.Op == LirOpcode.SUB)
                {
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                if (current.Op == LirOpcode.NOT && next.Op == LirOpcode.NOT)
                {
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                if (i + 2 < insts.Count && current.Op == LirOpcode.ROT && insts[i + 1].Op == LirOpcode.ROT && insts[i + 2].Op == LirOpcode.ROT)
                {
                    insts.RemoveAt(i + 2);
                    insts.RemoveAt(i + 1);
                    insts.RemoveAt(i);
                    i = Math.Max(i - 2, 0);
                    changed = true;
                    continue;
                }

                i += 1;
            }
        }

        return changed;
    }
}
