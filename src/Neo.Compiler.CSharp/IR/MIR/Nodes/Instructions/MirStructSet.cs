using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirStructSet : MirInst
{
    internal MirStructSet(MirValue obj, int index, MirValue value, MirStructType structType)
        : base(structType)
    {
        Object = obj ?? throw new ArgumentNullException(nameof(obj));
        Index = index;
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal MirValue Object { get; }
    internal int Index { get; }
    internal MirValue Value { get; }
    internal override MirEffect Effect => MirEffect.Memory;
}
