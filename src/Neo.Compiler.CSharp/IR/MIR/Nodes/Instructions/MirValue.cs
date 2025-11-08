using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal abstract class MirValue : MirNode
{
    protected MirValue(MirType type)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    internal MirType Type { get; private set; }

    internal void SetType(MirType type)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
