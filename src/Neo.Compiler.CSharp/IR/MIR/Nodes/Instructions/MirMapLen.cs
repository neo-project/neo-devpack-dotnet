using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirMapLen : MirInst
{
    internal MirMapLen(MirValue map)
        : base(MirType.TInt)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
    }

    internal MirValue Map { get; }
}
