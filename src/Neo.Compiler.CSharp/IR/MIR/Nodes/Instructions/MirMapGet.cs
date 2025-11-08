using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirMapGet : MirInst
{
    internal MirMapGet(MirValue map, MirValue key, MirType valueType)
        : base(valueType)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    internal MirValue Map { get; }
    internal MirValue Key { get; }
}
