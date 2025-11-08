using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirStructGet : HirInst
{
    public HirStructGet(HirValue obj, int index, HirType fieldType)
        : base(fieldType)
    {
        Object = obj ?? throw new ArgumentNullException(nameof(obj));
        Index = index;
    }

    public HirValue Object { get; set; }
    public int Index { get; }
}
