using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VSwitch : VTerminator
{
    internal VSwitch(VNode key, IReadOnlyList<(BigInteger Case, VBlock Target)> cases, VBlock defaultTarget)
    {
        Key = key;
        Cases = cases;
        DefaultTarget = defaultTarget;
    }

    internal VNode Key { get; }
    internal IReadOnlyList<(BigInteger Case, VBlock Target)> Cases { get; }
    internal VBlock DefaultTarget { get; }
}
