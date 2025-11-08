using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal enum HirFailPolicy
{
    Abort,
    Assume,
    PathSplit
}
