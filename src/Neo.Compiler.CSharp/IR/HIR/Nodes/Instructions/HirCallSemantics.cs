using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal enum HirCallSemantics
{
    Pure,
    ReadOnly,
    Effectful
}
