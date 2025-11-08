using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal enum HirCmpKind
{
    Eq,
    Ne,
    Lt,
    Le,
    Gt,
    Ge
}
