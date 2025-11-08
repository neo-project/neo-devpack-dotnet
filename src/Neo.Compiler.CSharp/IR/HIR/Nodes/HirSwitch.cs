using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirSwitch : HirTerminator
{
    public HirSwitch(HirValue key, IReadOnlyList<(BigInteger Case, HirBlock Target)> cases, HirBlock defaultTarget)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Cases = cases ?? throw new ArgumentNullException(nameof(cases));
        DefaultTarget = defaultTarget ?? throw new ArgumentNullException(nameof(defaultTarget));
    }

    public HirValue Key { get; }
    public IReadOnlyList<(BigInteger Case, HirBlock Target)> Cases { get; }
    public HirBlock DefaultTarget { get; }
}

