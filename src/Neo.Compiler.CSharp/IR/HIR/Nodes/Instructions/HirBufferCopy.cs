using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirBufferCopy : HirInst
{
    public HirBufferCopy(HirValue destination, HirValue source, HirValue destinationOffset, HirValue sourceOffset, HirValue length)
        : base(destination.Type)
    {
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        DestinationOffset = destinationOffset ?? throw new ArgumentNullException(nameof(destinationOffset));
        SourceOffset = sourceOffset ?? throw new ArgumentNullException(nameof(sourceOffset));
        Length = length ?? throw new ArgumentNullException(nameof(length));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public HirValue Destination { get; set; }
    public HirValue Source { get; set; }
    public HirValue DestinationOffset { get; set; }
    public HirValue SourceOffset { get; set; }
    public HirValue Length { get; set; }
    public override HirEffect Effect => HirEffect.Memory;
}
