using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirUnary : MirInst
{
    internal enum Op
    {
        Neg,
        Not,
        Abs,
        Sign,
        Inc,
        Dec,
        Sqrt
    }

    internal MirUnary(Op opCode, MirValue operand, MirType resultType)
        : base(resultType)
    {
        OpCode = opCode;
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    internal Op OpCode { get; }
    internal MirValue Operand { get; }
}
