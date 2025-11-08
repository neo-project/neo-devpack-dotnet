using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirNullCheck : HirInst
{
    public HirNullCheck(HirValue reference, HirFailPolicy policy)
        : base(HirType.VoidType)
    {
        Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        Policy = policy;
    }

    public HirValue Reference { get; set; }
    public HirFailPolicy Policy { get; }
}
