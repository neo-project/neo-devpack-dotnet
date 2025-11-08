using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirBufferNew : MirInst
{
    internal MirBufferNew(MirValue length)
        : base(MirType.TBuffer)
    {
        Length = length ?? throw new ArgumentNullException(nameof(length));
    }

    internal MirValue Length { get; }
    internal override MirEffect Effect => MirEffect.Memory;
}
