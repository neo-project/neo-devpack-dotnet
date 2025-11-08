using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirStructGet : MirInst
{
    internal MirStructGet(MirValue obj, int index, MirType fieldType)
        : base(fieldType)
    {
        Object = obj ?? throw new ArgumentNullException(nameof(obj));
        Index = index;
    }

    internal MirValue Object { get; }
    internal int Index { get; }
}
