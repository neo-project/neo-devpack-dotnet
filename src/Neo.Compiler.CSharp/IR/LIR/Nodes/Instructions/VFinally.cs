using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VFinally : VNode
{
    internal VFinally(VTry scope)
        : base(LirType.TVoid)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
    }

    internal VTry Scope { get; }
}
