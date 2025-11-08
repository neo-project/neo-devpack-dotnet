using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirCompare : MirInst
{
    internal enum Op
    {
        Eq,
        Ne,
        Lt,
        Le,
        Gt,
        Ge
    }

    internal MirCompare(Op opCode, MirValue left, MirValue right, bool unsigned = false)
        : base(MirType.TBool)
    {
        OpCode = opCode;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        Unsigned = unsigned;
    }

    internal Op OpCode { get; }
    internal MirValue Left { get; }
    internal MirValue Right { get; }
    internal bool Unsigned { get; }
}
