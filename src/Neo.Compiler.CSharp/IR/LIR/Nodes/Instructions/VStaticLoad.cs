using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VStaticLoad : VNode
{
    internal VStaticLoad(byte slot, LirType type)
        : base(type)
    {
        Slot = slot;
    }

    internal byte Slot { get; }
}
