using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirArrayLen : HirInst
{
    public HirArrayLen(HirValue array)
        : base(HirType.IntType)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
    }

    public HirValue Array { get; set; }
}
