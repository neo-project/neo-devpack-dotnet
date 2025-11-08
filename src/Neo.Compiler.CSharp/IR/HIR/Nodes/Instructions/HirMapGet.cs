using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirMapGet : HirInst
{
    public HirMapGet(HirValue map, HirValue key, HirType valueType)
        : base(valueType)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    public HirValue Map { get; set; }
    public HirValue Key { get; set; }
}
