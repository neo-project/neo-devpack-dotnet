using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirBufferSet : HirInst
{
    public HirBufferSet(HirValue buffer, HirValue index, HirValue @byte)
        : base(buffer.Type)
    {
        Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Byte = @byte ?? throw new ArgumentNullException(nameof(@byte));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public HirValue Buffer { get; set; }
    public HirValue Index { get; set; }
    public HirValue Byte { get; set; }
    public override HirEffect Effect => HirEffect.Memory;
}
