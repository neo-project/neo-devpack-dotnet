using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VEndFinally : VTerminator
{
    internal VEndFinally(VTry scope, VBlock target)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    internal VTry Scope { get; }
    internal VBlock Target { get; }
}
