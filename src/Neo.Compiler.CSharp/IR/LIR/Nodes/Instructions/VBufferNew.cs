using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VBufferNew : VNode
{
    internal VBufferNew(VNode length)
        : base(LirType.TBuffer)
    {
        Length = length;
    }

    internal VNode Length { get; }
}
