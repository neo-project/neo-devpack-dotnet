using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VUnary : VNode
{
    internal VUnary(VUnaryOp op, VNode operand, LirType type)
        : base(type)
    {
        Op = op;
        Operand = operand;
    }

    internal VUnaryOp Op { get; }
    internal VNode Operand { get; }
}
