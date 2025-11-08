using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirBoundsCheck : HirInst
{
    public HirBoundsCheck(HirValue index, HirValue length, HirFailPolicy policy)
        : base(HirType.VoidType)
    {
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Length = length ?? throw new ArgumentNullException(nameof(length));
        Policy = policy;
    }

    public HirValue Index { get; set; }
    public HirValue Length { get; set; }
    public HirFailPolicy Policy { get; }
}
