using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirStructSet : HirInst
{
    public HirStructSet(HirValue obj, int index, HirValue value, HirStructType structType)
        : base(structType)
    {
        Object = obj ?? throw new ArgumentNullException(nameof(obj));
        Index = index;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public override HirEffect Effect => HirEffect.Memory;
    public HirValue Object { get; set; }
    public int Index { get; }
    public HirValue Value { get; set; }
}
