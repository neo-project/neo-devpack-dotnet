using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VArrayNew : VNode
{
    internal VArrayNew(VNode length, LirType elemType)
        : base(new LirArrayType(elemType))
    {
        Length = length;
        ElementType = elemType;
    }

    internal VNode Length { get; }
    internal LirType ElementType { get; }
}
