using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirEndFinally : HirTerminator
{
    public HirEndFinally(HirTryFinallyScope scope, HirBlock target)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    public HirTryFinallyScope Scope { get; }
    public HirBlock Target { get; }
}
