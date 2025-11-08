using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirLoadLocal : HirInst
{
    public HirLoadLocal(HirLocal local)
        : base(local.Type)
    {
        Local = local ?? throw new ArgumentNullException(nameof(local));
    }

    public HirLocal Local { get; }
}
