using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VModPow : VNode
{
    internal VModPow(VNode value, VNode exponent, VNode modulus, LirType type)
        : base(type)
    {
        Value = value;
        Exponent = exponent;
        Modulus = modulus;
    }

    internal VNode Value { get; }
    internal VNode Exponent { get; }
    internal VNode Modulus { get; }
}
