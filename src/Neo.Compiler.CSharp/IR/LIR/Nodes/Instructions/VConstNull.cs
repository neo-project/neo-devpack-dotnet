using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;

internal sealed class VConstNull : VNode
{
    internal VConstNull()
        : base(LirType.TAny)
    {
    }
}
