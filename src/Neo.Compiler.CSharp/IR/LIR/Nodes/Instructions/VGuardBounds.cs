using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VGuardBounds : VNode
{
    internal VGuardBounds(VNode index, VNode length, VGuardFailKind failKind, VBlock? failTarget)
        : base(LirType.TVoid)
    {
        Index = index;
        Length = length;
        FailKind = failKind;
        FailTarget = failTarget;
    }

    internal VNode Index { get; }
    internal VNode Length { get; }
    internal VGuardFailKind FailKind { get; }
    internal VBlock? FailTarget { get; }
}
