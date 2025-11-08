using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirBufferCopy : MirInst
{
    internal MirBufferCopy(MirValue destination, MirValue source, MirValue destinationOffset, MirValue sourceOffset, MirValue length)
        : base(destination.Type)
    {
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        DestinationOffset = destinationOffset ?? throw new ArgumentNullException(nameof(destinationOffset));
        SourceOffset = sourceOffset ?? throw new ArgumentNullException(nameof(sourceOffset));
        Length = length ?? throw new ArgumentNullException(nameof(length));
    }

    internal MirValue Destination { get; }
    internal MirValue Source { get; }
    internal MirValue DestinationOffset { get; }
    internal MirValue SourceOffset { get; }
    internal MirValue Length { get; }
    internal override MirEffect Effect => MirEffect.Memory;
}
