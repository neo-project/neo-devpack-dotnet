using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VJmp : VTerminator
{
    internal VJmp(VBlock target) => Target = target;

    internal VBlock Target { get; }
}
