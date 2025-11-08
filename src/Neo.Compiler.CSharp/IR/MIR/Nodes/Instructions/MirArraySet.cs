using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirArraySet : MirInst
{
    internal MirArraySet(MirValue array, MirValue index, MirValue value)
        : base(array.Type)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal MirValue Array { get; private set; }
    internal MirValue Index { get; private set; }
    internal MirValue Value { get; private set; }
    internal override MirEffect Effect => MirEffect.Memory;

    internal void Update(MirValue array, MirValue index, MirValue value)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
