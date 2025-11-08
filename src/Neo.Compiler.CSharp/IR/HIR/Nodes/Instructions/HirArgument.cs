using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirArgument : HirValue
{
    public HirArgument(string name, HirType type, int index)
        : base(type)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Index = index;
    }

    public string Name { get; }
    public int Index { get; }
}
