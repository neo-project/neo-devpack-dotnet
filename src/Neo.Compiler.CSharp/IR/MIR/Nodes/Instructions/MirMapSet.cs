using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirMapSet : MirInst
{
    internal MirMapSet(MirValue map, MirValue key, MirValue value)
        : base(map.Type)
    {
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal MirValue Map { get; }
    internal MirValue Key { get; }
    internal MirValue Value { get; }
    internal override MirEffect Effect => MirEffect.Memory;
}
