using System;

namespace Neo.Compiler.HIR;

internal abstract class HirValue : HirNode
{
    protected HirValue(HirType type)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public HirType Type { get; private set; }

    public void SetType(HirType type)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}

