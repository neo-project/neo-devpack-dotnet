using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirBufferNew : HirInst
{
    public HirBufferNew(HirValue length)
        : base(HirType.BufferType)
    {
        Length = length ?? throw new ArgumentNullException(nameof(length));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public HirValue Length { get; set; }
    public override HirEffect Effect => HirEffect.Memory;
}
