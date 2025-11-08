using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirArrayGet : HirInst
{
    public HirArrayGet(HirValue array, HirValue index, HirType elementType)
        : base(elementType)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
        Index = index ?? throw new ArgumentNullException(nameof(index));
    }

    public HirValue Array { get; set; }
    public HirValue Index { get; set; }
}
