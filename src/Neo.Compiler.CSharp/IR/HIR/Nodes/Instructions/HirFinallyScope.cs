using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirFinallyScope : HirInst
{
    public HirFinallyScope(HirTryFinallyScope parent)
        : base(HirType.VoidType)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    public HirTryFinallyScope Parent { get; }
}
