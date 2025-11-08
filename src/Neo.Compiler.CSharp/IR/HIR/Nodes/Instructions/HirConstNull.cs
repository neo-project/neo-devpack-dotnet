using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConstNull : HirInst
{
    public HirConstNull()
        : base(HirType.NullType)
    {
    }
}
