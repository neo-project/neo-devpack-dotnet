using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VJmpIf : VTerminator
{
    internal VJmpIf(VNode condition, VBlock trueTarget, VBlock falseTarget)
    {
        Condition = condition;
        TrueTarget = trueTarget;
        FalseTarget = falseTarget;
    }

    internal VNode Condition { get; }
    internal VBlock TrueTarget { get; }
    internal VBlock FalseTarget { get; }
}
