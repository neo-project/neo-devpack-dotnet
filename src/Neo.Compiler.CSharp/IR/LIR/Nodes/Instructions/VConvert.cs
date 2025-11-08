using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VConvert : VNode
{
    internal VConvert(VConvertOp op, VNode value, LirType targetType)
        : base(targetType)
    {
        Op = op;
        Value = value;
    }

    internal VConvertOp Op { get; }
    internal VNode Value { get; }
}
