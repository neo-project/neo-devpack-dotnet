using System;

namespace Neo.Compiler.HIR;

internal sealed class HirBranch : HirTerminator
{
    public HirBranch(HirBlock target)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    public HirBlock Target { get; }
}

