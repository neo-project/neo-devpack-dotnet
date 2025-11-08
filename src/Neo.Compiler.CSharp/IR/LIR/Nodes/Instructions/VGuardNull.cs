using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VGuardNull : VNode
{
    internal VGuardNull(VNode reference, VGuardFailKind failKind, VBlock? failTarget)
        : base(LirType.TVoid)
    {
        Reference = reference;
        FailKind = failKind;
        FailTarget = failTarget;
    }

    internal VNode Reference { get; }
    internal VGuardFailKind FailKind { get; }
    internal VBlock? FailTarget { get; }
}
