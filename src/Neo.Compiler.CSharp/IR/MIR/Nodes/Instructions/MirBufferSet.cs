using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirBufferSet : MirInst
{
    internal MirBufferSet(MirValue buffer, MirValue index, MirValue @byte)
        : base(buffer.Type)
    {
        Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Byte = @byte ?? throw new ArgumentNullException(nameof(@byte));
    }

    internal MirValue Buffer { get; }
    internal MirValue Index { get; }
    internal MirValue Byte { get; }
    internal override MirEffect Effect => MirEffect.Memory;
}
