using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirMapHas : HirInst
{
    public HirMapHas(HirValue map, HirValue key)
        : base(HirType.BoolType)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    public HirValue Map { get; set; }
    public HirValue Key { get; set; }
}
