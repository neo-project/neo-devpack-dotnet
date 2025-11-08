using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirMapLen : HirInst
{
    public HirMapLen(HirValue map)
        : base(HirType.IntType)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
    }

    public HirValue Map { get; set; }
}
