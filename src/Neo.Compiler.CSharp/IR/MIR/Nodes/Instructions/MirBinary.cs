using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirBinary : MirInst
{
    internal enum Op
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        And,
        Or,
        Xor,
        Shl,
        Shr,
        Max,
        Min,
        Pow
    }

    internal MirBinary(Op opCode, MirValue left, MirValue right, MirType resultType)
        : base(resultType)
    {
        OpCode = opCode;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    internal Op OpCode { get; }
    internal MirValue Left { get; }
    internal MirValue Right { get; }
}
