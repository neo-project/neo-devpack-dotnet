using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirMapDelete : MirInst
{
    internal MirMapDelete(MirValue map, MirValue key)
        : base(map.Type)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    internal MirValue Map { get; }
    internal MirValue Key { get; }
    internal override MirEffect Effect => MirEffect.Memory;
}
