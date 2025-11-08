using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirArraySet : HirInst
{
    public HirArraySet(HirValue array, HirValue index, HirValue value)
        : base(array.Type)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public override HirEffect Effect => HirEffect.Memory;
    public HirValue Array { get; set; }
    public HirValue Index { get; set; }
    public HirValue Value { get; set; }
}
