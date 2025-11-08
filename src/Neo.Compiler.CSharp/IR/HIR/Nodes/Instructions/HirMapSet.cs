using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirMapSet : HirInst
{
    public HirMapSet(HirValue map, HirValue key, HirValue value)
        : base(map.Type)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public override HirEffect Effect => HirEffect.Memory;
    public HirValue Map { get; set; }
    public HirValue Key { get; set; }
    public HirValue Value { get; set; }
}
